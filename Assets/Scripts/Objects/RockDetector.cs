using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockDetector : MonoBehaviour
{
    [SerializeField]
    private MovingBridge bridge;
    [SerializeField]
    private SIDE side;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            bridge.ROP(OPS.ADD, side);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            bridge.ROP(OPS.SUB, side);
        }
    }
}
