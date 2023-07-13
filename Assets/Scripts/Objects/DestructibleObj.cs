using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObj : MonoBehaviour
{
    public int life = 5;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AllyDmg"))
        {
            Dmg(1);
        }
    }
    public void Dmg(int dmg)
    {
        if(life <= 0) return;
        life -= dmg;
        if(life <= 0)
        {
            Destroy(gameObject);
        }
    }
}
