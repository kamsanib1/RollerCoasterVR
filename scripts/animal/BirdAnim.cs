using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BirdState { LAND,FLY,LANDING,DEAD };

public class BirdAnim : MonoBehaviour {
    
    private int state = 0;
    private float time = 1;
    private float time_counter = 0;
    private float rotation = 0;
    private float rot_per_frame = 0;
    private float rot_factor = 0;
    private float heli_up = 0;
    private int move_dir = 1;
    public BirdState bird_state = BirdState.LAND;
    private AnimalStats stats;
    private CharacterController controller;
    private Animator anim;
    private Vector3 moveDirection = Vector3.zero;

    private string code;
    private Vector3 start_point = Vector3.zero;

    private bool shot_flag = false;
    private float shot_time = 0;
    private float shot_period = 100;
    private int shot_state;
    private const int KNOCKOUT_TIME = 10;

    private Interpretor _ip;

    // Use this for initialization
    void Start()
    {
        if (code == null) code = gameObject.GetComponent<MainObject>().script;

        Compiler.setCode(code);
        Compiler.compile();

        _ip = new Interpretor();
        _ip.setICode(Compiler.getICode());
        _ip.init();

        //-Debug.Log(code);


        //animation setup//
        anim = GetComponent<Animator>();
        if (anim == null) anim = GetComponentInChildren<Animator>();
        if (controller == null) controller = GetComponent<CharacterController>();
        if (controller == null) controller = GetComponentInChildren<CharacterController>();
        stats = gameObject.GetComponent<AnimalStats>();
        //stats.setType(0);

    }

    // Update is called once per frame
    float _atime = 0;//tmp variable for attack animation in vision test//
    int _vision_state; //tmp variable to store state.
    void Update()
    {
        //a particular action continues to take place until specified//
        //time quantum is finished. incase of time quantum finished, //
        //the state and new time quantum are updated.//

        if (!shot_flag)
        {
            if(bird_state == BirdState.LANDING) {
                land();
            }
                time_counter += Time.deltaTime;
                if (time_counter >= time) { time_counter = 0; recalculateState(); }
                performAction();
        }
        else
        {
            shot_time += Time.deltaTime;
            gravity(9.8f);
            if (shot_time > shot_period)
            {
                shot_flag = false;
                state = shot_state;
                anim.SetInteger("state", state);
                anim.SetTrigger("makeTransition");

            }
        }
    }

    //performs normal set of operations according to the state of the//
    //animal. The state is current animation and action.   //
    private void performAction()
    {
        float speed = 0;

        
        //update the required variables based on the state. The animation//
        //is also selected based on the state.//
        if (state == -1) { return; }
        if (state == 1 || state == 7) { speed = stats.walk_speed; }
        else if (state == 59 || state == 60) { speed = stats.run_speed; }
        else { speed = 0; }

        //The animals movement is controlled from here. The movement involve//
        //translation and rotation. The action is performed only when the   //
        //animal is grounded.//
        if (bird_state == BirdState.LAND)
        {
            if (!controller.enabled) { controller.enabled = true; }
            moveDirection = transform.forward * speed * move_dir;
            controller.Move(moveDirection * Time.deltaTime);
            moveDirection.y -= 20 * Time.deltaTime;
        }
        else
        {
            if (controller.enabled) { controller.enabled = false; }
            if (heli_up == 0)
                moveDirection = transform.forward * speed * move_dir;
            else moveDirection = transform.up * speed * heli_up;
            transform.position += moveDirection * Time.deltaTime;
        }
        //The rotation is based on the 'turn speed' or manuarbility of the      //
        //animal. The rotation is performed first then moves forward from there.//
        //At every turn the rotation is decreased until it reaches zero.//
        float turnspeed = rotation * Time.deltaTime / time;
        //if (rotation < turnspeed) { turnspeed = rotation; }
        transform.Rotate(0, turnspeed * rot_factor, 0);

    }

    //The new state of the animal is updated based on the code. All the variables are//
    //updated according to the state.//
    private void recalculateState()
    {
        if (state == -1) return;

        string[] instruction = _ip.nextIns();
        rotation = 0;
        move_dir = 1;
        bool flag = false;
        reset();

        bool one_time_animation = false;

        if (instruction == null) { if (state == 0) return; state = 0; }
        else if (instruction[0] == Compiler.EOP) { if (state == 0) return; state = 0; }
        else if (instruction[0] == "idle")
        {
            //bird has idle state on ground and sky
            if (bird_state == BirdState.LAND) state = 0;
            else state = 60;
            time = float.Parse(instruction[1]);
        }
        else if ((instruction[0] == "walk" || instruction[0] == "walkback") && bird_state == BirdState.LAND)
        {
            state = 1;

            float length = float.Parse(instruction[1]);   //length of track for a statement
            Direction direction = getDirection(instruction[2]);  //direction of a statement
            rotation = float.Parse(instruction[3]);    //angle read from statememnt

            time = length / stats.walk_speed;
            rot_factor = 1;
            if (direction == Direction.LEFT) rot_factor = -1;
            else if (direction == Direction.FORWARD) rot_factor = 0;

            if (direction == Direction.BACKWARD)
            {
                state = 7;
                move_dir = -1;
            }
            //-Debug.Log("rotation:" + rotation);
        }
        //else if (instruction[0] == "run" && bird_state == BirdState.LAND)
        //{
        //    state = 2;
        //    float length = float.Parse(instruction[1]);   //length of track for a statement
        //    Direction direction = getDirection(instruction[2]);  //direction of a statement
        //    rotation = float.Parse(instruction[3]);    //angle read from statememnt

        //    time = length / stats.run_speed;
        //    rot_factor = 1;
        //    if (direction == Direction.LEFT) rot_factor = -1;
        //    else if (direction == Direction.FORWARD) rot_factor = 0;
        //}
        //else if (instruction[0] == "turn" )
        //{
        //    Direction direction = getDirection(instruction[2]);  //direction of a statement
        //    rotation = float.Parse(instruction[3]);    //angle read from statememnt

        //    rot_factor = 1;
        //    if (direction == Direction.LEFT) rot_factor = -1;
        //    else if (direction == Direction.FORWARD) rot_factor = 0;
        //}
        else if ((instruction[0] == "fly" || instruction[0] == "glide") && bird_state == BirdState.FLY)
        {
            if(instruction[0] == "fly") state = 60;
            else if (instruction[0] == "glide") state = 59;
            heli_up = 0;
            bird_state = BirdState.FLY;

            float length = float.Parse(instruction[1]);   //length of track for a statement
            Direction direction = getDirection(instruction[2]);  //direction of a statement
            rotation = float.Parse(instruction[3]);    //angle read from statememnt

            time = length / stats.run_speed;
            rot_factor = 1;
            if (direction == Direction.LEFT) rot_factor = -1;
            else if (direction == Direction.FORWARD) rot_factor = 0;

            else if (direction == Direction.BACKWARD)
            {
                state = 7;
                move_dir = -1;
                rot_factor = 0;
            }
            else if (direction == Direction.UP) { heli_up = 1; }
            else if (direction == Direction.DOWN) { heli_up = -1; }

        }
        else if(instruction[0] == "land" && bird_state == BirdState.FLY)
        {
            state = 60;
            bird_state = BirdState.LANDING;
        }
        else if(instruction[0] == "takeoff" && bird_state == BirdState.LAND)
        {
            state = 57;
            time = 0.67f;
            bird_state = BirdState.FLY;
        }
        if (state < 0)
        {
            anim.SetInteger("state", 0);
            anim.SetTrigger("makeTransition");
        }
        else
        {
            anim.SetInteger("state", state);
            anim.SetTrigger("makeTransition");
        }
        if (one_time_animation)
        {
            //UnityEditor.Animations.AnimatorController ac = anim.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
            //UnityEditor.
            //ac.GetStateEffectiveMotion(0);
            if (state == 41) time = 7.3f;
            else if (state == 42) time = 4.83f;
            else if (state == 43) time = 3.33f;
            if (!flag) time = anim.GetCurrentAnimatorStateInfo(0).length;
        }
    }

    private void reset()
    {
        state = 0;
        time = 1;
        time_counter = 0;
        rotation = 0;
        rot_per_frame = 0;
        rot_factor = 0;
        heli_up = 0;
        move_dir = 1;
    }

    public void updateScript(string newcode)
    {
        //parser.logger.appendln("script problem");
        //parser.logger.appendln("code:\r\n" + newcode);

        //code = newcode;
        //parser = new Parser2();
        //parser.setType(3);
        //parser.parse(code);
        //state = 0;
        transform.position = start_point;
        Compiler.setCode(newcode);
        if (Compiler.compile())
        {
            _ip.setICode(Compiler.getICode());
            _ip.init();
        }
    }

    //public void setStartPoint(GameObject flag) { start_point = flag; }
    public void gotShot()
    {
        if (state == 3) { return; }
        Debug.Log("got shot");
        shot_state = state;
        state = 3;
        anim.SetInteger("state", state);
        anim.SetTrigger("makeTransition");
        shot_period = KNOCKOUT_TIME;
        shot_time = 0;
        shot_flag = true;
        if(bird_state == BirdState.FLY)
        {
            bird_state = BirdState.LAND;
            shot_state = 0;
            time = 0;
        }
        
        //bird related

    }

    public void setStartPoint(Vector3 start_point)
    {
        this.start_point = start_point;
    }

    private Direction getDirection(string dir)
    {
        if (dir.ToLower() == "up") { return Direction.UP; }
        else if (dir.ToLower() == "down") { return Direction.DOWN; }
        else if (dir.ToLower() == "left") { return Direction.LEFT; }
        else if (dir.ToLower() == "right") { return Direction.RIGHT; }
        else if (dir.ToLower() == "forward") { return Direction.FORWARD; }
        else if (dir.ToLower() == "backward") { return Direction.BACKWARD; }
        return Direction.FORWARD;
    }

    void OnDestroy()
    {
        if(anim == null) { anim = GetComponent<Animator>(); }
        anim.SetInteger("state", 0);
        anim.SetTrigger("makeTransition");

    }

    //bird faling on ground.
    void gravity(float speed) {
        if (!controller.enabled) controller.enabled = true;
        moveDirection = -transform.up * speed;
        controller.Move(moveDirection * Time.deltaTime);

    }

    void land()
    {
        
        time = 100;
        Ray ray = new Ray(this.transform.position, Vector3.down);
        if (Physics.Raycast(ray, 1)) {
            time = 1.5f;
            bird_state = BirdState.LAND;
            anim.SetInteger("state", 58);
            anim.SetTrigger("makeTransition");
        }
        else
        {
            gravity(stats.run_speed);
        }
    }
}
