using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class RightHandManager : HandManager
{
    //Limits
    private bool enabledDimSwap = true;

    //Actions
    public InputActionProperty worldAction;

    private float triggerValue;
    private bool pButtonValue;
    
    //Shooting
    public Transform shootPoint;
    public GameObject bulletPrefab;
    public float cadence = 0.2f;
    private float delay = 0f;
    private bool thrown = false;

    //Object Management
    private XRDirectInteractor handInteractor;
    private XRInteractionManager interactionManager;
    BodyMap bodyMap;
    private GameObject grabbedObj;


    //Hand UI
    HandUI handUI;

    //Remote Control
    private bool startRemote;
    private bool remoting;
    private const int framesTillRemote = 5;
    private int frames_remote = 0;
    private enum RemoteState
    {
        startRemote = 0,
        hooking = 1,
        hooked = 2,
    }
    RemoteState rem_st;


    // Start is called before the first frame update
    void Start()
    {
        handInteractor = GetComponent<XRDirectInteractor>();
        bodyMap = GetComponentInParent<BodyMap>();
        interactionManager = handInteractor.interactionManager;
        handUI = GetComponent<HandUI>();
        //remoteSphere.GetComponent<SphereDetection>().hm = this;
    }
    private void Awake()
    {
        print("Also B Awake");
        GrabAction.action.started += (context) => {
            startRemote = true;
        };
        GrabAction.action.canceled += (context) => {
            StopRemote();
            if (grabbedObj == null) GetComponent<SphereCollider>().enabled = true; 
        };
    }

    // Update is called once per frame
    void Update()
    {
        RemoteControl();
        triggerValue = shootAction.action.ReadValue<float>();
        pButtonValue = worldAction.action.triggered;
        if (delay > 0) delay -= Time.deltaTime;
        if(delay <= 0 && triggerValue > .5f)
        {
            if(grabbedObj == null)
            {
                GameObject bullet = Instantiate(bulletPrefab, shootPoint.position - .2f*shootPoint.up, shootPoint.rotation);
                bullet.GetComponent<Rigidbody>().velocity = -shootPoint.up * 8;
                Destroy(bullet, 3);
                delay = cadence;
            }
            else if (grabbedObj != null && !thrown)
            {
                thrown = true;
                delay = cadence;
                grabbedObj.GetComponent<XRGrabInteractable>().throwOnDetach = false;
                interactionManager.SelectCancel((IXRSelectInteractor)handInteractor, grabbedObj.GetComponent<XRGrabInteractable>());
                GetComponent<SphereCollider>().enabled = false;
                //grabbedObj.GetComponent<XRGrabInteractable>().interactionManager.SelectCancel(handInteractor, grabbedObj.GetComponent<XRGrabInteractable>()) = false;
                //grabbedObj.GetComponent<Collider>().enabled = false;
                //WorldChange.Instance.DropObj(grabbedObj);
                //grabbedObj = null;
                //thrown = true;
            }
        }
        if (pButtonValue && enabledDimSwap)
        {
            print("Changing");
            if(!WorldChange.Instance.SwitchWorld()) handUI.ShowUISeconds(1.8f);
        }
    }

    private void RemoteControl()
    {   //Called every frame
        if (startRemote)
        {
            frames_remote++;
            if (frames_remote >= framesTillRemote)
            {

            }
        }
        else if (remoting)
        {

        }
    }
    private void StopRemote()
    {
        startRemote = false;
        frames_remote = 0;
    }

    public void GrabObj()
    {
        StopRemote();
        //print(handInteractor.GetOldestInteractableSelected());
        grabbedObj = handInteractor.GetOldestInteractableSelected().transform.gameObject;
        grabbedObj.GetComponent<XRGrabInteractable>().throwOnDetach = true;
        grabbedObj.GetComponent<Rigidbody>().useGravity = true;
        print("Grabbed " + grabbedObj.tag);
        switch (grabbedObj.tag)
        {
            case "Knife":
                bodyMap.PullKnife();
                break;
            case "Interactable":
                WorldChange.Instance.GrabObj(grabbedObj);
                break;
        }
    }

    public void DropObj()
    {
        print(grabbedObj);
        //GameObject grabbedObj = handInteractor.GetOldestInteractableSelected().transform.gameObject;
        print("Dropped " + grabbedObj.tag);
        string tag = grabbedObj.tag;
        switch (tag)
        {
            case "Knife":
                bodyMap.StoreKnife(grabbedObj.transform, thrown);
                break;
            case "Interactable":
                WorldChange.Instance.DropObj(grabbedObj);
                break;
        }
        if(thrown)
        {
            print("Throwing");

            switch (tag)
            {
                case "Knife":
                    grabbedObj.GetComponent<Rigidbody>().velocity = -shootPoint.forward * 10; 
                    break;
                case "Interactable":
                    grabbedObj.GetComponent<Rigidbody>().velocity = -shootPoint.right * 10; 
                    break;
            }
            //grabbedObj.GetComponent<Rigidbody>().angularVelocity = Vector3.up * 10;
            //grabbedObj.GetComponent<XRGrabInteractable>().throwOnDetach = true;
            thrown = false;
        }
        grabbedObj = null;
    }
}
