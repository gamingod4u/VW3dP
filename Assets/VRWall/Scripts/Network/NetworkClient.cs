using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;


public class NetworkClient 
{

	public static Socket socket = null;
	
	private const int TIMEOUT = 5000;	
	private const int BUFFERSIZE = 2048;
	private static ManualResetEvent clientDone = new ManualResetEvent(false);
	
	public static string Connect(string hostOrIP, int portNum)
	{
		string results = string.Empty;
		
		IPHostEntry iphostInfo = Dns.GetHostEntry(hostOrIP);
		IPAddress ipAddress = iphostInfo.AddressList[0];
		IPEndPoint remoteEp = new IPEndPoint(ipAddress, portNum);
		
		socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		
		
		SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
		socketEventArg.RemoteEndPoint = remoteEp;
		
		
		socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(delegate (object s, SocketAsyncEventArgs e)
		{
			results = e.SocketError.ToString();
			clientDone.Set ();
		});
		
		clientDone.Reset();
		socket.ConnectAsync(socketEventArg);
		clientDone.WaitOne(TIMEOUT);
		return results;
	}
	
	public static string Receive()
	{
		string response = "Operation Timeout";
		
		if(socket != null)
		{
			SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
			socketEventArg.RemoteEndPoint = socket.RemoteEndPoint;
	
			socketEventArg.SetBuffer(new Byte[BUFFERSIZE], 0, BUFFERSIZE);
			
			socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(delegate(object s, SocketAsyncEventArgs e)
			{
				if (e.SocketError == SocketError.Success)
				{
				
					response = Encoding.ASCII.GetString(e.Buffer, e.Offset, e.BytesTransferred);
					
				}
				else
				{
					response = e.SocketError.ToString();
				}
				clientDone.Set ();
			});
			clientDone.Reset();
			socket.ReceiveAsync(socketEventArg);
			clientDone.WaitOne(TIMEOUT);
		}
		else
		{
			response = "Socket is not initialized!";
		}
		
		return response;
	}
		
	public static string Send(string requestCommand)
	{
		string response = "Operatin Timeout";
		
		if(socket != null)
		{
			SocketAsyncEventArgs socketEventargs = new SocketAsyncEventArgs();
			
			socketEventargs.RemoteEndPoint = socket.RemoteEndPoint;
			socketEventargs.UserToken = null;
			
			socketEventargs.Completed += new EventHandler<SocketAsyncEventArgs>(delegate(object s, SocketAsyncEventArgs e )
			{
				response = e.SocketError.ToString();
				clientDone.Set ();
			});
			
			byte [] payload = Encoding.ASCII.GetBytes(requestCommand);
			socketEventargs.SetBuffer(payload, 0 , payload.Length);
			
			clientDone.Reset();
			socket.SendAsync(socketEventargs);
			
			clientDone.WaitOne(TIMEOUT);
		}
		else
		{
			response = "Socket is not initialized!";
		}
		return response;
	}
	
	public static void Shutdown()
	{
		 if(socket != null)
		 {
		 	socket.Close();
		 }
	}			
}
