using UnityEngine;
using System.Collections;

public class HairyCopter : MonoBehaviour
{

    //VARIABLES
    public Rigidbody rb;
    private bool breaked;
    public AudioSource audSource;
    public Transform mainRotor;
    public Transform tailRotor;

    //engine
    public EngineConfiguration engineConfiguration;
    [System.Serializable]
    public class EngineConfiguration
    {
        public bool engineOn;
        public float engineAcceleration = 0.2f;
        public float engineDesacceleration = 0.1f;
        public float smoothAddPower = 0.5f;
        [System.NonSerialized]
        public bool isAccelerating;
        [Range(0f, 1.2f)]
        public float enginePower;
        public int engineForce = 15000;
    }

    //velocity
    public VelocityConfiguration velocityConfiguration;
    [System.Serializable]
    public class VelocityConfiguration
    {
        public float rotorVelocity = 2500f;
        public float maxVelocity = 30f;
        public float maxUpVelocity = 3f;
        public float stabilizeVerticalVelocity = 1f;
    }

    //rotation
    public RotationConfiguration rotationConfiguration;
    [System.Serializable]
    public class RotationConfiguration
    {
        public float spinSensitive = 0.8f;
        public float turnSensitive = 0.8f;
        [Range(0f, 90f)]
        public float maxRotation = 90f;
        public float stabilizeRotation = 0.8f;
    }

    //keyboard
    public Keyboard keyboard;
    [System.Serializable]
    public class Keyboard
    {
        public KeyCode engineOnOff = KeyCode.Z;

        public KeyCode acceleration = KeyCode.W;
        [System.NonSerialized]
        public bool isaccelerating;
        public KeyCode desacceleration = KeyCode.S;
        [System.NonSerialized]
        public bool isDesaccelerating;

        public KeyCode spinForward = KeyCode.UpArrow;
        public KeyCode spinBackward = KeyCode.DownArrow;
        public KeyCode spinRight = KeyCode.D;
        public KeyCode spinRight2 = KeyCode.RightArrow;
        public KeyCode spinLeft = KeyCode.A;
        public KeyCode spinLeft2 = KeyCode.LeftArrow;

        public KeyCode turnRight = KeyCode.E;
        public KeyCode turnLeft = KeyCode.Q;
    }

    void Start()
    {
        //audio
        audSource.volume = engineConfiguration.enginePower;
        audSource.pitch = engineConfiguration.enginePower;
    }

    void Update()
    {
        //engine on/off
        if (Input.GetKeyDown(keyboard.engineOnOff) && breaked == false)
        {
            engineConfiguration.engineOn = !engineConfiguration.engineOn;
        }
        if (Input.GetKeyDown(keyboard.engineOnOff) && breaked == true)
        {
            breaked = false;
            engineConfiguration.engineOn = true;
        }
        if (engineConfiguration.engineOn == true && engineConfiguration.enginePower >= 0.8f)
        {
            LimitVelocity();
        }
        //if no breaked
        if (breaked == false)
        {
            Rotor();
        }
        else
        {
            engineConfiguration.enginePower = 0;
        }
        Sound();
        Inputs();
        Engine();
        StabilizeRotation();

    }
    public void Breaked()
    {
        breaked = true;
        engineConfiguration.engineOn = false;
    }
    void Engine()//call in "Update" function
    {
        if (engineConfiguration.engineOn == true)
        {
            if (engineConfiguration.enginePower < 1f && engineConfiguration.isAccelerating == false)
            {
                engineConfiguration.enginePower += Time.deltaTime * engineConfiguration.engineAcceleration;
            }
        }
        else
        {
            if (engineConfiguration.enginePower > 0f)
            {
                engineConfiguration.enginePower -= Time.deltaTime * engineConfiguration.engineAcceleration;
            }
            else
            {
                engineConfiguration.enginePower = 0f;
            }
        }
    }

    void Inputs()//call in "Update" function
    {
        if (engineConfiguration.enginePower >= 0.8)
        {
            //acceleration
            if (Input.GetKey(keyboard.acceleration) && engineConfiguration.enginePower >= 1)
            {
                keyboard.isaccelerating = true;
                engineConfiguration.enginePower = Mathf.MoveTowards(engineConfiguration.enginePower, 1.2f, Time.deltaTime * engineConfiguration.smoothAddPower);
            }
            if (!Input.GetKey(keyboard.acceleration) && engineConfiguration.enginePower >= 1)
            {
                keyboard.isaccelerating = false;
                engineConfiguration.enginePower = Mathf.MoveTowards(engineConfiguration.enginePower, 1f, Time.deltaTime * engineConfiguration.smoothAddPower);
            }
            //deceleration
            if (Input.GetKey(keyboard.desacceleration) && engineConfiguration.enginePower >= 0.8f)
            {
                engineConfiguration.isAccelerating = true;
                engineConfiguration.enginePower = Mathf.MoveTowards(engineConfiguration.enginePower, 0.8f, Time.deltaTime * engineConfiguration.smoothAddPower);
            }
            if (!Input.GetKey(keyboard.desacceleration))
            {
                engineConfiguration.isAccelerating = false;
                if (!Input.GetKey(keyboard.acceleration) && engineConfiguration.enginePower >= 1)
                {
                    engineConfiguration.enginePower = Mathf.MoveTowards(engineConfiguration.enginePower, 1f, Time.deltaTime * engineConfiguration.smoothAddPower);
                }
            }
            /* //spin right/left
             if (Input.GetKey(keyboard.spinRight) || Input.GetKey(keyboard.spinRight2))
             {
                 transform.localRotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(-rotationConfiguration.maxRotation, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z), Time.deltaTime * (rotationConfiguration.spinSensitive * engineConfiguration.enginePower));
             }
             if (Input.GetKey(keyboard.spinLeft) || Input.GetKey(keyboard.spinLeft2))
             {
                 transform.localRotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotationConfiguration.maxRotation, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z), Time.deltaTime * (rotationConfiguration.spinSensitive * engineConfiguration.enginePower));
             }


             //spin forward/backward
             if (Input.GetKey(keyboard.spinForward))
             {
                 transform.localRotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, -rotationConfiguration.maxRotation), Time.deltaTime * (rotationConfiguration.spinSensitive * engineConfiguration.enginePower));
             }
             if (Input.GetKey(keyboard.spinBackward))
             {
                 transform.localRotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, rotationConfiguration.maxRotation), Time.deltaTime * (rotationConfiguration.spinSensitive * engineConfiguration.enginePower));
             } */

            //spin right/left
            if (Input.GetKey(keyboard.spinRight) || Input.GetKey(keyboard.spinRight2))
            {
                transform.localRotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, -rotationConfiguration.maxRotation), Time.deltaTime * (rotationConfiguration.spinSensitive * engineConfiguration.enginePower));
            }
            if (Input.GetKey(keyboard.spinLeft) || Input.GetKey(keyboard.spinLeft2))
            {
                transform.localRotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, rotationConfiguration.maxRotation), Time.deltaTime * (rotationConfiguration.spinSensitive * engineConfiguration.enginePower));
            }


            //spin forward/backward
            if (Input.GetKey(keyboard.spinForward))
            {
                transform.localRotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotationConfiguration.maxRotation, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z), Time.deltaTime * (rotationConfiguration.spinSensitive * engineConfiguration.enginePower));
            }
            if (Input.GetKey(keyboard.spinBackward))
            {
                transform.localRotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(-rotationConfiguration.maxRotation, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z), Time.deltaTime * (rotationConfiguration.spinSensitive * engineConfiguration.enginePower));
            }

            //turn right/left
            if (Input.GetKey(keyboard.turnRight))
            {
                transform.Rotate(new Vector3(0, 50 * Time.deltaTime * engineConfiguration.enginePower * rotationConfiguration.turnSensitive, 0), Space.World);
            }
            if (Input.GetKey(keyboard.turnLeft))
            {
                transform.Rotate(new Vector3(0, -50 * Time.deltaTime * engineConfiguration.enginePower * rotationConfiguration.turnSensitive, 0), Space.World);
            }
        }
    }
    void FixedUpdate()
    {
        if (keyboard.isaccelerating == true)
        {
            Acceleration();
        }
        if (engineConfiguration.engineOn == true)
        {
            StabilizeVerticalVelocity();
        }
    }

    void Sound()//call in "Update" function
    {
        audSource.volume = engineConfiguration.enginePower;
        audSource.pitch = engineConfiguration.enginePower;
        if (engineConfiguration.engineOn == true)
        {
            if (audSource.isPlaying == false)
            {
                audSource.Play();
            }

        }
        else if (audSource.isPlaying == true)
        {
            if (audSource.volume <= 0 || breaked == true)
            {
                audSource.Stop();
            }
        }
    }

    void Rotor()//call in "Update" function
    {
        if(mainRotor!=null) mainRotor.Rotate(new Vector3(0, 0, engineConfiguration.enginePower * Time.deltaTime * velocityConfiguration.rotorVelocity));
        if(tailRotor!=null) tailRotor.Rotate(new Vector3(0,  -engineConfiguration.enginePower * Time.deltaTime * velocityConfiguration.rotorVelocity,0));
    }
    void Acceleration()
    {
        rb.AddRelativeForce(new Vector3(0, engineConfiguration.engineForce * engineConfiguration.enginePower, 0), ForceMode.Force);
    }

    void StabilizeVerticalVelocity()//call in "FixedUpdate"
    {
        if (rb.velocity.y < 0)
        {
            rb.AddRelativeForce(new Vector3(0, rb.mass * (+Physics.gravity.y * -1) * (velocityConfiguration.stabilizeVerticalVelocity * engineConfiguration.enginePower), 0));
        }
    }
    void StabilizeRotation()//call in "Update"
    {
        //stabilize spin right/left
        if (!Input.GetKey(keyboard.spinRight) && !Input.GetKey(keyboard.spinRight2) && !Input.GetKey(keyboard.spinLeft) && !Input.GetKey(keyboard.spinLeft2))
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z), Time.deltaTime * (rotationConfiguration.stabilizeRotation * engineConfiguration.enginePower));
        }
        //stabilize spin forward/backward
        if (!Input.GetKey(keyboard.spinForward) && !Input.GetKey(keyboard.spinBackward))
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.z), Time.deltaTime * (rotationConfiguration.stabilizeRotation * engineConfiguration.enginePower));
        }
    }

    void LimitVelocity()//call in "FixedUpdate"
    {
        float velX = Mathf.Clamp(rb.velocity.x, -velocityConfiguration.maxVelocity, velocityConfiguration.maxVelocity);
        float velY = Mathf.Clamp(rb.velocity.y, rb.velocity.y, velocityConfiguration.maxUpVelocity);
        float velZ = Mathf.Clamp(rb.velocity.z, -velocityConfiguration.maxVelocity, velocityConfiguration.maxVelocity);
        rb.velocity = new Vector3(velX, velY, velZ);
    }

}