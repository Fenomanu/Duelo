using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public Animator animator;
    private BoxCollider keyholeCol;
    // Start is called before the first frame update
    void Start()
    {
        keyholeCol = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Key"))
        {
            other.transform.parent.gameObject.SetActive(false);
            keyholeCol.enabled = false;
            animator.SetBool("Open", true);
        }
    }
}
