using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : EnemyBase
{
    float angle = 0;
    float prevSpeed;
    float speed = (2 * Mathf.PI) / 10; //2*PI in degress is 360, so you get 5 seconds to complete a circle
    float radius = 5;
    float x, y;
    private Vector3 originalPos;
    private Vector3 dir;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        originalPos = transform.position;
        animator = GetComponent<Animator>();
    }
    void FixedUpdate()
    {
        if(invulnerable)
        {
            invTime -= Time.fixedDeltaTime;
            if(invTime <= 0)
            {
                speed = prevSpeed;
                invulnerable = false;
                animator.SetBool("Hit", false);
            }
            return;
        }

        angle += speed * Time.fixedDeltaTime; //if you want to switch direction, use -= instead of +=
        x = Mathf.Cos(angle) * radius;
        y = Mathf.Sin(angle) * radius;
        dir = speed * Time.fixedDeltaTime * ((originalPos + new Vector3(x, 0, y)) - transform.position).normalized;
        transform.position += dir;
        transform.LookAt(transform.position - dir);
    }

    public override bool DealDMG(int dmg)
    {
        if (invulnerable) return false;
        base.DealDMG(dmg);
        if(health > 0)
        {
            invulnerable = true;
            prevSpeed = speed;
            speed = 0;
            invTime = defInvTime;
            animator.SetBool("Hit", true);
        }
        else
        {
            speed = 0;
            invulnerable = true;
            invTime = 100;
            animator.SetBool("Dead", true);
        }
        return true;
    }

    public void Kill()
    {
        gameObject.SetActive(false);
    }
}
