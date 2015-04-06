using UnityEngine;
using System.Collections;
using System;

public class HeadTracking : MonoBehaviour 
{
	public	bool onOptions = false;
	public  bool onWalls = false;

	private WallController	wallController;
	private GameObject theWall;
	private GameObject theOptions;
	private Transform  theCam;
	private float 	   rotSpeed = .8f;
	void Awake()
	{
		Look2Select.OnHit += new Look2Select.HitEvent (OnHit);
	}
	// Use this for initialization
	void Start () 
	{
		theWall = GameObject.Find ("TheWall");
		wallController = theWall.GetComponent<WallController> ();
		theOptions = GameObject.Find("TheOptions");
		theCam = GameObject.Find ("OVRCameraRig/TrackingSpace/CenterEyeAnchor").GetComponentInChildren<Transform> ();

	}
	void Destroy()
	{
		Look2Select.OnHit -= new Look2Select.HitEvent (OnHit);
	}
	// Update is called once per frame
	void Update () 
	{
		if (onWalls)
			RotateStuff (theWall);
		else
			RotateStuff (theOptions);
	}

	private void OnHit(string name)
	{
		if (name.Contains("Video")) 
		{
			onWalls = true;
			onOptions = false;
		} 
		else if(name.Contains("Button"))
		{
			onOptions = true;
			onWalls = false;
		}
	}

	private void RotateStuff(GameObject go)
	{
	
		if (theCam.rotation.y > 0.3) 
		{
			wallController.rotateSpeed = -rotSpeed;
			go.transform.Rotate(Vector3.down * rotSpeed);

		} 
		else if (theCam.rotation.y < -0.3f) 
		{
			wallController.rotateSpeed = rotSpeed;
			go.transform.Rotate(Vector3.up * rotSpeed);
		} 
		else 
		{
			wallController.rotateSpeed = 0;
		}

	}
}
