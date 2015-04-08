using UnityEngine;
using System.Collections;

public class StateManager : MonoBehaviour 
{
	public enum AppState
	{
		LOADING, SCANNING, INGUAGING, ENJOYING
	};

	public delegate void AppUpdate();
	public static event AppUpdate OnUpdate;

	public static AppState appState;

	// Use this for initialization
	void Start () 
	{
		appState = AppState.LOADING;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (OnUpdate != null)
			OnUpdate ();
	}
}
