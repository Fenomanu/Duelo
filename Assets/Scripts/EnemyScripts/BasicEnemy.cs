using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : EnemyBase
{
    float angle = 0;
    float prevSpeed;
    float speed = (2 * Mathf.PI) / 5; //2*PI in degress is 360, so you get 5 seconds to complete a circle
    float radius = 5;
    float x, y;
    private Vector3 originalPos;
    private Vector3 dir;
    private Animator animator;

    // Player Following
    private Transform player;
    private enum FollowState
    {
        Follow = 0,
        Attack = 1,
        Turn = 2
    }
    FollowState curState = FollowState.Follow;
    private bool followPlayer;
    private bool rotLeft;

    // Start is called before the first frame update
    void Start()
    {
        originalPos = transform.position;
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            player = GameManager.Instance.player.playerCenter;
            followPlayer = true;
        }
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

        if (!followPlayer)
        {
            angle += speed * Time.fixedDeltaTime; //if you want to switch direction, use -= instead of +=
            x = Mathf.Cos(angle) * radius;
            y = Mathf.Sin(angle) * radius;
            dir = speed * Time.fixedDeltaTime * ((originalPos + new Vector3(x, 0, y)) - transform.position).normalized;
            transform.position += dir;
            transform.LookAt(transform.position - dir);
        }
        else
        {
            switch (curState)
            {
                case FollowState.Follow:
                    dir = speed * Time.fixedDeltaTime * (new Vector3(player.position.x - transform.position.x, 0f, player.position.z - transform.position.z)).normalized;
                    transform.position += dir;
                    transform.LookAt(transform.position - dir);
                    if ((transform.position - player.position).sqrMagnitude < 9)
                    {
                        //invulnerable = false;
                        prevSpeed = speed;
                        dir *= 1.5f;
                        //invTime = defInvTime * 10;
                        curState = FollowState.Attack;
                        animator.SetBool("Attack", true);
                    }
                    break;
                case FollowState.Attack:
                    transform.position += dir;
                    transform.LookAt(transform.position - dir);
                    break;
                case FollowState.Turn:
                    dir = speed * Time.fixedDeltaTime * (new Vector3(player.position.x - transform.position.x, 0f, player.position.z - transform.position.z)).normalized;
                    if (rotLeft)
                    {
                        transform.Rotate(Vector3.up, Time.fixedDeltaTime * -80);
                    }
                    else
                    {
                        transform.Rotate(Vector3.up, Time.fixedDeltaTime * 80);
                    }
                    //float angle = ;
                    //print(angle);
                    if (Vector3.Angle(dir, -transform.forward) < 5f)
                    {
                        curState = FollowState.Follow;
                    }
                    break;
            }
        }
    }
    float ABS(float f)
    {
        return f < 0 ? -f : f;
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
        player = GameManager.Instance.player.playerCenter;
        followPlayer = true;
        return true;
    }

    public void StopAttacking()
    {
        curState = FollowState.Turn;
        animator.SetBool("Attack", false);
        speed = prevSpeed; 
        dir = new Vector3(player.position.x - transform.position.x, 0f, player.position.z - transform.position.z);
        rotLeft = Vector3.Dot(dir, transform.right) >= 0 ? true : false;
        if (rotLeft) print("Rot Left"); else print("Rot Right"); 
    }

    public void Kill()
    {
        gameObject.SetActive(false);
    }
}
