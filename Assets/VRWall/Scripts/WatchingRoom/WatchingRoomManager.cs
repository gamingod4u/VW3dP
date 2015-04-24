using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class WatchingRoomManager : MonoBehaviour
{

    #region Class Variables
    public GameObject playButton;
    public GameObject pauseButton;
    public GameObject stopButton;
    private GameObject moviePlayer;
    private LookSelection lookSelect;
    private string url = string.Empty;
    
#if UNITY_STANDALONE
    private AVProQuickTimeMovie qtManager;

#endif

    #endregion

    #region Unity Functions

    void Awake() 
    {
        VideoButtons.ButtonPressed += new VideoButtons.VideoEvent(ButtonHit);
    }
    // Use this for initialization
	void Start () 
    {
        if (AppManager.instance.MovieURL == string.Empty)
            url = "http://smog-05.tnaflix.com/01/013930fb091d29f48a09/out8.mp4";
        else
            url = AppManager.instance.MovieURL;

        lookSelect = GameObject.Find("Look2Select").GetComponent<LookSelection>();
        StartCoroutine(lookSelect.Wait2Select());
        moviePlayer = GameObject.Find("MoviePlayer2");
        moviePlayer.SetActive(false);

       
#if UNITY_STANDALONE
        qtManager = moviePlayer.GetComponent<AVProQuickTimeMovie>();
#endif
        
        LoadMovie();
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    void OnDestroy() 
    {
        VideoButtons.ButtonPressed -= new VideoButtons.VideoEvent(ButtonHit);
    }
    #endregion

    #region Class Functions
    private void ButtonHit(string name) 
    {
        switch (name) 
        {
            case "play": 
            {
                if (playButton.activeInHierarchy) 
                {
                    playButton.SetActive(false);
                    AVProQuickTime movie = qtManager.MovieInstance;

                    if (!movie.IsPlaying)
                        qtManager.Play();

                    stopButton.SetActive(true);
                    pauseButton.SetActive(true);
                }
            } break;

            case "stop": 
            {
                if (stopButton.activeInHierarchy) 
                {
                    stopButton.SetActive(false);

                    AVProQuickTime movie = qtManager.MovieInstance;

                    if (movie.IsPlaying)
                        qtManager.Pause();

                    playButton.SetActive(true);
                    pauseButton.SetActive(true);
                }

            } break;

            case "pause": 
            {
                if (pauseButton.activeInHierarchy) 
                {
                    pauseButton.SetActive(false);

                    AVProQuickTime movie = qtManager.MovieInstance;

                    if (movie.IsPlaying)
                        qtManager.Pause();

                    playButton.SetActive(true);
                    stopButton.SetActive(true);
                }
            } break;
            case "fastforward": 
            {
                AVProQuickTime movie = qtManager.MovieInstance;
                movie.PositionSeconds += (16 * Time.deltaTime);
                

            } break;
            case "rewind": 
            {
                AVProQuickTime movie = qtManager.MovieInstance;
                movie.PositionSeconds -= (16 * Time.deltaTime);
               
            } break;
			case "home":
			{
				
			}break;

        }
    }
    private void LoadMovie() 
    {
        moviePlayer.SetActive(true);
#if UNITY_STANDALONE
      
        AVProQuickTime movie = qtManager.MovieInstance;

        if (movie != null)
            qtManager.UnloadMovie();

        qtManager._source = AVProQuickTimePlugin.MovieSource.URL;
        qtManager._filename = url;
        qtManager._playOnStart = true;
        qtManager.LoadMovie();

#endif
		StartCoroutine(WaitForPlay());
        Debug.Log("Started Movie:");
    }

	private IEnumerator WaitForPlay()
	{
		yield return new WaitForSeconds(1);
		playButton.GetComponent<VideoButtons>().isActive = false;
		playButton.SetActive(false);
	}
    #endregion
}
