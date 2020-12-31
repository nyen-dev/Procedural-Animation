using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CreatureAnimPhysics : AnimCreatureBase
{
    #region COMPONENTS
    private Rigidbody _rb;
    #endregion

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _rb = GetComponent<Rigidbody>();               
    }

    #region AnimCreatureBase
    protected override void TransformRot(Quaternion rot)
    {
        _rb.MoveRotation(transform.rotation * rot);
    }

    protected override void TransformTransl(Vector3 transl)
    {
        _rb.MovePosition(transform.position + transl);
    }
    #endregion
}