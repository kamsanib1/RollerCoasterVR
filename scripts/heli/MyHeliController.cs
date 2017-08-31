using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyHeliController : MonoBehaviour {

    public AudioSource music;

    public GameObject heli;

    float upSpeed;
    float fSpeed  ;
    float dSpeed  ;

    public float maxUpSpeed= 15;
    public float maxFSpeed = 40;
    public float maxTSpeed = 50;
    public float maxDSpeed = 30;

    public float engine;
    public float engineMin = 800;
    public float engineMax = 1200;
    public float engineStable = 1000;
    public float enginePower  = 150;

    public float throttle;
    public float throttleMax  = 100;
    public float throttlePower = 20;

    public float drift;
    public float driftMax = 100;
    public float driftPower = 20;

    public float maxSideTile = 50;
    public float maxForwardTile = 50;

    float upPowerRatio;
    float fPowerRatio;
    float dPowerRatio;

    bool resetFlagE = true;
    bool resetFlagT = true;
    bool resetFlagD = true;

    bool ready = false;
    bool stop = true;
    // Use this for initialization
    void Start () {
        //    controller = GetComponent<CharacterController>();
        if (music == null) music = GetComponent<AudioSource>();
        music.loop = true;
        music.volume = 0;
        music.Play();
	}
	
	// Update is called once per frame
	void Update () {
        
        resetFlagE = true;
        resetFlagT = true;
        resetFlagD = true;
        input();
        resetHelicopter();
        powerCopter();
        manageAudio();
        animateHeli();
    }

    void calculateRatio()
    {
        //upPowerRatio = (engineMax - engineStable) / maxUpSpeed;
        //fPowerRatio  = ;
        //dPowerRatio  = ;

    }

    void accelerate()
    {
        if (engine < engineMax) engine += enginePower * Time.deltaTime;
        if (engine > engineStable)
        {
            upSpeed = (engine - engineStable) / (engineMax - engineStable) * maxUpSpeed;
        }
    }

    void deaccelerate() {
        if (engine > engineMin) engine -= enginePower * Time.deltaTime;
        if (engine < engineStable)
        {
            upSpeed = (engine - engineStable) / (engineMax - engineStable) * maxUpSpeed;
        }
    }

    void throttleForward(bool forward)
    {
        
        float index = forward ? 1 : -1;
        if(throttle > -throttleMax && throttle < throttleMax) throttle += index * throttlePower * Time.deltaTime;
        else if (throttle > throttleMax) throttle = throttleMax - 1;
        else if (throttle < -throttleMax) throttle = -throttleMax + 1;
        fSpeed = throttle / throttleMax * maxFSpeed;
    }

    void driftRight(bool right)
    {
        float index = right ? 1 : -1;
        if (drift > -driftMax && drift < driftMax) drift += index * driftPower * Time.deltaTime;
        else if (drift > driftMax) drift = driftMax - 1;
        else if (drift < -driftMax) drift = -driftMax + 1;
        dSpeed = drift / driftMax * maxDSpeed;
    }

    void resetHelicopter()
    {
        if (resetFlagE)
        {
            float engineTmp = engine - engineStable;
            if (engineTmp < -20) accelerate();
            else if (engineTmp > 20) deaccelerate();
            else { engine = engineStable; upSpeed = 0; }
        }

        if (resetFlagT) {
            if (throttle < -5) throttleForward(true);
            else if (throttle > 5) throttleForward(false);
            else { throttle = 0;  fSpeed = 0; }
        }
        
        if (resetFlagD)
        {
            if (drift < -5) driftRight(true);
            else if (drift > 5) driftRight(false);
            else { drift = 0;  dSpeed = 0; }
        }

    }

    void powerCopter()
    {
        //check if helicopter is ready to fly.//
        if (!ready && engine > engineMin) ready = true;

        //calculate the engine power to move.//
        if (upSpeed > 0 && !cast(Vector3.up) || upSpeed < 0 && !cast(Vector3.down,0.5f)) transform.position += transform.up * upSpeed * Time.deltaTime;
        if (ready)
        {
            if ((fSpeed > 0 && !cast(transform.forward)) || (fSpeed < 0 && !cast(-transform.forward))) transform.position += transform.forward * fSpeed * Time.deltaTime;
            if ((dSpeed < 0 && !cast(-transform.right)) || (dSpeed > 0 && !cast(transform.right)))
            {
                transform.position += transform.right * dSpeed * Time.deltaTime;
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, drift / driftMax * maxTSpeed * 0.3f * Time.deltaTime, 0));
            }
        }
    }

    void input()
    {
        if (RCInput.palleteUpL )   { resetFlagE = false;  accelerate(); }
        if (RCInput.palleteDownL ) { resetFlagE = false;  deaccelerate(); }
        if (RCInput.palleteLeftL) transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles - new Vector3(0, maxTSpeed * Time.deltaTime, 0));
        if (RCInput.palleteRightL) transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, maxTSpeed * Time.deltaTime, 0));

        if (RCInput.palleteUpR )      { resetFlagT = false;  throttleForward(true);    }
        if (RCInput.palleteDownR)   { resetFlagT = false; throttleForward(false);    }
        if (RCInput.palleteLeftR )     { resetFlagD = false; driftRight(false);         }
        if (RCInput.palleteRightR ) { resetFlagD = false; driftRight(true); }
    }

    bool cast(Vector3 dir) {
        return cast(dir, 5);
    }

    bool cast(Vector3 dir,float length)
    {
        RaycastHit hit;// = new RaycastHit();
        Ray ray = new Ray(transform.position, dir);
        if(Physics.Raycast(ray,out hit, length)) {
            return true;
        }
        return false;
    }

    void manageAudio()
    {
        if (music != null)
        {
            float vol;
            if (engine > engineMin) vol = (engine - engineMin) / (engineMax - engineMin) * 0.5f + 0.5f;
            else vol = engine / engineMin * 0.5f;
            music.volume = vol;
        }
        else Debug.LogWarning("AudioSource component is missing in MyHeliController!!!");
    }

    public bool canGetOut()
    {
        return cast(Vector3.down);
    }

    public void Stop()
    {

        engine = 0;
        music.volume = 0;
    }

    public void animateHeli()
    {
        if (!ready) return;
        if(heli == null) { Debug.Log("helicopter mesh not attached!!!");return; }
        float side = drift / driftMax * maxSideTile;
        float forward = throttle / throttleMax * maxForwardTile;

        Vector3 rot = new Vector3(forward, 0, -side);
        heli.transform.localRotation = Quaternion.Euler(rot);
    }
}
