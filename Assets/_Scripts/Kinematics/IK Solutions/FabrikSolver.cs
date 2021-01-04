using UnityEngine;

/// <summary>
/// Fabrik IK Solver
/// </summary>
public class FabrikSolver : IKSolverBase
{
    #region EDITOR VALS
    [Header("FABRIK")]
    [SerializeField] private Transform Pole = null;

    /// <summary> Number of IK iterations per update </summary>   
    [SerializeField] private int Iterations = 10;

    /// <summary> Strength of going back to the start position </summary>
    [Range(0, 1), SerializeField] private float SnapBackStrength = 1f;
    #endregion

    #region REFS
    /// <summary> Position of all the joints </summary>
    private Vector3[] _positions = new Vector3[0];

    /// <summary> Length of each bone </summary>
    private float[] _boneLength; 

    /// <summary> Sum of all bone.length </summary>
    private float _completeLength;

    /// <summary> Initial joint offsets to the target in root space</summary>
    private Vector3[] _initOffsetsTargetRootTF;

    /// <summary> Initial joint rotations in root space</summary>
    private Quaternion[] _initRotationsRootTF;

    /// <summary> Initial target rotation in root space</summary>
    private Quaternion _initTargetRotationRootTF;
    #endregion

    #region IKSolverBase
    protected override void Initialize()
    {
        base.Initialize();

        //initial array
        _positions = new Vector3[ChainLength + 1];
        _boneLength = new float[ChainLength];
        _initOffsetsTargetRootTF = new Vector3[ChainLength + 1];
        _initRotationsRootTF = new Quaternion[ChainLength + 1];

        //init data
        _initTargetRotationRootTF = GetRotationRootSpace(Target);    

        Transform current = transform;
        _completeLength = 0;
        for (int i = Joints.Length - 1; i >= 0; i--)
        {
            _initRotationsRootTF[i] = GetRotationRootSpace(current);

            if (i == Joints.Length - 1) _initOffsetsTargetRootTF[i] = GetPositionRootSpace(Target) - GetPositionRootSpace(current);           
            else
            {
                //mid bone
                _initOffsetsTargetRootTF[i] = GetPositionRootSpace(Joints[i + 1]) - GetPositionRootSpace(current);
                _boneLength[i] = _initOffsetsTargetRootTF[i].magnitude;
                _completeLength += _boneLength[i];
            }
            current = current.parent;
        }
    }

    protected override void ResolveIK()
    {
        //get position
        for (int i = 0; i < Joints.Length; i++) _positions[i] = GetPositionRootSpace(Joints[i]);

        Vector3 targetPosition = GetPositionRootSpace(Target);
        Quaternion targetRotation = GetRotationRootSpace(Target);

        //1st is possible to reach?
        if ((targetPosition - GetPositionRootSpace(Joints[0])).sqrMagnitude >= _completeLength * _completeLength)
        {
            //just strech it
            Vector3 direction = (targetPosition - _positions[0]).normalized;
            //set everything after root
            for (int i = 1; i < _positions.Length; i++) _positions[i] = _positions[i - 1] + direction * _boneLength[i - 1];
        }
        else
        {
            for (int i = 0; i < _positions.Length - 1; i++)
                _positions[i + 1] = Vector3.Lerp(_positions[i + 1], _positions[i] + _initOffsetsTargetRootTF[i], SnapBackStrength);

            for (int iteration = 0; iteration < Iterations; iteration++)
            {
                //back
                for (int i = _positions.Length - 1; i > 0; i--)
                {
                    if (i == _positions.Length - 1) _positions[i] = targetPosition; //set it to target
                    else _positions[i] = _positions[i + 1] + (_positions[i] - _positions[i + 1]).normalized * _boneLength[i]; //set in line on distance
                }

                //forward
                for (int i = 1; i < _positions.Length; i++)
                    _positions[i] = _positions[i - 1] + (_positions[i] - _positions[i - 1]).normalized * _boneLength[i - 1];

                //close enough?
                if ((_positions[_positions.Length - 1] - targetPosition).sqrMagnitude < MaxError * MaxError)
                    break;
            }
        }

        //move towards pole
        if (Pole != null)
        {
            Vector3 polePosition = GetPositionRootSpace(Pole);
            for (int i = 1; i < _positions.Length - 1; i++)
            {
                Plane plane = new Plane(_positions[i + 1] - _positions[i - 1], _positions[i - 1]);
                Vector3 projectedPole = plane.ClosestPointOnPlane(polePosition);
                Vector3 projectedBone = plane.ClosestPointOnPlane(_positions[i]);
                float angle = Vector3.SignedAngle(projectedBone - _positions[i - 1], projectedPole - _positions[i - 1], plane.normal);
                _positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (_positions[i] - _positions[i - 1]) + _positions[i - 1];
            }
        }

        //set position & rotation
        for (int i = 0; i < _positions.Length; i++)
        {
            JointConstraints constraints = null;
            Constraints.TryGetValue(i, out constraints); 
            if (i == _positions.Length - 1) 
                SetRotationRootSpace(
                    Joints[i],
                    Quaternion.Inverse(targetRotation) * _initTargetRotationRootTF * Quaternion.Inverse(_initRotationsRootTF[i])
                );
            else SetRotationRootSpace(
                    Joints[i], 
                    Quaternion.FromToRotation(_initOffsetsTargetRootTF[i], _positions[i + 1] - _positions[i]) * Quaternion.Inverse(_initRotationsRootTF[i])
                );
            SetPositionRootSpace(Joints[i], _positions[i]);                   
                       
        }
    }
    #endregion

    #region PRIVATE METHODS
    private Vector3 GetPositionRootSpace(Transform joint)
    {
        return Vector3.Scale(Joints[0].InverseTransformPoint(joint.position), Joints[0].lossyScale);
    }

    private void SetPositionRootSpace(Transform joint, Vector3 position)
    {
        joint.position = Joints[0].position + Joints[0].rotation * position;
    }

    private Quaternion GetRotationRootSpace(Transform joint)
    {
        return Joints[0].rotation * Quaternion.Inverse(joint.rotation);
    }

    private void SetRotationRootSpace(Transform joint, Quaternion rotation)
    {
        Quaternion newRot = Joints[0].rotation * rotation;
        joint.rotation = newRot;
    }
    #endregion
}
