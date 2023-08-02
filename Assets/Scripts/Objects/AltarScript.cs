using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltarScript : MonoBehaviour
{
    Animator animator;
    PlayerManagement playerManagement;
    public Transform gauntlet;
    Transform hand;
    bool track = false;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (track)
        {
            print("Tracking");
            gauntlet.position += .8f*Time.deltaTime * (hand.position - gauntlet.position).normalized;
            gauntlet.rotation = Quaternion.Slerp(gauntlet.rotation, hand.rotation, .5f*Time.deltaTime);
            if((gauntlet.position - hand.position).sqrMagnitude < .001f)
            {
                print("Anclada");
                track = false;
                gauntlet.gameObject.SetActive(false);
                playerManagement.UnblockMovement();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetBool("Action", true);
            playerManagement = other.GetComponent<PlayerManagement>();
            hand = playerManagement.r_hand;
            playerManagement.BlockMovement();
            GetComponent<Collider>().enabled = false;
        }
    }
    public void TrackHand()
    {
        print("Tracking");
        track = true;
    }
    public void ActivateGauntlet()
    {
        gauntlet.gameObject.SetActive(true);
    }
}
