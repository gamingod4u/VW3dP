using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class VideoObject : MonoBehaviour {

	public Vector3 origPosition;

	private VideoData _data;

	public bool selected;

	private int _curRotationThumb;
	private int _maxRotationThumb;
	private float _rotationInterval;
	private bool _isPreviewing;
	private bool _isRotating;
	private bool _moveCloser;
	private bool _moveBack;
	private Vector3 _center;
	private bool _thumbLoaded;
	private bool _inFront;
	public bool _isActive;

	private Texture _firstThumb;
	private Texture2D _activeTexture;

#if UNITY_STANDALONE
	private MovieTexture _movie;
	private AVProQuickTimeMovie qtPlayer;
#endif
	private Renderer _progressRender;
	private GameObject _progressBar;
	private GameObject playerObject;
	private GameObject moviePlayer;
	private Vector3 _progressScale;

	// Use this for initialization
	void Awake () {
		_curRotationThumb = 1;
		_maxRotationThumb = 30;
		_rotationInterval = 0.5f;
		_isRotating = false;
		_moveCloser = false;
		_moveBack = false;
		_thumbLoaded = false;
		_isPreviewing = false;

//		thumbCache = new Texture[_maxRotationThumb+1];

		origPosition = transform.position;

		moviePlayer = GameObject.Find ("MoviePlayer");
#if UNITY_STANDALONE
		qtPlayer = (AVProQuickTimeMovie)moviePlayer.GetComponent(typeof(AVProQuickTimeMovie));
#endif
		playerObject= GameObject.Find("OVRPlayerController");
		_center = playerObject.transform.position;

		Transform t = gameObject.transform.Find ("progress");
		_progressBar = t.gameObject;
		_progressRender = t.GetComponent<Renderer> ();
		_progressScale = t.localScale;

		Transform preview = gameObject.transform.Find ("preview");
		preview.GetComponent<Renderer> ().enabled = false;

		t = gameObject.transform.Find ("thumbnail");
		t.GetComponent<Renderer> ().material.mainTexture = new Texture2D(256, 128, TextureFormat.ARGB32, false);

		hideProgressBar ();

		disableVideo ();
	}

	private void disableVideo ()
	{
		_isActive = false;
		Transform _thumbnail = gameObject.transform.Find ("thumbnail");
		_thumbnail.GetComponent<Renderer> ().enabled = false;

		disableCollider ();

		_data = null;
	}

	private void enableVideo ()
	{
		_isActive = true;
		Transform _thumbnail = gameObject.transform.Find ("thumbnail");
		_thumbnail.GetComponent<Renderer> ().enabled = true;

		enableCollider ();
	}

	public void loadVideo(VideoData data)
	{
		_data = data;

		enableVideo ();

		_isActive = true;

		loadThumbnail ();
	}

	public void unloadVideo()
	{
		_isActive = false;

		disableVideo ();
	}

	private void showProgressBar()
	{
		_progressRender.enabled = true;

		Vector3 size = _progressBar.transform.localScale;
		size.x = 0;

		_progressBar.transform.localScale = size;
	}

	private void hideProgressBar()
	{
		_progressRender.enabled = false;
	}

	private void loadThumbnail()
	{
		string url = "http://img3.tnastatic.com/a16:8q80w256r/thumbs/" + _data.viddir + "/" + _data.first_thumb + "_" + _data.VID + "l.jpg";
		_thumbLoaded = true;

		StartCoroutine (getImage (url));
	}

	public void startThumbRotation()
	{
//		startVideo ("https://smog-05.tnaflix.com/c7/c739741570954de8afee/Spoilt_student_gets_a_cock-480p_1.ogv");
//		startVideo ("http://smog-04.tnaflix.com/e3/e39ebb0dfb67445919f9/Yoga_Teen_Kitana_Lure_Anal_Creampied-720p.mp4");
//		startVideo ("http://smog-05.tnaflix.com/01/013930fb091d29f48a09/out8.mp4");
//		startVideo ("https://fck-c19.tnaflix.com/dev19/0/005/369/0005369176.mp4?key=3b4a9a0078f4215c1dc6ebcf927590cb&src=tna&hd=1&domain=tna360p");
		_moveCloser = true;
		_moveBack = false;
	}

	private void disableCollider()
	{
		Transform _thumbnail = gameObject.transform.Find ("thumbnail");

		foreach(Collider c in GetComponents<Collider> ()) {
			c.enabled = false;
		}
	}

	private void enableCollider()
	{
		Transform _thumbnail = gameObject.transform.Find ("thumbnail");

		foreach(Collider c in GetComponents<Collider> ()) {
			c.enabled = true;
		}
	}

	public void stopThumbRotation()
	{
		_inFront = false;
		_moveBack = true;
		_moveCloser = false;
		_isRotating = false;

		disableCollider ();

		if (_isPreviewing == true) {
			// We're done. reset thumbnail
			Transform t = gameObject.transform.Find ("thumbnail");
			t.GetComponent<Renderer>().material.mainTexture = _firstThumb;
			t.GetComponent<Renderer> ().enabled = true;

			// Hide preview panel
			t = gameObject.transform.Find ("preview");
			t.GetComponent<Renderer> ().enabled = false;

			_isPreviewing = false;
		}

		hideProgressBar ();
	}

	// Called when the thumbnail has reached its front position
	private void reachedFront()
	{
		showProgressBar ();

		_curRotationThumb = 1;
		
		_moveCloser = false;
		_isRotating = true;
		_inFront = true;

		if (true) {
			_isPreviewing = true;
			
#if UNITY_STANDALONE
			StartCoroutine (showPreview ());
#endif
		}
		//showPreview ();

//		InvokeRepeating ("rotateThumb", 0, 1);
	}

	#if UNITY_STANDALONE
	IEnumerator showPreview()
	{

		WWW www = new WWW ("https://smog-05.tnaflix.com/oculus/previews/7ccfcb1fa678d78284b7-1.ogv");
		
		MovieTexture m = www.movie;
		
		while (!m.isReadyToPlay)
			yield return www;

		if (_isPreviewing) {
			m.Play ();
		
			Transform t = gameObject.transform.Find ("thumbnail");
			t.GetComponent<Renderer> ().enabled = false;

			t = gameObject.transform.Find ("preview");
			t.GetComponent<Renderer> ().enabled = true;
			t.GetComponent<Renderer> ().material.mainTexture = m;
		}
	}
	#endif

	private void rotateThumb()
	{
		CancelInvoke ();
		
		if (_isRotating == false) {
			// We're done. reset thumbnail
			Transform t = gameObject.transform.Find ("thumbnail");
			t.GetComponent<Renderer>().material.mainTexture = _firstThumb;
			return;
		}

		string url = "http://img3.tnastatic.com/a16:8q80w256j/thumbs/" + _data.viddir + "/" + _curRotationThumb + "_" + _data.VID + "l.jpg";

		StartCoroutine (loadNextThumb (url, _curRotationThumb));

		_curRotationThumb++;

		if (_curRotationThumb > _maxRotationThumb)
			_curRotationThumb = 1;
	}

	void startVideo (string url)
	{
		GameObject player = GameObject.Find ("player");
		
		Vector3 pos = transform.position;
		pos.x = 0;
		pos.y = 0.7f;
		pos.z = 2.1f;
		player.transform.position = pos;
		
		Vector3 centerPt = _center;
		centerPt.y = 0.7f;
		
		player.transform.LookAt(centerPt);
//		player.transform.position = (player.transform.position - _center) * 0.50f;
		
#if UNITY_STANDALONE
		AVProQuickTime movie = qtPlayer.MovieInstance;
		if (movie != null)
			qtPlayer.UnloadMovie ();

		qtPlayer._playOnStart = true;
		qtPlayer._source = AVProQuickTimePlugin.MovieSource.URL;
		qtPlayer._filename = url;

		qtPlayer.LoadMovie ();
#endif
		Debug.Log("Start video: " + url);
	}

	IEnumerator getImage (string url)
	{
//		Texture tex = VideoCache.getCachedTexture (url);

		Transform t = gameObject.transform.Find ("thumbnail");
		
//		if (tex == null) {
			// Start a download of the given URL
			WWW www = new WWW (url);
			// wait until the download is done
			yield return www;

//			Debug.Log (url);

			www.LoadImageIntoTexture((Texture2D)t.GetComponent<Renderer> ().material.mainTexture);
//			t.GetComponent<Renderer> ().material.mainTexture = www.textureNonReadable;
			_firstThumb = t.GetComponent<Renderer> ().material.mainTexture;

//			VideoCache.setCachedTexture(url, _firstThumb);
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

		if (_isRotating == true) {
			Transform t = gameObject.transform.Find ("thumbnail");
			t.GetComponent<Renderer> ().material.mainTexture = tex;
		}

		InvokeRepeating ("rotateThumb", _rotationInterval, 1);
	}

	// Update is called once per frame
	void Update () {

		if (_activeTexture) {
//			_activeTexture.SetPixel (Random.Range (0, 128), Random.Range (0, 128), Color.red);
//			_activeTexture.Apply ();
		}

//		t.GetComponent<Renderer> ().material.mainTexture = tex;

		if (selected && _inFront && _progressBar.transform.localScale.x < _progressScale.x) {
			Vector3 size = _progressBar.transform.localScale;
			size.x += (0.50f * Time.deltaTime);

			if (size.x > _progressScale.x)
			{
				size.x = _progressScale.x;

				startVideo ("http://smog-05.tnaflix.com/01/013930fb091d29f48a09/out8.mp4");
//				startVideo ("http://smog-05.tnaflix.com/oculus/2cdb5eaa01977f56735e_720p_2_oculus.mp4");
//				startVideo ("http://smog-05.tnaflix.com/oculus/e39ebb0dfb67445919f9.mp4");
			}

			_progressBar.transform.localScale = size;
		}

		if (_moveCloser) {
			Vector3 destination = (origPosition - _center) * 0.65f;
//			transform.Translate(Vector3.forward*Time.deltaTime*6f);

			transform.position = Vector3.Lerp(transform.position, destination , 5f * Time.deltaTime);  

			float closeEnough = 0.010f;

			if (Vector3.Distance(destination, transform.position) < closeEnough)
			{
				reachedFront();
			}
		}

		if (_moveBack) {
			transform.position = Vector3.Lerp(transform.position, origPosition , 8f * Time.deltaTime);  
			
			float closeEnough = 0.010f;
			
			if (Vector3.Distance(origPosition, transform.position) < closeEnough)
			{
				transform.position = origPosition;
				_moveBack = false;

				enableCollider();
			}
		}
	}
}
