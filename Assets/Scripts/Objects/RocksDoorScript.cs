using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocksDoorScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponent<Animator>().SetBool("Close", true);
        }
    }
    public void NextLevel()
    {
        GameManager.Instance.NextLevel();
    }
}
