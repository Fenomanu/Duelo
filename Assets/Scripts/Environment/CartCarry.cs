using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartCarry : MonoBehaviour
{
    private bool started;
    private Transform player;
    private Transform _transform;
    private Vector3 lastPos = Vector3.zero;
    private void Start()
    {
        _transform = transform;
    }
    private void FixedUpdate()
    {
        if (started && player != null)
        {
            //player.GetComponent<CharacterController>().Move(_transform.position - lastPos);
            player.position += _transform.position - lastPos;
            lastPos = _transform.position;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            started = true;
            player = other.transform;
            lastPos = _transform.position;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            started = false;
            player = null;
        }
    }
}
