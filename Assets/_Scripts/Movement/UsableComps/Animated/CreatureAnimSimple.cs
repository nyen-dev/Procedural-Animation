using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CreatureAnimSimple : AnimCreatureBase
{
    #region COMPONENTS
    private CharacterController _characterController;
    #endregion

    #region UNITY METHODS
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _characterController = GetComponent<CharacterController>();
    }
    #endregion

    #region AnimCreatureBase
    protected override void TransformRot(Quaternion rot) {
        transform.rotation = transform.rotation * rot;
    }

    protected override void TransformTransl(Vector3 transl) {
        _characterController.Move(transl);
    }
    #endregion
}
