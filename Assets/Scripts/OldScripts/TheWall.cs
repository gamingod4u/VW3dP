using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContentColumn
{
	public int page;
	public float rotation;
	public GameObject[] obj;
}

public class TheWall : MonoBehaviour {

	public GameObject dataLoaderObject;

	private GameObject playerObject;
	private DataLoader _dataLoader;

	public int numObjects = 15;
	public float radius = 6.5f;
	public float rotateSpeed = 0.0f;
	public int videosPerCol = 4;
	public float vertDistance = 1.4f;

	private List<GameObject> _objects = new List<GameObject> ();

	private ContentColumn[] _contentGrid;

	// Indicate whether left/right scrolling is blocked
	// These get reset by scrolling in the opposite direction
	private bool haltPosScroll;
	private bool haltNegScroll;

	void Start() {

		_dataLoader = dataLoaderObject.GetComponent<DataLoader>();
		playerObject = GameObject.Find ("OVRPlaayerController");


		loadContent ();

		haltPosScroll = false;
		haltNegScroll = false;

	}

	void Update() {

		if (rotateSpeed != 0.0f) {

			if (rotateSpeed > 0 && haltNegScroll == true)
				haltNegScroll = false;
			if (rotateSpeed < 0 && haltPosScroll == true)
				haltPosScroll = false;

			if (!(rotateSpeed > 0 && haltPosScroll == true) && !(rotateSpeed < 0 && haltNegScroll == true))
			{

				Vector3 center = playerObject.transform.position;

				for (int col=0; col<numObjects; col++) {
					if (_contentGrid[col] == null)
						continue;

					bool bRowDeleted = false;

					for (int row=0;row<videosPerCol;row++) {

						if (_contentGrid[col].obj[row] == null)
							continue;

						GameObject obj = _contentGrid[col].obj[row];

						obj.transform.RotateAround(center, Vector3.up, rotateSpeed);

						// Same as above but without the transform object
						_dataLoader.setOriginalPosition(obj, Utils.RotateOrigPosition(_dataLoader.getOriginalPosition(obj), center, Vector3.up, rotateSpeed));

						// Update VideoColumn rotation
						_contentGrid[col].rotation = _contentGrid[col].obj[row].transform.rotation.eulerAngles.y;
					}

					if (bRowDeleted)
						_contentGrid[col] = null;
				}
			}

			// Find lowest and highest page
			int lowest = 999;
			int lowestIdx = 0;
			int highest = 0;
			int highestIdx = 0;

			int maxPage = (int)(Mathf.Floor(_dataLoader.getObjectCount() /videosPerCol)-1);

			// Find the highest and lowest page in our circle. 
			for (int col=0;col<numObjects;col++)
			{
				if (_contentGrid[col] != null && _contentGrid[col].page != -1)
				{
					if (_contentGrid[col].page < lowest) {
						lowest = _contentGrid[col].page;
						lowestIdx = col;
					}

					if (_contentGrid[col].page > highest) {
						highest = _contentGrid[col].page;
						highestIdx = col;
					}
				}
			}

			if (_contentGrid[lowestIdx].rotation > 180 && _contentGrid[lowestIdx].rotation < 190 && rotateSpeed < 0)
			{
				// Unload this column
				unloadColumn(lowestIdx);

			}else if (_contentGrid[lowestIdx].page > 0 && _contentGrid[lowestIdx].rotation > 190 && rotateSpeed > 0) {
				int prevcol = lowestIdx-1;
				if (prevcol == -1)
					prevcol = numObjects-1;

				loadColumn(prevcol, _contentGrid[lowestIdx].page-1);

			}else if (_contentGrid[lowestIdx].rotation > 0 && _contentGrid[lowestIdx].rotation < 30 && _contentGrid[lowestIdx].page == 0 && rotateSpeed > 0)
			{
				// End of the line reached
				haltPosScroll = true;
			}

			if (_contentGrid[highestIdx].rotation > 170 && _contentGrid[highestIdx].rotation < 180 && rotateSpeed > 0)
			{
				// Unload this column
				unloadColumn(highestIdx);

			}else if (_contentGrid[highestIdx].page < maxPage && _contentGrid[highestIdx].rotation < 170 && rotateSpeed < 0) {
				// First column is not being deleted, there is 1 free column before it and content is not on the first page
				// This means we can inject new content
				int nextcol = highestIdx+1;
				if (nextcol >= numObjects)
					nextcol = 0;

				loadColumn(nextcol, _contentGrid[highestIdx].page+1);

			}else if (_contentGrid[highestIdx].rotation > 0 && _contentGrid[highestIdx].rotation < 30 && _contentGrid[highestIdx].page == maxPage && rotateSpeed < 0)
			{
				// End of the line reached
				haltNegScroll = true;
			}			
		}
	}

	// Load specified page into column
	private void loadColumn(int col, int page)
	{
		for (int row=0;row<videosPerCol;row++) 
		{
			_dataLoader.loadObject(_contentGrid[col].obj[row],(page*videosPerCol)+row);
		}
		
		_contentGrid[col].page = page;
	}

	private void unloadColumn(int col)
	{
		for (int row=0; row<videosPerCol; row++) 
			_dataLoader.unloadObject (_contentGrid [col].obj [row]);

		_contentGrid[col].page = -1;
	}

	public void displayContent()
	{
		Vector3 center = playerObject.transform.position;

		_contentGrid = new ContentColumn[numObjects];

		int vididx = 0;
		int page = 0;

		for (int i = 0; i < numObjects; i++){

			int ioffset = (i-(numObjects/2));

			Vector3 pos = Utils.DrawCircle(ioffset, center, radius);
			Vector3 _direction = (center - pos).normalized;
			Quaternion _lookRotation = Quaternion.LookRotation(_direction);

			pos.y = 0-vertDistance;

			_contentGrid[i] = new ContentColumn();
			_contentGrid[i].page = -1;

			_contentGrid[i].obj = new GameObject[videosPerCol];

			if (i > numObjects/2)
			{
				_contentGrid[i].page = page;
				page++;
			}

			for (int idx=0; idx<videosPerCol; idx++)
			{
				GameObject o = (GameObject)_dataLoader.spawnObject(pos, _lookRotation);

				if (i > numObjects/2)
				{
					_dataLoader.loadObject(o, vididx++);
				}

				_contentGrid[i].obj[idx] = o;

				o.transform.Rotate(0,180,0);
				pos.y += vertDistance;
			}

			_contentGrid[i].rotation = _contentGrid[i].obj[0].transform.rotation.eulerAngles.y;
		}
	}

	public void onDataLoaded()
	{
		displayContent ();
	}

	public void loadContent()
	{
		_dataLoader.loadData (onDataLoaded);
	}
}
