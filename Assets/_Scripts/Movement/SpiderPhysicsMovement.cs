using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SpiderPhysicsMovement : MonoBehaviour, IMovement
{
    #region COMPONENTS
    private Rigidbody rb;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();        
    }

    public void Rotate(Quaternion rot)
    {
        rb.MoveRotation(transform.rotation * rot);
    }

    public void Translate(Vector3 transl)
    {
        rb.MovePosition(transform.position + transl);
    }
}