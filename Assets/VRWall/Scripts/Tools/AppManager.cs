using UnityEngine;
using System.Collections;

public class AppManager : MonoBehaviour 
{
    public static AppManager instance;


    private bool isVRConnected = false;
    private string url = "";


    public bool VRConnected 
    {
        get { return isVRConnected; }
        set { isVRConnected = value;}
    }

    public string MovieURL 
    {
        get { return url;}
        set { url = value; }
    }

    void Awake() 
    {
        if (instance != null)
            Destroy(this);
        else
            instance = this;


        Screen.showCursor = false;
        if (OVRManager.display.isPresent)
            isVRConnected = true;
        else
            isVRConnected = false;
    }
	// Use this for initialization
	void Start () 
    {
        DontDestroyOnLoad(this.gameObject);

	}
    void OnDestroy() 
    {
        Screen.showCursor = true;
    }
	// Update is called once per frame
	void Update () 
    {
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}
}
