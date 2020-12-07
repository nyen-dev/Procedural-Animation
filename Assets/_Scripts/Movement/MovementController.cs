using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IMovement))]
[RequireComponent(typeof(IMovementInput))]
public class MovementController : MonoBehaviour
{
    #region EDITOR_VALS
    /// <summary>Controls the translation </summary>
    [SerializeField] private TransformationControl movementControl = new TransformationControl();

    /// <summary>Controls the rotation </summary>
    [SerializeField] private TransformationControl rotationControl = new TransformationControl();
    #endregion

    #region COMPONENTS
    private IMovement movement;
    private IMovementInput movementInput;
    #endregion

    public float LinearVelocity { get; private set; } = 0;

    public float AngularVelocity { get; private set; } = 0;

    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<IMovement>();
        movementInput = GetComponent<IMovementInput>();
    }

    void FixedUpdate()
    {
        float translationInput = movementInput.TranslationInput();
        float rotationInput = movementInput.RotationInput(); ;

        if (rotationInput != 0)
        {
            rotationInput *= AngularVelocity * Mathf.Rad2Deg * Time.fixedDeltaTime;            
            if (AngularVelocity < rotationControl.maxVelocity) AngularVelocity += rotationControl.acceleration;           
        }
        else
        {
            AngularVelocity = 0;;
        }

        movement.Rotate(Quaternion.Euler(0, rotationInput, 0));

        if (translationInput != 0)
        {
            translationInput *= Time.fixedDeltaTime; //Dont add acceleration when moving backwards
            if (translationInput > 0) translationInput *= LinearVelocity;           
            if (LinearVelocity < movementControl.maxVelocity) LinearVelocity += movementControl.acceleration;           
        }
        else
        {
            LinearVelocity = 0;          
        }

        movement.Translate(transform.forward * translationInput);
    }
}
