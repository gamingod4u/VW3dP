using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VideoCache {

	private static SortedList<string, WWW> thumbCache = new SortedList<string, WWW>();

	public static WWW getCachedTexture(string url)
	{
		if (thumbCache.ContainsKey (url))
			return thumbCache [url];
		else
			return null;
	}

	public static void setCachedTexture(string url, WWW tex)
	{
		thumbCache [url] = tex;
	}
}
