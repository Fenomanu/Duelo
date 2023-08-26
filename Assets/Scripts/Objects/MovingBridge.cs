using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBridge : MonoBehaviour
{
    [SerializeField]
    private Transform cart;
    [SerializeField]
    private Transform platform;
    [SerializeField]
    private float leftLimit = -5.8f;
    [SerializeField]
    private float rightLimit = 5.8f;
    private int l_Objs;
    private int r_Objs;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    int Abs(int a)
    {
        return a < 0 ? -a : a;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        int rot = 0;
        int dif = Abs(r_Objs - l_Objs);
        if (l_Objs == r_Objs)
        {

        }
        else if (l_Objs > r_Objs)
        {
            rot = dif * -5;
            if (rot > 15) rot = 15;
            if (cart.localPosition.z > leftLimit)
            {
                cart.localPosition -= Vector3.forward * Time.fixedDeltaTime * (.5f + (.5f*dif));
                if (cart.localPosition.z < leftLimit) cart.localPosition = Vector3.forward * leftLimit;
            }
        }
        else // l_Objs < r_Objs
        {
            rot = dif * 5;
            if (rot > 15) rot = 15;
            if (cart.localPosition.z < rightLimit)
            {
                cart.localPosition += Vector3.forward * Time.fixedDeltaTime * (.5f + (.5f * dif));
                if (cart.localPosition.z > rightLimit) cart.localPosition = Vector3.forward * rightLimit;
            }
        }
        //print(platform.eulerAngles);
        float obj = To360(platform.eulerAngles.x);
        if (obj < rot)
        {
            platform.Rotate(Time.fixedDeltaTime * 3 * Vector3.right);
            if (To360(platform.eulerAngles.x) > rot) platform.eulerAngles = Vector3.right * rot;
        }
        else if (obj > rot) 
        {
            platform.Rotate(Time.fixedDeltaTime * -3 * Vector3.right);
            if (To360(platform.eulerAngles.x) < rot) platform.eulerAngles = Vector3.right * rot;
        }
    }
    private float To360(float rot)
    {
        if (rot > 180) rot -= 360;
        return rot;
    }
    public void ROP(OPS op, SIDE side)
    {
        switch (op)
        {
            case OPS.ADD:
                if (side == SIDE.LEFT) l_Objs++;
                else r_Objs++;
                break;
            case OPS.SUB:
                if (side == SIDE.LEFT) l_Objs--;
                else r_Objs--;
                break;
        }
    }
}
