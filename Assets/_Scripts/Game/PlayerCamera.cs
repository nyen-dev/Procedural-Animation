using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    #region EDITOR VALS
    /// <summary> Target to follow and look around </summary>
    [SerializeField] private Transform Target = null;

    /// <summary> Rotationspeed X-axis </summary>
    [SerializeField] private float VertSpeed = 1f;

    /// <summary> Rotationspeed Y-axis</summary>
    [SerializeField] private float HorizSpeed = 1f;

    /// <summary> Mouserotation constraint X-axis </summary>
    [SerializeField] private Vector2 XAngleLimits = Vector2.zero;

    /// <summary> Mouserotation constraint Y-axis </summary>
    [SerializeField] private Vector2 YAngleLimits = Vector2.zero;
    #endregion

    #region REFS
    /// <summary> Offset to the target </summary>
    private Vector3 _offset;

    /// <summary> Store the mouserotation </summary>
    private Quaternion _mouseRotation = Quaternion.identity;

    /// <summary> Initial rotation </summary>
    private Quaternion _startRotation;
    #endregion

    void Start() {
        _offset = transform.position - Target.position;
        _startRotation = transform.rotation;
        if (Target == null) Target = GameObject.FindWithTag("Player").transform;
    }

    void Update() {
        // Mouse input
        float horizontal = Input.GetAxis("Mouse X") * HorizSpeed;
        float vertical = Input.GetAxis("Mouse Y") * VertSpeed;

        // Store rotation
        _mouseRotation *= Quaternion.Euler(vertical, horizontal, 0);

        // Mouserotation constraints
        Vector3 angles = (Target.rotation * _startRotation * _mouseRotation).eulerAngles; 
        angles.x = Mathf.Clamp(angles.x > 180 ? angles.x - 360 : angles.x, XAngleLimits.x, XAngleLimits.y); 
        angles.y = Mathf.Clamp(angles.y > 180 ? angles.y - 360 : angles.y, YAngleLimits.x, YAngleLimits.y);
        angles.z = 0; 

        // New position and rotation
        transform.eulerAngles = angles;
        Quaternion deltaRot = transform.rotation * Quaternion.Inverse(_startRotation);
        transform.position = Target.position + deltaRot * _offset;
    }
}
