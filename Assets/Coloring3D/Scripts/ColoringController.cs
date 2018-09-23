using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vuforia;

public class ColoringController : MonoBehaviour {
    public enum TrackingType {
        unknown = -1,
        easyAR,
        VP,
        Lighting
    }

    public TrackingType trakcingType = TrackingType.easyAR;
    public List<GameObject> trackingPrefabList;

    GameObject _trackingObj;
    Vector3 _origionPosition;
    Quaternion _origionRotation;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnTrackingFind()
    {
        if (IsTrackingFound()) return;

        GameObject prefab = trackingPrefabList[(int)trakcingType];

        _origionPosition = prefab.transform.position;
        _origionRotation = prefab.transform.rotation;

        _trackingObj = Instantiate(prefab, prefab.transform.position, prefab.transform.rotation) as GameObject;
        _trackingObj.transform.parent = VuforiaManager.Instance.ARCameraTransform;
        _trackingObj.GetComponent<TrackingControl>().OnTrackingFind(this);
    }

    public void OnTrackingFound()
    {
        _trackingObj.transform.localRotation = _origionRotation;
        _trackingObj.transform.localPosition = _origionPosition;
    }

    public void OnTrackingLost()
    {
        //Clear();
    }

    bool IsTrackingFound()
    {
        return _trackingObj != null;
    }

    void Clear()
    {
        Destroy(_trackingObj);
    }
}
