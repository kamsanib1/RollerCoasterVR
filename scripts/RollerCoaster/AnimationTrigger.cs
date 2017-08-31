using UnityEngine;
using System.Collections;

public class AnimationTrigger : MonoBehaviour {
    public bool runTrain = true;

	void OnTriggerEnter(Collider collider)
    {
        Debug.Log("trigger enter");
        if (runTrain) Data.runTrain = true;
        else Data.trainRide = true;
        Data.triggerObj = this.gameObject;
    }

    void OnTriggerExit(Collider collider)
    {
        Debug.Log("trigger exit");
        if (runTrain) Data.runTrain = false;
        else Data.trainRide = false;
        Data.triggerObj = null;
    }

    //void OnTriggerStay()
    //{
    //    Debug.Log("trigger enter");
    //    if (runTrain) Data.runTrain = true;
    //    else Data.trainRide = true;
    //    Data.triggerObj = this.gameObject;

    //}
}
