using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformElevator : MonoBehaviour
{
    [SerializeField]
    private Transform _transform;
    [SerializeField]
    private Rigidbody _rigidbody;
    [SerializeField]
    private float speed;
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _transform = transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += Time.fixedDeltaTime * speed * Vector3.up ;
    }
}
