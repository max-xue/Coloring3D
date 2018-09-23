using UnityEngine;
using System.Collections;
using Vuforia;

public class TrackableEventHandler : MonoBehaviour, ITrackableEventHandler
{

    #region PRIVATE_MEMBERS
    private TrackableBehaviour mTrackableBehaviour;
    #endregion // PRIVATE_MEMBERS

    public GameObject controller;

    #region MONOBEHAVIOUR_METHODS
    void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
    }
    #endregion //MONOBEHAVIOUR_METHODS

    /// <summary>
    /// Implementation of the ITrackableEventHandler function called when the
    /// tracking state changes.
    /// </summary>
    public void OnTrackableStateChanged(
                                    TrackableBehaviour.Status previousStatus,
                                    TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            OnTrackingFound();
        }
        else if (previousStatus == TrackableBehaviour.Status.UNKNOWN &&
                 newStatus == TrackableBehaviour.Status.NOT_FOUND)
        {
            // Ignore this specific combo
            return;
        }
        else
        {
            OnTrackingLost();
        }
    }

    #region PRIVATE_METHODS
    private void OnTrackingFound()
    {
        Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
        controller.GetComponent<ColoringController>().OnTrackingFind();
    }

    private void OnTrackingLost()
    {
        Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");

        controller.GetComponent<ColoringController>().OnTrackingLost();
    }
    #endregion
}
