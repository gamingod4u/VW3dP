using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ContentColumn
{
	public int page;
	public float rotation;
	public GameObject[] obj;

}

public class WallController : MonoBehaviour
{

    #region Class Variables
    public List<GameObject> 	videoObjects;
	public GameObject			playerObject;
	public DataLoader 			dataLoader;
	public float 				rotateSpeed = 0;

	private List<GameObject> 	objects;
	private ContentColumn[] 	contentGrid;
    private Look2Select         look2;
	private bool 				haltNegScroll;
	private bool				haltPosScroll;

	private int 				objectCount = 0;
	private int 				MaxColumns = 15;
	private int 				MaxRows = 4;
    #endregion

    #region Unity Functions
    void Awake() 
    {
        
    }
	// Use this for initialization
	void Start () 
	{
        look2 = GameObject.Find("Look2Select").GetComponent<Look2Select>();
        look2.enabled = false;
    	contentGrid = new ContentColumn[MaxColumns];
		dataLoader = GameObject.FindGameObjectWithTag ("DataLoader").GetComponent<DataLoader> ();
		LoadContent ();

        if (AppManager.instance.VRConnected)
            playerObject = GameObject.Find("OVRPlayerController");
        else
            playerObject = GameObject.Find("Camera");
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (rotateSpeed != 0.0f) 
		{
			if (rotateSpeed > 0 && haltNegScroll == true)
				haltNegScroll = false;
			if (rotateSpeed < 0 && haltPosScroll == true)
				haltPosScroll = false;

			if (!(rotateSpeed > 0 && haltPosScroll == true) && !(rotateSpeed < 0 && haltNegScroll == true) && look2.canSelect)
			{
				float angle = 0;
				for (int col = 0; col < MaxColumns; col++)
				{
			
					Vector3 directionToTarget = contentGrid[col].obj[0].transform.position - playerObject.transform.position;
					angle = Vector3.Angle(playerObject.transform.forward, directionToTarget);
					contentGrid[col].rotation = angle;
				
				}
			}
			// Find lowest and highest page
			int lowest = 999;
			int lowestIdx = 0;
			int highest = 0;
			int highestIdx = 0;
			
			int maxPage = (int)(Mathf.Floor(dataLoader.getObjectCount() / MaxRows) - 1);
			
			// Find the highest and lowest page in our circle. 
			for (int col = 0; col < MaxColumns; col++)
			{
				if (contentGrid[col] != null && contentGrid[col].page != -1)
				{
					if (contentGrid[col].page < lowest) 
					{
						lowest = contentGrid[col].page;
						lowestIdx = col;
					}
					
					if (contentGrid[col].page > highest)
					{
						highest = contentGrid[col].page;
						highestIdx = col;
					}
				}
			}
			
			
			
			if (contentGrid[lowestIdx].rotation > 90 && contentGrid[lowestIdx].rotation < 100 && rotateSpeed < 0)
			{
				// Unload this column
				UnloadColumn(lowestIdx);
				Debug.Log("Did this low");
				
			}
			else if (contentGrid[lowestIdx].page > 0 && contentGrid[lowestIdx].rotation < 150 && contentGrid[lowestIdx].rotation > 90 && rotateSpeed > 0) 
			{
				int prevcol = lowestIdx-1;
				if (prevcol == -1)
					prevcol = MaxColumns-1;
				
				LoadColumn(prevcol, contentGrid[lowestIdx].page-1);
				
			}
			else if (contentGrid[lowestIdx].rotation > 0 && contentGrid[lowestIdx].rotation < 30 && contentGrid[lowestIdx].page == 0 && rotateSpeed > 0)
			{
				// End of the line reached
				haltPosScroll = true;
			}

			//highest
			if (contentGrid[highestIdx].rotation > 150 && contentGrid[highestIdx].rotation < 170  && rotateSpeed > 0)
			{
				// Unload this column
				UnloadColumn(highestIdx);
				Debug.Log("Did this high");
				
			}
			else if (contentGrid[highestIdx].page < maxPage && contentGrid[highestIdx].rotation < 150 && contentGrid[highestIdx].rotation > 90 && rotateSpeed < 0) 
			{

				int nextcol = highestIdx+1;
				if (nextcol >= MaxColumns)
					nextcol = 0;
				
				LoadColumn(nextcol, contentGrid[highestIdx].page+1);
				
			}
			else if (contentGrid[highestIdx].rotation > 0 && contentGrid[highestIdx].rotation < 30 && contentGrid[highestIdx].page == maxPage && rotateSpeed < 0)
			{
				// End of the line reached
				haltNegScroll = true;
			}		
		}
	}
    #endregion

    #region Class Functions
    public void DisplayContent()
	{
	
		int page = 0;

		for (int i = 0; i < MaxColumns; i++)
		{
			contentGrid[i] = new ContentColumn();
			contentGrid[i].obj = new GameObject[MaxRows];
			contentGrid[i].page = page;
			page++;
			
			for (int idx = 0; idx < MaxRows; idx++)
			{
				//if(i > MaxColumns/2)
					dataLoader.loadObject(videoObjects[objectCount], objectCount);

				contentGrid[i].obj[idx] = videoObjects[objectCount];	
				objectCount++;
			}
            StartCoroutine("WaitToLook", 4f);
		}
	}

	public void LoadContent()
	{
		dataLoader.loadData (OnDataLoad);
	}

	private void LoadColumn(int col, int page)
	{
		for (int row = 0;row < MaxRows; row++) 
		{
			dataLoader.loadObject(contentGrid[col].obj[row],(page * MaxRows) + row);
		}
		
		contentGrid[col].page = page;
	}
	
	private void UnloadColumn(int col)
	{
		for (int row = 0; row < MaxRows; row++) 
			dataLoader.unloadObject (contentGrid[col].obj[row]);
		
		contentGrid[col].page = -1;
	}

	public void OnDataLoad()
	{
		DisplayContent ();
    }
    #endregion

    #region Class Coroutines

    private IEnumerator WaitToLook(float time) 
    {
        yield return new WaitForSeconds(time);
        look2.enabled = true;
    }
    #endregion
}
