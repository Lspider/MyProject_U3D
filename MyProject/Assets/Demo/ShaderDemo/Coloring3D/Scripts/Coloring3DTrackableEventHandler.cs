using UnityEngine;
using System.Collections;
///maxx-m using Vuforia;

///maxx-m public class Coloring3DTrackableEventHandler : MonoBehaviour, ITrackableEventHandler
public class Coloring3DTrackableEventHandler : MonoBehaviour
{

    ///maxx-m #region PRIVATE_MEMBERS
    ///maxx-m private TrackableBehaviour mTrackableBehaviour;
    ///maxx-m #endregion // PRIVATE_MEMBERS

    public GameObject controller;

    ///maxx-m 
    //#region MONOBEHAVIOUR_METHODS
    //void Start()
    //{
    //    mTrackableBehaviour = GetComponent<TrackableBehaviour>();
    //    if (mTrackableBehaviour)
    //    {
    //        mTrackableBehaviour.RegisterTrackableEventHandler(this);
    //    }
    //}
    //#endregion //MONOBEHAVIOUR_METHODS

    ///maxx-m 
    ///// <summary>
    ///// Implementation of the ITrackableEventHandler function called when the
    ///// tracking state changes.
    ///// </summary>
    //public void OnTrackableStateChanged(
    //                                TrackableBehaviour.Status previousStatus,
    //                                TrackableBehaviour.Status newStatus)
    //{
    //    if (newStatus == TrackableBehaviour.Status.DETECTED ||
    //        newStatus == TrackableBehaviour.Status.TRACKED ||
    //        newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
    //    {
    //        OnTrackingFound();
    //    }
    //    else if (previousStatus == TrackableBehaviour.Status.UNKNOWN &&
    //             newStatus == TrackableBehaviour.Status.NOT_FOUND)
    //    {
    //        // Ignore this specific combo
    //        return;
    //    }
    //    else
    //    {
    //        OnTrackingLost();
    //    }
    //}

    #region PRIVATE_METHODS
    private void OnTrackingFound()
    {
        //Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
        controller.GetComponent<Coloring3DController>().OnTrackingFound();
    }

    private void OnTrackingLost()
    {
        //Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
        if (controller)
        {
            controller.GetComponent<Coloring3DController>().OnTrackingLost();
        }

    }
    #endregion
}
