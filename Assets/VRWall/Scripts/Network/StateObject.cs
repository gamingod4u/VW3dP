using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;


public sealed class StateObject : IStateObject 
{
	private const int bufferSize = 2048;
	private readonly byte[] buffer = new Byte[bufferSize];
	private readonly Socket listener;
	private readonly int 	id;
	private StringBuilder 	sb;


	public StateObject(Socket listener, int id = -1)
	{
		this.listener = listener;
		this.id = id;
		this.Close = false;
		this.Reset();
	}
	public int ID
	{
		get{return this.id;}
	}

	public bool Close{get;set;}

	public int BufferSize 
	{
		get {return bufferSize;}
	}

	public byte[] Buffer
	{
		get{return buffer;}
	}

	public Socket Listner
	{
		get
		{
			return this.listener;
		}
	}

	public void Append(string text)
	{
		this.sb.Append(text);
	}

	public void Reset()
	{
		this.sb = new StringBuilder();
	}
}
