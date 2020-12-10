using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SpiderAnimPhysicsMovement : SpiderAnimationMovement
{
    #region COMPONENTS
    private Rigidbody rb;
    #endregion

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();               
    }

    protected override void TransformRot(Quaternion rot)
    {
        rb.MoveRotation(transform.rotation * rot);
    }

    protected override void TransformTransl(Vector3 transl)
    {
        rb.MovePosition(transform.position + transl);
    }
}