using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKSolver : MonoBehaviour
{
    [SerializeField] private Joint[] joints;

    [SerializeField] private Quaternion samplingDistance;

    [SerializeField] private int stepSize;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Vector3 ForwardKinematics(Quaternion[] rotations)
    {
        Vector3 parentPos = joints[0].transform.position;
        Quaternion rotation = Quaternion.identity;
        for (int i = 1; i < joints.Length; i++)
        {
            //Rotate the point around its parentPoint
            rotation *= rotations[i - 1];
            Vector3 newPoint = parentPos + rotation * joints[i].getOffset();

            parentPos = newPoint;
        }

        return parentPos; //The final position actually
    }

    public float DistanceFromTarget(Vector3 target, Quaternion[] rotations)
    {
        Vector3 point = ForwardKinematics(rotations);
        return Vector3.Distance(point, target);
    }
  
}
