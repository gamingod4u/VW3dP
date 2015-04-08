using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;

public class NetworkTest : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	public static void Recieve(Socket socket, byte[] Buffer, int offset, int size, int timeout)
	{
		int startTickCount = Environment.TickCount;
		int received = 0;
		
		do
		{
			if(Environment.TickCount > startTickCount + timeout)
				throw new Exception("Timeout.");
			try
			{
				received += socket.Receive(Buffer, offset + received, size - received, SocketFlags.None);
			}
			catch(SocketException ex)
			{
				if(	ex.SocketErrorCode == SocketError.WouldBlock 	||
					ex.SocketErrorCode == SocketError.IOPending		||
					ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
				{
						Thread.Sleep(30);
				}
				else
					throw ex;	
			}
		}while(received < size);
	}
	
	public static void Send(Socket socket, byte[] buffer, int offset, int size, int timeout)
	{
		int startTickCount = Environment.TickCount;
		int sent = 0;
		
		do
		{
			if(Environment.TickCount > startTickCount + timeout)
				throw new Exception("Timeout.");
				try
				{
					sent += socket.Send(buffer, offset + sent, size - sent, SocketFlags.None);
				}
				catch(SocketException ex)
				{
					if(	ex.SocketErrorCode == SocketError.WouldBlock 	|| 
						ex.SocketErrorCode == SocketError.IOPending 	||
						ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
					{
						Thread.Sleep(30);
					}
					else
						throw ex;					
				}
		}
		while (sent < size);
	}
	
}
