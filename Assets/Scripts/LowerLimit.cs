using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowerLimit : MonoBehaviour
{
    [SerializeField]
    private Transform spawnPoint;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            InteractableExpansion exp = other.GetComponent<InteractableExpansion>();
            if(exp != null) exp.OnReset();
        }
        else if (other.CompareTag("Player"))
        {
            other.transform.position = spawnPoint.position;
        }
    }
}
