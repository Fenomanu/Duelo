using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartMovement : MonoBehaviour
{
    Animator animator;
    bool go = true;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void Action()
    {
        go = !go;
        animator.SetBool("Go", go);
        animator.SetTrigger("Action");
    }
}
