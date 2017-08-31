
using UnityEngine;
 using System.Collections;
 using Valve.VR;
 
 public class ViveMovement : MonoBehaviour
{
    public GameObject player;
    
    SteamVR_Controller.Device device;
    SteamVR_TrackedObject controller;

    Vector2 touchpad;

    private float sensitivityX = 1.5F;
    private Vector3 playerPos;
    private bool is_triggered = false;
    InputData data;

    void Start()
    {
        controller = gameObject.GetComponent<SteamVR_TrackedObject>();
        data = GameObject.Find("Input Data").GetComponent<InputData>();
    }

    // Update is called once per frame
    void Update()
    {
        device = SteamVR_Controller.Input((int)controller.index);
        //If finger is on touchpad
        if (device.GetPress(SteamVR_Controller.ButtonMask.Trigger)) {
            //teleport();
        }
        //else { data.setTrigger(false); }

        if (device.GetTouch(SteamVR_Controller.ButtonMask.Touchpad))
        {
            //Read the touchpad values
            touchpad = device.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad);


            // Handle movement via touchpad
            if (touchpad.y > 0.2f || touchpad.y < -0.2f)
            {
                float speed = 8;
                // Move Forward
                //if (device.GetPress(SteamVR_Controller.ButtonMask.Trigger)) { speed = 20; data.setTrigger(true); }
                //else { data.setTrigger(false); }

                    player.transform.position += player.transform.forward * Time.deltaTime * (touchpad.y * speed);
                // Adjust height to terrain height at player positin
                //playerPos = player.transform.position;
                //playerPos.y = Terrain.activeTerrain.SampleHeight(player.transform.position);
                //player.transform.position = playerPos;
            }

            // handle rotation via touchpad
            if (touchpad.x > 0.3f || touchpad.x < -0.3f)
            {
                player.transform.Rotate(0, touchpad.x * sensitivityX, 0);
            }

            //Debug.Log ("Touchpad X = " + touchpad.x + " : Touchpad Y = " + touchpad.y);
        }
    }

    public bool isTriggered() { return is_triggered; }
}