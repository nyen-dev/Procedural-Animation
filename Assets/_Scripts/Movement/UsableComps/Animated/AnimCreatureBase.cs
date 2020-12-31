using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class AnimCreatureBase : MonoBehaviour, IMovement
{
    #region COMPONENTS
    private Animator _spiderAnimator;
    #endregion

    #region UNITY METHODS
    // Start is called before the first frame update
    protected virtual void Start()
    {  
        _spiderAnimator = GetComponent<Animator>();
    }
    #endregion

    #region IMovement
    public void Rotate(Quaternion rot)
    {     
        TransformRot(rot);

        float angularVelocity = rot.eulerAngles.y * Mathf.Deg2Rad / Time.fixedDeltaTime;
        _spiderAnimator.SetFloat("Angular Velocity", angularVelocity);      
    }

    public void Translate(Vector3 transl)
    {
        TransformTransl(transl);

        float linearVelocity = transl.magnitude / Time.fixedDeltaTime;
        if (linearVelocity > 0) _spiderAnimator.SetBool("Is Walking", true);
        else _spiderAnimator.SetBool("Is Walking", false);
        _spiderAnimator.SetFloat("Linear Velocity", linearVelocity);      
    }
    #endregion

    protected abstract void TransformRot(Quaternion rot);

    protected abstract void TransformTransl(Vector3 transl);
}
