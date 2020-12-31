using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GradientDescSolver : IKSolverBase
{
    #region EDITOR
    [Header("Gradient Descent")]
    /// <summary>
    /// DeltaX for the partial gradient
    /// To calculate a pseudo derivative 
    /// </summary>
    [SerializeField] private float SamplingDistance = .1f;

    /// <summary>
    /// Learning rate of the gradient descent
    /// Decides how fast the end effector approaches the target
    /// </summary>
    [SerializeField] private int LearningRate = 30;
    #endregion
    
    #region REFS
    /// <summary> Rotation of all the joints </summary>
    private Quaternion[] _rotations = new Quaternion[0];

    /// <summary>
    /// Parent of the root joint
    /// </summary>
    private Transform _rootParent = null;
    #endregion

    #region IKSolverBase
    protected override void Initialize()
    {
        base.Initialize();
        _rootParent = Joints[0].parent;
        _rotations = new Quaternion[ChainLength];
        _rotations[0] = Joints[0].rotation; 
        for(int i = 1; i < ChainLength; i++) _rotations[i] = Joints[i].localRotation; 
    }

    protected override void ResolveIK() {               
        // Version 1:
        InverseKinematics(Target.position, ref _rotations); 

        // Version 2: 
        //foreach(Vector3 axis in THREE_ROTATION_DOF) InverseKinematicsV2(m_Target.position, ref m_Rotations, axis);               
    }
    #endregion

    #region PRIVATE METHODS 
    /// <summary>
    /// Calculates the position of the end effector with the given rotations
    /// </summary>
    /// <param name="rotations">Joint rotations</param>
    /// <returns>Vector3 position of end effector using FK</returns>
    private Vector3 ForwardKinematics(Quaternion[] rotations)
    {       
        // Root position
        Vector3 currentPos = Joints[0].position;
        currentPos = _rootParent != null ? Vector3.Scale(currentPos, _rootParent.lossyScale) : currentPos;
        Quaternion rotation = Quaternion.identity;
        for (int i = 1; i < Joints.Length; i++)
        {
            rotation *= rotations[i - 1];
            //Transformation scaling affects the position of the child
            Vector3 jointOffset = Vector3.Scale(Joints[i].localPosition, Joints[i - 1].lossyScale);
            Vector3 nextPos = currentPos + rotation * jointOffset;
            currentPos = nextPos;     
        }

        return currentPos; 
    }

    /// <summary>
    /// Error function for the gradient descent
    /// </summary>
    /// <param name="target">Target position</param>
    /// <param name="rotations">Joint rotations</param>
    /// <returns>Distance between end effector with given chain rotations and target</returns>
    private float Error(Vector3 target, Quaternion[] rotations)
    {
        Vector3 endPosition = ForwardKinematics(rotations);
        return Vector3.Distance(endPosition, target);
    }

    /// <summary>
    /// Calculate the partial gradient for the given axis
    /// </summary>
    /// <param name="target">Target position</param>
    /// <param name="rotations">Joint rotations</param>
    /// <param name="i">The joint index</param>
    /// <param name="axis">The rotation axis</param>
    /// <returns>Partial Gradient</returns>
    private float PartialGradient(Vector3 target, Quaternion[] rotations, int i, Vector3 axis)
    {
        Quaternion prevRot = rotations[i];
        
        // Derivative : (f(x+deltaX) - f(x)) / deltaX;  
        float f_x = Error(target, rotations);   
        rotations[i] *= Quaternion.Euler(SamplingDistance * axis);     
        float f_x_plus_deltaX = Error(target, rotations);   
        float gradient = (f_x_plus_deltaX - f_x) / SamplingDistance;
    
        rotations[i] = prevRot;
        return gradient;
    }

    /* 
    Either have gradient:
        per dof (IKv1) -> calculate partialGradient for each DOF
        per joint (IKv2) -> call IK for each DOF
    */ 

    /// <summary>
    /// IK using Gradient descent per joint
    /// </summary>
    /// <param name="target">Target position</param>
    /// <param name="rotations">Current joint rotations</param>
    private void InverseKinematics(Vector3 target, ref Quaternion[] rotations)
    {     
        if (Error(target, rotations) < MaxError) return;   
    
        // Gradient descent Starting at the last joint
        for (int i = ChainLength - 1; i >= 0; i --)
        {
            // DOF of the joint
            JointConstraints constraints = null;
            bool constrained = Constraints.TryGetValue(i, out constraints);        
            Vector3[] rotationAxis = constrained ? constraints.RotationAxis.ToArray() : THREE_ROTATION_DOF;

            // Calculate the gradient descent
            Vector3 descentVals = Vector3.zero;
            foreach(Vector3 axis in rotationAxis) {
                float partialGradient = PartialGradient(target, rotations, i, axis);
                descentVals += partialGradient * axis;
            }    

            Vector3 newAngles = rotations[i].eulerAngles - (LearningRate * descentVals);

            // Joint Axis Constraints
            if(constrained) {
                foreach(Vector3 axis in rotationAxis) { 
                    Vector3 angleOfInterest = Vector3.Scale(newAngles, axis);
                    float newAngle = angleOfInterest.x + angleOfInterest.y + angleOfInterest.z;
                    // aQuaternion.eulerAngles always returns positive values
                    newAngle = newAngle > 180 ? newAngle - 360 : newAngle;
                    newAngle = Mathf.Clamp
                    (
                        newAngle,  
                        constraints.MinAngle(axis), 
                        constraints.MaxAngle(axis)
                    );

                    // Sum up the final calculation
                    Vector3 unaffectedAxis = new Vector3(1, 1, 1) - axis;
                    newAngles = Vector3.Scale(newAngles, unaffectedAxis) + newAngle * axis;
                }
            }   
                  
            // Apply new rotation to the joint
            Quaternion newLocalRot = Quaternion.Euler(newAngles);
            if(i == 0) Joints[0].localRotation = _rootParent != null ? Quaternion.Inverse(_rootParent.rotation) * newLocalRot : newLocalRot;
            else Joints[i].rotation = Joints[i].parent.rotation * newLocalRot;   
            rotations[i].eulerAngles = newAngles;      

            // Early termination
            if (Error(target, rotations) < MaxError) return;               
        }                   
    }   

    /// <summary>
    /// IK using Gradient descent per degree of freedom
    /// </summary>
    /// <param name="target">Target position</param>
    /// <param name="rotations">Current joint rotations</param>
    private void InverseKinematicsV2(Vector3 target, ref Quaternion[] rotations, Vector3 axis)
    {     
        if (Error(target, rotations) < MaxError) return;   
    
        // Gradient descent Starting at the last joint
        for (int i = Joints.Length - 2; i >= 0; i --)
        {
            // Joint Rotation Constraints
            JointConstraints constraints = null;
            bool constrained = Constraints.TryGetValue(i, out constraints);                 

            if ((constrained && constraints.RotationAxis.Contains(axis)) || !constrained) {
                float partialGradient = PartialGradient(target, rotations, i, axis);
                Vector3 descent = LearningRate * partialGradient * axis;

                Vector3 newAngles = rotations[i].eulerAngles - descent;

                // Joint Axis Constraints
                if (constrained) {
                    Vector3 angles = Vector3.Scale(newAngles, axis);
                    float newAngle = angles.x + angles.y + angles.z; 
                    newAngle = newAngle > 180 ? newAngle - 360 : newAngle;           
                    newAngle = Mathf.Clamp
                    (
                        newAngle,  
                        constraints.MinAngle(axis), 
                        constraints.MaxAngle(axis)
                    );       

                    // Sum up the final calculation
                    Vector3 unaffectedAxis = new Vector3(1, 1, 1) - axis;
                    newAngles = Vector3.Scale(newAngles, unaffectedAxis) + newAngle * axis;
                }

                rotations[i].eulerAngles = newAngles;    

                // Apply new rotation to the joint
                Joints[i].localEulerAngles = newAngles;                

                // Early termination
                if (Error(target, rotations) < MaxError) return; 
            }                                 
        }                   
    } 
    #endregion
}
