using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joint : MonoBehaviour
{
    #region MEMBERS
    [SerializeField] private Vector3 m_rotationAxis;

    [SerializeField] private float m_minAngle;

    [SerializeField] private float m_maxAngle;
    #endregion

    #region PROPERTIES
    public Vector3 RestingPosition { get; private set; }

    public Vector3 RotationAxis { get { return m_rotationAxis; } }

    public float MinAngle { get { return m_minAngle; }}

    public float MaxAngle { get { return m_maxAngle; } }
    #endregion
    
    void Awake()
    {
        RestingPosition = transform.localPosition;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
