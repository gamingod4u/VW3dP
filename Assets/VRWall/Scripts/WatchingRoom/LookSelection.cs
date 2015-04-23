using UnityEngine;
using System.Collections;

public class LookSelection : MonoBehaviour 
{
    public delegate void HitEvent(string name, string tag);
    public static event HitEvent OnHit;

    private GameObject      selectedObject;
    private GameObject       camera;
    private GameObject       reticle;
    private RaycastHit      hit;
    private bool            canSelect = false;

	// Use this for initialization
	void Start () 
    {
        if (AppManager.instance.VRConnected)
            camera = GameObject.Find("OVRPlayerController");
        else
            camera = GameObject.Find("Camera");

        reticle = GameObject.Find("Reticle");

	}
	
	// Update is called once per frame
	void Update () 
    {
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit))
        {
            if (OnHit != null)
                OnHit(hit.transform.name, hit.transform.tag);

            reticle.renderer.enabled = true;
            reticle.transform.position = hit.point;
            Debug.DrawLine(camera.transform.position, hit.point, Color.red);

            if (selectedObject != null && selectedObject.GetInstanceID() == hit.transform.gameObject.GetInstanceID())
            {

                return;
            }
            else if (selectedObject != null && selectedObject.GetInstanceID() != hit.transform.gameObject.GetInstanceID())
            {

                if (selectedObject.tag == "menuButtons") 
                {
                    MenuButtons button = selectedObject.GetComponent<MenuButtons>();
                    button.LoogkingAt(false);
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
                        button.LoogkingAt(true);
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
                        video.startThumbRotation();
                        selectedObject = hit.transform.gameObject;
                        canSelect = false;
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
        yield return new WaitForSeconds(1f);
        canSelect = true;
    }
}
