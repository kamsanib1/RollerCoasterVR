using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementLib : MonoBehaviour {

    float time = 0;
    float counter = 0;
    float _speed = 0;
    string _dir;
    bool _isArch = false;
    float rotAngle;

    Interpretor _ip;
    string code;
    // Use this for initialization
	void Start () {
        if (code == null) code = gameObject.GetComponent<MainObject>().script;
        Compiler.setCode(code);
        Compiler.compile();

        _ip = new Interpretor();
        _ip.setICode(Compiler.getICode());
        _ip.init();

        //add libraries.//
        SensingLib sl = gameObject.AddComponent<SensingLib>();
        sl.setInterpretor(_ip);

    }

    // Update is called once per frame
    void Update () {
        
        if (counter <= time)
        {
            performAction();
            counter += Time.deltaTime;
        }
        else { recalculateState(); }
	}

    void recalculateState() {

        string[] instruction = _ip.nextIns();

        bool one_time_animation = false;

        if (instruction == null) { return; }
        else if (instruction[0] == Compiler.EOP) { return; }
        else if (Library.isSensingLibFunc(instruction[0])) { gameObject.GetComponent<SensingLib>().execute(instruction); time = 0; recalculateState(); return; }
        else { execute(instruction); }

        counter = 0;
    }

    void performAction()
    {
        Vector3 dir = Vector3.zero;
        float rot_factor = 1;
        if (_isArch) {
            dir = transform.forward;
            if (_dir == "left") { rot_factor = -1; }
            else if (_dir == "right") { rot_factor = 1; }
        }
        else
        {
            if (_dir == "forward") { dir = transform.forward; }
            else if (_dir == "backward") { dir = -transform.forward; }
            else if (_dir == "up") { dir = transform.up; }
            else if (_dir == "down") { dir = -transform.up; }
            else if (_dir == "left") { dir = -transform.right; }
            else if (_dir == "right") { dir = transform.right; }
            else if (_dir == "none") { dir = Vector3.zero; }
        }
        Vector3 moveDirection = dir * _speed ;
        transform.position += moveDirection * Time.deltaTime;

        if (time != 0)
        {
            float turnspeed = rotAngle * Time.deltaTime * rot_factor / time;
            //if (rotation < turnspeed) { turnspeed = rotation; }
            transform.Rotate(0, turnspeed, 0);
        }
    }

    public void execute(string[] ins) {
        if(ins[0] == "move2") { move(ins,false); }
        else if(ins[0] == "rotate") { rotate(ins); }
        else if(ins[0] == "movearch") { move(ins,true); }
        else if(ins[0] == "setspeed") { }
        else if(ins[0] == "destroy") { destroy(); }
    }

    void move(string[] ins, bool isArch)
    {
        float len = float.Parse(ins[1]);
        _dir = ins[2];
        rotAngle = float.Parse(ins[3]);
        _speed = float.Parse(ins[4]);

        _isArch = isArch;
        time = len / _speed;
        //Debug.Log("dir:"+_dir);
    }

    void rotate(string[] ins)
    {
        rotAngle = float.Parse(ins[1]);
        time = float.Parse(ins[2]);
        _speed = 0;
        //gameObject.transform.Rotate(new Vector3(0, rot, 0));
    }

    public void gotShot() {
        _ip.shot = true;
        destroy();
        Debug.Log("got shot balloon");
        AudioSource src = gameObject.GetComponent<AudioSource>();
        src.spatialBlend = 1;
        if (src != null && src.clip != null)
            src.PlayOneShot(src.clip);
    }
    void destroy()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;

        
        //gameObject.GetComponent<Collider>().enabled
    }
    void OnDestroy()
    {
        Destroy(gameObject.GetComponent<SensingLib>());
    }
}
