//Generates tracks and stores them using ObjectManager class.
//Multiple points are generated even between tracks for animation and mesh purpose.
//Number of points generated is based on points_per_track variable.
//every points has two objects one of which is parent to other.
//The parent stores the position and twist. Child takes care of self rotation.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrackBuilder
{
    public List<GameObject> points = new List<GameObject>(); //stores all the child objects.
    public List<SpeedTrigger> speedTriggers;
    public List<Props> props;
    public List<Tunnel> tunnels;
    public int type = 0;
    public int model = 1;
    public int top_object = 0;          //top point index of roller coaster

    private float points_per_track;  //no of points for rail mesh.
    private float speed_points;      //no of points for animation
    private float top_point = 0f;        //top point y-coordinate
    private float spacing = .5f;         //spacing between tracks.
    private bool ready_flag = false;
    private GameObject _trackPrefab;

    //private
    int track_counter = 0;          //count for number of objects
    float total_rotation = 0f;      //total rotaion of track for self rotation
    bool set_top_flag = true;       //flag to indicate top point
    RailGenerator rg;
    GameObject path;
    GameObject platform;

    Interpretor _ip;
    //GameObject _previous_rail;
    //recives prefab for track
    //receives physic material for rails
    public TrackBuilder( GameObject __startingPoint)
    {
        if (__startingPoint == null) platform = GameObject.Find("Platform");
        else platform = __startingPoint;
        top_point = platform.transform.position.y;
        speed_points = Data.data.speed_points;
        points_per_track = Data.data.points_per_track;
        props = new List<Props>();
        speedTriggers = new List<SpeedTrigger>();
    }

    //generates track and stores those objects
    public void generate(string code)
    {
        string[] track; //track variable information 

        //initializing compiler and compiling code.//
        //parse the whole code.
        Compiler.setCode(code);
        if (!(Compiler.compile()))
        {
            return;
        }

        //interpretor setup and initilisation//
        _ip = new Interpretor();
        _ip.setICode(Compiler.getICode());
        _ip.init();

        rg = new RailGenerator(this);
        path = new GameObject("path");
        rg.setPath(path);

        speedTriggers = new List<SpeedTrigger>();
        tunnels = new List<Tunnel>();

        points_per_track = (int)(speed_points / points_per_track);  //number of points used to develop mesh
        

        //Debug.Log("track generation sarted-------------------");
        //Debug.Log("s:" + speed_points + ",p:" + points_per_track);

        //each track instruction is provided 
        while ((track = _ip.nextIns()) != null)
        {
            string __st = track[0];
            if (__st == Compiler.EOP) break;
            else if(__st == "track") { trackFunc(track); }
            else if (__st == "tunnel") { tunnelFunc(track); }
            else if (__st == "generate") { generateFunc(track); }
            else if (__st == "brake") { brakeFunc(track); }
            else if (__st == "boost") { boostFunc(track); }
            else if (isProp(__st)) { propFunc(track); }
            
        }
        Debug.Log("track count:" + points.Count);
        load();
    }

    bool isProp(string __prop) { return true; }

    void trackFunc(string[] __ins) {
        float length;   //length of track for a statement
        Direction direction;  //direction of a statement
        float angle;    //angle read from statememnt
        float rotation; //self rotation of statement
        float twist;    //twist variable in statememnt
        float dis_bt_track = spacing / speed_points;    //distance between points between tracks.

        float angle_per_track;  //angle distributed between each point.
        float rot_per_track;    //self rotation distributed between each point.
        float twist_per_track;  //twist distributed between each point.

        GameObject last_rail;   //previous rail pointer
        GameObject new_rail;    //new rail that holds prefab.it holds the position and rotation
        GameObject new_rail_p;  //parent to new rail object. It holds the self rotation.
                                //all variables are retrieved
        GameObject new_empty;   //temporary pointer
        GameObject rail;        //*not used
        new_empty = new GameObject("new empty");
        rail = new GameObject("rail");

        length = int.Parse(__ins[1]) * speed_points;
        direction = getDirection(__ins[2]);
        angle = float.Parse(__ins[3]);
        rotation = float.Parse(__ins[4]);
        twist = float.Parse(__ins[5]);
    
        //variables are calculated for points
        angle_per_track = angle / length;
        rot_per_track = rotation / length;
        twist_per_track = twist / length;
    
        //loops to calculate the points for every statement.
        while (length > 0)
        {
            //first track 
            if (track_counter == 0)
            {
                new_rail = new GameObject("rail" + track_counter);
                new_rail_p = new GameObject("track" + track_counter);
                //new_rail_p.transform.position = platform.transform.position;
                new_rail_p.transform.parent = path.transform;
                new_rail.transform.parent = new_rail_p.transform;
                new_rail.transform.localPosition = Vector3.zero;
                new_empty.transform.localPosition = platform.transform.position;
            }
            //rest of tracks.
            else
            {
                last_rail = points[points.Count - 1].transform.parent.gameObject;
                new_rail_p = new GameObject("track" + track_counter);
                new_rail = new GameObject("rail" + track_counter);
                new_rail_p.transform.parent = path.transform;
                new_rail.transform.parent = new_rail_p.transform;
    
                new_empty.transform.parent = last_rail.transform;
    
                //-----------setting local position of track---------------//
                new_rail.transform.parent = new_rail_p.transform;
                new_empty.transform.localPosition = Vector3.forward * dis_bt_track;
            }
    
            //-----------setting the direction of track---------------//
            switch (direction)
            {
                case Direction.UP:
                    new_empty.transform.localRotation = Quaternion.Euler(-angle_per_track, 0, twist_per_track);
                    break;
                case Direction.RIGHT:
                    new_empty.transform.localRotation = Quaternion.Euler(0, angle_per_track, twist_per_track);
                    break;
                case Direction.DOWN:
                    new_empty.transform.localRotation = Quaternion.Euler(angle_per_track, 0, twist_per_track);
                    break;
                case Direction.LEFT:
                    new_empty.transform.localRotation = Quaternion.Euler(0, -angle_per_track, twist_per_track);
                    break;
                case Direction.FORWARD:
                    new_empty.transform.localRotation = Quaternion.Euler(0, 0, twist_per_track);
                    break;
            }
    
            //-----------settting the self-rotation of track----------------//
            total_rotation += rot_per_track;
            total_rotation %= 360;
            new_rail.transform.localRotation = Quaternion.Euler(0, 0, total_rotation);
    
            //setting the object transform
            new_rail_p.transform.position = new_empty.transform.position;
            new_rail_p.transform.rotation = new_empty.transform.rotation;
            new_rail.transform.parent = new_rail_p.transform;
    
            //storing the objects.
            //object_mgr.add(new_rail_p);
            //last_rail = new_rail_p;
            points.Add(new_rail);
    
            //if (track_counter % points_per_track == 0) { path_orientation.add(new_rail); }
            if (track_counter % speed_points == 0)
            {
                //object_mgr_rp.add(new_rail);
                if (!ready_flag) ready_flag = true;
            }
            if (track_counter % speed_points == 0)
            {
                //rail = Object.Instantiate(track_prefab, Vector3.zero, Quaternion.identity) as GameObject;
                rail.transform.parent = new_rail.transform;
            }
            track_counter++;
            length--;
    
            //set top point
            if (set_top_flag && new_rail.transform.position.y >= top_point)
            {
                //Debug.Log("top_point:" + top_point);
                top_point = new_rail.transform.position.y;
                top_object = track_counter;
            }
            else set_top_flag = false;
        }
        Object.Destroy(new_empty);
        Object.Destroy(rail);
    }
    void generateFunc(string[] __ins) {

    }
    //tunnel function and variables.
    bool _tunnelFlag = false;
    int _tunnelStart = 0;
    void tunnelFunc(string[] __ins) {
        if(__ins[1] == "start" && !_tunnelFlag) {
            _tunnelStart = points.Count-1;
            _tunnelFlag = true;
        }
        else if(__ins[1] == "end" && _tunnelFlag) {
            Tunnel t = new Tunnel(_tunnelStart, points.Count - 1);
            tunnels.Add(t);
            _tunnelFlag = false;
        }
    }
    void propFunc(string[] __ins)
    {
        //trigger point
        int start = points.Count - 1;
        //index of prop in the data list.
        int index;
        int __time;

        //Debug.Log(__ins[0] + ":" + __ins[1]);
        //set the start index.
        if ((index=TriggerLibrary.Exists(__ins[0]))>=0)
        {
            index %= 100;
            if (points.Count-1 > 20*speed_points)
                start -= 20*(int)speed_points;
            else
                start = 0;
            if (__ins.Length == 2) __time = int.Parse(__ins[1]);
            else __time = 10;
            
            //set prop transformation w.r.t 
            GameObject tmp = new GameObject(__ins[0]);
            GameObject animal = Object.Instantiate(Data.data.objects[index]);

            tmp.transform.position = points[points.Count - 1].transform.position;
            tmp.transform.rotation = points[points.Count - 1].transform.rotation;
            animal.transform.parent = tmp.transform;
            animal.transform.localRotation = Quaternion.identity;
            animal.transform.localPosition = new Vector3(0, 0, 0);

            //audio setup.
            AudioSource __as = animal.AddComponent<AudioSource>();
            __as.clip = Data.data.object_voice[index];
            animal.SetActive(false);

            //create new prop and push it in.
            Props prop = new Props();
            prop.point = start;
            prop.prop = animal;
            prop.time = __time;
            props.Add(prop);
            //Debug.Log("prop added.");
        }
        //Debug.Log("index:" + index);
    }
    void brakeFunc(string[] __ins) {
        SpeedTrigger st = new SpeedTrigger();
        st.point = points.Count;
        st.value = int.Parse(__ins[1]);
        st.brake = true;
        speedTriggers.Add(st);
    }
    void boostFunc(string[] __ins) {
        SpeedTrigger st = new SpeedTrigger();
        st.point = points.Count;
        st.value = int.Parse(__ins[1]);
        st.brake = true;
        speedTriggers.Add(st);
    }

    //returns top pint of track.
    public float getTopPoint()
    {
        return top_point;
    }

    //restore all values
    public bool reset()
    {
        if (track_counter == 0) return true;
        total_rotation = 0;
        track_counter = 0;
        set_top_flag = true;
        top_point = 0f;
        ready_flag = false;
        tunnels = new List<Tunnel>();
        rg.flush();
        for (int i = 0; i < props.Count; i++)
            GameObject.Destroy(props[i].prop);
        props.Clear();
        return true;
    }
    public void load()
    {
        if (type == 0)
        {
            Debug.Log("loading track...\nmodel:" + model);
            if (model == 0) rg.woodCoaster();
            else if (model == 1) rg.steelCoaster();
        }
        else
        {
            Debug.Log("loading wall graphics...");
            if (model == 1) rg.wall();
            else if (model == 2) rg.fenceWall();
        }
    }
    public void addTrigger(int no) { /*trigger_point.Add(new trigger((int)(object_mgr_2.getCount() / speed_points), no)); */}
    public void setModel(string m) {
        model = 1;
        if (m == "wooden") model = 0;
        if(m=="steel") model = 1;
    }
    public void setType(int t) { type = t; }
    public GameObject getMainObject() { return rg.getMainObject(); }
    private int getTrackCount(int __pos) { return __pos / (int)speed_points; }
    private Direction getDirection(string dir)
    {
        if(dir.ToLower() == "up") { return Direction.UP; }
        else if(dir.ToLower() == "down") { return Direction.DOWN; }
        else if (dir.ToLower() == "left") { return Direction.LEFT; }
        else if(dir.ToLower() == "right") { return Direction.RIGHT; }
        else if (dir.ToLower() == "forward") { return Direction.FORWARD; }
        return Direction.FORWARD;
    }
}


