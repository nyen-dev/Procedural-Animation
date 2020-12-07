using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joint : MonoBehaviour
{
    private Vector3 offset;

    [SerializeField] private Quaternion maxRotation;

    [SerializeField] private Quaternion minRotation;

    void Awake()
    {
        offset = transform.localPosition;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 getOffset() { return offset; }
}
