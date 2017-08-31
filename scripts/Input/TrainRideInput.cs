using UnityEngine;
using System.Collections;
using Valve.VR;


public class TrainRideInput : InputManager
{
    
    SteamVR_Controller.Device device;
    SteamVR_TrackedObject controller;

    Vector2 touchpad;

    private float sensitivityX = 1.5F;

    TrainAnimation _ta;
    float _time = 0;
    bool _flag = true;
    const float _TIME = 2;
     
     void Start()
    {
        controller = gameObject.GetComponent<SteamVR_TrackedObject>();
        _ta = GetComponent<TrainAnimation>();
        if (_ta == null) _ta = GetComponentInChildren<TrainAnimation>();
        if (_ta == null) _ta = GetComponentInParent<TrainAnimation>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Data.vrEnabled)
        {
            device = SteamVR_Controller.Input((int)controller.index);

            if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                base.triggerDown = true;
            }
            if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            { base.triggerDown = false; }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                base.triggerDown = true;
            }
            if (Input.GetMouseButtonUp(0))
            { base.triggerDown = false; }
        }
        if (_ta.isRideEnd() && _flag)
        {
            _time += Time.deltaTime;
            if (_time >= _TIME)
            {
                base.triggerDown = true;
                _flag = false;
            }
        }
        base.update();
    }

    void OnDestroy()
    {
        base.triggerDown = false;
    }

    public void setPlayer(GameObject __player)
    {
        base.player = __player;
    }
}