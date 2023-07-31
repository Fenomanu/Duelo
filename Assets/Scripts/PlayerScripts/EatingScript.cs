using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatingScript : MonoBehaviour
{
    public PlayerManagement playerManagement;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            other.transform.parent.gameObject.SetActive(false);
            Heal();
        }
    }
    void Heal()
    {
        playerManagement.Heal(1);
    }
}
