using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDetection : MonoBehaviour
{

    public PlayerManagement playerManagement;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            print("EnemyHitting");
            playerManagement.Damage(1);
        }
    }
}
