using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPanelScript : MonoBehaviour
{
    [SerializeField]
    private Transform bridge;
    [SerializeField]
    private Vector3[] positions;
    private int limit = 3;
    private int[] pointer = { 0, 0 }; // 0 = VERTICAL, 1 = HORIZONTAL
    // Start is called before the first frame update
    void Start()
    {
        bridge.position = positions[limit*pointer[0] + pointer[1]];
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 obj = positions[limit * pointer[0] + pointer[1]];
        if (bridge.position != obj)
        {
            bridge.position += Time.deltaTime * 2 * (obj - bridge.position).normalized;
            if ((bridge.position - obj).magnitude < Time.deltaTime * 2) bridge.position = obj;
            
        }
    }
    public void Move(DIR dir)
    {
        switch (dir)
        {
            case DIR.UP:
                if(pointer[0] > 0) pointer[0]--;
                break;
            case DIR.DOWN:
                if (pointer[0] < limit-1) pointer[0]++;
                break;
            case DIR.LEFT:
                if (pointer[1] > 0) pointer[1]--;
                break;
            case DIR.RIGHT:
                if (pointer[1] < limit-1) pointer[1]++;
                break;
        }
    }
}
