using UnityEngine;
using System.Collections;
using System;

public class HeadTracking : MonoBehaviour
{
    #region Class Variables

    public bool canRotate = true;
    public float rotSpeed = 30f;
    
    private Look2Select     look2; 
    private GameObject      theWall;
	private GameObject      theOptions;
	private Transform       theCam;   
    private bool            onOptions = false;
	private bool            onWalls = false;   
    private bool            isStopped = false;
    #endregion

    #region Unity Functions
    void Awake()
	{
        LookSelection.OnHit += new LookSelection.HitEvent(OnHit);
	}
	// Use this for initialization
	void Start () 
	{
        if (Application.loadedLevel == 1)
        {
            theOptions = GameObject.Find("TheOptions");
            theWall = GameObject.Find("TheWall");
        }
        if (AppManager.instance.VRConnected)
            theCam = GameObject.Find("CenterEyeAnchor").GetComponent<Transform>();
        else
            theCam = GameObject.Find("Camera").GetComponent<Transform>();
	}
	void Destroy()
	{
		LookSelection.OnHit -= new LookSelection.HitEvent (OnHit);
	}
	// Update is called once per frame
	void Update () 
	{
        if (Application.loadedLevel == 1)
        {
            if (onWalls)
                RotateStuff(theWall);
            else
                RotateStuff(theOptions);
        }
	}

    #endregion 

    #region Class Functions
    private void OnHit(string name, string tag)
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
			go.transform.Rotate(Vector3.down * rotSpeed * Time.deltaTime);
            isStopped = false;
		} 
		else if (theCam.rotation.y < -0.2f && canRotate) 
		{
            go.transform.Rotate(Vector3.up * rotSpeed * Time.deltaTime);
            isStopped = false;
		} 
		else 
		{
            if (!isStopped)
            {
                isStopped = true;
            }
		}

	}

    public IEnumerator WaitToRotate() 
    {
        yield return new WaitForSeconds(1);
        canRotate = true;
    }
    #endregion
}
