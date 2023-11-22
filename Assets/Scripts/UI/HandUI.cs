using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandUI : MonoBehaviour
{
    Animator animator;
    private bool showingUI = false;
    float secondsToHide = 0f;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (showingUI)
        {
            secondsToHide -= Time.deltaTime;
            if(secondsToHide <= 0)
            {
                showingUI = false;
                animator.SetBool("ShowUI", showingUI);
            }
        }
    }
    public void ShowUISeconds(float s, WC_RES res)
    {
        showingUI = true;
        animator.SetInteger("UIAnimID", (int)res);
        animator.SetBool("ShowUI", showingUI);
        secondsToHide = s;
    }

    public void SwitchUI()
    {
        showingUI = !showingUI;
        animator.SetBool("ShowUI", showingUI);
    }
}
