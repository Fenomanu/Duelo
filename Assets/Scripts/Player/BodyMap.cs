using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Maps Knife relative to point
[System.Serializable]
public class VrMap
{
    public Transform vrTarget;
    public Transform rigTarget;
    public Vector3 trackingPositonOffset;
    public Vector3 trackingRotationOffset;
    
    public void Map()
    {
        rigTarget.position = vrTarget.TransformPoint(trackingPositonOffset);
        rigTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
    }
}

public class BodyMap : MonoBehaviour
{
    public float timeToStore;
    private float timeFreeLeft;
    public Transform knifeStorage;
    public Transform knifePoint;
    public Transform knifeParent;
    public Transform cameraOff;
    public Vector3 cameraOffset;
    private Vector3 KnifeObj;
    private KnifeScript knifeScript;
    private bool kSStored;
    private bool storeKnife = true;
    private bool mapKnife = true;

    public VrMap knife;
    // Start is called before the first frame update
    void Start()
    {
        cameraOffset = knifeStorage.position - cameraOff.position;
        //print(cameraOffset);
    }

    // Update is called once per frame
    void Update()
    {
        if (storeKnife && mapKnife && !kSStored)
        {
            knifeScript.stored = true;
            kSStored = true;
        }
        else if ((!storeKnife || !mapKnife) && kSStored)
        {
            knifeScript.stored = false;
            kSStored = false;
        }

    }
    private void LateUpdate()
    {
        knifeStorage.position = cameraOff.position + cameraOffset;
        //knifeStorage.forward = Vector3.ProjectOnPlane(cameraOff.forward, Vector3.up).normalized;
        //print(cameraOff.name);
        Vector3 dir = Vector3.ProjectOnPlane(cameraOff.up, Vector3.up).normalized;
        if (dir != Vector3.zero) knifeStorage.forward = dir;
        if (storeKnife)
        {
            if (mapKnife)
            {
                knife.Map();
            }
            else
            {
                timeFreeLeft -= Time.deltaTime;
                if(timeFreeLeft <= 0)
                {
                    StartMap();
                }
            }
        }   
    }

    public void StoreKnife(Transform knifeT, bool thrown)
    {
        knifeT.parent = null;
        if (!thrown && (knifePoint.position - knifeT.position).sqrMagnitude < .01f)
        {
            StartMap();
            print("Storing = " + ((knifePoint.position - knifeT.position).sqrMagnitude));
        }
        else
        {
            mapKnife = false;
            //print("Dropping");
            //print(knifeT.position);
            //print("In");
            //print(knifeStorage.position + knife.trackingPositonOffset);
            timeFreeLeft = timeToStore;
        }
        storeKnife = true;
    }
    public void PullKnife()
    {
        storeKnife = false;
    }
    private void StartMap()
    {
        mapKnife = true;

        knife.rigTarget.parent = knifeParent;
    }

    public void SetKnifeScript(KnifeScript ks)
    {
        knifeScript = ks;
        knifeScript.stored = true;
    }
}