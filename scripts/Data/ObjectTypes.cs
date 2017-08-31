using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum Direction { UP, DOWN, LEFT, RIGHT, FORWARD, BACKWARD };
public enum ObjectType { ANIMAL, TRAIN, WALL, MONSTER, GRAPH, HUMAN, LANDSCAPE, RESPAWN, DRIVING, PLANT,FLOWER, BIRD, ROBOT, ROBOTPUPPIES, ZOMBIE,SNOWMAN, DEFAULT,BALLOON , REFLECTOR};//Assigns values to objects based on their order. 
//the DEFAULT type is used for display in menu (not for any purpose!!!).//

public enum GameState { PAUSE,EDIT,TRAIN,ROAM,DRIVE};
//synchronised enums//
//syncronised with input data feilds such as animal list, train list and wall list.//
//wall :: 0:fence wall//
public enum WallModel { FENCE};
//trains :: 0:wood, 1:steel//
public enum RCModel { WOOD,STEEL};
public class MainObject :MonoBehaviour{
    public string nameO;
    public string script;
    public List<string> icode;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public string model;
    public string scriptRef;
    public int id;
    public ObjectType type;
    public int userType;

    public static implicit operator MainObject(GameObject v)
    {
        throw new NotImplementedException();
    }
}

public class Animal : MonoBehaviour
{
    public GameObject animal;
}

public class staticObjects : MonoBehaviour
{
    public GameObject staticObj;
}

public class RollerCoaster : MonoBehaviour
{
    public TrackBuilder tb;
    public GameObject trigger;
    public GameObject train;
}

public class Wall : MonoBehaviour
{
    public WallBuilder wb;
}
public class Graph : MonoBehaviour
{
    public GraphPlotter gp;
}
public class data
{
    public string name;
    public int value;
    public bool local;
    public int type;

    public data(string a, int b)
    {
        this.name = a;
        this.value = b;
        this.local = false;
        this.type = 0;
    }

    public data(string a, int b, bool c)
    {
        this.name = a;
        this.value = b;
        this.local = c;
        this.type = 0;
    }

    public void setValue(int val)
    {
        this.value = val;
    }

    public void setType(int t)
    {
        this.type = t;
    }
}

public class error
{
    public int err_no;
    public string des;
    public int err_line;

    public error(int a, int b)
    {
        this.err_no = a;
        this.err_line = b;
        this.des = string.Empty;
    }

    public error(int a, int b, string c)
    {
        this.err_no = a;
        this.err_line = b;
        this.des = c;
    }
}

public class Trigger {
    public int point;
}

public class SpeedTrigger :Trigger {
    public int value;
    public bool brake = false;
}

public class Props : Trigger {
    public GameObject prop;
    public int time;
}

public class Tunnel {
    public int start;
    public int end;

    public Tunnel(int __start,int __end)
    {
        start = __start;
        end = __end;
    }
}

public class RCDirectory {
    public string rcname;
    public List<RCDirectory> dirs;
    public List<RCFile> files;
}

[System.Serializable]
public class RCFile {
    public string rcname;
    public ObjectType type;
    public GameObject file;
    public Texture2D icon;
}

public class DynamicObjects
{
    public string rcname;
    public GameObject gameObject;
}

[System.Serializable]
public class AudioObject
{
    public string rcname;
    public AudioClip clip;
}

public class Globals
{
    public string rcname;
    public double val;

    public Globals(string name,double value) { rcname = name; val = value; }
}