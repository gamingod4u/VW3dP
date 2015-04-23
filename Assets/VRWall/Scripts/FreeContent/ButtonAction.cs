using UnityEngine;
using System.Collections;

public class ButtonAction : MonoBehaviour {

	public bool selected;
	public float selectSpeed;
#if UNITY_STANDALONE
	public AVProQuickTimeMovie qtPlayer;
#endif
	public string btnType;

	private Renderer _progressRender;
	private GameObject _progressBar;
	private Vector3 _progressScale;

	private bool _btnActivated;

	// Use this for initialization
	void Start () {
		Transform t = gameObject.transform.Find ("progress");
		_progressBar = t.gameObject;
		_progressRender = t.GetComponent<Renderer> ();
		_progressScale = t.localScale;

		if (selectSpeed == 0)
			selectSpeed = 0.25f;

		hideProgressBar ();
	}

	public void lookingAt(bool state)
	{
		if (state == true) {
			showProgressBar ();
			selected = true;
		} else {
			hideProgressBar ();
			selected = false;
			_btnActivated = false;
		}
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

	// Update is called once per frame
	void Update () {
	
		if (selected && _progressBar.transform.localScale.x <= _progressScale.x) {
			Vector3 size = _progressBar.transform.localScale;
			size.x += (selectSpeed * Time.deltaTime);
			
			if (size.x > _progressScale.x)
			{
				size.x = _progressScale.x;	

				if (btnType == "home")
				{
					Application.LoadLevel("mainmenu");
				}

				if (btnType == "T9")
				{
					Application.LoadLevel("t9input");
				}
				if (btnType == "start_featured")
				{
					Application.LoadLevel("VideoWall3D");
/*
					GameObject spawnCircle = GameObject.Find ("SpawnCircle");
					SpawnCircle circle = (SpawnCircle)spawnCircle.GetComponent(typeof(SpawnCircle));

					GameObject mainGUI = GameObject.FindGameObjectWithTag("maingui");
					Destroy (mainGUI);

					circle.LoadFeaturedVideos();
*/				}

				if (btnType == "seekbck")
				{
#if UNITY_STANDALONE
					AVProQuickTime movie = qtPlayer.MovieInstance;
					movie.PositionSeconds -= (16 * Time.deltaTime);
#endif
				}

				if (btnType == "seekfwd")
				{
#if UNITY_STANDALONE
					AVProQuickTime movie = qtPlayer.MovieInstance;
					movie.PositionSeconds += (16 * Time.deltaTime);
#endif
				}

				if (btnType == "play" && !_btnActivated)
				{
#if UNITY_STANDALONE
					AVProQuickTime movie = qtPlayer.MovieInstance;
					if (movie.IsPlaying)
						qtPlayer.Pause();
					else
						qtPlayer.Play();

					_btnActivated = true;
#endif
				}

				if (btnType == "close")
				{
#if UNITY_STANDALONE
					qtPlayer.UnloadMovie();

					GameObject player = GameObject.Find ("player");
					
					Vector3 pos = transform.position;
					pos.x = 0;
					pos.y = -500f;
					pos.z = 0f;
					player.transform.position = pos;
#endif
				}
			}
			
			_progressBar.transform.localScale = size;
		}
		
	}
}
