using UnityEngine;
using System.Collections;

public class LookSelection : MonoBehaviour
{
    #region Class Variables
    public delegate void HitEvent(string name, string tag);
    public static event HitEvent OnHit;
    public bool canSelect = false;

    private HeadTracking head;
    private GameObject      selectedObject;
    private GameObject      camera;
    private GameObject      reticle;
    private RaycastHit      hit;
    
    private bool            firstPass = false;
    #endregion

    #region Unity Functions
    // Use this for initialization
	void Start () 
    {
        reticle = GameObject.Find("Reticle");
        head = GameObject.Find("HeadTracking").GetComponent<HeadTracking>();
	}
	
	// Update is called once per frame
	void Update () 
    {

        if (!firstPass)
        {
            if (AppManager.instance.VRConnected)
                camera = GameObject.Find("CenterEyeAnchor");
            else
                camera = GameObject.Find("Camera");

            firstPass = true;
            StartCoroutine(Wait2Select());
        }
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit))
        {
            if (OnHit != null)
                OnHit(hit.transform.name, hit.transform.tag);

            reticle.renderer.enabled = true;
            reticle.transform.position = hit.point;


            if (selectedObject != null && selectedObject.GetInstanceID() == hit.transform.gameObject.GetInstanceID())
            {

                return;
            }
            else if (selectedObject != null && selectedObject.GetInstanceID() != hit.transform.gameObject.GetInstanceID())
            {

                if (selectedObject.tag == "menuButtons") 
                {
                    MenuButtons button = selectedObject.GetComponent<MenuButtons>();
                    button.LookingAt(false);
                    StartCoroutine(Wait2Select());
                }
                else if (selectedObject.tag == "videoButtons") 
                {
                    VideoButtons button = selectedObject.GetComponent<VideoButtons>();
                    button.LookingAt(false);
                    StartCoroutine(Wait2Select());
                }
                else if (selectedObject.tag == "thumbnail")
                {
                    VideoObject video = (VideoObject)selectedObject.GetComponent(typeof(VideoObject));
                    video.selected = false;
                    video.stopThumbRotation();
                    StartCoroutine(head.WaitToRotate());
                    StartCoroutine(Wait2Select());
                }
                
                selectedObject = null;
            }
            else
            {

                if (canSelect)
                {
                    if (hit.transform.gameObject.tag == "menuButtons") 
                    {
                        MenuButtons button = hit.transform.gameObject.GetComponent<MenuButtons>();
                        button.LookingAt(true);
                        selectedObject = hit.transform.gameObject;
                        canSelect = false;
                    }
                    else if (hit.transform.gameObject.tag == "videoButtons")
                    {
                        VideoButtons button = hit.transform.gameObject.GetComponent<VideoButtons>();
                        button.LookingAt(true);
                        selectedObject = hit.transform.gameObject;
                        canSelect = false;
                    }

                    if (hit.transform.gameObject.tag == "thumbnail")
                    {
                        VideoObject video = hit.transform.gameObject.GetComponent<VideoObject>();
                        video.selected = true;
                        canSelect = false;
                        video.startThumbRotation();
                        head.canRotate = false;
                        selectedObject = hit.transform.gameObject;
                    }
                }
            }
        }
        else
        {
            reticle.renderer.enabled = false;
            selectedObject = null;
        }
    }

    public IEnumerator Wait2Select() 
    {
        yield return new WaitForSeconds(1.5f);
        canSelect = true;
    }

    #endregion
}
