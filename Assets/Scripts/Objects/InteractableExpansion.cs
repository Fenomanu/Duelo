using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableExpansion : MonoBehaviour
{
    private Vector3 initialP;
    private Quaternion initialR;
    private void Start()
    {
        initialP = transform.position;
        initialR = transform.rotation;
    }
    public void OnReset()
    {
        transform.position = initialP;
        transform.rotation = initialR;
    }
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Interactable"))
    //    {
    //        InteractableExpansion exp = other.GetComponent<InteractableExpansion>();
    //        exp.
    //    }
    //}
}
