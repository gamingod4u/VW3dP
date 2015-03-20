using UnityEngine;
using System.Collections;

public class T9Look2Select : MonoBehaviour {
	
	private GameObject _selectedObject;
	
	// Use this for initialization
	void Start () {
		_selectedObject = null;
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit hit;
		
		Transform cam = GameObject.Find("OVRCameraRig/TrackingSpace/CenterEyeAnchor/Camera").GetComponentInChildren<Camera>().transform;
		
		bool hasHit = Physics.Raycast (cam.position, cam.forward, out hit);
		
		if (hasHit && _selectedObject != null && _selectedObject.GetInstanceID() == hit.transform.gameObject.GetInstanceID())
			return;
		
		// Check if a different object got selected
		if (_selectedObject != null && (!hasHit || (hasHit && _selectedObject.GetInstanceID() != hit.transform.gameObject.GetInstanceID()))) 
		{
			if (_selectedObject.tag == "button")
			{
				T9Button button = (T9Button)_selectedObject.GetComponent(typeof(T9Button));
				
				button.lookingAt(false);
			}
			
			_selectedObject = null;
		}
		
		if (hasHit) 
		{
			if (hit.transform.gameObject.tag == "button")
			{
				T9Button button = (T9Button)hit.transform.gameObject.GetComponent(typeof(T9Button));
				
				button.lookingAt(true);
				_selectedObject = hit.transform.gameObject;
			}			
		}
	}
}
