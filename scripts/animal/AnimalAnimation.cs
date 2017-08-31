using UnityEngine;
using System.Collections;
using System;

//animal animation and movement tracking script.//
public class AnimalAnimation : MonoBehaviour {

    private int state = 0;
    private float time = 1;
    private float time_counter = 0;
    private float rotation = 0;
    private float rot_per_frame = 0;
    private float rot_factor = 0;
    private float heli_up = 0;
    private int move_dir=1;
    private float _speed = 1;

    private AnimalStats stats;
    private CharacterController controller;
    private Animator anim;
    private Vector3 moveDirection  = Vector3.zero;

    private string code;
    private Vector3 start_point = Vector3.zero;

    private bool shot_flag = false;
    private float shot_time = 0;
    private float shot_period = 100;
    private int shot_state;
    private const int KNOCKOUT_TIME = 10;
    //fun animation TMP//true
    public bool follow = false;
    public bool follow_mode = false;
    bool eat_trigger = false;
    VisionScript2 vision;

    private Interpretor _ip;

    //data related to compiler
    int objType = 0;
    // Use this for initialization
    void Start () {
        if(code == null)code = gameObject.GetComponent<MainObject>().script;

        Compiler.setCode(code);
        Compiler.compile();

        _ip = new Interpretor();
        _ip.setICode(Compiler.getICode());
        _ip.init();
        
        //animation setup//
        anim = GetComponent<Animator>();
        if (anim == null) anim = GetComponentInChildren<Animator>();
        if (controller == null) controller = GetComponent<CharacterController>();
        if (controller == null) controller = GetComponentInChildren<CharacterController>();
        stats = gameObject.GetComponent<AnimalStats>();
        _speed = stats.walk_speed;
        //stats.setType(0);

        //TMP fun animation//
        vision = gameObject.GetComponent<VisionScript2>();
        if(vision == null) vision = gameObject.AddComponent<VisionScript2>();
        vision.setSpeed(stats.walk_speed);
        vision.setTurnSpeed(stats.turn_speed);
        vision.enabled = false;

        //add libraries.//
        SensingLib sl = gameObject.AddComponent<SensingLib>();
        sl.setInterpretor(_ip);

        //audio setup//
        audioSetup();

        //tmp code
        tantest();
    }

    // Update is called once per frame
    float _atime=0;//tmp variable for attack animation in vision test//
    int _vision_state; //tmp variable to store state.
    void Update () {
        //a particular action continues to take place until specified//
        //time quantum is finished. incase of time quantum finished, //
        //the state and new time quantum are updated.//
        //Debug.Log(gameObject.name+"::mode:" + follow_mode + "::follow:" + follow);
        //if(follow_mode) vision.enabled = follow;
        //if (vision.enabled && !follow) vision.enabled = false;

        if (!shot_flag)
        {
            if (vision.enabled && vision.encountered())
            {
                //Debug.Log("following...");
                if (_vision_state != vision.getState())
                {
                    //Debug.Log("player saw me!");
                    _vision_state = vision.getState();
                    anim.SetInteger("state", _vision_state);
                    anim.SetTrigger("makeTransition");
                }
                else if (vision.getState() == 5)
                {
                    if (_atime >= 1) { anim.SetTrigger("makeTransition"); _atime = 0; }
                    _atime += Time.deltaTime;
                }
            }
            else {
                time_counter += Time.deltaTime;
                if (time_counter >= time) { time_counter = 0; recalculateState(); }
                performAction();
            }
        }
        else
        {
            shot_time += Time.deltaTime;
            if (shot_time > shot_period)
            {
                shot_flag = false;
                //_ip.shot = false;
                if (follow_mode) vision.enabled = true;
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

        moveDirection.y -= 20 * Time.deltaTime;
        
        //update the required variables based on the state. The animation//
        //is also selected based on the state.//
        if (state == -1) { return; }
        if (state == 1||state == 7) { speed = _speed; }
        else if (state == 2) { speed = stats.run_speed; }
        else { speed = 0; }

        //The animals movement is controlled from here. The movement involve//
        //translation and rotation. The action is performed only when the   //
        //animal is grounded.//
        if (controller != null)
        {
            if (controller.isGrounded)
            {
                moveDirection = transform.forward * speed * move_dir;
            }
            controller.Move(moveDirection * Time.deltaTime);
        }
        else
        {
            if (heli_up == 0)
                moveDirection = transform.forward * speed * move_dir;
            else moveDirection = transform.up * speed * heli_up;
            transform.position += moveDirection * Time.deltaTime;
            //Debug.Log("helicopter running." + speed + ":heli up:" + heli_up + ":move dir" + moveDirection);
        }
        //The rotation is based on the 'turn speed' or manuarbility of the      //
        //animal. The rotation is performed first then moves forward from there.//
        //At every turn the rotation is decreased until it reaches zero.//
        float turnspeed = rotation * Time.deltaTime/time;
            //if (rotation < turnspeed) { turnspeed = rotation; }
            transform.Rotate(0, turnspeed * rot_factor , 0);
        
    }

    //The new state of the animal is updated based on the code. All the variables are//
    //updated according to the state.//
    private void recalculateState() {
        if (state == -1) return;

        string[] instruction = _ip.nextIns();
        rotation = 0;
        move_dir = 1;
        bool flag = false;
        reset();

        bool one_time_animation = false;

        if (instruction == null) { if (state == 0) return; state = 0; }
        else if (instruction[0] == Compiler.EOP) { if (state == 0) return; state = 0; }
        else if (Library.isSensingLibFunc(instruction[0])) { gameObject.GetComponent<SensingLib>().execute(instruction); time = 0; recalculateState(); return; }
        else if (instruction[0] == "raycast") { raycast(instruction); time = 0; recalculateState(); return; }
        else if (instruction[0] == "collidercoords") { getColliderLocation(instruction); time = 0; recalculateState(); return; }
        else if (instruction[0] == "collidertype") { getColliderType(instruction); time = 0; recalculateState(); return; }
        else if (instruction[0] == "colliderangle") { getColliderAngle(instruction); time = 0; recalculateState(); return; }
        else if (instruction[0] == "releasecollider") { releaseCollider(); time = 0; recalculateState(); return; }
        else if(instruction[0] == "setspeed") { setSpeed(instruction);time = 0; recalculateState();return; }
        else if (instruction[0] == "getposition") { getPosition(instruction); time = 0; recalculateState(); return; }
        else if (instruction[0] == "getangle") { getAngleY(instruction); time = 0; recalculateState(); return; }
        else if (instruction[0] == "setposition") { setPosition(instruction); time = 0; recalculateState(); return; }
        else if (instruction[0] == "anglebtw2obj") { getAngleBtw2Obj(instruction); time = 0; recalculateState(); return; }
        else if (instruction[0] == "follow") {
            //if(instruction[1] == "attack")
            { follow_mode = true; vision.enabled = true; }
            //else if (instruction[1] == "eat") { eat_trigger = true; }
            
        }
        else if (instruction[0] == "idle")
        {
            state = 0;
            time = float.Parse(instruction[1]);
        }
        else if (instruction[0] == "walk" || instruction[0] == "walkback")
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
        else if (instruction[0] == "run")
        {
            state = 2;
            float length = float.Parse(instruction[1]);   //length of track for a statement
            Direction direction = getDirection(instruction[2]);  //direction of a statement
            rotation = float.Parse(instruction[3]);    //angle read from statememnt

            time = length / stats.run_speed;
            rot_factor = 1;
            if (direction == Direction.LEFT) rot_factor = -1;
            else if (direction == Direction.FORWARD) rot_factor = 0;
        }
        else if (instruction[0] == "turn")
        {
            Direction direction = getDirection(instruction[2]);  //direction of a statement
            rotation = float.Parse(instruction[3]);    //angle read from statememnt

            rot_factor = 1;
            if (direction == Direction.LEFT) rot_factor = -1;
            else if (direction == Direction.FORWARD) rot_factor = 0;
        }
        else if (instruction[0] == "roar")
        {
            state = 4;
            one_time_animation = true;
            playAudio("roar");
        }
        else if (instruction[0] == "attack")
        {
            state = 5;
            one_time_animation = true;
        }
        else if (instruction[0] == "sleep")
        {
            state = 6;
            time = float.Parse(instruction[1]);
        }
        else if (instruction[0] == "eat")
        {
            state = 8;
            time = float.Parse(instruction[1]);
        }
        else if (instruction[0] == "jump") { state = 9; }
        else if(instruction[0] == "move")
        {
            state = 1;
            heli_up = 0;

            float length = float.Parse(instruction[1]);   //length of track for a statement
            Direction direction = getDirection(instruction[2]);  //direction of a statement
            rotation = float.Parse(instruction[3]);    //angle read from statememnt

            time = length / stats.walk_speed;
            rot_factor = 1;
            if (direction == Direction.LEFT) rot_factor = -1;
            else if (direction == Direction.FORWARD) rot_factor = 0;

            else if (direction == Direction.BACKWARD)
            {
                state = 7;
                move_dir = -1;
            }
            else if(direction == Direction.UP) { heli_up = 1; }
            else if (direction == Direction.DOWN) { heli_up = -1; }

        }

        else if (instruction[0].Contains("b_")) {
            
            state = boyFunctions(instruction,out flag);
            one_time_animation = true;
            flag = true;
        }
        else if (instruction[0].Contains("p_")) {
            //bool flag;
            state = princessFunctions(instruction, out flag);
            one_time_animation = true;
            flag = true;
        }
        else if (instruction[0].Contains("r_"))
        {
            //bool flag;
            state = roboFunctions(instruction, out flag);
            one_time_animation = true;
            flag = true;
        }
        else if (instruction[0].Contains("s_"))
        {
            //bool flag;
            state = snowmanFunctions(instruction, out flag);
            one_time_animation = true;
            flag = true;
        }
        else if (instruction[0].Contains("z_"))
        {
            //bool flag;
            state = zombieFunctions(instruction, out flag);
            one_time_animation = true;
            flag = true;
        }
        /*else if(instruction[0] == "collision")
        {
            collisionHandler(instruction[1]);
            
        }*/

        if (state < 0)
        {
            anim.SetInteger("state", 0);
            anim.SetTrigger("makeTransition");
        }
        else {
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
            if(!flag) time = anim.GetCurrentAnimatorStateInfo(0).length;
        }
    }

   

    private int boyFunctions(string[] instruction,out bool flag) {
        int state = -1;
        flag = true;

        //boy
        if (instruction[0] == "b_ahoy") { state = 11; time = 1.7f; }
        else if (instruction[0] == "b_aid") { state = 12; time = 2.43f; }
        else if (instruction[0] == "b_defend") { state = 13; time = 1; }
        else if (instruction[0] == "b_crouch1") { state = 14; time = 1.1f; }
        else if (instruction[0] == "b_crouch2") { state = 15; time = 1.76f; ; }
        else if (instruction[0] == "b_sitt_g_start") { state = 16; time = 1.667f; }
        else if (instruction[0] == "b_sitt_g_loop") { state = 17; time = 3.33f; }
        else if (instruction[0] == "b_sitt_g_end") { state = 18; time = 2.167f; }
        else if (instruction[0] == "b_sitt_s_start") { state = 19; time = 1.3f; }
        else if (instruction[0] == "b_sitt_s_loop") { state = 20; time = 3.33f; }
        else if (instruction[0] == "b_sitt_s_end") { state = 21; time = 1.667f; }
        else if (instruction[0] == "b_spider_walk") { state = 22; time = 1.3f; }
        else if (instruction[0] == "b_sidewalk_right") { state = 23; time = 0.633f; }
        else if (instruction[0] == "b_sidewalk_left") { state = 24; time = 0.6f; }
        else if (instruction[0] == "b_talk2") { state = 25; time = 4.7f; }
        else if (instruction[0] == "b_zombie_run") { state = 26; time = 0.867f; }
        else if (instruction[0] == "b_dance_happy") { state = 27; time = 1.5f; }
        else if (instruction[0] == "b_rock_dance") { state = 28; time = 25f; }
        else if (instruction[0] == "b_claps") { state = 29; time = 2.63f; }
        else if (instruction[0] == "b_leg_movement") { state = 30; time = 1.7f; }
        else if (instruction[0] == "b_attack_hand2") { state = 31; time = 2.3f; }
        else if (instruction[0] == "b_kick") { state = 32; time = 1.533f; }

        return state;
    }

    private int princessFunctions(string[] instruction,out bool flag) {
        int state = -1;
        flag = true;
        //princess
        //princess animations
        if (instruction[0] == "p_step_right") { state = 34; time = 1.0f; }
        else if (instruction[0] == "p_step_left") { state = 35; time = 1.0f; }
        else if (instruction[0] == "p_turn_right") { state = 36; time = 1; }
        else if (instruction[0] == "p_turn_left") { state = 37; time = 1; }
        else if (instruction[0] == "p_swim") { state = 38; time = 2; }
        else if (instruction[0] == "p_dive") { state = 39; time = 2; }
        else if (instruction[0] == "p_twirl") { state = 40; time = .667f; }
        else if (instruction[0] == "p_dance_01") { state = 41; time = 7.3f; }
        else if (instruction[0] == "p_dance_02") { state = 42; time = 4.83f; }
        else if (instruction[0] == "p_dance_03") { state = 43; time = 3.33f; }
        else if (instruction[0] == "p_climb_up") { state = 44; }
        else if (instruction[0] == "p_climb_down") { state = 45; }
        else if (instruction[0] == "p_sit_down") { state = 46; }
        else if (instruction[0] == "p_sit_idle") { state = 47; }
        else if (instruction[0] == "p_sit_end") { state = 48; }
        else if (instruction[0] == "p_pick_from_tree") { state = 49; time = 1.333f; }
        else if (instruction[0] == "p_pick_from_table") { state = 50; time = 1.3f; }
        else if (instruction[0] == "p_pick_from_floor") { state = 51; time = 1.33f; }
        else if (instruction[0] == "p_throw") { state = 52; time = 1; }
        else if (instruction[0] == "p_hugging") { state = 53; time = 1.33f; }
        else if (instruction[0] == "p_waving") { state = 54; time = 1.667f; }
        else if (instruction[0] == "p_joking") { state = 55; time = 2.5f; }

        return state;
    }

    private int roboFunctions(string[] instruction, out bool flag)
    {
        int state = -1;
        flag = true;
        //Robot
        //Robot animations
        if (instruction[0] == "r_bake") { state = 67; time = 3.167f; }
        else if (instruction[0] == "r_carrying") { state = 68; time = 1.33f; }
        else if (instruction[0] == "r_climb_ladder") { state = 69; time = 1.0f; }
        else if (instruction[0] == "r_cackle") { state = 70; time = 1.333f; }
        else if (instruction[0] == "r_climp_stairs_up") { state = 71; time = 1.0f; }
        else if (instruction[0] == "r_climb_stairs_down") { state = 72; time = 1.0f; }
        else if (instruction[0] == "r_climb_ladder_down") { state = 73; time = 1.0f; }
        else if (instruction[0] == "r_eat") { state = 74; time = 1.667f; }
        else if (instruction[0] == "r_guitar") { state = 76; time = 3.0f; }
        else if (instruction[0] == "r_push") { state = 77; time = 1.0f; }
        else if (instruction[0] == "r_injection") { state = 78; time = 3.333f; }
        else if (instruction[0] == "r_take") { state = 79; time = 1.0f; }
        else if (instruction[0] == "r_build") { state = 81; time = 2.5f; }
        else if (instruction[0] == "r_cooking") { state = 82; time = 2.333f; }
        else if (instruction[0] == "r_aggressive") { state = 83; time = 2.0f; }
        else if (instruction[0] == "r_cheer") { state = 84; time = 1.0f; }
        else if (instruction[0] == "r_deactivation") { state = 85; time = 1.6670f; }
        else if (instruction[0] == "r_dancing1") { state = 41; time = 3.333f; }
        else if (instruction[0] == "r_getup") { state = 86; time = 1.0f; }
        else if (instruction[0] == "r_joy1") { state = 87; time = 1.0f; }
        else if (instruction[0] == "r_evil") { state = 88; time = 1.0f; }
        else if (instruction[0] == "r_joking") { state = 89; time = 2.0f; }
        else if (instruction[0] == "r_mimic") { state = 90; time = 1.0f; }
        else if (instruction[0] == "r_action1") { state = 91; time = 1.333f; }
        else if (instruction[0] == "r_clearing") { state = 92; time = 3.330f; }
        else if (instruction[0] == "r_applaud") { state = 93; time = 1.3330f; }
        else if (instruction[0] == "r_fall") { state = 94; time = 3.0f; }
        else if (instruction[0] == "r_duck") { state = 95; time = 1.333f; }
        else if (instruction[0] == "r_drilling") { state = 96; time = 3.1670f; }
        else if (instruction[0] == "r_empty") { state = 97; time = 2.0f; }
        else if (instruction[0] == "r_pick") { state = 99; time = .6670f; }
        else if (instruction[0] == "r_talk") { state = 100; time = 4.3330f; }
        else if (instruction[0] == "r_hitfall") { state = 101; time = 0.6670f; }
        else if (instruction[0] == "r_attack_kick") { state = 104; time = 1.333f; }
        else if (instruction[0] == "r_attack_punch") { state = 105; time = 1.8330f; }
        else if (instruction[0] == "r_singing") { state = 106; time = 3.1670f; }
        else if (instruction[0] == "r_seperating") { state = 107; time = 3.1670f; }
        else if (instruction[0] == "r_eating") { state = 108; time = 1.6660f; }
        else if (instruction[0] == "r_rolling") { state = 109; time = 3.167f; }
        else if (instruction[0] == "r_trip") { state = 110; time = 1.167f; }
        else if (instruction[0] == "r_gym") { state = 111; time = 3.333f; }
        else if (instruction[0] == "r_dodge_left") { state = 114; time = 0.667f; }
        else if (instruction[0] == "r_drinking") { state = 115; time = 2.1670f; }
        else if (instruction[0] == "r_drop_object") { state = 116; time = 0.6670f; }
        //else if (instruction[0] == "r_evil2") { state = 117; time = f; }
        else if (instruction[0] == "r_painting") { state = 118; time = 1.0f; }
        else if (instruction[0] == "r_hit1") { state = 119; time = 0.3330f; }
        else if (instruction[0] == "r_jump") { state = 9; time = 0.833f; }

        return state;
    }
    private int snowmanFunctions(string[] instruction, out bool flag)
    {
        int state = -1;
        flag = true;
        if (instruction[0] == "s_idle") { state = 0; time = 3.333f; }
        else if (instruction[0] == "s_walk") { state = 1; time = 1.000f; }
        else if (instruction[0] == "s_run") { state = 2; time = 0.667f; }
        else if (instruction[0] == "s_death") { state = 3; time = 1.333f; }
        else if (instruction[0] == "s_walk_back") { state = 7; time = 1.0f; }
        else if (instruction[0] == "s_eat") { state = 8; time = 2.333f; }
        else if (instruction[0] == "s_jumpy") { state = 9; time = 1.0f; }
        else if (instruction[0] == "s_hit_01") { state = 144; time = 0.667f; }
        else if (instruction[0] == "s_runjump") { state = 145; time = 3.333f; }
        else if (instruction[0] == "s_emotion_01") { state = 146; time = 3.333f; }
        else if (instruction[0] == "s_emotion_02") { state = 147; time = 4.333f; }
        else if (instruction[0] == "s_emotion_03") { state = 148; time = 3.333f; }
        else if (instruction[0] == "s_emotion_04") { state = 149; time = 2.667f; }
        else if (instruction[0] == "s_emotion_05") { state = 150; time = 2.333f; }
        else if (instruction[0] == "s_listen") { state = 151; time = 3.667f; }
        else if (instruction[0] == "s_punch") { state = 152; time = 1.333f; }
        else if (instruction[0] == "s_scared") { state = 153; time = 2.333f; }
        else if (instruction[0] == "s_stendup") { state = 154; time = 6.000f; }
        else if (instruction[0] == "s_dance_1A") { state = 155; time = 1.667f; }
        else if (instruction[0] == "s_dance_1J") { state = 156; time = 1.667f; }
        else if (instruction[0] == "s_dance_2A") { state = 157; time = 3.333f; }

        return state;
    }
    private int zombieFunctions(string[] instruction, out bool flag)
    {
        int state = -1;
        flag = true;
        if (instruction[0] == "z_idel") { state = 0; time = 2.667f; }
        else if (instruction[0] == "z_walk") { state = 1; time = 1.967f; }
        else if (instruction[0] == "z_run") { state = 2; time = 1.300f; }
        else if (instruction[0] == "z_dead") { state = 6; time = 0.833f; }
        else if (instruction[0] == "z_attack") { state = 5; time = 2.000f; }
        else if (instruction[0] == "z_get_hit") { state = 10; time = 0.833f; }
        else if (instruction[0] == "z_attack2") { state = 11; time = 3.667f; }
        else if (instruction[0] == "z_attack3") { state = 12; time = 2.000f; }
        else if (instruction[0] == "z_shout") { state = 4; time = 2.667f; }
        
        return state;
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
        //TMP fun animation//

        follow_mode = false;
        if(vision) vision.enabled = false;
    }

    //public void setStartPoint(GameObject flag) { start_point = flag; }
    //functions related to animal raycast.
    public void gotShot()
    {
        if(state==3) { return; }
        Debug.Log("got shot");

        shot_state = state;
        state = 3;
        anim.SetInteger("state", state);
        anim.SetTrigger("makeTransition");
        shot_period = KNOCKOUT_TIME;
        shot_time = 0;
        shot_flag = true;
        _ip.shot = true;
        playAudio("die");
        //tmp code for animal intelligenge//
        if (follow_mode)
        {
            vision.setDead();
            vision.enabled = false;
        }
        //end of tmp code.//
    }
    public void setType(string[] ins) {
        
    }
    public void getType() { }
    GameObject collider;
    string[] typeMap = { "animal", "ground", "bird", "train", "wall" };
    public void raycast(string[] ins) {
        float dist = float.Parse(ins[1]);
        float angle = float.Parse(ins[2]);
        Vector3 dir = gameObject.transform.forward + new Vector3(0, 0, angle);
        double val = 0;

        Ray ray = new Ray(transform.position+new Vector3(0,.5f,0.2f),dir);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, dist)) {
            val = 1;
            if(hit.collider.name == gameObject.name)
            {
                ray = new Ray(hit.point + transform.forward*0.5f, dir);
                if(Physics.Raycast(ray,out hit, dist))
                {
                    collider = hit.collider.gameObject;
                    Debug.Log("raycast value:" + val + ":" + ins[3] + ":tag:" + hit.collider.tag + ":name:" + hit.collider.name);
                }
            }
        }
        _ip.setValue(ins[3], val);
    }

    public void getColliderLocation(string[] ins) {
        float x = 0;
        float y = 0;
        float z=0;
        Debug.Log("collider location:" + ins[1]);
        if (collider != null) {
            Debug.Log("collider is working."+collider.name);
            x = collider.transform.position.x;
            y = collider.transform.position.y;
            z = collider.transform.position.z;
        }
        _ip.setValue(ins[1], x);
        _ip.setValue(ins[2], y);
        _ip.setValue(ins[3], z);
    }
    public void getColliderType(string[] ins) {
        int type = -1;

        if (collider != null)
        {
            for (int i = 0; i < typeMap.Length; i++)
            {
                if (collider.tag == typeMap[i]) { type = i; break; }
            }
            Debug.Log("collider type is working." + collider.name);
        }

        _ip.setValue(ins[1], type);
    }
    public void getColliderAngle(string[] ins) {
        float angle = 0;
        if (collider != null)
        {
            angle = collider.transform.rotation.eulerAngles.y;
            Debug.Log("collider angle is working:" + angle + "::name:" + collider.name);
        }
        _ip.setValue(ins[1], angle);
    }
    public void releaseCollider()
    {
        collider = null;
    }
    public void setSpeed(string[] ins)
    {
        float speed = float.Parse(ins[1]);
        //Debug.Log("speed:" + ins[1]);
        _speed = speed;
    }
    //end of functions related to animal raycast.
    public void getPosition(string[] ins)
    {
        string name = ins[1].Replace("\"", "");
        GameObject go = null;
        
        for (int i = 0; i < Data.objects.Count; i++)
        {
            MainObject mo = Data.objects[i].GetComponent<MainObject>();
            if (mo.nameO == name) { go = Data.objects[i]; break; }
        }
        if (go != null)
        {
            _ip.setValue(ins[2], go.transform.position.x);
            _ip.setValue(ins[3], go.transform.position.y);
            _ip.setValue(ins[4], go.transform.position.z);
        }
    }
    public void getAngleY(string[] ins)
    {
        string name = ins[1].Replace("\"", "");
        GameObject go = null;

        for (int i = 0; i < Data.objects.Count; i++)
        {
            MainObject mo = Data.objects[i].GetComponent<MainObject>();
            if (mo.nameO == name) { go = Data.objects[i]; break; }
        }
        if (go != null)
        {
            float ang = go.transform.rotation.eulerAngles.y;
            ang %= 360;
            if (ang<0) ang += 360;
            _ip.setValue(ins[2], ang);
        }
    }
    public void getAngleBtw2Obj(string[] ins)
    {
        string name = ins[1].Replace("\"", "");
        GameObject go = null;

        for (int i = 0; i < Data.objects.Count; i++)
        {
            MainObject mo = Data.objects[i].GetComponent<MainObject>();
            if (mo.nameO == name) { go = Data.objects[i]; break; }
        }
        if (go != null)
        {
            //Vector3 dir = go.transform.position - transform.position;
            //dir = go.transform.InverseTransformDirection(dir);
            //float ang = Mathf.Atan2(dir.z,dir.x)*Mathf.Rad2Deg;
            //ang = ang % 360;
            //if (ang < 0) ang += 360;
            //Vector2 target = new Vector2(go.transform.position.x, go.transform.position.z);
            //Vector2 origin = new Vector2(transform.position.x, transform.position.z);
            //Vector2 dir = (target - origin).normalized;

            //Vector2 forward = ()
            //_ip.setValue(ins[2],ang);
            //Vector3 dir = (go.transform.position - transform.position).normalized;
            //float ang = Vector3.Angle(transform.forward, dir);
            //float angb = ang;
            //ang += 180;
            //Debug.Log("anglebwnobj:" +angb+"::"+ ang);
            //_ip.setValue(ins[2], ang);
            float y2 = go.transform.position.z;
            float y1 = transform.position.z;
            float x2 = go.transform.position.x;
            float x1 = transform.position.x;
            float dx = x2 - x1;
            float dy = y2 - y1;
            float theta = Mathf.Atan(dx/dy) * Mathf.Rad2Deg;
            float ot = theta;
            if ((y2 > y1) && (x2 > x1))
                theta = theta;
            else if (y2 < y1 && x2 > x1)
                theta = 180 + theta;
            else if (y2 < y1 && x2 < x1)
                theta = 180 + theta;
            else if (y2 > y1 && x2 < x1)
                theta = 360 + theta;
            else if (y1 == y2) {
                if (x2 > x1)
                    theta = 90;
                else theta = 270;
            }
            else if (x1 == x2)
            {
                if (y2 > y1)
                    theta = 0;
                else theta = 180;
            }
   
            float ang = theta - transform.rotation.eulerAngles.y;
            if( ang < 0)
                ang = 360 + ang;
            else if (ang> 360)
                ang = ang - 360;
            Debug.Log("ot:"+ot+"("+dx+","+dy+") theta:" + theta +" ra:"+transform.rotation.eulerAngles.y+ " ang:" + ang);
            _ip.setValue(ins[2],ang);
        }
        
    }
    public void setPosition(string[] ins)
    {
        string name = ins[1].Replace("\"", "");
        GameObject go = null;

        for (int i = 0; i < Data.objects.Count; i++)
        {
            MainObject mo = Data.objects[i].GetComponent<MainObject>();
            if (mo.nameO == name) { go = Data.objects[i]; break; }
        }
        if (go != null)
        {
            float x = float.Parse(ins[1]);
            float y = float.Parse(ins[2]);
            float z = float.Parse(ins[3]);
            go.transform.position = new Vector3(x,y,z);
        }
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

    private void OnCollisionEnter(Collision collision)
    {
        _ip.collision = true;
        Debug.Log("collision true00");
    }

    private void OnCollisionExit(Collision collision)
    {
        _ip.collision = false;
        Debug.Log("collision false");
    }

    private void OnTriggerEnter(Collider other)
    {
        _ip.collision = true;
    }
    private void OnTriggerExit(Collider other)
    {
        _ip.collision = false;
    }
    void OnDestroy()
    {
        if(anim == null) { Debug.Log("anim is null!!!");anim = GetComponent<Animator>(); }
        anim.SetInteger("state", 0);
        anim.SetTrigger("makeTransition");
        Destroy(gameObject.GetComponent<SensingLib>());
    }

    /*void collisionHandler(string __var)
    {
        double val = 0;
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, 2f)) { val = 1; }
        _ip.setValueParameter(__var, val);

        Debug.Log("collision:"+val);
    }*/

    EffectsData ed;
    AudioSource src;
    AudioClip roar = null;
    AudioClip die = null;
    void audioSetup()
    {
        ed = gameObject.GetComponent<EffectsData>();
        if (ed == null) return;

        src = gameObject.AddComponent<AudioSource>();
        src.spatialBlend = 1;
        for (int i = 0; i < ed.clips.Length; i++)
        {
            if (ed.clips[i].rcname == "roar") roar = ed.clips[i].clip;
            else if (ed.clips[i].rcname == "die") die = ed.clips[i].clip;
        }
    }
    void playAudio(string clipname)
    {
        AudioClip clip = null;
        if (clipname == "roar") clip = roar;
        else if (clipname == "die") clip = die;
        if (clip != null)
        {
            src.PlayOneShot(clip);
        }
    }

    //tmp test code
    void tantest()
    {
        for (float i = -500f; i < 500; i += 50f)
            Debug.Log(i + ":"+Mathf.Atan2(i, 1)*Mathf.Rad2Deg);
    }
}







