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
    public Material[] materials = new Material[4];
    public MeshRenderer lifeIndicator;

    protected bool invulnerable;
    public float defInvTime = .5f;
    protected float invTime;
    Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

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
        
    }
}
