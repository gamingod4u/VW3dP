using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;






public class ThreadSocketTest : MonoBehaviour
{
	
	public int 		portNum = 0;
	public string 	hostName = string.Empty;
	
	private EndPoint        bindEndPoint;
	private NetworkData		data;
	private Socket          receiveSocket;
	private Socket 			sendSocket;
	private byte[] 			recBuffer, buffer;
	
	void Start()
	{
			data = gameObject.GetComponent<NetworkData>();
			sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			IPAddress sendTo = FirstDnsEntry(hostName);
			EndPoint sendEndPoint = new IPEndPoint(sendTo, portNum);
			buffer = new byte[9999];
			buffer= System.Text.Encoding.ASCII.GetBytes("004https://img.tnastatic.com/a3:2q81w278r/thumbs/36/4_1761957l.jpg\n");
			sendSocket.SendTo(buffer, buffer.Length, SocketFlags.None,sendEndPoint);
			
			receiveSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			receiveSocket.Blocking = false;
			bindEndPoint = new IPEndPoint(IPAddress.Any, portNum);
			recBuffer = new byte[9999];
			receiveSocket.Bind(bindEndPoint);
			receiveSocket.BeginReceiveFrom(recBuffer, 0, recBuffer.Length, SocketFlags.None, ref bindEndPoint, new AsyncCallback(MessageReceivedCallback),(object)this); 		
	}

 	void MessageReceivedCallback(IAsyncResult result)
 	{
 		EndPoint remoteEndPoint  = new IPEndPoint(0,0);
 		try
 		{
			int bytesRead = receiveSocket.EndReceiveFrom(result, ref remoteEndPoint);
			Debug.Log(System.Text.Encoding.ASCII.GetString(recBuffer));
		}    
		catch (SocketException e) 
		{
			Console.WriteLine("Error: {0} {1}", e.ErrorCode, e.Message);
		}
		receiveSocket.BeginReceiveFrom(recBuffer, 0, recBuffer.Length, 
		                               SocketFlags.None, ref bindEndPoint, 
		                               new AsyncCallback(MessageReceivedCallback), (object)this);
		                               
		 data.FromBuffer(recBuffer, 0);
	}

	IPAddress FirstDnsEntry(string hostName)
	{
		IPHostEntry ipHost = Dns.GetHostEntry(hostName);
		IPAddress[] addr = ipHost.AddressList;
		if(addr.Length == 0)
			throw new Exception("No IpAddress!");
		
		return addr[0];
		
		
	}
}
