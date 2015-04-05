using UnityEngine;
using System.Collections;
using System;

public class HeadTracking : MonoBehaviour {

	private TheWall _circle;
	
	
	public	bool onMenus = false;
	public  bool onWalls = false;

	// Use this for initialization
	void Start () {
		GameObject c = GameObject.Find ("TheWall");
		_circle = (TheWall)c.GetComponent(typeof(TheWall));
	}
	
	// Update is called once per frame
	void Update () {
		Transform cam = GameObject.Find("OVRCameraRig/TrackingSpace/RightEyeAnchor").GetComponentInChildren<Camera>().transform;
		
		if (cam.rotation.y > 0.5) {
			_circle.rotateSpeed = -0.1f * (float)Math.Exp(cam.rotation.y*2);
		} else if (cam.rotation.y < -0.5f) {
			_circle.rotateSpeed = 0.1f * (float)Math.Exp(0-(cam.rotation.y*2));
		} else {
			_circle.rotateSpeed = 0.0f;
		}
	}
}
