using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPClass : MonoBehaviour
{
	public int 			portNum;
	public string 		hostName;
	public string 		lastReceivedUDPPacket = "";
	public string 		allReceivedUDPPackets = ""; // clean up this from time to time!
	
	private Thread 		receivedThread;
	private IPAddress 	serverIpAddress;
	private IPEndPoint	remoteEndpoint;
	private UdpClient	client;
	private string 		strMessage = "";
	
	
	void Start()
	{
		remoteEndpoint = new IPEndPoint(FirstDnsEntry(hostName), portNum);
		client = new UdpClient();
		client.Client.Blocking = false;
		
		receivedThread = new Thread(
			new ThreadStart(ReceiveData));
		receivedThread.IsBackground = true;
		receivedThread.Start();
		
		
		
	}
	
	void OnDestroy() 
	{ 
		if ( receivedThread!= null) 
			receivedThread.Abort(); 
		
		client.Close(); 
	} 
	
	private void SendString(string message)
	{
		try
		{
			if (message != "")
			{
				byte[] data = Encoding.ASCII.GetBytes(message);
				client.Send(data, data.Length, remoteEndpoint);
			}
		}
		catch (Exception err)
		{
			Debug.Log(err.ToString());
		}
	}
	
	private  void ReceiveData()
	{
		
		client = new UdpClient(portNum);
		//client.Client.Blocking = false;
		while (true)
		{
			
			try
			{
				
				IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, portNum);
				byte[] data = client.Receive(ref anyIP);
				string text = Encoding.ASCII.GetString(data);
				Debug.Log(">> " + text);
				
				lastReceivedUDPPacket=text;
				allReceivedUDPPackets=allReceivedUDPPackets+text;
				
			}
			catch (Exception err)
			{
				print(err.ToString());
			}
		}
	}

	public string getLatestUDPPacket()
	{
		allReceivedUDPPackets="";
		return lastReceivedUDPPacket;
	}

	private IPAddress FirstDnsEntry(string hostName)
	{
		IPHostEntry ipHost = Dns.GetHostEntry(hostName);
		IPAddress[] addr = ipHost.AddressList;
		if(addr.Length == 0)
			throw new Exception("No IpAddress!");
	
		return addr[0];
	}
	
	void OnGUI()
	{
		Rect rectObj=new Rect(40,380,200,400);
		GUIStyle style = new GUIStyle();
		style.alignment = TextAnchor.UpperLeft;
		GUI.Box(rectObj,"# UDPSend-Data: " + FirstDnsEntry(hostName) + " #\n"
		        + "shell> nc -lu 127.0.0.1  "+portNum+" \n"
		        ,style);
		
		// ------------------------
		// send it
		// ------------------------
		strMessage=GUI.TextField(new Rect(600,450,140,40),strMessage);
		if (GUI.Button(new Rect(600,420,40,20),"send"))
		{
			SendString(strMessage);
		}  
		
		    
		Rect rectObj2=new Rect(40,10,20,400);
		GUIStyle style2 = new GUIStyle();
		style2.alignment = TextAnchor.UpperCenter;
		GUI.Box(rectObj2,"# UDPReceive\n127.0.0.1 "+portNum+" #\n"
		        + "shell> nc -u 127.0.0.1 : "+portNum+" \n"
		        + "\nLast Packet: \n"+ lastReceivedUDPPacket
		        + "\n\nAll Messages: \n"+allReceivedUDPPackets
		        ,style2);            
	}
	
}



