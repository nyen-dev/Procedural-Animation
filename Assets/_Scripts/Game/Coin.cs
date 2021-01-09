using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Coin : MonoBehaviour
{
    #region EDITOR VALS
    [Header("Rotation")]
    [SerializeField] private Vector3 axis = Vector3.up;

    [SerializeField] private float angularVelocity = 0.7f;

    [Header("Linear Movement")]

    [SerializeField] private Vector3 endPosition = Vector3.zero;

    [SerializeField] private float lerpTime = 2f;

    [SerializeField] private float waitTime = 1f;
    #endregion

    #region REFS
    private Vector3 startPosition = Vector3.zero;
    #endregion

    #region EVENTS
    /// <summary> Called whenever a coin has been collected </summary>
    public Action OnPointAchieved;
    #endregion

    private void Start() 
    {
        startPosition = transform.position;
        StartCoroutine(SmoothLerpVector3(startPosition, endPosition, lerpTime, waitTime, (val) => transform.position = val));
        transform.localRotation *= Quaternion.Euler(axis * UnityEngine.Random.Range(-360, 361));
    }

    private void FixedUpdate()
    {
        Quaternion deltaRot = Quaternion.Euler(axis * angularVelocity * Mathf.Rad2Deg * Time.fixedDeltaTime);
        transform.localRotation *= deltaRot;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            OnPointAchieved?.Invoke();
            Destroy(gameObject);
        }
    }

    /// <summary>
	/// Smoothly adjustes a Vector to its targetValue over duration time
	/// </summary>
	/// <param name="initValue">Starting vector</param>
	/// <param name="targetValue">Final vector</param>
	/// <param name="duration">Lerp over given duration</param>
    /// <param name="waitTime">Waiting time between intervals</param>   
	/// <param name="setFloat">Function to set the vector</param>
	private IEnumerator SmoothLerpVector3(Vector3 initValue, Vector3 targetValue, float duration, float waitTime, Action<Vector3> setVector) {
        float startTime = Time.time;
        while (Time.time < startTime + duration) {
            setVector(Vector3.Lerp(initValue, targetValue, (Time.time - startTime) / duration));
            yield return null;
        }
        setVector(targetValue);

        //Recursion to make it periodic
        yield return new WaitForSecondsRealtime(waitTime);
        StartCoroutine(SmoothLerpVector3(targetValue, initValue, duration, waitTime, setVector)); 
    }
}
