using System.Collections.Generic;
using UnityEngine;

public class AxisMovementInput : MonoBehaviour, IMovementInput
{
    public float TranslationInput()
    {
        return Input.GetAxis("Vertical");
    }

    public float RotationInput()
    {
        return Input.GetAxis("Horizontal");
    }
}
