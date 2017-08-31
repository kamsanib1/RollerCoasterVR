//Generates tracks and stores them using ObjectManager class.
//Multiple points are generated even between tracks for animation and mesh purpose.
//Number of points generated is based on points_per_track variable.
//every points has two objects one of which is parent to other.
//The parent stores the position and twist. Child takes care of self rotation.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WallBuilder
{
    public List<GameObject> points = new List<GameObject>(); //stores all the child objects.
    public int model = 0;
    public float spacing = 0.5f;

    private bool ready_flag = false;
    private GameObject _trackPrefab;

    //private
    int track_counter = 0;          //count for number of objects
    float total_rotation = 0f;      //total rotaion of track for self rotation
    WallGenerator rg;
    GameObject path;
    GameObject platform;

    Interpretor _ip;
    //GameObject _previous_rail;
    //recives prefab for track
    //receives physic material for rails
    public WallBuilder(GameObject __startingPoint)
    {
        if (__startingPoint == null) platform = new GameObject("wall");
        else platform = __startingPoint;
    }

    //generates track and stores those objects
    public void generate(string code)
    {
        string[] track; //track variable information 

        //initializing compiler and compiling code.//
        Compiler.setCode(code);
        Compiler.compile();

        //interpretor setup and initilisation//
        _ip = new Interpretor();
        _ip.setICode(Compiler.getICode());
        _ip.init();

        rg = new WallGenerator(this);
        path = new GameObject("path");
        rg.setPath(path);


        //parse the whole code.
        Compiler.setCode(code);
        if (!(Compiler.compile()))
        {
            return;
        }

        //Debug.Log("track generation sarted-------------------");
        //Debug.Log("s:" + speed_points + ",p:" + points_per_track);

        //each track instruction is provided 
        while ((track = _ip.nextIns()) != null)
        {
            string __st = track[0];
            if (__st == Compiler.EOP) break;
            else if (__st == "wall") { wallFunc(track); }
           
        }
        load();
    }

    void wallFunc(string[] __ins)
    {
        float length;   //length of track for a statement
        Direction direction;  //direction of a statement
        float angle;    //angle read from statememnt
        float rotation; //self rotation of statement

        float angle_per_track;  //angle distributed between each point.
        float rot_per_track;    //self rotation distributed between each point.
 
        GameObject last_rail;   //previous rail pointer
        GameObject new_rail;    //new rail that holds prefab.it holds the position and rotation
                                //all variables are retrieved
        GameObject new_empty;   //temporary pointer
        GameObject rail;        //*not used
        new_empty = new GameObject("new empty");
        rail = new GameObject("rail");

        length = int.Parse(__ins[1]) ;
        direction = getDirection(__ins[2]);
        angle = float.Parse(__ins[3]);
        rotation = float.Parse(__ins[4]);

        //variables are calculated for points
        angle_per_track = angle / length;
        rot_per_track = rotation / length;
 
        //loops to calculate the points for every statement.
        while (length > 0)
        {
            //first track 
            if (track_counter == 0)
            {
                new_rail = new GameObject("point" + track_counter);
                new_rail.transform.parent = path.transform;
                new_rail.transform.localPosition = Vector3.zero;
                new_empty.transform.localPosition = platform.transform.position;
            }
            //rest of tracks.
            else
            {
                last_rail = points[points.Count - 1];//.transform.parent.gameObject;
                new_rail = new GameObject("point" + track_counter);
                new_rail.transform.parent = path.transform;
                
                new_empty.transform.parent = last_rail.transform;

                //-----------setting local position of track---------------//
                new_empty.transform.position = findGround(last_rail.transform.position + last_rail.transform.forward * spacing);
                //new_empty.transform.localPosition = last_rail.transform.forward * spacing;
            }

            total_rotation += rot_per_track;
            total_rotation %= 360;
            //-----------setting the direction of track---------------//
            switch (direction)
            {
                  case Direction.RIGHT:
                    new_empty.transform.localRotation = Quaternion.Euler(0, angle_per_track, total_rotation);
                    break;
                 case Direction.LEFT:
                    new_empty.transform.localRotation = Quaternion.Euler(0, -angle_per_track, total_rotation);
                    break;
                case Direction.UP:
                case Direction.DOWN:
                case Direction.FORWARD:
                    new_empty.transform.localRotation = Quaternion.Euler(0, 0, total_rotation);
                    break;
            }

            //setting the object transform
            new_rail.transform.position = new_empty.transform.position;
            new_rail.transform.rotation = new_empty.transform.rotation;
            
            //storing the objects.
            points.Add(new_rail);
                        
            track_counter++;
            length--;

           
        }
        Object.Destroy(new_empty);
        Object.Destroy(rail);
    }

    Vector3 findGround(Vector3 initPos)
    {
        Vector3 point = initPos;
        bool groundHit = false;
        RaycastHit[] hits;
        Ray ray;
        ray = new Ray(initPos, Vector3.up);
        hits = Physics.RaycastAll(ray);
        for(int i = 0; i < hits.Length; i++)
        {
            if(hits[i].collider.tag == "ground")
            {
                point = hits[i].point;
                //Debug.Log("raycast up hit:" +i+"::point:" + point);
                groundHit = true;
                break;
            }
        }
        if (!groundHit)
        {
            ray = new Ray(initPos, Vector3.down);
            hits = Physics.RaycastAll(ray);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.tag == "ground")
                {
                    point = hits[i].point;
                    //Debug.Log("raycast down hit:" + i + "::point:" + point);
                    groundHit = true;
                    break;
                }
            }
        }
        return point;
    }
  
    //restore all values
    public bool reset()
    {
        if (track_counter == 0) return true;
        total_rotation = 0;
        track_counter = 0;
        ready_flag = false;
        rg.flush();

        return true;
    }
    public void load()
    {
          Debug.Log("loading wall graphics...");
          if (model == 0) rg.fenceWall();
          else if (model == 1) rg.wallGenerate();
   }
    
   public void setModel(string m) { model = 0; }
    public GameObject getMainObject() { return rg.getMainObject(); }
    private Direction getDirection(string dir)
    {
        if (dir.ToLower() == "up") { return Direction.UP; }
        else if (dir.ToLower() == "down") { return Direction.DOWN; }
        else if (dir.ToLower() == "left") { return Direction.LEFT; }
        else if (dir.ToLower() == "right") { return Direction.RIGHT; }
        else if (dir.ToLower() == "forward") { return Direction.FORWARD; }
        return Direction.FORWARD;
    }
}


