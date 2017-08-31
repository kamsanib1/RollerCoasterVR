using UnityEngine;
using System.Collections;

public class ViveTeleport : MonoBehaviour {

    public GameObject player;
    public float max_dist = 30;
    public bool teleport_active = true;
    //false when player is on train.//
    //restricts jumping out of train.//

    SteamVR_Controller.Device device;
    SteamVR_TrackedObject controller;
    SteamVR_LaserPointer laser_line;

    //InputData data;

    // Use this for initialization
    void Start()
    {
        controller = gameObject.GetComponent<SteamVR_TrackedObject>();
        //data = GameObject.Find("Input Data").GetComponent<InputData>();
        if (teleport_active)
        {
            laser_line = gameObject.GetComponent<SteamVR_LaserPointer>();
            laser_line.active = false;
        }
    }

    // Update is called once per frame
    void Update () {
        if (!Data.movementFlag) return;
        device = SteamVR_Controller.Input((int)controller.index);
        if(Data.gameState != GameState.ROAM && device.GetPress(SteamVR_Controller.ButtonMask.Trigger)) {  }
        else if (device.GetPress(SteamVR_Controller.ButtonMask.Grip) && device.GetPress(SteamVR_Controller.ButtonMask.Trigger)) {  }
        else {
            
            if (teleport_active && device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) { laser_line.active = true; }
            if (teleport_active && device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                laser_line.active = false;
                RaycastHit hit;
                if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, max_dist))
                {
                    player.transform.position = hit.point + new Vector3(0, 0.5f, 0);
                }
                else
                {
                    Vector3 pos = player.transform.position + this.transform.forward * max_dist;
                    if (Physics.Raycast(player.transform.position, new Vector3(0, -1, 0), out hit, 1000))
                    {
                        player.transform.position.Set(pos.x, hit.point.y + 0.5f, pos.z);
                    }
                    else if (Physics.Raycast(player.transform.position, new Vector3(0, 1, 0), out hit, 1000))
                    {
                        player.transform.position.Set(pos.x, hit.point.y + 0.5f, pos.z);
                    }
                }
            }
        }
    }
}
