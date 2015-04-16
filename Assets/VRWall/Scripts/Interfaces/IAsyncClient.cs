using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;


public interface IAsyncClient :  IDisposable
{
	void Init(string hostName, int portNum);
	
	void StartClient();

	bool IsConnected();

	void Receive();

	void Send(string msg, bool close);
}
