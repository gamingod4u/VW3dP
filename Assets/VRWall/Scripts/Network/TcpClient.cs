using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;





/*public class StateObjects
{
	public Socket workSocket = null;
	public const int bufferSize = 2048;
	public byte[] buffer = new byte[bufferSize];
	public StringBuilder builder = new StringBuilder();
}

*/
public class TcpClient  
{
	/*
	private const int portNum = 9998;
	
	private static ManualResetEvent connectDone = new ManualResetEvent(false);
	private static ManualResetEvent sendDone = new ManualResetEvent(false);
	private static ManualResetEvent receiveDone = new ManualResetEvent(false);
	
	private static string thisResponse;
	
	public static Socket client;
	
	public static void StartClient()
	{
		StateObjects state = new StateObjects();
		thisResponse = "";
		try
		{
			IPHostEntry iphostInfo = Dns.GetHostEntry("ac.tnaflix.com");
			IPAddress ipAddress = iphostInfo.AddressList[0];
			IPEndPoint remoteEp = new IPEndPoint(ipAddress, portNum);
			client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			
			client.BeginConnect(remoteEp, new AsyncCallback(ConnectCallback), client);
			connectDone.WaitOne(2000);
			
			Send(client, "");
			sendDone.WaitOne(2000);
			
			Receive(client);
			receiveDone.WaitOne(2000);
			
			Debug.Log("Response received: " + thisResponse.ToString());
			
		}
		catch(Exception e)
		{
			Debug.Log(e.ToString());
		}
		
	}

	public static void Shutdown()
	{
		client.Shutdown(SocketShutdown.Both);
		
	}
	private static void ConnectCallback(IAsyncResult ar)
	{
		try
		{
			Socket client = (Socket)ar.AsyncState;
			client.EndConnect(ar);
			Debug.Log("Socket connected to " + client.RemoteEndPoint.ToString());
			connectDone.Set();
		}
		catch(Exception e)
		{
			Debug.Log(e.ToString());
		}
		
	}
	public static void Receive(Socket socket)
	{
		try
		{
			StateObjects state = new StateObjects();
			state.workSocket = socket;
			state.buffer = new byte[2048];
			socket.BeginReceive(state.buffer,0,StateObjects.bufferSize, 0, new AsyncCallback(ReceiveCallback), state);
		}
		catch(Exception e)
		{
			Debug.Log(e.ToString());
		}
	}
	
	private static void ReceiveCallback(IAsyncResult ar)
	{
		
		try
		{
			StateObjects state = (StateObjects)ar.AsyncState;
			Socket client = state.workSocket;
			int byteRead = client.EndReceive(ar);
		
			if(byteRead > 0)
			{
				state.builder.Append(Encoding.ASCII.GetString(state.buffer, 0, byteRead));
				client.BeginReceive(state.buffer, 0, StateObjects.bufferSize, 0,new AsyncCallback(ReceiveCallback), state);
			}
			else
			{
				if(state.builder.Length > 1)
				{
					thisResponse = state.builder.ToString();
				}
				receiveDone.Set ();
			}
		}
		catch(Exception e)
		{
			Debug.Log(e.ToString());
		}
	}

	public static void Send(Socket socket, string data)
	{
		byte[] byteData = Encoding.ASCII.GetBytes(data);
		socket.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback),socket);
	}

	
	private static void SendCallback(IAsyncResult ar)
	{
		try
		{
			Socket client = (Socket)ar.AsyncState;
			int bytesSent = client.EndSend(ar);
			Debug.Log("Sent " + bytesSent + " bytes to server.");
			sendDone.Set();
		}
		catch(Exception e)
		{
			Debug.Log(e.ToString());
		}
	}
	
*/
}
