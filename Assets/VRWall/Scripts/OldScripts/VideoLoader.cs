using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class VideoLoader : DataLoader {

	public GameObject 		spawnPrefab;
	public string			loaderType;
	public string			loaderArgument;

	private VideoDataFeed 	_dataFeed;
	private List<VideoData> _data;

	public void Awake()
	{
		_dataFeed = new VideoDataFeed ();
	}

	public override void loadObject(GameObject o, int idx)
	{
		VideoObject video = o.GetComponent<VideoObject>();

		if (video) {
			video.loadVideo(_data[idx]);
		}
	}

	public override void unloadObject(GameObject o)
	{
		VideoObject video = o.GetComponent<VideoObject> ();
		
		if (video) {
			video.unloadVideo();
		}
	}
	
	public override GameObject spawnObject(Vector3 pos, Quaternion rotation)
	{
		GameObject o = (GameObject)Instantiate(spawnPrefab, pos, rotation);

		return o;
	}

	public override int getObjectCount()
	{
		return _data.Count;
	}

	public void onVideosLoaded(List<VideoData> d, Action callBack)
	{
		_data = d;

		callBack ();
	}

	public override Vector3 getOriginalPosition(GameObject o)
	{
		VideoObject video = o.GetComponent<VideoObject> ();

		return video.origPosition;
	}

	public override void setOriginalPosition(GameObject o, Vector3 pos)
	{
		VideoObject video = o.GetComponent<VideoObject> ();

		video.origPosition = pos;
	}

	public override void loadData(Action callBack)
	{
		List<VideoData> data = new List<VideoData> ();

		if (loaderType == "featured") {
			StartCoroutine (_dataFeed.loadJSON ("https://alpha.tnaflix.com/json_test.php", data, callBack, onVideosLoaded));
		}
	}
}
