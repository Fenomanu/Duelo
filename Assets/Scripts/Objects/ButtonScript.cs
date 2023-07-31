using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public CartMovement mov;
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.V))
    //    {
    //        mov.Action();
    //    }
    //}
    private void OnTriggerEnter(Collider other)
    {
        mov.Action();
    }
}
