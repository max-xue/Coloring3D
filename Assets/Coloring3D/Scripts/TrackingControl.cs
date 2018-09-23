using UnityEngine;
using System.Collections;

public class TrackingControl : MonoBehaviour
{

    public GameObject trackingObj;
    // Use this for initialization
    void Start()
    {

    }


    public void OnTrackingFind(ColoringController controller)
    {
        trackingObj.GetComponent<BaseTrackingBehaviour>().OnTrackingFind(controller);
    }

}
