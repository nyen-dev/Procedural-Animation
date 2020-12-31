using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CreatureKinematic : MonoBehaviour, IMovement
{
    #region EDITOR VALS
    /// <summary> All foots (leg end effectors) </summary>
    [SerializeField] private IKSolverBase[] FootIK = new IKSolverBase[0];

    /// <summary> Step distance </summary>
    [SerializeField] private float StepDistance = 1;
    #endregion

    #region COMPONENTS
    private Rigidbody _rigidBody = null;
    #endregion

    #region REFS
    /// <summary> Foot target positions </summary>
    private Transform[] _footTargets = new Transform[0];

    private const int RAYCAST_LAYERMASK = ~(1 << 8);
    #endregion

    #region UNITY METHODS
    // Start is called before the first frame update
    void Start()
    {   
        _rigidBody = GetComponent<Rigidbody>();

        _footTargets = new Transform[FootIK.Length];
        for(int i = 0; i < FootIK.Length; i++) {
            _footTargets[i] = new GameObject(gameObject.name + string.Format(" Foot[{0}]-Target", i)).transform;
            _footTargets[i].position = FootIK[i].transform.position;
            _footTargets[i].rotation = Quaternion.identity;
            _footTargets[i].localScale = Vector3.one;
            _footTargets[i].parent = transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < FootIK.Length; i++) {  
            // FootTarget should always be grounded        
            RaycastHit hit;  
            if (Physics.Raycast(_footTargets[i].position + Vector3.up, -_footTargets[i].up, out hit, 1f, RAYCAST_LAYERMASK)) {
                
                Vector3 groundFootPos = hit.point;
                _footTargets[i].position = groundFootPos;              
            }    
            // Check if Stepping distance has been reached
            if(Vector3.Distance(_footTargets[i].position, FootIK[i].GetTargetPosition()) > StepDistance)
                FootIK[i].SetTargetPosition(_footTargets[i].position);          
        }
    }
    void OnDrawGizmos()
    {       
        Gizmos.color = Color.blue;
        for (int i = 0; i < _footTargets.Length ;i++)
        {
            Gizmos.DrawSphere(_footTargets[i].position, .01f);
            Gizmos.DrawLine(_footTargets[i].position, FootIK[i].GetTargetPosition());
        }      
    }
    #endregion
    
    #region IMovement
    public void Rotate(Quaternion rot)
    {       
        _rigidBody.MoveRotation(transform.rotation * rot);
    }

    public void Translate(Vector3 transl)
    {
        _rigidBody.MovePosition(transform.position + transl);
    }
    #endregion
}
