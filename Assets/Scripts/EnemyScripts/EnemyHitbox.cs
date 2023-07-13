using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    public AudioSource audioSourceHit;
    private void Update()
    {
    }
    private void OnTriggerEnter(Collider other)
    {
        audioSourceHit.Play();
        print("Ouch");
        if (GetComponentInParent<EnemyBase>().DealDMG(1))
        {
            audioSourceHit.Play();
        }
    }
}
