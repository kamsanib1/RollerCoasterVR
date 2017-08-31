using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViveLeftInput : MonoBehaviour {
    SteamVR_Controller.Device device;
    SteamVR_TrackedObject controller;

    // Use this for initialization
    void Start () {
        controller = gameObject.GetComponent<SteamVR_TrackedObject>();

    }

    // Update is called once per frame
    void Update () {
        device = SteamVR_Controller.Input((int)controller.index);

        RCInput.palleteUpL =    false;
        RCInput.palleteDownL =  false;
        RCInput.palleteRightL = false;
        RCInput.palleteLeftL =  false;

        RCInput.triggerL = device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger);

        if (device.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {
            Vector2 touchpad = (device.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0));
            //print("Pressing Touchpad");

            if (touchpad.y > 0.7f)
            {
                RCInput.palleteUpL = true;
                //  print("Moving Up");
            }

            else if (touchpad.y < -0.7f)
            {
                RCInput.palleteDownL = true;
                //print("Moving Down");
            }

            if (touchpad.x > 0.7f)
            {
                RCInput.palleteRightL = true;
            //    print("Moving Right");

            }

            else if (touchpad.x < -0.7f)
            {
                RCInput.palleteLeftL = true;
              // print("Moving left");
            }

        }
    }
}
