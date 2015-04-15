using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;



public interface IStateObject
{
	int 	BufferSize 	{get;}

	int 	Id 			{get;}

	bool 	Close 		{get; set;}

	byte[] 	Buffer 		{get;}

	Socket 	Listener 	{get;}

	string  Response	{get;}

	void 	Append(string text);
	void 	Reset();
}
