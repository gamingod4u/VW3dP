using UnityEngine;
using System.Collections;

public class ConsolScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{	
		Debug.Log(this.transform.rotation.eulerAngles.y);
	}
}
