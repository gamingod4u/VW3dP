using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;


/// <summary>
/// can select thumbnail is set to true when teh wall is not moving;
/// </summary>
public class VideoObject : MonoBehaviour
{
    #region Class Variables

    public Vector3 origPosition;
    public bool selected;
    public bool isActive;

    private GameObject thumbNail;
    private GameObject preview;
    private GameObject progressBar;
    private GameObject playerObject;
    //private GameObject moviePlayer;
    private Quaternion origRotation;
    private Texture2D activeTexture;
    private VideoData videoData;
    private Renderer progressRender;
    private Texture firstThumb;
    private Vector3 center;
    private Vector3 progressScale;
    private Vector3 currentPosition;
	private float rotationInterval;
	private bool isPreviewing;
	private bool isRotating;
	private bool moveClose;
	private bool inFront;
	private bool moveBack;
	private bool thumbLoaded;
    private int currRotationThumb;
    private int maxRotationThumb;


#if UNITY_STANDALONE
	private MovieTexture _movie;
	private AVProQuickTimeMovie qtPlayer;
#endif
	

    #endregion

    #region Unity Functions
    // Use this for initialization
	void Awake ()
    {
		currRotationThumb = 1;
		maxRotationThumb = 30;
		rotationInterval = 0.5f;
		isRotating = false;
		moveClose = false;
		moveBack = false;
		thumbLoaded = false;
		isPreviewing = false;

//		thumbCache = new Texture[maxRotationThumb+1];
        origPosition = transform.localPosition;
        origRotation = transform.localRotation;
	}
	void Start()
	{
		/*moviePlayer = GameObject.Find ("MoviePlayer2");
    #if UNITY_STANDALONE
		qtPlayer = moviePlayer.GetComponent<AVProQuickTimeMovie>();
    #endif*/

        if (AppManager.instance.VRConnected)
            playerObject = GameObject.Find("OVRPlayerController");
        else
            playerObject = GameObject.Find("Camera");

		center = playerObject.transform.position;
      
		Transform t = gameObject.transform.Find ("progress");
	    progressBar = t.gameObject;
		progressScale = t.localScale;


        t = gameObject.transform.Find("preview");
        preview = t.gameObject;
		preview.GetComponent<Renderer> ().enabled = false;

		t = gameObject.transform.Find ("thumbnail");
		t.GetComponent<Renderer> ().material.mainTexture = new Texture2D(256, 128, TextureFormat.ARGB32, false);
        thumbNail = t.gameObject;

		hideProgressBar ();

		disableVideo ();
	}
    void Update()
    {
        /*
       if (activeTexture)
       {
            activeTexture.SetPixel (Random.Range (0, 128), Random.Range (0, 128), Color.red);
            activeTexture.Apply ();
        }
        t.GetComponent<Renderer> ().material.mainTexture = tex;
        */

        if (selected && inFront && progressBar.transform.localScale.x < progressScale.x)
        {
            Vector3 size = progressBar.transform.localScale;
            size.x += (0.5f * Time.deltaTime);

            if (size.x > progressScale.x)
            {
                size.x = progressScale.x;

                startVideo("http://smog-05.tnaflix.com/01/013930fb091d29f48a09/out8.mp4");
                //startVideo ("http://smog-05.tnaflix.com/oculus/2cdb5eaa01977f56735e_720p_2_oculus.mp4");
                //startVideo ("http://smog-05.tnaflix.com/oculus/e39ebb0dfb67445919f9.mp4");
            }

            progressBar.transform.localScale = size;
        }
        
        if (moveClose)
        {
            Vector3 destination = (currentPosition - center) * .65f;
           //transform.Translate(Vector3.forward*Time.deltaTime*6f);

            transform.position = Vector3.Lerp(transform.position, destination, 5f * Time.deltaTime);

            float closeEnough = 0.010f;

            if (Vector3.Distance(destination, transform.position) < closeEnough)
            {
                reachedFront();
            }
        }
        if (moveBack)
        {
            transform.position = Vector3.Lerp(transform.position, currentPosition, 10f * Time.deltaTime);

            float closeEnough = 0.050f;

            if (Vector3.Distance(currentPosition, transform.position) < closeEnough)
            {
                transform.localPosition = origPosition;
                transform.localRotation = origRotation;
                moveBack = false;
                enableCollider();
            }
        }
    }
    #endregion

    #region Class Functions
    private void disableVideo ()
	{
		isActive = false;
		thumbNail.renderer.enabled = false;
		disableCollider ();
		videoData = null;
	}

	private void enableVideo ()
	{
		isActive = true;
		thumbNail.renderer.enabled = true;
		enableCollider ();
	}

	public void loadVideo(VideoData data)
	{
		videoData = data;
		enableVideo ();
		isActive = true;    
		loadThumbnail ();
	}

	public void unloadVideo()
	{
		isActive = false;
		disableVideo ();
	}

	private void showProgressBar()
	{
        progressBar.renderer.enabled = true;
		Vector3 size = progressBar.transform.localScale;
		size.x = 0;
		progressBar.transform.localScale = size;
	}

	private void hideProgressBar()
	{
        progressBar.renderer.enabled = false;
	}

	private void loadThumbnail()
	{
		string url = "http://img3.tnastatic.com/a16:8q80w256r/thumbs/" + videoData.viddir + "/" + videoData.first_thumb + "_" + videoData.VID + "l.jpg";
		thumbLoaded = true;
		StartCoroutine (getImage (url));
	}

	private void disableCollider()
	{
        
		foreach(Collider c in GetComponents<Collider> ())
		{
			c.enabled = false;
		}
	}
	private void enableCollider()
	{
     
		foreach(Collider c in GetComponents<Collider> ()) 
		{
			c.enabled = true;
		}
	}

	// Called when the thumbnail has reached its front position
	private void reachedFront()
	{
		showProgressBar ();
       
		currRotationThumb = 1;
		moveClose = false;
		isRotating = true;
		inFront = true;

		if (true) 
        {
			isPreviewing = true;
			
#if UNITY_STANDALONE
			StartCoroutine (showPreview ());
#endif
		}
		//showPreview ();

//		InvokeRepeating ("rotateThumb", 0, 1);
	}

	private void rotateThumb()
	{
		CancelInvoke ();
		
		if (isRotating == false) 
        {
			// We're done. reset thumbnail
			thumbNail.renderer.material.mainTexture = firstThumb;
			return;
		}

		string url = "http://img3.tnastatic.com/a16:8q80w256j/thumbs/" + videoData.viddir + "/" + currRotationThumb + "_" + videoData.VID + "l.jpg";
		StartCoroutine (loadNextThumb (url, currRotationThumb));
		currRotationThumb++;

		if (currRotationThumb > maxRotationThumb)
			currRotationThumb = 1;
	}

    public void startThumbRotation()
    {
        //		startVideo ("https://smog-05.tnaflix.com/c7/c739741570954de8afee/Spoilt_student_gets_a_cock-480p_1.ogv");
        //		startVideo ("http://smog-04.tnaflix.com/e3/e39ebb0dfb67445919f9/Yoga_Teen_Kitana_Lure_Anal_Creampied-720p.mp4");
        //		startVideo ("http://smog-05.tnaflix.com/01/013930fb091d29f48a09/out8.mp4");
        //		startVideo ("https://fck-c19.tnaflix.com/dev19/0/005/369/0005369176.mp4?key=3b4a9a0078f4215c1dc6ebcf927590cb&src=tna&hd=1&domain=tna360p");
     
     
        moveClose = true;
        moveBack = false;
        currentPosition = transform.position;
    }

	void startVideo (string url)
	{
        AppManager.instance.MovieURL = url;
        Application.LoadLevel(2);
	}

    public void stopThumbRotation()
    {
        inFront = false;
        moveBack = true;
        moveClose = false;
        isRotating = false;
        disableCollider();

        if (isPreviewing == true)
        {
            // We're done. reset thumbnail
            thumbNail.renderer.material.mainTexture = firstThumb;
            thumbNail.renderer.enabled = true;
            preview.renderer.enabled = false;
            isPreviewing = false;
        }

        hideProgressBar();
    }
    #endregion

    #region Class Coroutines

    IEnumerator getImage (string url)
	{
//		Texture tex = VideoCache.getCachedTexture (url);		
//		if (tex == null) {
			// Start a download of the given URL
			WWW www = new WWW (url);
			// wait until the download is done
			yield return www;
//			Debug.Log (url);
			www.LoadImageIntoTexture((Texture2D)thumbNail.renderer.material.mainTexture);
//			t.GetComponent<Renderer> ().material.mainTexture = www.textureNonReadable;
			firstThumb = thumbNail.renderer.material.mainTexture;
//			VideoCache.setCachedTexture(url, firstThumb);
//		} else {
//			t.GetComponent<Renderer> ().material.mainTexture = tex;
//		}
	}

	IEnumerator loadNextThumb (string url, int index)
	{
		Texture tex;

//		if (thumbCache[index] == null) {
			WWW www = new WWW (url);
			yield return www;
			tex = (Texture)www.texture;

//			thumbCache[index] = tex;
//		} else {
//			tex = thumbCache[index];
//		}
		if (isRotating == true) {
			thumbNail.renderer.material.mainTexture = tex;
		}
		InvokeRepeating ("rotateThumb", rotationInterval, 1);
	}

    IEnumerator showPreview()
    {
        WWW www = new WWW("https://smog-05.tnaflix.com/oculus/previews/7ccfcb1fa678d78284b7-1.ogv");
        MovieTexture m = www.movie;
        while (!m.isReadyToPlay)
            yield return www;

        if (isPreviewing)
        {
            m.Play();
            thumbNail.renderer.enabled = false;
            preview.GetComponent<Renderer>().enabled = true;
            preview.renderer.material.mainTexture = m;
        }
    }
    #endregion
}
