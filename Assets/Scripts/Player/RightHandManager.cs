using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class RightHandManager : HandManager
{
    //Limits
    private bool remoteControlEnabled = false;
    private bool enabledDimSwap = false;
    private bool shootingEnabled = false;

    //Actions
    public InputActionProperty worldAction;

    //Inputs
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
    [SerializeField]
    private float handRemoteMax;
    private bool startHolding;
    public float remotingSpeed;
    private Vector3 lastHandPos;
    private Vector3 sphereDirection;
    public Transform remoteSphere;
    public float timeTillRemote = 5;
    private float time_holding = 0;
    private float sphereScale = .3f;
    private enum RemoteState
    {
        Idle = 0,
        Holding = 1,
        Throwing = 2,
        Hooked = 3,
    }
    RemoteState rem_st = RemoteState.Idle;


    //Visuals
    [SerializeField]
    private MeshRenderer hand;
    [SerializeField]
    private GameObject gauntlet;

    // Start is called before the first frame update
    void Start()
    {
        handInteractor = GetComponent<XRDirectInteractor>();
        bodyMap = GetComponentInParent<BodyMap>();
        interactionManager = handInteractor.interactionManager;
        remoteSphere.GetComponent<SphereDetection>().hm = this;
        handUI = GetComponent<HandUI>();
        sphereScale = remoteSphere.localScale.x;
        //remoteSphere.GetComponent<SphereDetection>().hm = this;
    }
    private void Awake()
    {
        //print("Also B Awake");
        GrabAction.action.started += (context) => {
            startHolding = true;
            //print("StartedHolding");
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
        if(delay <= 0 && (triggerValue > .5f || Input.GetButtonDown("Fire1")))
        {
            if(grabbedObj == null && shootingEnabled)
            {
                GameObject bullet = Instantiate(bulletPrefab, shootPoint.position - .2f*shootPoint.up, shootPoint.rotation);
                bullet.GetComponent<Rigidbody>().velocity = -shootPoint.up * 16;
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
            WC_RES res = WorldChange.Instance.SwitchWorld();
            switch(res){
                case WC_RES.OBSTRUCTION:
                    handUI.ShowUISeconds(1.8f, res);
                    break;
                case WC_RES.SWITCHING:
                    break;
                case WC_RES.TIMEOUT:
                    handUI.ShowUISeconds(1f, res);
                    break;
            }
        }
    }

    private void RemoteControl()
    {   //Called every frame
        switch (rem_st)
        {
            case RemoteState.Idle:
                if (startHolding && remoteControlEnabled)
                {
                    print("Starting");
                    time_holding += Time.deltaTime;
                    if (time_holding >= timeTillRemote)
                    {
                        print("started");
                        rem_st = RemoteState.Holding;
                        remoteSphere.gameObject.SetActive(true);
                        remoteSphere.position = shootPoint.position;
                        sphereDirection = -shootPoint.up;
                        lastHandPos = transform.position;
                        remoteSphere.transform.parent = null;
                        //remoteSphere.localScale = Vector3.zero;
                        remoteSphere.localScale = Vector3.one * .05f;
                    }
                }
                break;
            case RemoteState.Holding:
                //remoteSphere.position += remotingSpeed * Time.deltaTime * (sphereDirection + (transform.position - lastHandPos));
                remoteSphere.position += remotingSpeed * Time.deltaTime * (sphereDirection - (shootPoint.up));
                if (remoteSphere.localScale.x < sphereScale) 
                {
                    remoteSphere.localScale += Time.deltaTime * 3 * Vector3.one;
                    if (remoteSphere.localScale.x > sphereScale) remoteSphere.localScale = sphereScale * Vector3.one;
                }
                if (remotingObject != null)
                {
                    lastHandPos = transform.position;
                    remoteSphere.gameObject.SetActive(false);
                    rem_st = RemoteState.Throwing;
                }
                break;
            case RemoteState.Throwing:
                //remotingObject.GetComponent<Rigidbody>().velocity += remotingSpeed * Time.deltaTime * (transform.position - lastHandPos);
                //float dist = (transform.position - lastHandPos).sqrMagnitude;
                if ((transform.position - lastHandPos).sqrMagnitude > handRemoteMax * handRemoteMax) lastHandPos = transform.position + (lastHandPos - transform.position).normalized * handRemoteMax;
                remotingObject.transform.position += remotingSpeed * 1.5f * Time.deltaTime * (transform.position - lastHandPos);
                if ((remotingObject.transform.position - transform.position).sqrMagnitude > 30 * 30)
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
    public void RemoteEnabler(bool e)
    {
        remoteControlEnabled = e;
    }
    public void SwapEnabler(bool e)
    {
        enabledDimSwap = e;
    }
    public void ShootEnabler(bool e)
    {
        shootingEnabled = e;
    }
    public void SetGauntlet()
    {
        hand.enabled = false;
        gauntlet.SetActive(true);
    }
}
