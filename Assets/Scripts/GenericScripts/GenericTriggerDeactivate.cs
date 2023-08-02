using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericTriggerDeactivate : MonoBehaviour
{
    public GameObject _object;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) _object.SetActive(false);
    }
}
