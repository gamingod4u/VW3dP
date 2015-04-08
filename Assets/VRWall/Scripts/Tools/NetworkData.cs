using UnityEngine;
using System.Collections;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

public class NetworkData : MonoBehaviour 
{

	public byte code, url;
		
	public int ToBuffer(byte[] buffer, int pos)
	{
		int newPos = pos;
		buffer[newPos++] = code;
		buffer[newPos++] = url;
		return newPos - pos;
	}
	
	public void FromBuffer(byte[] buffer, int pos)
	{
		string message = System.Text.Encoding.UTF8.GetBytes(buffer, 0, buffer.Length);
	}
}