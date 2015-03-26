using UnityEngine;
using System.Collections;

public class SplashScreen : MonoBehaviour 
{
	public GameObject[] splashAssets;
	public GameObject[] sponsorLogos;

	private float 		tweenTimer = 1f;
	private float 		waitTimer = 15f;
	private bool 		isRunning = true;
	private int 		menuCount = -1;

	// Use this for initialization
	void Awake () 
	{
		foreach(GameObject assets in splashAssets)
			assets.SetActive(false);					/// make sure nothing is on;
		foreach(GameObject logos in sponsorLogos)
			logos.SetActive(false);

		StartCoroutine("WaitToEnd");
	}
	
	// Update is called once per frame
	void Update () 
	{
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
				for(int i = 0; i < 4; i++)
				{
					sponsorLogos[i].SetActive(true);
					StartCoroutine("TweenAlpha", sponsorLogos[i]);
				}
			}
			else if( menuCount == 3)
			{
				StartCoroutine("WaitToEnd");
			}
		}
	}

	private IEnumerator TweenAlpha(GameObject thisObject)
	{
		float alphaColor = thisObject.renderer.material.color.a;
		yield return new WaitForSeconds(tweenTimer);
		while(alphaColor > 0)
		{
			alphaColor -= Time.deltaTime * .5f;
			thisObject.renderer.material.color = new Color(255,255,255,alphaColor);
			yield return 0;
		}   
		yield return new WaitForSeconds(tweenTimer);
		menuCount++;
		thisObject.SetActive(false);
		isRunning = false;
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
