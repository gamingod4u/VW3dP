using UnityEngine;
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class VideoDataFeed 
{
	public IEnumerator loadJSON (string url, List<VideoData> data, Action callBack, Action<List<VideoData>, Action> dataCallback)
	{
		Debug.Log (url);
		// Start a download of the given URL
		WWW www = new WWW(url);
		// wait until the download is done
		yield return www;
		
		JSONNode N = JSON.Parse (www.text);
		
		for (int i=0; i<N.Count; i++)
		{
			data.Add(newVideoData(N[i]["VID"].AsInt, N[i]["viddir"].Value, N[i]["first_thumb"].AsInt));
		}
		Debug.Log ("FoundX " + data.Count + " videos");

		dataCallback (data, callBack);
		yield break;
	}

	private VideoData newVideoData(int iVID, string sviddir, int ifirst_thumb)
	{
		VideoData data = new VideoData ();
		
		data.VID = iVID;
		data.viddir = sviddir;
		data.first_thumb = ifirst_thumb;
		
		return data;
	}
}
