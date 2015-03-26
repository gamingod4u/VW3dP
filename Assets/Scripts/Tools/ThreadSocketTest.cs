using UnityEngine;
using System.Collections;

using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class ThreadSocketTest : MonoBehaviour 
{
	private Thread 				recieveThread;
	private UdpClient			client;

	// Use this for initialization
	void Start () 
	{
		Init();

	}

	void Init()
	{
		recieveThread = new Thread(new ThreadStart(ReceivedData));

		recieveThread.IsBackground = true;
		recieveThread.Start();
	}

	private void ReceivedData()
	{
		try 
		{	

		}
		catch(Exception e)
		{
			Debug.Log(e);
		}
	}

	private void SendData()
	{
		try 
		{	
			
		}
		catch(Exception e)
		{
			Debug.Log(e);
		}
	}

	void OnApplicationQuit()
	{
		recieveThread.Abort(); 

		if(client != null)
			client.Close();
	}
}
