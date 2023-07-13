using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerManagement : MonoBehaviour
{
    ContinuousMoveProviderBase movement;
    private float prevSpeed = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BlockMovement()
    {
        prevSpeed = movement.moveSpeed;
        movement.moveSpeed = 0;
    }

    public void UnblockMovement()
    {
        movement.moveSpeed = prevSpeed;
    }
}
