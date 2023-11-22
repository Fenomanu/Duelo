using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class HandManager : MonoBehaviour
{
    public InputActionProperty GrabAction;
    public InputActionProperty shootAction;
    public GameObject remotingObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Awake()
    {
        print("Also A Awake");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
