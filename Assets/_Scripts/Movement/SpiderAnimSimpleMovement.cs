using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SpiderAnimSimpleMovement : SpiderAnimationMovement
{
    #region COMPONENTS
    private CharacterController characterController;
    #endregion

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        characterController = GetComponent<CharacterController>();
    }

    protected override void TransformRot(Quaternion rot) {
        transform.rotation = transform.rotation * rot;
    }

    protected override void TransformTransl(Vector3 transl) {
        characterController.Move(transl);
    }
}
