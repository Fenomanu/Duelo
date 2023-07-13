using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeScript : MonoBehaviour
{
    public float speedToDmg;
    private float sqrSpeed;
    public bool stored;
    public Transform bladeP;
    private Vector3 lastPos;
    private BoxCollider col;
    public AudioSource audioSource;
    private BodyMap bodyMap;
    public TrailRenderer trail;
    // Start is called before the first frame update
    void Start()
    {
        GetComponentInParent<BodyMap>().SetKnifeScript(this);
        lastPos = bladeP.position;
        sqrSpeed = speedToDmg * speedToDmg;
        col = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void FixedUpdate()
    {
        sqrSpeed = speedToDmg * speedToDmg;
        if (stored) return;
        //print((lastPos - transform.position).sqrMagnitude / Time.deltaTime);
        float dist = (lastPos - transform.position).sqrMagnitude;
        float vel = dist / Time.fixedDeltaTime;
        if (vel > sqrSpeed)
        {
            //print("Fast Enough = " + vel + " With dist = " + dist);
            if (!col.enabled)
            {
                col.enabled = true;
                //print("Hitting");
                audioSource.Play();
                trail.emitting = true;
            }
        }
        else if (vel <= sqrSpeed && col.enabled)
        {
            //print("Not Fast Enough = " + vel + " With dist = " + dist);
            //Debug.Break();
            trail.emitting = false;
            col.enabled = false;
            audioSource.Pause();
        }
        lastPos = transform.position;
    }
}
