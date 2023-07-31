using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttableRope : MonoBehaviour
{
    bool cut = false;
    public GenericAnimationAction action;
    private void OnTriggerEnter(Collider other)
    {
        if (cut) return;
        action.Action();
        cut = true;
    }
}
