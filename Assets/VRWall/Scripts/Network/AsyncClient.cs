using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;


public delegate void ConnectedHandler(IAsyncClient a);
public delegate void ClientMessageReceivedHandler(IAsyncClient a, string msg);
public delegate void ClientMessageSubmittedHandler(IAsyncClient a, bool close);

public sealed class AsyncClient : IAsyncClient 
{
	public static int portNum;
	public static string hostName;

	private Socket listener; 
	private bool 	close;

	private readonly ManualResetEvent connect = new ManualResetEvent(false);
	private readonly ManualResetEvent sent = new ManualResetEvent(false);
	private readonly ManualResetEvent received = new ManualResetEvent(false);

	public event ConnectedHandler Connected;
	public event ClientMessageReceivedHandler MessageReceived;
	public event ClientMessageSubmittedHandler MessageSubmitted;

	public void StartClient()
	{
		IPHostEntry host = Dns.GetHostEntry(hostName);
		IPAddress ipAddress = host.AddressList[0];
		IPEndPoint remoteEndpoint = new IPEndPoint(ipAddress, portNum);

		try
		{
			listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			listener.BeginConnect(remoteEndpoint, OnConnectCallback, listener);
			connect.WaitOne(5000);

			var connectHandler = Connected;

			if(connectHandler != null)
				connectHandler(this);
		}
		catch (SocketException e)
		{
			Debug.Log(e.ToString());
		}
	}

	public bool IsConnected()
	{
		return !(listener.Poll(1000, SelectMode.SelectRead) && listener.Available == 0);
	}

	private void OnConnectCallback(IAsyncResult result)
	{
		var server = (Socket) result.AsyncState;

		try
		{
			server.EndConnect(result);
			connect.Set();
		}
		catch(SocketException e)
		{
			Debug.Log(e.ToString());
		}
	}

	public void Receive()
	{
		var state = new StateObject(this.listener);

		state.Listner.BeginReceive(state.Buffer, 0, state.BufferSize, SocketFlags.None, this.ReceiveCallback, state);
	}

	private void ReceiveCallback(IAsyncResult result)
	{
		var state = (IStateObject) result.AsyncState;
		var receive = state.Listener.EndReceive(result);
		if(receive > 0)
		{
			state.Append(Encoding.ASCII.GetString(state.Buffer,0,receive));
		}
		if(receive == state.BufferSize)
		{
			state.Listener.BeginReceive(state.Buffer, 0, state.BufferSize, SocketFlags.None, this.ReceiveCallback, state);
		}
		else
		{
			var messageReceive = this.MessageReceived;

			if(messageReceive != null)
			{
				messageReceive(this, state.Response);
			}
			state.Reset();
			this.received.Set();
		}
	}
	public void Send(string msg, bool close)
	{
		if(!this.IsConnected())
			throw new Exception("Socket is not connected to server");

		var response  = Encoding.ASCII.GetBytes(msg);

		this.close = close;
		this.listener.BeginSend(response, 0 , response.Length, SocketFlags.None, this.SendCallBack, this.listener);
	}

	private void SendCallBack(IAsyncResult result)
	{
		try
		{
			var receiver = (Socket)result.AsyncState;
			receiver.EndSend(result);
		}
		catch(ObjectDisposedException e)
		{
			Debug.Log(e.ToString());
		}

		var messageSubmitted = this.MessageSubmitted;

		if(messageSubmitted != null)
		{
			messageSubmitted(this, this.close);
		}

		this.sent.Set();
	}
	public void Close()
	{
		try
		{
			if(!this.IsConnected())
				return;

			this.listener.Shutdown(SocketShutdown.Both);
			this.listener.Close();
		}
		catch(Exception e)
		{
			Debug.Log(e.ToString());
		}
	}

	public void Dispose()
	{
		this.connect.Close();
		this.sent.Close();
		this.received.Close();
		this.Close();
	}
}
