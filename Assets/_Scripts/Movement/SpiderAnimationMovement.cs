using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class SpiderAnimationMovement : MonoBehaviour, IMovement
{
    #region COMPONENTS
    private CharacterController characterController;

    private Animator spiderAnimator;

    private MovementController movementController;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        spiderAnimator = GetComponent<Animator>();
        movementController = GetComponent<MovementController>();
    }

    public void Rotate(Quaternion rot)
    {
        transform.rotation = transform.rotation * rot;
        float angularVelocity = rot.eulerAngles.y * Mathf.Deg2Rad / Time.fixedDeltaTime;
        spiderAnimator.SetFloat("Angular Velocity", angularVelocity);
    }

    public void Translate(Vector3 transl)
    {
        characterController.Move(transl);
        float linearVelocity = transl.magnitude / Time.fixedDeltaTime;
        if (linearVelocity > 0) spiderAnimator.SetBool("Is Walking", true);
        else spiderAnimator.SetBool("Is Walking", false);
        spiderAnimator.SetFloat("Linear Velocity", linearVelocity);
    }
}
