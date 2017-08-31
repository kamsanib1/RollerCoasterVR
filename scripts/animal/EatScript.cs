using UnityEngine;
using System.Collections;

public class EatScript : MonoBehaviour {
    const float HUNGER_INTERVAL = 2;
    const float EAT_INTERVAL = 0.5F;
    const float HUNGER_THRESHOLD = 70;
    const float DISTANCE_THRESHOLD = 70;
    const float EAT_DISTANCE_THRESHOLD = 5;

    float _hungerTimer=0;
    float _hunger = 75;
    float _eat_timer = 0;

    bool _followFlag = false;
    bool _eatFlag = false;
    bool _enableFlag = false;

    public GameObject player;
    private CharacterController controller;
    private AnimalStats stats;
    GameObject head;
    // Use this for initialization
    void Start () {
        //player = Data.
        controller = gameObject.GetComponent<CharacterController>();

        head = GameObject.Find(gameObject.name + "/head");
        stats = this.GetComponent<AnimalStats>();
    }

    // Update is called once per frame
    void Update () {

        if (distance() < DISTANCE_THRESHOLD)
        {
            _enableFlag = true;
            if (_followFlag) {
                if (distance() < EAT_DISTANCE_THRESHOLD) { _eatFlag = true; _followFlag = false;eat(); }
                else { follow(); }
            }
            else { }
        }
        else { _enableFlag = false; }
        if (_eatFlag)
        {
            if (distance() > EAT_DISTANCE_THRESHOLD) { _followFlag = true; _eatFlag = false; }
        }

        _hungerTimer += Time.deltaTime;
        if (_hunger > HUNGER_THRESHOLD)
        {
            if (_hunger < 100) _hunger++;
            _hungerTimer = 0;
        }
    }

    float distance()
    {
        float dist;
        dist = Vector3.Distance(gameObject.transform.position,player.transform.position);
        return dist;
    }
    void eat() {

    }
    void follow()
    {
          //movement//
        Vector3 moveDirection = transform.forward * stats.walk_speed;
        moveDirection.y -= 20 * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
        Debug.Log("eat::following player...");
        //rotation//
        float tspeed = stats.turn_speed * Time.time;
        Vector3 from = player.transform.rotation.eulerAngles;
        Vector3 target = (player.transform.position - transform.position);
        Vector3 to = new Vector3(from.x, target.y, from.z);
        //transform.LookAt(to);
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(to), tspeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(player.transform.position - transform.position), tspeed);
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(0, (player.transform.position - transform.position).y, 0)), tspeed);

     }
}
