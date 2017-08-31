using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carrunning : MonoBehaviour {
    //If you do not use center of mass the wheels base it off of the colliders
    //By using center of mass you can control where it is.
    public Transform t_CenterOfMass;
    bool sound = false;

    //The maximum amount of power put out by each wheel.
    public float maxTorque = 500f;
    public bool BrakeAllowed = false;
    public bool turning;
    public float m_Brake = 10000f;
    //The max distance a wheel can turn.
    public float maxSteerAngle = 45f;
    public float maxDecelerationSpeed = 500f;
    //Each wheel needs its own mesh
    public Transform[] wheelMesh = new Transform[4];
    public Transform[] tires;
    //The physics of the wheels, max 20 axels.
    //WheelCollider[4] 4 is how many wheels we have.
    public WheelCollider[] wheelCollider = new WheelCollider[4];
    public float currentSpeed;
    public float reducedSpeed;
    //Ridged body accessor.
    private Rigidbody r_Ridgedbody;
    public AudioSource music;
    public AudioClip start;
    public AudioClip running;

    private Interpretor _ip;

    float time = 1;
    float vol = 0.5f;
    float step = .01f;
    public void Start()
    {
        // This sets where the center of mass is, if you look r_Ridgedbody."centerOfMass" is a function of ridged body.
        r_Ridgedbody = GetComponent<Rigidbody>();
        r_Ridgedbody.centerOfMass = t_CenterOfMass.localPosition;
        r_Ridgedbody.useGravity = false;
        r_Ridgedbody.isKinematic = true;
        if (music == null) music = GetComponent<AudioSource>();
        music.clip = running;
        music.loop = true;
        //music.PlayOneShot(start,0.5f);
    }

    public void Update()
    {
        //Sets the wheel meshs to match the rotation of the physics WheelCollider.
        UpdateMeshPosition();
        HandBrake();
        
    }

    public void FixedUpdate()
    {
        carmovement();
        DecelerationSpeed();
        CarSound();
    }
    private void carmovement()
    {
        //Turn the wheels to a set max, with an input.
       
        currentSpeed = wheelCollider[2].radius * wheelCollider[2].rpm * 60 / 1000 * Mathf.PI;
       
        if (currentSpeed < 500f && r_Ridgedbody.velocity.magnitude <= 60f)
        {
            if (vol < 1) { vol += step; }

            for (int i = 0; i < 4; i++)
            {
                float axisV = 0.3f;
                if (RCInput.palleteUpR)
                {
                    Debug.Log("up");
                    axisV = 1;
                }
                if (RCInput.palleteDownR)
                {
                    Debug.Log("down");
                    axisV = -1;
                }
                wheelCollider[i].motorTorque = axisV * maxTorque;
                
            }
        }
        float axisH = 0;
        if (RCInput.palleteRightR) axisH = 1;
        if (RCInput.palleteLeftR) axisH = -1;

        float steer = axisH * maxSteerAngle;
        wheelCollider[0].steerAngle = steer;
        wheelCollider[1].steerAngle = steer;
        if (axisH != 0)
        {
            turning = true;
        }
        else
        {
            turning = false;
        }

        

    }
    public void DecelerationSpeed()
    {
        if(!BrakeAllowed && (RCInput.palleteDownR == false && RCInput.palleteUpR == false))
        {
            for(int i=0;i<4;i++)
            {
                wheelCollider[i].brakeTorque = currentSpeed/20;
                //if (wheelCollider[i].motorTorque > 0) wheelCollider[i].motorTorque -= wheelCollider[i].brakeTorque;
                //else wheelCollider[i].motorTorque = 0;
            }
        }
    }
    //Sets each wheel to move with the physics WheelColliders.
    public void UpdateMeshPosition()
    {
        for (int i = 0; i < 4; i++) 
        {
            Quaternion quat;
            Vector3 pos;

            //Gets the current position of the physics WheelColliders.
            wheelCollider[i].GetWorldPose(out pos, out quat);

            ///Sets the mesh to match the position and rotation of the physics WheelColliders.
            wheelMesh[i].position = pos;
            wheelMesh[i].rotation = quat;
        }
    }
    private void HandBrake()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            BrakeAllowed = true;

        }
        else
        {
            BrakeAllowed = false;
            
        }
        if (r_Ridgedbody.velocity.magnitude<=10f && BrakeAllowed && turning)
        {
            
            m_Brake = 20f;
           
        }
        else if(BrakeAllowed)
        {
            
            for (int i = 0; i < 4; i++)
            {
                wheelCollider[i].brakeTorque = m_Brake;
                if (wheelCollider[i].motorTorque > 0) wheelCollider[i].motorTorque -= wheelCollider[i].brakeTorque;
                else wheelCollider[i].motorTorque = 0;
                vol = 0.5f;
            }
            r_Ridgedbody.drag = 0.4f;
        }
        else if (!BrakeAllowed && (RCInput.palleteDownR || RCInput.palleteUpR))
        {
            for (int i = 0; i < 4; i++)
            {
                wheelCollider[i].brakeTorque = 0;
                   
            }
            r_Ridgedbody.drag = 0.1f;
            
        }
    }
    private void RotatingRTires()
    {
        for (int i = 0; i < 4; i++)
        {
            tires[i].Rotate(wheelCollider[i].rpm / 60 * 360 * Time.deltaTime, 0f, 0f);
        }
        for(int i=0;i<2;i++)
        {
            tires[0].localEulerAngles = new Vector3(tires[0].localEulerAngles.x, wheelCollider[0].steerAngle - tires[0].localEulerAngles.z, tires[0].localScale.z);
            tires[1].localEulerAngles = new Vector3(tires[1].localEulerAngles.x, wheelCollider[1].steerAngle - tires[1].localEulerAngles.z, tires[1].localScale.z);
        }
    }

    //Method to control the sound of the car
    public void CarSound()
    {
        if (music == null)
        {
            music = GetComponent<AudioSource>();
            music.Play();
        }
        if(!music.isPlaying) music.Play(); ;
        if (Input.GetKey(KeyCode.Space))
        {
            Debug.Log("car music off.");
            vol = 0.5f;
            BrakeAllowed = true;
        }
        else
        {
            Debug.Log("car music on.");
            BrakeAllowed = false;
        }
        if (r_Ridgedbody.velocity.magnitude <= 10f && BrakeAllowed && turning)
        {
            Debug.Log("car music on.");
            music.mute = false;
        }
        else if (BrakeAllowed || currentSpeed==0)//Only if the car comes to complete halt is the sound played.
        {
            Debug.Log("car music off.");
            //music.mute = true;
            vol = 0.5f;
        }
        else if(!BrakeAllowed && (RCInput.palleteDownR || RCInput.palleteUpR))
        {
            Debug.Log("car music on.");
            music.mute = false;
        }
        music.volume = vol;
    }

    public void startCar() {
        music.Play();
        music.volume = 1f;
        music.PlayOneShot(start);
        music.volume = 0.5f;
        music.loop = true;
        r_Ridgedbody.useGravity = true;
        r_Ridgedbody.isKinematic = false;
    }

    public void stopCar()
    {
        music.Stop();
        r_Ridgedbody.useGravity = false;
        r_Ridgedbody.isKinematic = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        _ip.collision = true;
    }
    private void OnTriggerExit(Collider other)
    {
        _ip.collision = false;
    }

}
