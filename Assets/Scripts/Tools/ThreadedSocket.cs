using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

public class ThreadedSocket : MonoBehaviour {
	
	
	public delegate void RequestReceivedEventHandler(string message);
	public event RequestReceivedEventHandler OnRequestReceived;
	
	// Use this to trigger the event
	protected virtual void ThisRequestReceived(string message)
	{
		RequestReceivedEventHandler handler = OnRequestReceived;
		if(handler != null)
		{
			handler(message);
		}
	}
	
	// We use this to keep tasks needed to run in the main thread
	private static readonly Queue<Action> tasks = new Queue<Action>();
	
	public int requestPort = 9998;
	private string hostName = "a7x.overflow.biz";
	
	UdpClient udpRequestSender;
	UdpClient udpRequestReceiver;
	
	
	// Use this for initialization
	void Start () {
		IPHostEntry ipHostInfo = Dns.GetHostEntry(hostName);
		IPAddress ipAddress = ipHostInfo.AddressList[0];
		
		// Set up the sender for requests
		this.udpRequestSender = new UdpClient();
		IPEndPoint requestGroupEP = new IPEndPoint(ipAddress, this.requestPort);
		this.udpRequestSender.Connect(requestGroupEP);
		
		
		// Set up the receiver for the requests
		// Listen for anyone looking for us
		this.udpRequestReceiver = new UdpClient();
		this.udpRequestReceiver.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
		IPEndPoint request2GroupEP = new IPEndPoint(IPAddress.Any, this.requestPort);
		this.udpRequestReceiver.Client.Bind(request2GroupEP);
		this.udpRequestReceiver.BeginReceive(new AsyncCallback(AsyncRequestReceiveData), null);
		
		// Listen for the request
		this.OnRequestReceived += (message) => {
			Debug.Log("Request Received: " + message);
			// Do some more stuff when we get a request
			// Use `Network.maxConnections` for example
		};
		
		// Send out the request
		this.SendRequest();
		
	}
	
	void Update () {
		this.HandleTasks();
	}
	
	void HandleTasks() {
		while (tasks.Count > 0)
		{
			Action task = null;
			
			lock (tasks)
			{
				if (tasks.Count > 0)
				{
					task = tasks.Dequeue();
				}
			}
			
			task();
		}
	}
	
	public void QueueOnMainThread(Action task)
	{
		lock (tasks)
		{
			tasks.Enqueue(task);
		}
	}
	
	void SendRequest()
	{
		try
		{
			string message = "004https://img.tnastatic.com/a3:2q81w278r/thumbs/36/4_1761957l.jpg";
			
			if (message != "")
			{
				Debug.Log("Sendering Request: " + message);
				this.udpRequestSender.Send(System.Text.Encoding.ASCII.GetBytes(message),message.Length);
			}
		}
		catch (ObjectDisposedException e)
		{
			Debug.LogWarning("Trying to send data on already disposed UdpCleint: " + e);
			return;
		}
	}
	
	
	void AsyncRequestReceiveData(IAsyncResult result)
	{
		IPEndPoint receiveIPGroup = new IPEndPoint(IPAddress.Any, this.requestPort);
		byte[] received;
		if (this.udpRequestReceiver != null) {
			received = this.udpRequestReceiver.EndReceive(result, ref receiveIPGroup);
		} else {
			return;
		}
		this.udpRequestReceiver.BeginReceive (new AsyncCallback(AsyncRequestReceiveData), null);
		string receivedString = System.Text.Encoding.ASCII.GetString(received);
		Debug.Log(receivedString);
		
		this.QueueOnMainThread(() => {
			// Fire the event
			this.ThisRequestReceived(receivedString);
		});
		
	}
	
	
}