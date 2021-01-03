using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CreatureKinematic : MonoBehaviour, IMovement
{
    #region CLASS LegPair
    [Serializable]
    private class LegPair {
        public IKSolverBase primaryLeg = null;
        public IKSolverBase oppositeLeg = null;
    }
    #endregion

    #region EDITOR VALS
    /// <summary> All foots (leg end effectors) </summary>
    [SerializeField] private LegPair[] FootPairs = new LegPair[0];

    /// <summary> Step distance </summary>
    [SerializeField] private float StepDistance = 1;

    /// <summary> Max difference between angle new target and current</summary>
    [SerializeField] private float AngleDistance = 10f;
    #endregion

    #region COMPONENTS
    private Rigidbody _rigidBody = null;
    #endregion

    #region REFS
    /// <summary> Primary foot target positions </summary>
    private Transform[] _footTargets = new Transform[0];

    private Vector3 _rootOffset = Vector3.zero;

    private bool _movePrimaryLeg = true; 

    private const int RAYCAST_LAYERMASK = ~(1 << 8);
    #endregion

    #region UNITY METHODS
    // Start is called before the first frame update
    void Start()
    {   
        _rigidBody = GetComponent<Rigidbody>();

        // Initialize footTargets
        _footTargets = new Transform[FootPairs.Length * 2];
        for(int i = 0; i < FootPairs.Length; i++) {
            int footTargetIndex = 2 * i;
            _footTargets[footTargetIndex] = SpawnFootTarget(FootPairs[i].primaryLeg, footTargetIndex);
            _footTargets[footTargetIndex + 1] = SpawnFootTarget(FootPairs[i].oppositeLeg, footTargetIndex + 1);
        }

        // Initialize rootOffset
        Vector3 center = RectangleCenterPoint(_footTargets.Select(target => target.position).ToArray());
        _rootOffset = transform.position - center;
    }

    // Update is called once per frame
    void Update()
    {   
        // FootTargets should always be grounded 
        for (int i = 0; i < FootPairs.Length; i++) {          
            RaycastHit hit;  
            if (Physics.Raycast(_footTargets[i].position + Vector3.up * 5f, -_footTargets[i].up, out hit, 1f, RAYCAST_LAYERMASK)) {
                Vector3 groundFootPos = hit.point;
                _footTargets[i].position = groundFootPos;
            }
        }

        // Move foot if a certain distance is reached
        if(MoveFoot()) _movePrimaryLeg = !_movePrimaryLeg; 

        // Body rotation  
    }

    void OnDrawGizmos() {
        if (_footTargets.Length > 0) {
            Gizmos.color = Color.blue;
            for(int i = 0; i < FootPairs.Length; i++) {
                int footTargetIndex = 2 * i;
                Gizmos.DrawSphere(_footTargets[footTargetIndex].position, FootPairs[i].primaryLeg.GetMaxError());
                Gizmos.DrawLine(_footTargets[footTargetIndex].position, FootPairs[i].primaryLeg.GetTargetPosition());

                Gizmos.DrawSphere(_footTargets[footTargetIndex + 1].position, FootPairs[i].oppositeLeg.GetMaxError());
                Gizmos.DrawLine(_footTargets[footTargetIndex + 1].position, FootPairs[i].oppositeLeg.GetTargetPosition());
            }
        }    
    }
    #endregion
    
    private Vector3 RectangleCenterPoint(Vector3[] borderPoints) {
        Vector3 center = Vector3.zero;
        for (int i = 0; i < borderPoints.Length; i++) { 
            center += borderPoints[i];
        }

        float avgScale = 1f/borderPoints.Length;
        center = Vector3.Scale(center, new Vector3(avgScale, avgScale, avgScale));

        return center;
    }
    
    private Transform SpawnFootTarget(IKSolverBase foot, int footNumber) {
        Transform footTarget = new GameObject(gameObject.name + string.Format(" Foot[{0}]-Target", footNumber)).transform;
        footTarget.position = foot.transform.position;
        footTarget.rotation = Quaternion.identity;
        footTarget.localScale = Vector3.one;
        footTarget.parent = transform;

        return footTarget;
    }

    private bool MoveFoot() {
        // Check if Stepping distance has been reached  
        bool moved = false;
        for (int i = 0; i < FootPairs.Length; i ++) {
            IKSolverBase footIK = _movePrimaryLeg ? FootPairs[i].primaryLeg : FootPairs[i].oppositeLeg;
            IKSolverBase otherFootIK = _movePrimaryLeg ? FootPairs[i].oppositeLeg : FootPairs[i].primaryLeg;
            int footTargetIndex = _movePrimaryLeg ? 2 * i : 2 * i + 1;
            int otherFootTargetIndex = _movePrimaryLeg ? 2 * i + 1 : 2 * i;

            Vector3 footCurrentPos = footIK.GetTargetPosition();
            Vector3 otherFootCurrentPos = otherFootIK.GetTargetPosition();
            Vector3 footTargetPos = _footTargets[footTargetIndex].position;   
            Vector3 otherFootTargetPos = _footTargets[otherFootTargetIndex].position; 
            
            if((
                    // StepDistance reached or
                    Vector3.Distance(footTargetPos, footCurrentPos) > StepDistance ||
                    // Angle between them reached AngleDistance and either
                    Vector3.Angle(footTargetPos, footCurrentPos) > AngleDistance
                ) && (
                    // other foot reached current target or
                    otherFootIK.GetCurrentError() < otherFootIK.GetMaxError() || 
                    // other foot also has reached the StepDistance
                    Vector3.Distance(otherFootTargetPos, otherFootCurrentPos) > StepDistance ||
                    // other foot also reached AngleDistance
                    Vector3.Angle(otherFootTargetPos, otherFootCurrentPos) > AngleDistance
                )
            ) {
                footIK.SetTargetPosition(footTargetPos);
                moved = true;
            }
        }

        return moved;
    }

    #region IMovement
    public void Rotate(Quaternion rot)
    {       
        _rigidBody.MoveRotation(transform.rotation * rot);
        // rot != Quaternion.identity && 
    }

    public void Translate(Vector3 transl)
    {
        _rigidBody.MovePosition(transform.position + transl);
        // transl.magnitude > 0 &&
    }
    #endregion
}
