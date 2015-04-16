using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

public class StateObject
{
	public Socket workSocket = null;
	public const int bufferSize = 2048;
	public byte[] buffer = new byte[bufferSize];
	public StringBuilder builder = new StringBuilder();
}


public class TcpClient  
{

	private const int portNum = 9998;
	
	public static ManualResetEvent connectDone = new ManualResetEvent(false);
	public static ManualResetEvent sendDone = new ManualResetEvent(false);
	public static ManualResetEvent receiveDone = new ManualResetEvent(false);
	
	private static string thisResponse;

	
	public static Socket StartClient(Socket socket, string hostName, int portNum)
	{
		StateObject state = new StateObject();
		thisResponse = "";
		try
		{
			IPHostEntry iphostInfo = Dns.GetHostEntry(hostName);
			IPAddress ipAddress = iphostInfo.AddressList[0];
			IPEndPoint remoteEp = new IPEndPoint(ipAddress, portNum);
			socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socket.BeginConnect(remoteEp, new AsyncCallback(ConnectCallback), socket);
			connectDone.WaitOne(2000);
			Send(socket, "003\n");
			sendDone.WaitOne(1000);
			Receive(socket);
			receiveDone.WaitOne(1000);		

			
		}
		catch(Exception e)
		{
			Debug.Log(e.ToString());
			return null;
		}
		
		return socket;
	}

	public static void Shutdown(Socket socket)
	{
		socket.Shutdown(SocketShutdown.Both);
		
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
			StateObject state = new StateObject();
			state.workSocket = socket;
			socket.BeginReceive(state.buffer,0,StateObject.bufferSize, 0, new AsyncCallback(ReceiveCallback), state);
		}
		catch(Exception e)
		{
			Debug.Log(e.ToString());
		}
	}
	private static int totalBytesRead = 0;
	
	private static void ReceiveCallback( IAsyncResult ar )
	{
		try 
		{
			StateObject state = (StateObject) ar.AsyncState;
			Socket client = state.workSocket;
			
			int bytesRead = client.EndReceive(ar);
			
			if (bytesRead > 0) 
			{
				state.builder.Append(Encoding.ASCII.GetString(state.buffer,0,bytesRead));
				client.BeginReceive(state.buffer,0,StateObject.bufferSize,0,new AsyncCallback(ReceiveCallback), state);
				totalBytesRead += bytesRead;
			} 
			else 
			{
				// All the data has arrived; put it in response.
				if (state.builder.Length > 1) 
				{
					thisResponse = state.builder.ToString();
					Debug.Log(thisResponse);
				}
				totalBytesRead = 0;
				// Signal that all bytes have been received.
				receiveDone.Set();
			}
		} 
		catch (Exception e) 
		{
			Console.WriteLine(e.ToString());
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
	

}
