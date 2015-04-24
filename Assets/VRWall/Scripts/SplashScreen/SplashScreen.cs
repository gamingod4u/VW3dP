using UnityEngine;
using System.Collections;

public class SplashScreen : MonoBehaviour 
{
	public GameObject[] splashAssets;
	public GameObject[] sponsorLogos;

	private float 		tweenTimer = 3f;
	private float 		waitTimer = 10f;
	private bool 		isRunning = true;
    private bool firstPass = false;
	private int 		menuCount = -1;

	// Use this for initialization
	void Awake () 
	{
		foreach(GameObject assets in splashAssets)
			assets.SetActive(false);					/// make sure nothing is on;
		foreach(GameObject logos in sponsorLogos)
			logos.SetActive(false);
	}

    void Start() 
    {

    }
	
	// Update is called once per frame
	void Update () 
	{
        if (!firstPass)
        {
            StartCoroutine("WaitFor1Second");
            StartCoroutine("WaitToEnd");
            firstPass = false;
        }
            
		if(!isRunning)
		{
			isRunning = true;

			if(menuCount < 2 && menuCount >= 0)
			{
				splashAssets[menuCount].SetActive(true);
				StartCoroutine("TweenAlpha",splashAssets[menuCount]);
			}
			else if(menuCount == 2)
			{
				splashAssets[menuCount].SetActive(true);

				for(int i = 0; i < 5; i++)
				{
					sponsorLogos[i].SetActive(true);
                    Debug.Log("here for :" + i);
					StartCoroutine("TweenAlpha", sponsorLogos[i]);
				}
			}
			else if( menuCount == 3)
			{
                Application.LoadLevel(1);
			}
		}
	}

	private IEnumerator TweenAlpha(GameObject thisObject)
	{
	
		yield return new WaitForSeconds(tweenTimer);

        if (menuCount <= 2)
            menuCount++;
        else
            menuCount = 3;
       
		thisObject.SetActive(false);
		isRunning = false;
	}

    private IEnumerator WaitFor1Second() 
    {
        yield return new WaitForSeconds(1);
            waitTimer = 3;
      
    }
	private IEnumerator WaitToEnd()
	{
		if(menuCount == -1)
			yield return new WaitForSeconds(waitTimer);
		else 
			yield return new WaitForSeconds(tweenTimer);

		if(menuCount == -1)
		{
			menuCount = 0;
			isRunning = false;
		}
		else 
			Debug.Log("We have ended");

	}
}
