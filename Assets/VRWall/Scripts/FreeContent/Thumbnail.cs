using UnityEngine;
using System.Collections;

public class Thumbnail : MonoBehaviour 
{
	#region Class Variables
	
	// public variables
	public Transform 			thumbnail;
	public Transform			progressBar;
	public Transform			playerObject;
	public Transform 			moviePlayer;
	public Transform			preview;
	public bool 				isActive = false;
	public bool					selected = false;
	
	//private variables
	private VideoData 			data;
	private Texture 			oldThumb;
	private Texture 			activeThumb;	
	private Vector3 			origPosition;
	private Vector3				progressScale;
	private Vector3 			center;
	private float 				rotationTime = .5f;
	private int 				currentThumb = 1;
	private int 				maxThumb = 30;
	private bool 				isPreviewing = false;
	private bool 				isRotating = false;
	private bool 				moveCloser = false;
	private bool 				moveBack = false;
	private bool 				isFront = false;
	private bool 				thumbnailLoaded = false;
#if UNITY_STANDALONE
	private AVProQuickTimeMovie	qtPlayer;
	private MovieTexture 		movie;
#endif
	#endregion

	#region Unity Functions

	// Use this for initialization
	void Start () 
	{
		Transform t = this.transform.Find("thumbnail");
		thumbnail = t;
		t = this.transform.Find("progress");
		progressBar = t;
		t = this.transform.Find("preview");
		preview = t;
		
		
		//preview = this.transform.Find("preview").GetComponent<GameObject>()as GameObject;
		//progressBar = this.transform.Find("progress").GetComponent<GameObject>();
		//thumbnail = this.transform.Find("thumbnail").GetComponent<GameObject>();
		
		qtPlayer = moviePlayer.GetComponent<AVProQuickTimeMovie>();
		progressScale = progressBar.transform.localScale;
		preview.renderer.enabled = false;	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(selected && isFront && progressBar.transform.localScale.x < progressScale.x)
		{
			float size = progressBar.transform.localScale.x + (.5f * Time.deltaTime);
			progressBar.transform.localScale = new Vector3(size, progressBar.transform.localScale.y, progressBar.transform.localScale.z);
			
			if(progressBar.transform.localScale.x > progressScale.x)
				StartVideo("http://smog-05.tnaflix.com/01/013930fb091d29f48a09/out8.mp4");
		}	
		
		if(moveCloser)
		{
			Vector3 destination = (origPosition - center)*.65f;
			transform.position = Vector3.Lerp (transform.position, destination, 5f * Time.deltaTime);
			
			if(Vector3.Distance(destination, transform.position) < .010f)
				ReachedFront();
		}
		
		if(moveBack)
		{
			transform.position = Vector3.Lerp(transform.position, origPosition, 8f * Time.deltaTime);
			
			if(Vector3.Distance(origPosition, transform.position) < 0.010f)
			{
				transform.position = origPosition;
				moveBack = false;
				
				thumbnail.collider.enabled = true;
			}
		}
	}
	#endregion

	#region Class Functions
	
	public void DisableVideo()
	{
		isActive = false;
		thumbnail.renderer.enabled = false;
		
		data = null;
	}
	
	public void EnableVideo()
	{
		isActive = true;
		thumbnail.renderer.enabled = true;
		
	}
	
	public void HideProgressBar()
	{
		progressBar.renderer.enabled = false;
	}
	
	
	private void LoadThumbnail()
	{
		string url = "http://img3.tnastatic.com/a16:8q80w256r/thumbs/" + data.viddir + "/" + data.first_thumb + "_" + data.VID + "l.jpg";
		thumbnailLoaded = true;	
		StartCoroutine (GetImage (url));
	}
	
	public void LoadVideo(VideoData video)
	{
		data = video;
		EnableVideo();
		isActive = true;
		
	}
	private void ReachedFront()
	{
		ShowProgressBar();
		
		moveCloser = false;
		isRotating = true;
		isFront = true;
		
		if(true)
		{
			isPreviewing = true;
			
#if UNITY_STANDALONE 
			StartCoroutine(ShowPreview());
#endif 
		}
	}
	private void RotateThumb()
	{
		CancelInvoke ();
		
		if(!isRotating)
			thumbnail.renderer.material.mainTexture = oldThumb;
			
		string url =  "http://img3.tnastatic.com/a16:8q80w256j/thumbs/" + data.viddir + "/" + currentThumb + "_" + data.VID + "l.jpg";
		StartCoroutine(LoadNextThumb(url, currentThumb));
		currentThumb++;
		if(currentThumb > maxThumb)
			currentThumb = 1;
	}
	private void ShowProgressBar()
	{
		progressBar.renderer.enabled = true;
		progressBar.transform.localScale = new Vector3(0, progressBar.transform.localScale.y, progressBar.transform.localScale.z);	
		
	}
	public void StartThumbRotation()
	{
		moveCloser = true;
		moveBack = false;
	}
	private void StartVideo(string url)
	{
		playerObject.transform.position = new Vector3(0,0.7f,2.1f);
		playerObject.transform.LookAt(new Vector3(0,0.7f,0));
		
#if UNITY_STANDALONE
		
		if(qtPlayer.MovieInstance != null)
			qtPlayer.UnloadMovie();
		
		qtPlayer._playOnStart = true;
		qtPlayer._source = AVProQuickTimePlugin.MovieSource.URL;
		qtPlayer._filename = url;
		qtPlayer.LoadMovie();	
#endif
		Debug.Log("Start video" + url);
	}
	
	public void StopThumbRotation()
	{
		isFront = false;
		moveBack = true;
		moveCloser = false;
		isRotating = false;
		
		thumbnail.collider.enabled = false;
		
		if(isPreviewing)
		{
			thumbnail.renderer.material.mainTexture = oldThumb;
			preview.renderer.enabled = false;
			
			isPreviewing = false;
		}
		
		HideProgressBar();
	}
	
	public void UnloadVideo()
	{
		isActive = false;
		DisableVideo();
	}

	#endregion

	#region Coroutines
	
	IEnumerator GetImage(string url)
	{
		WWW www = new WWW(url);
		yield return www;
		www.LoadImageIntoTexture((Texture2D)thumbnail.renderer.material.mainTexture);
		oldThumb = thumbnail.renderer.material.mainTexture;
	}
	
	IEnumerator ShowPreview()
	{
		WWW www = new WWW("https://smog-05.tnaflix.com/oculus/previews/7ccfcb1fa678d78284b7-1.ogv");
		MovieTexture m = www.movie;
		
		while(!m.isReadyToPlay)
			yield return www;
		
		if(isPreviewing)
		{
			m.Play();
			thumbnail.renderer.enabled = false;
			preview.renderer.enabled = true;
			preview.renderer.material.mainTexture = m;
		}
	}
	
	IEnumerator LoadNextThumb(string url, int index)
	{
		Texture tex;
		WWW www = new WWW(url);
		yield return www;
		
		tex = (Texture)www.texture;
		
		if(isRotating)
			thumbnail.renderer.material.mainTexture = tex;
			
		InvokeRepeating("RotateThumb",rotationTime, 1f);
		
	}
	#endregion
}
