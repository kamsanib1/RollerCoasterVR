using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour
{
    public GameObject player;
    public GameObject bullet;
    public GameObject inventory;
    public AudioClip sound;

    bool _trainReady = false;
    GameState prevState;

    protected GameObject gun;
    
    protected bool triggerDown;
    protected bool triggerUp;
    protected bool triggerPress;
    protected bool pauseGame;
    protected bool _changeWeapon;

    int _trainTriggerDist = 10;
    AudioSource _src;
    float time = 0;
    float TIME = .5f;
    // Update is called once per frame
    void Start() {
        _src = gameObject.AddComponent<AudioSource>();
    }
    protected void update()
    {
        if (pauseGame)
        {
            pause();
        }
        if (triggerDown && time>TIME)
        {
            //Debug.Log("trigger down:"+GameState.TRAIN);
            triggerDown = false;
            time = 0;

            //if(Data.gameState == GameState.ROAM) shoot();
            if (Data.gameState == GameState.TRAIN) { manageRide(); }
            else if(Data.gameState == GameState.DRIVE) { manageDrive(); }
            //else if (Data.trainRide || Data.runTrain) runTrain();
            else shoot(gun.transform.position,gun.transform.forward,true);
        }
        time += Time.deltaTime;
    }
    void runTrain() { runTrain(false); }

    void runTrain(bool coasterMode)
    {
        Debug.Log("run train successfull");
        GameObject obj = Data.triggerObj;
        GameObject train;

        RollerCoaster rc;
        rc = obj.GetComponent<RollerCoaster>();
        if (rc == null) rc = obj.GetComponentInParent<RollerCoaster>();
        if (rc == null) rc = obj.GetComponentInChildren<RollerCoaster>();

        if (!coasterMode) train = Instantiate(Data.data.train_normal);
        else
        {
            Data.gameState = GameState.TRAIN;
            if (Data.vrEnabled) train = Instantiate(Data.data.train_vr);
            else train = Instantiate(Data.data.train_cam);
            Data.train = train;
            player.SetActive(false);

            TrainRideInput tri = train.GetComponent<TrainRideInput>();
            if (tri == null) tri = train.GetComponentInChildren<TrainRideInput>(); 
            tri.setPlayer(player);
        }

        if (_changeWeapon)
        {
            //changeWeapon();
        }

        TrainAnimation trainAnim = train.AddComponent<TrainAnimation>();
        trainAnim.setTrackBuilder(rc.tb);
        if (!coasterMode) trainAnim.runInLoop();

        if (rc.train != null) Destroy(rc.train);
        rc.train = train;
    }

    void manageRide()
    {
        TrainAnimation trainAnim = Data.train.GetComponent<TrainAnimation>();
        if (trainAnim.isRideEnd())
        {
            Debug.Log("!!!!!1");
            Destroy(Data.train);
            player.SetActive(true);
            Data.gameState = GameState.ROAM;
        }
        else trainAnim.Play();
    }

    public void manageDrive()
    {
        Data.gameState = GameState.ROAM;
        //Data.triggerObj.GetComponent<HeliMonitor>().endRide();
        player.SetActive(true);
        player.transform.position = Data.triggerObj.transform.position + new Vector3(0, 5, 0);
        Data.triggerObj = null;
    }

    void startDrive(RaycastHit hit) {
        Data.triggerObj = hit.collider.gameObject;
        Data.gameState = GameState.DRIVE;
        player.SetActive(false);
        Data.triggerObj.GetComponent<HeliMonitor>().startRide(player);
    }

    void pause()
    {
        if (Data.gameState == GameState.TRAIN)
        {
            Data.destroyTrain = true;
            //set to false in 
        }
        else
        {
            if (Data.gameState == GameState.PAUSE)
            {
                Data.gameState = prevState;
                Time.timeScale = 1;
            }
            else
            {
                prevState = Data.gameState;
                Data.gameState = GameState.PAUSE;
                Time.timeScale = 0;
            }

        }
    }
    void shoot(Vector3 position,Vector3 direction,bool audioFlag)
    {
        //Camera cam = Camera.main;
        //Debug.Log("shoot");
        RaycastHit hit = new RaycastHit();
        Ray ray = new Ray(position, direction);
        if (Physics.Raycast(ray, out hit) )
        {
            Debug.Log("hit name:"+hit.collider.name);
            string tag = hit.collider.tag;
            ShootingHandler sh = hit.collider.gameObject.GetComponent<ShootingHandler>();
            if (sh != null) {
                //Debug.Log("shooting handler working.");
                sh.gotShot();
            }
            else if (tag == "animal" || tag == "bird")
            {
                placeBullet(hit.point);
                //animal handling
                AnimalAnimation animA = hit.collider.gameObject.GetComponent<AnimalAnimation>();

                if (animA)
                {
                    animA.gotShot();
                }
                //bird handling
                else
                {
                    BirdAnim animB = hit.collider.gameObject.GetComponent<BirdAnim>();

                    if (animB)
                    {
                        animB.gotShot();
                    }
                }
            }
            else if(tag == "run train") {
                if (distance(hit.point) < _trainTriggerDist) {
                    Data.triggerObj = hit.collider.gameObject;
                    runTrain(false);
                }
            }
            else if(tag == "train ride") {
                if (distance(hit.point) < _trainTriggerDist) {
                    Data.triggerObj = hit.collider.gameObject;
                    runTrain(true);
                }
            }
            else if(tag == "vehicle")
            {
                if (distance(hit.point) < _trainTriggerDist)
                {
                    Debug.Log("vehcle running.");
                    startDrive(hit);
                    //runTrain(true);
                }
            }
            else if (tag == "reflector")
            {
                Vector3 newDir = getReflection(direction, hit.collider.transform.forward);
                Debug.Log("reflector:" + hit.collider.name + "\tinbound:" + direction + "normal:" + hit.collider.transform.forward + "point:" + hit.point + "dir:" + newDir); ;
                shoot(hit.point, newDir, false);
            }
            else {

                placeBullet(hit.point);
            }
        }
        if (audioFlag)
        {
            if (_src == null) { _src = gameObject.AddComponent<AudioSource>(); }
            _src.PlayOneShot(sound);
        }
    }

    Vector3 getReflection(Vector3 d,Vector3 n) {
        Vector3 r;
        r = d - 2 * (Vector3.Dot(d,n)) * n;
        return r;
    }
    //void changeWeapon()
    //{
    //    inventory 
    //}
    //helper functions. Strictly for shoot//
    void placeBullet(Vector3 pos)
    {
        //bullet//
        GameObject b = Instantiate(bullet);
        b.transform.position = pos;
        DestroyScript ds = b.AddComponent<DestroyScript>();
        ds.setTime(5);

        //bullet//

    }
    float distance(Vector3 dest) {
        return Vector3.Distance(player.transform.position, dest);
    }
}
