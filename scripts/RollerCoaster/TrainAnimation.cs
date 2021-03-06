﻿/*
animates the train. doesnot use unity physics.
does not use beizer curves.
path is collection of points. updates train position every 1/24s(frame).
caluculates the distance i.e, the next point and updates the train transform.  
*/

using UnityEngine;
using System.Collections;

public class TrainAnimation : MonoBehaviour {

    public float velocity = 600.0f;     //initial velocity to climb hill    
    public float vel_multiplier = 3f;   //velocity multiplier to enhance animation
    public TrackBuilder gt;            //pointer to track generatorobject
    public bool loop_run = false;       //flag to run train in a loop
    public AudioClip clip;              //audio file
    public float friction = 0.0002f;          //friction on track
    public float mass = 10;
    public bool top_point_reached = false;     //flag to indicate the top point

    //train animation variables
    int pos =0;
    GameObject rail,target;
    private GameObject train_holder;
    int max =1;
    public float total_energy ;
    bool end = false;
    //audio variables
    AudioSource source;                 //audio player
    AudioSource air;
    int last_sound =0;
    bool play = false;          //falg to pause/play animation
    float max_vel = 100;
    float top_point = 0f;
    float _initialHeight = 0;

    void Awake()
    {
        //-------------audio management--------------------//
        source = gameObject.AddComponent<AudioSource>();
        air = gameObject.AddComponent<AudioSource>();
        
    }
    //--------------------------awake()-----------------------------awake()------------------------------//

    void Start()
    {
        //-----------------setting objects in hierarchy to attain animation-----------//
        train_holder = new GameObject("train");
        
        transform.parent = train_holder.transform;
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        train_holder.transform.position = gt.points[0].transform.position;
        max = gt.points.Count-1;
        
        //energy calculation
        friction = Data.data.friction;
        mass = Data.data.mass;
        top_point = gt.points[gt.top_object].transform.position.y;
        _initialHeight = gt.points[0].transform.position.y;
        total_energy = mass * (top_point-_initialHeight) * 9.8f + mass * Data.data.drop_vel * Data.data.drop_vel /2;
        vel_multiplier = Data.data.vel_multiplier;
        max_vel = Mathf.Sqrt(total_energy*2/mass)*vel_multiplier;
        //Debug.Log("vel mul:" + vel_multiplier + "::max vel:" + max_vel+"::top point:"+top_point+"::top object:"+gt.top_object);
        //audio
        air.clip = Data.data.air_background;
        air.Play();
        air.loop = true;
        air.volume = 0;
    }

    //--------------------------------update function----------------------------------------------------//
    void Update()
    {
        //-------------animation control-----------------------//
        //pause/play animation
        if (play)
        {
            trainMotion();
        }
        else
        {
            air.Pause();
        }
    }
    //--------------------------------end of update function----------------------------------------------//

    void OnDestroy()
    {
        Destroy(train_holder);
    }

    public int getPoint()
    {
        return pos/Data.data.speed_points;
    }

    private void trainMotion() {
        

        //-----------------------animation update-------------------------------//
        if (!end)
        {
            if (pos == 0) {
                air.Play();
                Debug.Log("------------train started running--------------");
            }
            if (!air.isPlaying) air.UnPause();
            //----------animation----------------//
            rail = gt.points[pos];
            train_holder.transform.position = rail.transform.position;
            train_holder.transform.rotation = rail.transform.rotation;

            float pe = 0;

            //-------------------calculating next position based on velocity------------------------//
            //calculate the velocity if top point is reached using below equation.
            if (top_point_reached)
            {
                pe = mass * 9.8f * (rail.transform.position.y-_initialHeight);
                float tmp = 2 * (total_energy - pe) / mass;//rail.transform.position.y;

                if (tmp <= 0)
                {
                    end = true;
                    velocity = 0;
                }
                else
                {
                    velocity = Mathf.Sqrt(tmp) * vel_multiplier;
                    vel_multiplier = Data.data.vel_multiplier - Data.data.factor + (gt.top_object - rail.transform.position.y) * Data.data.factor * 2 / top_point;
                }
                //Debug.Log("tmp:" + tmp + "::vel multiplier:" + vel_multiplier+"::vel:"+velocity + "::total_energy:" + total_energy + "::friction_force:" + friction_force + "::pe:" + pe);

                if (velocity < 0)
                {
                    end = true;
                    velocity = 0;
                }

            }
            //checking if top point of track is reached
            else
            {
                if (pos >= gt.top_object) { top_point_reached = true; Debug.Log("top point reached:"); }
                if (pos >= last_sound && !loop_run)
                {
                    last_sound = pos+(int)Data.data.points_per_track;
                    source.PlayOneShot(Data.data.clip, 0.15f);
                }
            }

            float dis = velocity * Time.deltaTime;
            float mid = (float)((int)dis + (int)(dis + 1)) / 2;
            if (dis > mid) dis++;
            if ((int)dis <= 0) pos++;
            else pos = (int)(pos + dis);
            if (pos > max) { end = true; }

            //calculation frictional force and decreasing the total energy//
            if (top_point_reached)
            {
                float angle = (float)rail.transform.rotation.z * Mathf.Deg2Rad;
                float friction_force = friction * mass * 9.8f * Mathf.Cos(angle);
                if (friction_force < 0) friction_force = 0;
                total_energy -= friction_force * dis;
            }
            triggers();

            //-----audio management------//
            float _tmp = velocity / (max_vel);
            if (top_point_reached) air.volume = _tmp;// 1.3f;
            else air.volume = _tmp * .7f;
            //Debug.Log(air.volume);
            //Debug.Log("pos:" + pos + "::vel:" + velocity + "::dis:" + dis + "::time:"+Time.deltaTime+"::height:" + rail.transform.position.y+"::volume:"+air.volume);
        }
        else
        {
            if (end && loop_run) { reset(); }
        }
    }

    private int _propsPtr = 0;
    private int _speedPtr = 0;
    private void triggers()
    {
        //props are only called in train ride.//
        if (_propsPtr < gt.props.Count && pos >= gt.props[_propsPtr].point) { if(!loop_run)propSetup(); }
        if (_speedPtr < gt.speedTriggers.Count && pos >= gt.speedTriggers[_speedPtr].point) { speedMgr(); }
    }

    private void propSetup() {
        GameObject prop = gt.props[_propsPtr].prop;
        if(!prop.active)prop.SetActive(true);

        //audio setup//
        AudioSource __as = prop.GetComponent<AudioSource>();
        __as.loop = true;
        __as.Play();

        //attach destroy script to delete the object.
        int time = gt.props[_propsPtr].time;
        DestroyScript ds = prop.AddComponent<DestroyScript>();
        ds.setInactivateFlag();
        ds.setTime(time);

        //next prop.
        _propsPtr++;
    }

    private void speedMgr() {
        if (top_point_reached)
        {
            SpeedTrigger st = gt.speedTriggers[_speedPtr];
            float vel = st.value;
            if (st.brake) vel = -vel;
            addEnergy(vel);
            //Debug.Log("brake/boost");
        }
        _speedPtr++;
    }

    public void addEnergy(float v)
    {
        float num2 = (1.0f / 2.0f) * mass * v * v;
        if (v < 0) { num2 *= -1; }
        this.total_energy += total_energy * v / 100.0f;
        Debug.Log("energy:" + num2 + "::total energy:" + this.total_energy);
    }

    public void setTrackBuilder(TrackBuilder trackBuilder) { gt = trackBuilder; }

    public bool isRideEnd()
    {
        //Debug.Log("ride end:" + end);
        return end;
    }

    public void runInLoop() {
        loop_run = true;
        play = true;
    }

    public void Play() { play = true; }

    void reset()
    {
        play = true;
        pos = 0;
        end = false;
        top_point_reached = false;
        total_energy = mass * (top_point - _initialHeight) * 9.8f + mass * Data.data.drop_vel * Data.data.drop_vel / 2;
    }
}
