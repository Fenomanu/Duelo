using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WaterManager : MonoBehaviour
{
    //[SerializeField]
    //private float waterStrength;

    //Objects
    [SerializeField]
    private Transform waterLevel;
    private PlayerManagement p_Management;
    private Transform p_Transform;
    private CharacterController p_Controller;
    private Transform p_head_Transform;
    private bool inTP;
    private BoxCollider col;

    private bool player_on;
    private bool filter_on = false;
    private enum VEL_PHASES
    {
        V_SET,
        V_GET,
        V_STATIC
    }
    private VEL_PHASES velocity_phase;

    private float playerTime;
    private Vector3 prevPos;

    private Vector3 p_Velocity;
    private float p_Speed;


    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider>();
        WorldChange.Instance.SetWater(this); 
    }

    // Update is called once per frame
    void Update()
    {
        if (player_on)
        {
            switch (velocity_phase)
            {
                case VEL_PHASES.V_SET:
                    prevPos = p_Transform.position;
                    playerTime = Time.time;
                    p_Speed = .08f;
                    velocity_phase = VEL_PHASES.V_GET;
                    break;
                case VEL_PHASES.V_GET:
                    p_Management.NotUseGravity();
                    p_Velocity = (Time.deltaTime * (p_Transform.position - prevPos)) / (Time.time - playerTime);
                    if(!inTP) p_Controller.Move(p_Velocity);
                    if (p_Velocity.sqrMagnitude > .01f) p_Velocity = p_Velocity.normalized*.1f;

                    prevPos = p_Transform.position;
                    velocity_phase = VEL_PHASES.V_STATIC;
                    break;
                case VEL_PHASES.V_STATIC:

                    //p_Speed += .004f;
                    //if (p_Speed > .08f) p_Speed = .08f;
                    p_Velocity += Time.deltaTime * p_Speed * Vector3.up;

                    //Vector3 pos = (p_Transform.position - prevPos);
                    //pos.Set(pos.x, 0f, pos.z);
                    if (!inTP) p_Controller.Move(p_Velocity);
                    prevPos = p_Transform.position;
                    break;
            }

            if (p_head_Transform.position.y > waterLevel.position.y && filter_on)
            {
                FilterOff();
            }
            else if(p_head_Transform.position.y < waterLevel.position.y && !filter_on)
            {
                FilterOn();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            p_Management = other.GetComponent<PlayerManagement>();
            p_Controller = other.GetComponent<CharacterController>();
            p_head_Transform = p_Management.GetHead();
            p_Transform = p_Management.transform;
            

            player_on = true;
            velocity_phase = VEL_PHASES.V_SET;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player_on = false;
            p_Management.UseGravity();
            //p_Controller.Move(p_Velocity);
            if (filter_on)
            {
                FilterOff();
            }
        }
    }
    private void FilterOn()
    {
        p_Management.SetFilter(FilterColors.WATER);
        filter_on = true;
    }
    private void FilterOff()
    {
        p_Management.DelFilter();
        filter_on = false;
    }
    public void InTP()
    {
        inTP = true;
        col.enabled = false;
        if (player_on)
        {
            player_on = false;
            p_Management.UseGravity();
            //p_Controller.Move(p_Velocity);
            if (filter_on)
            {
                FilterOff();
            }
        }
    }
    public void NotInTP()
    {
        inTP = false;
        col.enabled = true;
    }
}
