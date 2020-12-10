using System;
using UnityEngine;

public class IKSolver : MonoBehaviour
{
    [SerializeField] private Joint[] joints;

    [SerializeField] private float samplingDistance;

    [SerializeField] private int stepSize;

    [SerializeField] private float distanceThreshold;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Vector3 ForwardKinematics(float[] angles)
    {
        Vector3 parentPos = joints[0].transform.position;
        Quaternion rotation = Quaternion.identity;
        for (int i = 1; i < joints.Length; i++)
        {
            //Rotate the point around its parentPoint
            rotation *= Quaternion.AngleAxis(angles[i - 1], joints[i - 1].RotationAxis);
            Vector3 newPoint = parentPos + rotation * joints[i].RestingPosition;

            parentPos = newPoint;
        }

        return parentPos; //The final position actually
    }

    private float DistanceToTarget(Vector3 target, float[] angles)
    {
        Vector3 point = ForwardKinematics(angles);
        return Vector3.Distance(point, target);
    }

    private float PartialGradient (Vector3 target, float[] angles, int i)
    {
        float[] anglesCopy = new float[angles.Length]; //to not modify angles
        Array.Copy(angles, anglesCopy, angles.Length);
    
        // Gradient : (f(x+deltaX) - f(x)) / deltaX;  
        float f_x = DistanceToTarget(target, angles);
    
        anglesCopy[i] += samplingDistance;
        float f_x_plus_deltaX = DistanceToTarget(target, anglesCopy);
    
        float gradient = (f_x_plus_deltaX - f_x) / samplingDistance;
    
        return gradient;
    }

    private void InverseKinematics (Vector3 target, float [] angles)
    {
        if (DistanceToTarget(target, angles) < distanceThreshold) return;
    
        for (int i = joints.Length -1; i >= 0; i --)
        {
            // Gradient descent
            float gradient = PartialGradient(target, angles, i);
            angles[i] -= stepSize * gradient;
    
            angles[i] = Mathf.Clamp(angles[i], joints[i].MinAngle, joints[i].MaxAngle);

            // Early termination
            if (DistanceToTarget(target, angles) < distanceThreshold) return;
        }
    }
}
