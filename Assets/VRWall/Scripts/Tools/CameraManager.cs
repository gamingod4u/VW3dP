using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour 
{

    // This script manages which camera should be active in the scenes. Maybe they dont want to use VR
    public GameObject ovrCamera;
    public GameObject regCamera;

	// Use this for initialization
	void Start ()
    {
        ovrCamera = GameObject.Find("OVRPlayerController");
        regCamera = GameObject.Find("Camera");



        if (!AppManager.instance.VRConnected)
            ovrCamera.SetActive(false);
        else
            regCamera.SetActive(false);
	}
}
