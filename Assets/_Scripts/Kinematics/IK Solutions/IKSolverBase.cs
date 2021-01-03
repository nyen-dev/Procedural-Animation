using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IKSolverBase : MonoBehaviour
{
    #region EDITOR VALS
    /// <summary> Target transform </summary>
    [SerializeField] protected Transform Target = null;

    /// <summary> Chain Length </summary>
    [SerializeField] protected int ChainLength = 0;

    /// <summary>
    /// Threshhold of the error to the target
    /// decides where to stop the IK iteration  
    /// </summary>
    [SerializeField] protected float MaxError = .09f;
    #endregion

    #region REFS
    /// <summary> 
    /// IK Joints
    /// Last joint is simply the end effector
    /// </summary>
    protected Transform[] Joints = new Transform[0];

    /// <summary>
    /// Joint constrains
    /// </summary>
    /// <typeparam name="int">Joint Index</typeparam>
    /// <typeparam name="ConstrainedJoint">Constraints</typeparam>
    protected Dictionary<int, JointConstraints> Constraints = new Dictionary<int, JointConstraints>();

    /// <summary> Every rotation axis </summary>
    protected static Vector3[] THREE_ROTATION_DOF = new Vector3[3] {
        Vector3.right,
        Vector3.up,
        Vector3.forward
    };
    #endregion

    #region UNITY METHODS
    void Awake() {
        Initialize();
    }

    void FixedUpdate()
    {        
        if (Joints.Length != ChainLength + 1) Initialize();  
        ResolveIK();    
    }

    void OnDrawGizmos()
    {       
        #if UNITY_EDITOR
        Transform current = transform;
        Gizmos.color = Color.green;
        for (int i = 0; i < ChainLength && current != null && current.parent != null; i++)
        {
            Gizmos.DrawLine(current.parent.position, current.position);
            Gizmos.DrawSphere(current.parent.position, .01f);
            current = current.parent;
        }
        #endif 

        if (Target != null) {
            // Draw a red sphere at the target position
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(Target.position, MaxError);
            Gizmos.DrawLine(transform.position, Target.position);
        }         
    }
    #endregion

    /// <summary>
    /// Final IK Solver
    /// </summary>
    protected abstract void ResolveIK();

    /// <summary>
    /// Initialize values
    /// </summary>
    protected virtual void Initialize() {
        Joints = new Transform[ChainLength + 1];           

        // initialize chain
        Transform current = transform;
        for(int i = ChainLength; i >= 0; i--) 
        {    
            if (current == null) throw new UnityException("ChainLength greater than actual chain length");
            JointConstraints constraints = current.GetComponent<JointConstraints>();
            if (constraints != null) Constraints.Add(i, constraints);
            Joints[i] = current;         
            current = current.parent;          
        }     

        // initialize target if not given
        if (Target == null) 
        {
            Target = new GameObject(gameObject.name + " IK-Target").transform;
            Target.position = transform.position;
            Target.rotation = Quaternion.identity;
            Target.localScale = Vector3.one;
        }
    }

    #region PUBLIC METHODS
    /// <summary>
    /// Set the target position
    /// </summary>
    /// <param name="position">new target position</param>
    public virtual void SetTargetPosition(Vector3 position) {
        Target.position = position;
    }   

    /// <summary>
    /// Get the target position
    /// </summary>
    /// <param name="position">target position</param>
    public virtual Vector3 GetTargetPosition() {
        return Target.position;
    }  

    /// <summary>
    /// Get the max IK error
    /// </summary>
    /// <returns>max error of IK algorithm</returns>
    public virtual float GetMaxError() {
        return MaxError;
    }

    /// <summary>
    /// Current Error
    /// </summary>
    /// <returns></returns>
    public virtual float GetCurrentError() {
        return Vector3.Distance(Target.position, transform.position);
    }
    #endregion 
}
