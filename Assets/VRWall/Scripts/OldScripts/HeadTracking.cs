using UnityEngine;
using System.Collections;
using System;

public class HeadTracking : MonoBehaviour 
{
	public  GameObject theWall;
	public  GameObject theOptions;
	public  Transform  theCam;
	public	bool onOptions = false;
	public  bool onWalls = false;
	
	private WallController	wallController;
	

	private float 	   rotSpeed = 30f;
	
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
		theCam = GameObject.Find ("OVRCameraRig/CenterEyeAnchor").GetComponentInChildren<Transform> ();

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
	
		if (theCam.rotation.y > 0.1) 
		{
			wallController.rotateSpeed = -rotSpeed;
			go.transform.Rotate(Vector3.down * rotSpeed * Time.deltaTime);

		} 
		else if (theCam.rotation.y < -0.1f) 
		{
			wallController.rotateSpeed = rotSpeed;
			go.transform.Rotate(Vector3.up * rotSpeed * Time.deltaTime);
		} 
		else 
		{
			wallController.rotateSpeed = 0;
		}

	}
}
