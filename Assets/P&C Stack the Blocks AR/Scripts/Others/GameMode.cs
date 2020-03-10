using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ARENABLED
using UnityEngine.XR.ARFoundation;
using UnityEngine.SpatialTracking;
#endif
//To avoid unncessary warnings due to AR preprocessor directives
#pragma warning disable
    
/// <summary>
/// Manages modes for the game : AR and Normal.
/// 1. Enables/disables AR funtionality completely in project
/// 2. With AR functionality disabled, the game will run in normal mode
/// 3. With AR functionality enabled , both the modes will be available with switching button in homescreen.
/// </summary>
public class GameMode : MonoBehaviour
{
    /// <summary> Whether AR funcnationality is enabled or not, please note that it's not for switching between modes </summary>
    [HideInInspector]
    public bool isAREnabled;

    [Tooltip("Scale of gameobjects in the real world")]
    [SerializeField]
    [Range(0, 1)]
    private float ARScaleFactor = 0.3f;

    /// <summary>Bigger value means smaller scale </summary>
    private float AROriginScale
    {
        get
        {
            return ARScaleFactor * 100;
        }
    }

    ///<summary>Instrution for finding an AR surface </summary>
    [SerializeField]
    private GameObject SurfaceNotFoundGraphic = default;
    [SerializeField]
    /// <summary>Game start instruction to show when surface is found</summary>
    public GameObject startGameGraphic = default;

    /// <summary>To switch between AR and normal mode. 
    /// This is only applicable if AR functionality is enabled, else the game will run in normal mode by default
    /// </summary>
    [HideInInspector]
    public bool isARModeOn;
    ///<summary>References required for raycasting to AR surface</summary>
    private Camera arCamera;
#if ARENABLED
    private ARRaycastManager aRRaycastManager;
#endif

    ///<summary>Parent Gameobject for the stack </summary>
    [SerializeField]
    private GameObject stack = default;
    ///<summary>list of all non AR gameobjects in heirarchy </summary>
    [SerializeField]
    private List<GameObject> nonARAssets = default;
    ///<summary>list of all AR gameobjects in heirarchy  </summary>
    private List<GameObject> ARAssets = new List<GameObject>();
    ///<summary>Depth mask plane with shadow for AR  </summary>
    [SerializeField]
    private GameObject maskPlane = default;
    ///<summary>For toggling shadow. AR uses shadow </summary>
    private Light directionLight;

#if ARENABLED
    private void Start()
    {
        StartCoroutine(SetupAR());
        SurfaceNotFoundGraphic.SetActive(false);
        maskPlane.SetActive(false);
        directionLight = FindObjectOfType<Light>();
    }

   
    /// <summary>
    /// Sets up AR
    /// Disables the game mode switching button on homescreen if AR is not supported on device
    /// </summary>
    IEnumerator SetupAR()
    {
        //Disable the button first
        UIManager.Instance.ARBtn.gameObject.SetActive(false);

        //AR session
        GameObject arSessionGO = new GameObject("AR Session");
        arSessionGO.AddComponent<ARSession>();

        //if AR is not supported on device destroy AR session and exit from coroutine.
        if ((ARSession.state == ARSessionState.None) || (ARSession.state == ARSessionState.CheckingAvailability))
        {
            yield return ARSession.CheckAvailability();
        }
        if (ARSession.state == ARSessionState.Unsupported)
        {
            Destroy(arSessionGO);
            yield break;
        }

        //AR is supported. Do the rest of the set up
        UIManager.Instance.ARBtn.gameObject.SetActive(true);
        arSessionGO.SetActive(false);

        arSessionGO.AddComponent<ARInputManager>();

        //AR session Origin
        GameObject aRsessionOriginGO = new GameObject("AR Session Origins");
        aRsessionOriginGO.SetActive(false);
        var origin = aRsessionOriginGO.AddComponent<ARSessionOrigin>();
        aRRaycastManager = aRsessionOriginGO.AddComponent<ARRaycastManager>();
        aRsessionOriginGO.AddComponent<ARPlaneManager>();

        //AR Camera  
        GameObject arCamerGO = new GameObject("AR Camera");
        arCamerGO.transform.SetParent(aRsessionOriginGO.transform);
        arCamera = arCamerGO.AddComponent<Camera>();
        arCamera.clearFlags = CameraClearFlags.Color;
        arCamera.backgroundColor = Color.black;
        arCamera.nearClipPlane = 0.1f;
        arCamera.farClipPlane = 80;
        origin.camera = arCamera;
        var tpd = arCamerGO.AddComponent<TrackedPoseDriver>();
        arCamerGO.AddComponent<ARCameraManager>();
        arCamerGO.AddComponent<ARCameraBackground>();
        tpd.SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRDevice, TrackedPoseDriver.TrackedPose.ColorCamera);

        //Setting the scale of AR world
        aRsessionOriginGO.transform.localScale = Vector3.one * AROriginScale;

        //Adding all AR assets to the list
        ARAssets.Add(arSessionGO);
        ARAssets.Add(aRsessionOriginGO);
        ARAssets.Add(maskPlane);
        ARAssets.Add(SurfaceNotFoundGraphic);
    }

#endif

    /// <summary>
    /// Switchs the Game mode and enables/disables the respective components.
    /// </summary>
    public void SwitchGameMode()
    {
#if ARENABLED
        isARModeOn = !isARModeOn;

        //Set respective assets active state
        ARAssets.ForEach((obj) => obj.SetActive(isARModeOn));
        nonARAssets.ForEach((obj) => obj.SetActive(!isARModeOn));

        //Reset stak position
        stack.transform.position = new Vector3(5, -0.5f, 0);

        if (!isARModeOn)
        {
            stack.gameObject.SetActive(true);
            stack.transform.rotation = Quaternion.identity;
        }
        //Enabling shadows for AR mode. Comment this if shadows are required in normal mode
        directionLight.shadows = (LightShadows)(isARModeOn ? 2 : 0);
        UIManager.Instance.ToggleARModeSprite(isARModeOn);
#endif
    }

#if ARENABLED
    private void Update()
    {
        if (isARModeOn)
        {
            UpdateStartBlockARPos();
        }
    }

    /// <summary>
    /// Finds an AR surface and places the starting block.
    /// </summary>
    void UpdateStartBlockARPos()
    {
        var screenPos = arCamera.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        var didHitPlane = false;

        //Position the stack and shdowplane on hit point.
        if (aRRaycastManager.Raycast(screenPos, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            didHitPlane = true;
            stack.transform.position = hits[0].pose.position;
            maskPlane.transform.position = stack.transform.position;

            var lookPos = new Vector3(arCamera.transform.position.x, hits[0].pose.position.y, arCamera.transform.position.z);
            var lookRot = Quaternion.LookRotation(lookPos - hits[0].pose.position) * Quaternion.Euler(0, -45, 0);

            //To avoid the long rotation initially
            if (Quaternion.Angle(stack.transform.rotation,lookRot) > 20)
            {
                stack.transform.rotation = lookRot;
            }
            else
            {
                stack.transform.rotation = Quaternion.Lerp(stack.transform.rotation, lookRot, Time.deltaTime * 10);
            }

        }
        stack.SetActive(didHitPlane);
        SurfaceNotFoundGraphic.SetActive(!didHitPlane);
        startGameGraphic.SetActive(didHitPlane);
    }
#endif

    /// <summary>
    /// Start the game
    /// In AR Mode: start if the start block is active i.e surface is found.
    /// </summary>
    public void StartGame()
    {
        if (isARModeOn)
        {
            //start game if stack is active which means a placement position has been found in AR.
            if (stack.activeSelf)
            {
                GameManager.Instance.StartGame();
                SoundManager.Instance.playSound(AudioClips.UI);
                this.enabled = false;
            }
        }
        else
        {
            GameManager.Instance.StartGame();
            SoundManager.Instance.playSound(AudioClips.UI);
            this.enabled = false;
        }
    }
}
