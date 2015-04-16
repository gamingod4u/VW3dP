using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;


public class NetworkTest : MonoBehaviour 
{
	Socket mySocket;
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(Input.GetKeyUp(KeyCode.S))
		{
			ClientSend();
		}
	}
	private void ClientSend()
	{
		
		mySocket = TcpClient.StartClient(mySocket, "ac.tnaflix.com", 9998);
		TcpClient.Send(mySocket, "004http://img.tnastatic.com/a3:2q81w278r/thumbs/36/4_1761957l.jpg\n");
		TcpClient.Receive(mySocket);
		
		
	}
	
	
	void OnDisable()
	{
		TcpClient.Shutdown(mySocket);
	}
}

