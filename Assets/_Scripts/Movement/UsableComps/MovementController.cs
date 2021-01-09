using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IMovement))]
[RequireComponent(typeof(IMovementInput))]
public class MovementController : MonoBehaviour
{
    #region EDITOR_VALS
    /// <summary>Controls the translation </summary>
    [SerializeField] private TransformationControl MovementControl = new TransformationControl();

    /// <summary>Controls the rotation </summary>
    [SerializeField] private TransformationControl RotationControl = new TransformationControl();
    #endregion

    #region COMPONENTS
    private IMovement _movement;
    private IMovementInput _movementInput;
    #endregion

    #region PROPERTIES
    public float LinearVelocity { get; private set; } = 0;

    public float AngularVelocity { get; private set; } = 0;
    #endregion

    #region EVENTS
    /// <summary>Event called when the player is on its back and cannot move</summary>
    static public Action OnPlayerStuck; 
    #endregion

    #region UNITY METHODS
    // Start is called before the first frame update
    void Start()
    {
        _movement = GetComponent<IMovement>();
        _movementInput = GetComponent<IMovementInput>();
    }

    void FixedUpdate()
    {
        float translationInput = _movementInput.TranslationInput();
        float rotationInput = _movementInput.RotationInput(); ;

        if (rotationInput != 0)
        {
            rotationInput *= AngularVelocity * Mathf.Rad2Deg * Time.fixedDeltaTime;            
            if (AngularVelocity < RotationControl.MaxVelocity) AngularVelocity += RotationControl.Acceleration;           
        }
        else
        {
            AngularVelocity = 0;;
        } 

        _movement.Rotate(Quaternion.Euler(0, rotationInput, 0));
        
        if (translationInput != 0)
        {
            translationInput *= Time.fixedDeltaTime; 
            // Dont add acceleration when moving backwards -> max -1ms backwards velocity
            if (translationInput > 0) translationInput *= LinearVelocity;           
            if (LinearVelocity < MovementControl.MaxVelocity) LinearVelocity += MovementControl.Acceleration;                
        }
        else
        {
            LinearVelocity = 0;          
        }   

        _movement.Translate(transform.forward * translationInput);        
    }

    void LateUpdate() 
    {
        if (transform.up.y <= 0 || transform.position.y < -1) {     
            OnPlayerStuck?.Invoke();
        }
    }
    #endregion
}
