using UnityEngine;
using System.Collections;
using Valve.VR;

public class GunStats
{
    public float power=50;
    public float range=100;
    public float accuracy=80;
    public float bullet_interval = .3f;
    public float reload = 1;
    public float capacity = 10;

    public void setStrength(float power,float range, float accuracy)
    {
        this.power = power;
        this.range = range;
        this.accuracy = accuracy;
    }

    public void setTiming(float bullet_interval,float reload,float capacity)
    {
        this.bullet_interval = bullet_interval;
        this.reload = reload;
        this.capacity = capacity;
    }
}

public class BowStats
{
    public bool draw;
    public float power;
    public float power_inc;
    public float max_power;
    public float damage;

    public void addPower() { power += power_inc; }
    public void reset() { power = 0; }
}
public class ViveRightInput : InputManager
{
    public GameObject gun_object;
    
    SteamVR_Controller.Device device;
    SteamVR_TrackedObject controller;

    Vector2 touchpad;

    private float sensitivityX = 1.5F;
    private Vector3 playerPos;
    //private InputData data;
    LineRenderer laserLine;
    
    //1:bow
    //2:gun
    int weapon = 2;
    //gun stats//
    //bow states//
    bool armed = true;
    bool draw  = true;
    //bow stats//
    float power = 0;
    float max_power = 10f;
    //time variables//
    float time = 0;
    float time_interval = .2f;
    float time_last;

    //guns
    GunStats rifle;

    void Start()
    {
        controller = gameObject.GetComponent<SteamVR_TrackedObject>();
       base.gun = gun_object;
        //    //setup guns:
        //    rifle = new GunStats();
        //    rifle.setStrength(300, 1000, 300);
        //    rifle.setTiming(1.3f, 1.5f, 2);
        //
    }

    // Update is called once per frame
    void Update()
    {
        device = SteamVR_Controller.Input((int)controller.index);

      

        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            base.triggerDown = true;
        }
        if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
        { base.triggerDown = false; }

        RCInput.palleteUpR = device.GetPress(SteamVR_Controller.ButtonMask.Axis1);
        RCInput.palleteRightR = device.GetPress(SteamVR_Controller.ButtonMask.Axis2);
        RCInput.palleteDownR  = device.GetPress(SteamVR_Controller.ButtonMask.Axis3);
        RCInput.palleteLeftR  = device.GetPress(SteamVR_Controller.ButtonMask.Axis4);

        base.update();
    }

    void playSound(int type) {
        //data.gameObject.GetComponent<SoundManager>().playOneShot(data.gun_sound[0]);
    }

       
   
}