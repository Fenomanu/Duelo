using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelButton : MonoBehaviour
{
    [SerializeField]
    private DIR dir;
    [SerializeField]
    private ControlPanelScript panel;
    Animation anim;
    private void Start()
    {
        anim = GetComponent<Animation>();
    }
    private void OnTriggerEnter(Collider other)
    {
        print("YES");
        anim.Play();
        panel.Move(dir);
    }
}
