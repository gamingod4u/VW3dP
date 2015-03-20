using UnityEngine;
using System.Collections;

public class T9Look2Select : MonoBehaviour 
{
	
	private GameObject _selectedObject;
	private Transform  _cam;
	private RaycastHit _hit;
	// Use this for initialization
	void Start () 
	{
		_cam = GameObject.Find("OVRCameraRig/TrackingSpace/CenterEyeAnchor/Camera").GetComponentInChildren<Camera>().transform;
		_selectedObject = null;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Physics.Raycast (_cam.position, _cam.forward, out _hit)) 
		{
			if (_selectedObject != null && _selectedObject.GetInstanceID () == _hit.transform.gameObject.GetInstanceID ())
				return;
			else if (_selectedObject != null && _selectedObject.GetInstanceID () != _hit.transform.gameObject.GetInstanceID ()) 
			{
				if (_selectedObject.tag == "button") 
				{
					T9Button button = (T9Button)_selectedObject.GetComponent (typeof(T9Button));
					
					button.lookingAt (false);
				}
				
				_selectedObject = null;
			} 
			else 
			{
				if (_hit.transform.gameObject.tag == "button") 
				{
					T9Button button = (T9Button)_hit.transform.gameObject.GetComponent (typeof(T9Button));
					
					button.lookingAt (true);
					_selectedObject = _hit.transform.gameObject;
				}
			}
		}
		else 
			_selectedObject = null;
	}
}
