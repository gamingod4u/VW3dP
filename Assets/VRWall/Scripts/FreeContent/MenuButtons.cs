using UnityEngine;
using System.Collections;

public class MenuButtons : MonoBehaviour 
{
	public GameObject 	progress;
	public string	  	buttonName = string.Empty;
	public float 		selectSpeed = 0;
	
	private Renderer    progressRenderer;
	private Vector3		barSize = Vector3.zero;
	private Vector3 	fullBarSize = Vector3.zero;
	private string 		lastName = string.Empty;
	public bool 		selected = false; 
	private int 		buttonState = 0; // none = 0, 1 = Home, 2 = Search, 3 = catagory, 4 = keywords, 5 = profile, 6 = faves, 7 = playlists, 8 = login/logout
	private int 		lastState = 0;
	
	// Use this for initialization
	void Start () 
	{
		progressRenderer = progress.renderer;
		fullBarSize = progress.transform.localScale;
		progress.transform.localScale = new Vector3(0, progress.transform.localScale.y, progress.transform.localScale.z);
	
		lastName = "none";
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(this.selected)
			if(barSize.x < fullBarSize.x)
			{
				barSize = new Vector3(selectSpeed * Time.deltaTime, 0, 0);
				progress.transform.localScale += barSize;
			}
			else
				if(lastName != buttonName)
					SwitchState();
		else 
		{
		   buttonState = 0;
		}	
			
		SwitchActionState();
		
	}
	
	private void HideProgressBar()
	{
		progress.renderer.enabled = false;
	}
	
	public void LoogkingAt(bool state)
	{
		if(state)
		{
			ShowProgressBar();
			selected = true;
		}
		else
		{
			HideProgressBar();
			selected = false;
		}
	}
	private void ShowProgressBar()
	{
		progress.renderer.enabled = true;
		barSize = new Vector3(0, progress.transform.localScale.y, progress.transform.localScale.z);	
	}
	private void SwitchActionState()
	{
		
		switch(buttonState)
		{
			
			case 0: lastName = "none";break;

			
		}
	}
	private void SwitchState()
	{
		switch(buttonName)
		{
			case "none":		buttonState = 0;break;
			case "home": 		buttonState = 1;break;
			case "search":		buttonState = 2;break;
			case "category":	buttonState = 3;break;
			case "keywords":	buttonState = 4;break;
			case "profile":		buttonState = 5;break;
			case "faves":		buttonState = 6;break;
			case "playlists": 	buttonState = 7;break;
			case "log":			buttonState = 8;break;
			
		}
		lastName = buttonName;
	}
	

}