using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WorldChange : MonoBehaviour
{
    public static WorldChange Instance { get; private set; }
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    //World Changing
    //public GameObject lightWorld;
    //public GameObject darkWorld;
    private TeleportationProvider tp;
    
    private bool switchWorlds;

    public Vector3 dimOffset = new Vector3(0, 0, 100);
    
    public Camera lightCam;
    public Camera darkCam;
    public Camera mainCam;
    private Camera secCam;

    public RectTransform vignetteUI;
    //public TunnelingVignetteController vignette;
    private bool animationEnded = true;
    
    private LayerMask lightMask;
    private LayerMask darkMask;
    private LayerMask defaulMask;

    private enum World
    {
        Light = 0,
        Dark = 1
    }
    World curState;


    //Player Management    
    public XROrigin origin;
    public CharacterController controller;
    public BodyMap bodyMap;
    public ContinuousMoveProviderBase move;


    //Water Preventing
    private WaterManager water { get; set; }
    public void SetWater(WaterManager w) { water = w; }

    //GIZMOS
    private Vector3 c1;
    private Vector3 c2;
    private float rd;
    
    // Start is called before the first frame update
    void Start()
    {
        lightMask = LayerMask.NameToLayer("LightWorld");
        darkMask = LayerMask.NameToLayer("DarkWorld");
        defaulMask = LayerMask.NameToLayer("Default");
        mainCam = darkCam;
        secCam = lightCam;
        controller = origin.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(switchWorlds)
        {
            switchWorlds = false;
            print("Switching");  
            switch (curState)
            {
                case World.Light:
                    darkCam.enabled=true;
                    move.forwardSource = darkCam.transform;
                    lightCam.enabled = false;
                    mainCam = darkCam;
                    secCam = lightCam;
                    origin.Camera = mainCam;
                    bodyMap.cameraOff = mainCam.transform;

                    curState = World.Dark;
                    origin.transform.position += dimOffset;
                    break;
                case World.Dark:
                    lightCam.enabled = true;
                    move.forwardSource = lightCam.transform;
                    darkCam.enabled = false;
                    mainCam = lightCam;
                    secCam = darkCam;
                    origin.Camera = mainCam;
                    bodyMap.cameraOff = mainCam.transform;

                    curState = World.Light;
                    origin.transform.position -= dimOffset;
                    break;
            }
            if (water != null) water.NotInTP();
        }
        secCam.transform.position = mainCam.transform.position;
        secCam.transform.rotation = mainCam.transform.rotation;
    }


    public WC_RES SwitchWorld()
    {
        if (!animationEnded) return WC_RES.TIMEOUT;
        Vector3 center = controller.transform.position + controller.center;
        switch (curState)
        {
            case World.Light:
                center += dimOffset;
                break;
            case World.Dark:
                center -= dimOffset;
                break;
        }
        float height = controller.height;
        float rad = controller.radius;
        Collider[] cols = Physics.OverlapCapsule(center + (height / 2 - rad) * Vector3.up,
                center - (height / 2 - rad) * Vector3.up, rad, ~(1<<14));
        c1 = center - (height / 3) * Vector3.up;
        c2 = center + (height / 2) * Vector3.up;
        rd = controller.radius;
        if (cols.Length > 0)
        {
            print("Overlapping");
            return WC_RES.OBSTRUCTION;
        }
        animationEnded = false;
        vignetteUI.sizeDelta = new Vector2(50, 30);
        StartCoroutine(StartDimSwap());
        return WC_RES.SWITCHING;
    }

    IEnumerator StartDimSwap()
    {
        
        while (vignetteUI.sizeDelta.y > 0)
        {
            vignetteUI.sizeDelta -= new Vector2(0, 120 * Time.deltaTime);
            //vignette.currentParameters.apertureSize -= Time.deltaTime;
            yield return null;
        }
        vignetteUI.sizeDelta = 50*Vector2.right;
        if(water != null) water.InTP();
        switchWorlds = true;
        yield return new WaitForSeconds(.01f);
        while (vignetteUI.sizeDelta.y < 30)
        {
            vignetteUI.sizeDelta += new Vector2(0, 120 * Time.deltaTime);
            //vignette.currentParameters.apertureSize += Time.deltaTime;
            yield return null;
        }
        animationEnded = true;
    }

    public LayerMask GetDefaultLayer()
    {
        print(defaulMask);
        return defaulMask;
    }

    public LayerMask GetWorldLayer()
    {
        switch (curState)
        {
            case World.Light:
                return lightMask;
            case World.Dark:
                return darkMask;
        }
        print("Error en curState");
        return lightMask;
    }

    //Object managing 
    public void GrabObj(GameObject obj)
    {
        obj.layer = GetDefaultLayer();
        obj.transform.parent = origin.transform;
    }
    public void DropObj(GameObject obj)
    {
        if (!obj.activeSelf) return;
        obj.layer = GetWorldLayer();
        obj.transform.parent = null;
        //obj.transform.parent = curState == World.Light ? lightWorld.transform : darkWorld.transform;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
        Gizmos.DrawSphere(c1, rd);
        Gizmos.DrawSphere(c2, rd);
    }
}