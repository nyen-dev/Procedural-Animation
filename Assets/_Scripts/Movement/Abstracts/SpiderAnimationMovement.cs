using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class SpiderAnimationMovement : MonoBehaviour, IMovement
{
    #region COMPONENTS
    private Animator spiderAnimator;
    #endregion

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
        spiderAnimator = GetComponent<Animator>();
    }

    public void Rotate(Quaternion rot)
    {       
        TransformRot(rot);
        float angularVelocity = rot.eulerAngles.y * Mathf.Deg2Rad / Time.fixedDeltaTime;
        spiderAnimator.SetFloat("Angular Velocity", angularVelocity);
    }

    public void Translate(Vector3 transl)
    {
        TransformTransl(transl);
        float linearVelocity = transl.magnitude / Time.fixedDeltaTime;
        if (linearVelocity > 0) spiderAnimator.SetBool("Is Walking", true);
        else spiderAnimator.SetBool("Is Walking", false);
        spiderAnimator.SetFloat("Linear Velocity", linearVelocity);
    }

    protected abstract void TransformRot(Quaternion rot);

    protected abstract void TransformTransl(Vector3 transl);
}
