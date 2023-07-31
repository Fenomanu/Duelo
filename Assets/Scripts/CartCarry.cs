using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartCarry : MonoBehaviour
{
    private bool started;
    private Vector3 lastPos = Vector3.zero;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            started = true;
            lastPos = transform.position;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            started = false;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (started)
        {
            other.transform.position += transform.position - lastPos;
            lastPos = transform.position;
        }
    }
}
