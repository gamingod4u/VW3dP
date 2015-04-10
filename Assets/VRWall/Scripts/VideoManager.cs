using UnityEngine;
using System.Collections;

public class VideoManager : MonoBehaviour
{
	#region Class Variables
	public GameObject[] panels; // 0 = bottom video options; 1 = left home; 2 = right social;
	public GameObject[] mainButtons; // 0 = play; 1 = home; 2 = options;
	public Texture[] 	enabledButtons; // 0 = play; 1 = home; 2 = options;
	public Texture[]    disabledButtons;// 0 = play; 1 = home; 2 = options;
	public float 		lastHitTime = 0;


	#endregion


	#region Getters/Setters

	public float HitSomething
	{
		set{lastHitTime = value;}
	}
	#endregion

	#region Unity Functions
	void Awake()
	{
		Look2Select.OnHit += new Look2Select.HitEvent(ThisHit);
	}

	// Use this for initialization
	void Start () 
	{
	
	}

	void OnDestroy()
	{
		Look2Select.OnHit -= new Look2Select.HitEvent(ThisHit);
	}

	// Update is called once per frame
	void Update () 
	{
		if(lastHitTime + 1 < Time.time)
		{
			Hide();
		}
	}

	#endregion

	#region Class Functions
	private void ThisHit(string name)
	{

		switch(name)
		{
			case "home": 
			{
				lastHitTime = Time.time;
				Hide();
				mainButtons[1].renderer.material.mainTexture = enabledButtons[1];
				panels[1].SetActive(true);
				
					
			}break;
			case "options":
			{
				lastHitTime = Time.time;
				Hide();
				mainButtons[2].renderer.material.mainTexture = enabledButtons[1];
				panels[2].SetActive(true);
		}break;
			case "play":
			{
				Hide();
				mainButtons[0].renderer.material.mainTexture = enabledButtons[1];
				panels[0].SetActive(true);
				lastHitTime = Time.time;
			}break;
		}
	}

	private void Hide()
	{
		for(int i = 0; i < 3; i++)
		{
			mainButtons[i].renderer.material.mainTexture = disabledButtons[i];
			panels[i].SetActive(false);
		}
	}
	#endregion
}
