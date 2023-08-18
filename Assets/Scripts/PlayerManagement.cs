using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerManagement : MonoBehaviour
{
    public ContinuousMoveProviderBase movement;
    private float prevSpeed = 0f;
    public Transform playerCenter; // Knife Storage
    public Transform r_hand;

    //Life System
    private float life = 3f;
    public int maxLife = 3;
    [SerializeField]
    private Material[] materials = new Material[4];
    public MeshRenderer lifeIndicator;

    protected bool invulnerable;
    public float defInvTime = .5f;
    protected float invTime;
    Animator animator;

    //FilterManager
    [Header("Filter Manager")]
    [SerializeField]
    private Color[] filters = new Color[2];
    [SerializeField]
    private Image filter;
    [SerializeField]
    private Transform head;

    [SerializeField]
    private LeftHandManager lHand;
    [SerializeField]
    private RightHandManager rHand;

    [Header("Power Bools")]
    [SerializeField]
    private bool dimSwap = false;
    [SerializeField]
    private bool remoteControl = false;
    [SerializeField]
    private bool shooting = false;
    [SerializeField]
    private bool updateBools = false;



    //Testing
    private bool isLevitating;
    CharacterController controller;
    [SerializeField]
    private float levSpeed;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        SetPowers();
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (updateBools)
        {
            updateBools = false;
            SetPowers();
        }
        if (invulnerable)
        {
            invTime -= Time.fixedDeltaTime;
            if (invTime <= 0)
            {
                invulnerable = false;
                animator.SetBool("Hit", false);
            }
            return;
        }
        if (Input.GetKey(KeyCode.L))
        {
            isLevitating = true;
            //if (isLevitating) controller.Move(Time.deltaTime * levSpeed * Vector3.up);
        }
        else
        {
            isLevitating = false;
        }
    }
    private void FixedUpdate()
    {
        if (isLevitating) controller.Move(Time.fixedDeltaTime * levSpeed * Vector3.up);
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
    public void UseGravity()
    {
        movement.useGravity = true;
    }

    public void NotUseGravity()
    {
        movement.useGravity = false;
    }

    public void Heal(float h)
    {
        life += h;
        if(life >= maxLife)
        {
            life = maxLife;
            int index = (life > materials.Length) ? materials.Length - 1 : (int)life - 1;
            lifeIndicator.material = materials[index];
        }
    }

    public void Damage(float d)
    {
        life -= d;
        if (life <= 0)
        {
            life = 0;
            animator.SetBool("Dead", true);
        }
        else
        {
            invulnerable = true;
            invTime = defInvTime;
            animator.SetBool("Hit", true);
        }
        int index = (life > materials.Length) ? materials.Length - 1 : (int)life - 1;
        lifeIndicator.material = materials[index];
    }
    public void Kill()
    {
        life -= life;
        if (life <= 0)
        {
            life = 0;
            animator.SetBool("Dead", true);
        }
    }

    public void EnablePowers()
    {
        dimSwap = true;
        remoteControl = true;
        shooting = true;
        SetPowers();
        rHand.SetGauntlet();
    }

    private void SetPowers()
    {
        lHand.RemoteEnabler(remoteControl);
        rHand.RemoteEnabler(remoteControl);
        rHand.ShootEnabler(shooting);
        rHand.SwapEnabler(dimSwap);
    }

    public void SetFilter(FilterColors color)
    {
        switch (color)
        {
            case FilterColors.FIRE:
                filter.color = filters[0];
                    break;
            case FilterColors.WATER:
                filter.color = filters[1];
                    break;
        }
        filter.enabled = true;
    }
    public void DelFilter()
    {
        filter.enabled = false;
    }
    public Transform GetHead()
    {
        return head;
    }
}