using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public int health;
    protected bool invulnerable;
    public float defInvTime;
    protected float invTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual bool DealDMG(int dmg)
    {
        health -= dmg;
        if (health <= 0) print("Dead");
        return true;
    }
}
