using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointConstraints : MonoBehaviour
{
    #region ROTATION DOF CLASS
    [Serializable]
    private class AxisConstraints 
    {
        [Serializable]
        public class Constraints 
        {
            public bool Locked = false;

            public float MinAngle = -360;

            public float MaxAngle = 360;
        }

        public Constraints XAxis = new Constraints();

        public Constraints YAxis = new Constraints();

        public Constraints ZAxis = new Constraints();
    }
    #endregion

    #region EDITOR VALS
    [SerializeField] private AxisConstraints Constraints = new AxisConstraints();
    #endregion

    #region REFS
    private Vector3 _minAngles;

    private Vector3 _maxAngles;
    #endregion

    #region PROPERTIES
    public List<Vector3> RotationAxis { get; private set; } = new List<Vector3>();  
    #endregion
    
    #region UNITY METHODS
    void Awake()
    {
        // Initialize properties
        if (!Constraints.XAxis.Locked) RotationAxis.Add(Vector3.right);
        if (!Constraints.YAxis.Locked) RotationAxis.Add(Vector3.up);
        if (!Constraints.ZAxis.Locked) RotationAxis.Add(Vector3.forward);
        _minAngles = new Vector3
        (
            Constraints.XAxis.MinAngle,
            Constraints.YAxis.MinAngle,
            Constraints.ZAxis.MinAngle
        );
        _maxAngles = new Vector3
        (
            Constraints.XAxis.MaxAngle,
            Constraints.YAxis.MaxAngle,
            Constraints.ZAxis.MaxAngle
        );          
    }
    #endregion

    #region PUBLIC METHODS
    public float MinAngle(Vector3 axis) {
        Vector3 angle = Vector3.Scale(_minAngles, axis);
        return angle.x + angle.y + angle.z;
    }

    public float MaxAngle(Vector3 axis) {
        Vector3 angle = Vector3.Scale(_maxAngles, axis);
        return angle.x + angle.y + angle.z;
    }
    #endregion
}
