using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LeftHandManager : HandManager
{

    //Object Management
    private XRDirectInteractor handInteractor;
    private XRInteractionManager interactionManager;
    BodyMap bodyMap;
    private GameObject grabbedObj;
    public Transform shootPoint;
    private bool thrown = false;

    private float triggerValue;

    public float cadence = 0.2f;
    private float delay = 0f;


    //Remote Control
    private bool startHolding;
    public float remotingSpeed;
    private Vector3 lastHandPos;
    private Vector3 sphereDirection;
    public Transform remoteSphere;
    public float timeTillRemote = 5;
    private float time_holding = 0;
    private enum RemoteState
    {
        Idle = 0,
        Holding = 1,
        Throwing = 2,
        Hooked = 3,
    }
    RemoteState rem_st = RemoteState.Idle;

    // Start is called before the first frame update
    void Start()
    {
        handInteractor = GetComponent<XRDirectInteractor>();
        bodyMap = GetComponentInParent<BodyMap>();
        interactionManager = handInteractor.interactionManager;
        remoteSphere.GetComponent<SphereDetection>().hm = this;
    }

    private void Awake()
    {
        print("Also B Awake");
        GrabAction.action.started += (context) => {
            startHolding = true;
            print("StartedHolding");
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
        if (delay > 0) delay -= Time.deltaTime;
        if (delay <= 0 && triggerValue > .5f)
        {
            if (grabbedObj != null && !thrown)
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
    }
    private void RemoteControl()
    {   //Called every frame
        switch (rem_st)
        {
            case RemoteState.Idle:
                if (startHolding)
                {
                    print("Starting");
                    time_holding += Time.deltaTime;
                    if (time_holding >= timeTillRemote)
                    {
                        print("started");
                        rem_st = RemoteState.Holding;
                        remoteSphere.gameObject.SetActive(true);
                        remoteSphere.position = shootPoint.position;
                        sphereDirection = shootPoint.up;
                        lastHandPos = transform.position;
                        remoteSphere.transform.parent = null;
                    }
                }
                break;
            case RemoteState.Holding:
                //remoteSphere.position += remotingSpeed * Time.deltaTime * (sphereDirection + (transform.position - lastHandPos));
                remoteSphere.position += remotingSpeed * Time.deltaTime * (sphereDirection + (shootPoint.up));
                if(remotingObject != null)
                {
                    lastHandPos = transform.position;
                    remoteSphere.gameObject.SetActive(false);
                    rem_st = RemoteState.Throwing;
                }
                break;
            case RemoteState.Throwing:
                //remotingObject.GetComponent<Rigidbody>().velocity += remotingSpeed * Time.deltaTime * (transform.position - lastHandPos);
                remotingObject.transform.position += remotingSpeed * 1.5f * Time.deltaTime * (transform.position - lastHandPos);
                if((remotingObject.transform.position - transform.position).sqrMagnitude > 30 * 30)
                {
                    StopRemote();
                }
                //lastHandPos = transform.position;
                break;
            case RemoteState.Hooked:

                break;
        }
    }
    private void StopRemote()
    {
        if (remotingObject != null)
        {
            remotingObject.GetComponent<Rigidbody>().useGravity = true;
            remotingObject.GetComponent<Rigidbody>().velocity = (transform.position - lastHandPos) * remotingSpeed;
        }
        remotingObject = null;
        rem_st = RemoteState.Idle;
        remoteSphere.gameObject.SetActive(false);
        startHolding = false;
        time_holding = 0f;
        print("Break");
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
        if (thrown)
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
