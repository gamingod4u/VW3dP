using UnityEngine;
using System.Collections;

public class Look2Select : MonoBehaviour 
{
	public delegate void HitEvent (string name);
	public static event HitEvent OnHit;
    public WallController theWall;
    public GameObject reticle;
    public bool canSelect = false;

   
    private GameObject _selectedObject;
    private HeadTracking headTracker;
	private Transform _cam;
	private RaycastHit _hit;
    
	// Use this for initialization
	void Start () 
	{

        if (AppManager.instance.VRConnected)
            _cam = GameObject.Find("OVRPlayerController").GetComponent<Transform>();
        else
            _cam = GameObject.Find("Camera").GetComponent<Transform>();

        theWall = GameObject.Find("TheWall").GetComponent<WallController>();
        headTracker = GameObject.Find("HeadTracking").GetComponent<HeadTracking>();
		_selectedObject = null;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
		if (Physics.Raycast (_cam.position, _cam.forward, out _hit)) 
		{	
			if(OnHit != null)
				OnHit(_hit.transform.name);

			reticle.transform.position = _hit.point;
            Debug.DrawLine(_cam.position, _hit.point, Color.red);
      
            if (_selectedObject != null && _selectedObject.GetInstanceID() == _hit.transform.gameObject.GetInstanceID())
            {
                
                return;
            }
            else if (_selectedObject != null && _selectedObject.GetInstanceID() != _hit.transform.gameObject.GetInstanceID())
            {
               
                if (_selectedObject.tag == "button")
                {
                    ButtonAction button = (ButtonAction)_selectedObject.GetComponent(typeof(ButtonAction));
                    button.lookingAt(false);
                }

                if (_selectedObject.tag == "thumbnail")
                {
                    VideoObject video = (VideoObject)_selectedObject.GetComponent(typeof(VideoObject));
                    video.selected = false;
                    video.stopThumbRotation();
                    headTracker.canRotate = false;
                }
                         
                _selectedObject = null;
            }
            else
            {

                if (canSelect)
                {
                    if (_hit.transform.gameObject.tag == "button")
                    {
                        ButtonAction button = (ButtonAction)_hit.transform.gameObject.GetComponent(typeof(ButtonAction));
                        button.lookingAt(true);
                        _selectedObject = _hit.transform.gameObject;
                    }

                    if (_hit.transform.gameObject.tag == "thumbnail")
                    {
                        VideoObject video = (VideoObject)_hit.transform.gameObject.GetComponent(typeof(VideoObject));
                        video.selected = true;
                        video.startThumbRotation();
                        _selectedObject = _hit.transform.gameObject;
                    }
                }
            }
		} 
		else
			_selectedObject = null;
	}


    public IEnumerator Wait2Select(float time) 
    {
        yield return new WaitForSeconds(time);
        canSelect = true;
    }
}
