using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViveRightInput2 : MonoBehaviour {
    SteamVR_Controller.Device device;
    SteamVR_TrackedObject controller;

    // Use this for initialization
    void Start()
    {
        controller = gameObject.GetComponent<SteamVR_TrackedObject>();

    }

    // Update is called once per frame
    void Update()
    {
        device = SteamVR_Controller.Input((int)controller.index);
       
        RCInput.palleteUpR = false;
        RCInput.palleteDownR = false;
        RCInput.palleteRightR = false;
        RCInput.palleteLeftR = false;

        RCInput.triggerR = device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger);

        if (device.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {
            Vector2 touchpad = (device.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0));
            //print("Pressing Touchpad");

            if (touchpad.y > 0.7f)
            {
                RCInput.palleteUpR = true;

                //  print("Moving Up");
            }

            else if (touchpad.y < -0.7f)
            {
                RCInput.palleteDownR = true;
                //print("Moving Down");
            }

            if (touchpad.x > 0.7f)
            {
                RCInput.palleteRightR = true;
                //    print("Moving Right");

            }

            else if (touchpad.x < -0.7f)
            {
                RCInput.palleteLeftR = true;
                // print("Moving left");
            }

        }

    }
}
