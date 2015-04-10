using UnityEngine;
using System.Collections;

public class Look2Select : MonoBehaviour 
{
	public delegate void HitEvent (string name);
	public static event HitEvent OnHit;


	public GameObject reticle;
	private GameObject _selectedObject;
	private Transform _cam;
	private RaycastHit _hit;

	// Use this for initialization
	void Start () 
	{
		_cam = GameObject.Find("OVRCameraRig/RightEyeAnchor").GetComponentInChildren<Camera>().transform;
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

			if (_selectedObject != null && _selectedObject.GetInstanceID () == _hit.transform.gameObject.GetInstanceID ())
				return;
			else if (_selectedObject != null && _selectedObject.GetInstanceID () != _hit.transform.gameObject.GetInstanceID ()) 
			{

			/*	if (_selectedObject.tag == "button") 
				{
					ButtonAction button = (ButtonAction)_selectedObject.GetComponent (typeof(ButtonAction));
		
					button.lookingAt (false);
				}
				
				if (_selectedObject.tag == "thumbnail") 
				{
					VideoObject video = (VideoObject)_selectedObject.GetComponent (typeof(VideoObject));
					video.selected = false;
					video.stopThumbRotation ();
				}
		*/
				_selectedObject = null;
			}
			else
			{

				/*if (_hit.transform.gameObject.tag == "button") 
				{
					ButtonAction button = (ButtonAction)_hit.transform.gameObject.GetComponent (typeof(ButtonAction));
					
					button.lookingAt (true);
					_selectedObject = _hit.transform.gameObject;
				}
				
				if (_hit.transform.gameObject.tag == "thumbnail") 
				{
					VideoObject video = (VideoObject)_hit.transform.gameObject.GetComponent (typeof(VideoObject));
					
					video.selected = true;
					video.startThumbRotation ();
					
					_selectedObject = _hit.transform.gameObject;
				}*/
			}
		} 
		else
			_selectedObject = null;
	}
}
