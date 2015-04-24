using UnityEngine;
using System.Collections;

public class VideoButtons : MonoBehaviour
{
    #region Class Variables
    public delegate void VideoEvent(string name);
    public static event VideoEvent ButtonPressed;


    private Renderer    enabledMat;
    private Texture     disabledMat;
    private string lastName = string.Empty;
    private float selectTime = 1;
    private float selectTimer = 0;
    public bool selected = false;
    public bool isActive = false;
    #endregion 

    #region Unity Functions
    // Use this for initialization
	void Start () 
    {
        Transform t = gameObject.transform.Find("enabled");
        enabledMat = t.gameObject.GetComponent<Renderer>();
        t.SetParent(gameObject.transform.parent);
        disabledMat = this.renderer.material.mainTexture;

		if(this.transform.name == "play")
		{
			isActive = false;
			this.gameObject.SetActive(false);

		}
 	}
	
	// Update is called once per frame
	void Update () 
    {
        if (selected)
        {
            this.renderer.material.SetTexture(0,enabledMat.material.mainTexture);
            CheckButton();
        }
        else
        {
            this.renderer.material.mainTexture = disabledMat;
            isActive = false;
        }
    }

    #endregion

    #region Class Functions
    private void CheckButton()
    {
        switch (transform.name) 
        {
            case "play": 
            {
                if (!isActive) 
                {
                    if (ButtonPressed != null)
                        ButtonPressed("play");

                    isActive = true;
                }
            }
            break;
            case "stop":
            {
                if (!isActive)
                {
                    if (ButtonPressed != null)
                        ButtonPressed("stop");

                    isActive = true;
                }
            }
            break;
            case "pause":
            {
                if (!isActive)
                {
                    if (ButtonPressed != null)
                        ButtonPressed("pause");

                    isActive = true;
                }
            }
            break;
            case "fastforward": 
            {
                if (ButtonPressed != null)
                    ButtonPressed("fastforward");
            }break;
            case "rewind":
            {
                if (ButtonPressed != null)
                    ButtonPressed("rewind");
            } break;
			case "home":
			{
				if (ButtonPressed != null)
					ButtonPressed("home");
			} break;
			case "logout":
			{
				if (ButtonPressed != null)
					ButtonPressed("logout");
			} break;
        }
    
    }
    public void LookingAt(bool state) 
    {
        if (state)
            selected = true;
        else
            selected = false;

    }


    #endregion
}
