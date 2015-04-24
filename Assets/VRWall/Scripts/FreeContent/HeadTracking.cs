using UnityEngine;
using System.Collections;
using System;

public class HeadTracking : MonoBehaviour 
{
	public GameObject theWall;
	public GameObject theOptions;
	public Transform  theCam;
    public float rotSpeed = 30f;
	public bool onOptions = false;
	public bool onWalls = false;
    public bool canRotate = true;
	private WallController	wallController;
    private Look2Select look2;
    private bool isStopped = false;
	
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
        look2 = GameObject.Find("Look2Select").GetComponent<Look2Select>();
        if (AppManager.instance.VRConnected)
            theCam = GameObject.Find("CenterEyeAnchor").GetComponent<Transform>();
        else
            theCam = GameObject.Find("Camera").GetComponent<Transform>();
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
	
		if (theCam.rotation.y > 0.2 && canRotate) 
		{
			wallController.rotateSpeed = -rotSpeed;
			go.transform.Rotate(Vector3.down * rotSpeed * Time.deltaTime);
            isStopped = false;
		} 
		else if (theCam.rotation.y < -0.2f && canRotate) 
		{
            
			wallController.rotateSpeed = rotSpeed;
			go.transform.Rotate(Vector3.up * rotSpeed * Time.deltaTime);
            isStopped = false;
		} 
		else 
		{
            if (!isStopped)
            {
                wallController.rotateSpeed = 0;
                StartCoroutine(look2.Wait2Select(2f));
                isStopped = true;
            }
		}

	}

    public IEnumerator WaitToRotate() 
    {
        yield return new WaitForSeconds(1);
        canRotate = true;
    }
}
