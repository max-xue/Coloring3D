using UnityEngine;
using System.Collections;

public class BaseTrackingBehaviour : MonoBehaviour {

    protected ColoringController _controller;

    // Use this for initialization
    void Start () {
	
	}

    public virtual void OnTrackingFind(ColoringController controller)
    {
        _controller = controller;
    }
}
