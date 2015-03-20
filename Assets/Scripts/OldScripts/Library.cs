using UnityEngine;
using System.Collections;
using System;

public abstract class DataLoader : MonoBehaviour 
{

	public abstract void loadObject(GameObject o, int idx);
	public abstract void unloadObject(GameObject o);

	public abstract Vector3 getOriginalPosition(GameObject o);
	public abstract void setOriginalPosition(GameObject o, Vector3 pos);

	public abstract GameObject spawnObject(Vector3 pos, Quaternion rotation);
	public abstract int getObjectCount();

	public abstract void loadData(Action callBack); 
}

public class VideoData 
{

	public int VID;
	public string viddir;
	public int first_thumb;
}
