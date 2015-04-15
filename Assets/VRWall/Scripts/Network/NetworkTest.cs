using UnityEngine;
using System.Collections;

public class NetworkTest : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
			
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(Input.GetKeyDown(KeyCode.C))
		{
			TcpClient.StartClient();
		}
		if(Input.GetKeyDown(KeyCode.E))
		{
			ClientSend();
		}
	}
	private void ClientSend()
	{
		
		TcpClient.Send(TcpClient.client,"004http://img.tnastatic.com/a3:2q81w278r/thumbs/36/4_1761957l.jpg\n");
		TcpClient.Receive(TcpClient.client);
	}
	
	
	void OnDisable()
	{
		TcpClient.Shutdown();
	}
}

