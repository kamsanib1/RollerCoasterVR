using UnityEngine;
using System.Collections;

public class VisionScript2 : MonoBehaviour {
    public GameObject player;
    public GameObject head;

    //attributes//
    public float vision_degree = 90.0f;
    public float _player_vision = 60.0f;
    public float vision_range = 80.0f;
    public float speed = 5.0f;
    public float turn_speed = 2.0f;
    public bool lion = false;

    private CharacterController controller;
    private bool encounter = false;
    private int state = 0;
    private string animal;
    private AnimalStats stats;

    private int loop_num = 0;

    // Use this for initialization
    void Start () {
        controller = gameObject.GetComponent<CharacterController>();
        

        animal = gameObject.ToString().Replace("(Clone)", "").Replace("UnityEngine.GameObject", "");
        head = GameObject.Find(gameObject.name + "/head");
        Debug.Log(head.name);
        stats = this.GetComponent<AnimalStats>();
    }

    // Update is called once per frame
    void Update () {
        //Debug.Log("loop num:" + loop_num);
        if(!player) player = GameObject.Find("Follow");
        if (!player) return;
        switch (loop_num)
        {
            case 0: seePlayerBack(); break;
            case 1: followPlayerBack(); break;
            case 2: attack(); break;
            case 3: runAway(); break;
        }
	}

    private void seePlayerBack() {
        if (state != 1)
        {
            state = 1;
            speed = stats.walk_speed;
        }
        //return false;
        //Debug.Log("Vision test");
        Vector3 direction = (player.transform.position - head.transform.position).normalized;
        float angle = Vector3.Angle(head.transform.forward, player.transform.forward);

        if (angle < vision_degree)
        {
            //Debug.Log(animal + "->in vision range :)");
            Ray ray = new Ray(head.transform.position, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, vision_range))
            {
                //Debug.Log("vision hit:" + hit.collider.gameObject + "angle:" + angle + "::coord:" + hit.point + "::start:" + transform.position);
                //Debug.Log(gameObject.name+"vision hit:" + hit.collider.gameObject.name);
                if (hit.collider.tag == "Player")
                {
                    //player found//
                    //Debug.Log(animal + "->Detected player");
                    encounter = true;
                    loop_num = 1;
                    state = 1;
                }

                //redundant code//
                //in case of self hit//
                //special case for lion//
                //else if (hit.collider.name == gameObject.name)
                //{
                //    RaycastHit[] hits = Physics.RaycastAll(ray, vision_range);
                //    int i = 0;
                //    int size = hits.Length;
                //    while (i < size && hits[i].collider.name == gameObject.name ) i++;
                //    if (i<size && hits[i].collider.tag == "Player")
                //    {
                //        //player found//
                //        //Debug.Log(animal + "->Detected player");
                //        encounter = true;
                //        loop_num = 1;
                //    }

                //}
            }//redundant code ends//

        }

    }

    private void followPlayerBack() {
        float dis = Vector3.Distance(gameObject.transform.position, player.transform.position);

        if (dis < 1.5f) {
            state = 5;
            loop_num = 2;
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(player.transform.position - transform.position), turn_speed * Time.deltaTime);
        }
        else
        {
            if(state == 5) { loop_num = 3;state = 2;speed = stats.run_speed; }
           
            //movement//
            Vector3 moveDirection = transform.forward * speed;
            moveDirection.y -= 20 * Time.deltaTime;
            controller.Move(moveDirection * Time.deltaTime);
            //Debug.Log("following player...");
            //rotation//
            float tspeed = turn_speed * Time.time;
            Vector3 from = player.transform.rotation.eulerAngles;
            Vector3 target = (player.transform.position-transform.position);
            Vector3 to = new Vector3(from.x, target.y, from.z);
            //transform.LookAt(to);
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(to), tspeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(player.transform.position - transform.position), tspeed);
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(0, (player.transform.position - transform.position).y, 0)), tspeed);

            //state change condition//
            Vector3 direction = (transform.position - player.transform.position).normalized;
            float angle = Vector3.Angle(player.transform.forward, direction);
            if (angle < _player_vision) { loop_num = 3; }
        }
    }

    private void attack() {
        if (distance() > 5)
        {
            loop_num = 0;
        }
    }

    private void runAway()
    {
        float dis = Vector3.Distance(gameObject.transform.position, player.transform.position);
        Vector3 moveDirection = transform.forward * speed;
        moveDirection.y -= 20 * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);

        float tspeed = turn_speed * Time.time;
        Vector3 from = transform.rotation.eulerAngles;
        Vector3 target = player.transform.rotation.eulerAngles;
        Vector3 to = new Vector3(from.x,target.y,from.z);

        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(to), tspeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(transform.position - player.transform.position), tspeed);
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(0,(transform.position - player.transform.position).y,0)), tspeed);

        if (dis > vision_range/3) { loop_num = 0; encounter = false; }
    
    }

    public bool encountered() { return encounter; }
    public int  getState() { return state; }

    public void setSpeed(float _speed) { speed = _speed; }
    public void setTurnSpeed(float _turn_speed) { turn_speed = _turn_speed; }
    public void setDead() { loop_num = 0;state = 1;encounter = false; }

    float distance()
    {
        float dist;
        dist = Vector3.Distance(gameObject.transform.position, player.transform.position);
        return dist;
    }
}
