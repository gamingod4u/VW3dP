using UnityEngine;
using System.Collections;

public class MenuButtons : MonoBehaviour 
{
	public GameObject 	progress;
	public GameObject 	dropDownWall;
	public string	  	buttonName = string.Empty;
	public float 		selectSpeed = 0;
	
	private Renderer    progressRenderer;
	private Vector3		barSize = Vector3.zero;
	private Vector3 	fullBarSize = Vector3.zero;
	private Vector3     startPosition = Vector3.zero;
	private Vector3     endPosition = Vector3.zero;
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
		startPosition = dropDownWall.transform.position;
		endPosition = new Vector3(dropDownWall.transform.position.x, dropDownWall.transform.position.y - 11, dropDownWall.transform.position.z);
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
	
	private void LoogkingAt(bool state)
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
			case "none":		buttonState = 0;StartCoroutine(MoveObject(endPosition,startPosition,1));break;
			case "home": 		buttonState = 1;StartCoroutine(MoveObject(startPosition,endPosition,1));break;
			case "search":		buttonState = 2;StartCoroutine(MoveObject(startPosition,endPosition,1));break;
			case "category":	buttonState = 3;StartCoroutine(MoveObject(startPosition,endPosition,1));break;
			case "keywords":	buttonState = 4;StartCoroutine(MoveObject(startPosition,endPosition,1));break;
			case "profile":		buttonState = 5;StartCoroutine(MoveObject(startPosition,endPosition,1));break;
			case "faves":		buttonState = 6;StartCoroutine(MoveObject(startPosition,endPosition,1));break;
			case "playlists": 	buttonState = 7;StartCoroutine(MoveObject(startPosition,endPosition,1));break;
			case "log":			buttonState = 8;StartCoroutine(MoveObject(startPosition,endPosition,1));break;
			break;	
		}
		lastName = buttonName;
	}
	
	
	private IEnumerator MoveObject(Vector3 startPos, Vector3 endPos, float time)
	{
		if(dropDownWall.transform.position != endPos)
		{
	    	float currTime = 0;
	    	float rate = 1/ time;
	    	while(currTime < time)
	    	{
	    		currTime += Time.deltaTime * rate;
	    		dropDownWall.transform.position = Vector3.Lerp(startPos, endPos, currTime);
	    		yield return null;
	    	}
	    }
	    else
	    	yield return null;
	}
}