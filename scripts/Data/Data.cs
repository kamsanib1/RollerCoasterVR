using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Data {
    public static InputData data;
    public static List<Animal> animals = new List<Animal>();
    public static List<RollerCoaster> rollerCoasters = new List<RollerCoaster>();
    public static List<Wall> walls = new List<Wall>();
    public static List<Graph> graphs = new List<Graph>();
    public static List<GameObject> objects = new List<GameObject>();
    public static List<DynamicObjects> dObjects = new List<DynamicObjects>();
    public static List<Globals> globals = new List<Globals>();
    public static string gameName;
    public static string sceneName;

    public static string output;
    public static int activeObj;
    //public static GameType gameType;

    public static GameObject landscape;
    public static Vector3 edit_cam_pos = new Vector3(0,10,-10);
    public static Quaternion edit_cam_rot = Quaternion.Euler(new Vector3(45,0,0));
    public static int startingPointIndex = -1;
    public static int music = -1;
    ///rollercoaster specific data///
    public const int speed_points = 50;
    public const int points_per_track = 1;
    //public static int 
    ///rollercoaster specific data///
    
    //interface data//
    public static int _inspectorWidth;
    public const int _menuHeight = 30;
    public static bool _menuOpen = false;
    public static RCDirectory home;
    public static List<RCDirectory> dirs = new List<RCDirectory>();
    public static List<RCFile> files = new List<RCFile>();
    //interface data//
    
    ///game playing data//
    public static bool vrEnabled = false;
    public static bool runTrain;
    public static bool trainRide;
    public static GameObject train;
    public static GameObject triggerObj;
    public static GameState gameState;
    public static bool destroyTrain;
    public static int playerItem;
    public static bool movementFlag = true;
    ///game playing data//
    //Debug specific variables//

    //Debug specific variables//

    private static bool _initFlag = false;
    public static void init() {
        if (!_initFlag)
        {
           _initFlag = true;
            loadFiles();
            loadDir();
            TriggerLibrary.init();
            FileManager.initGameData();
         }
    }
    public static void reset() {
        for(int i = 0; i < objects.Count; i++)
        {
            if(objects[i].tag == "train") { TrackBuilder tb = objects[i].GetComponent<RollerCoaster>().tb; if (tb!=null) tb.reset(); }
            else if (objects[i].tag == "wall") { WallBuilder wb = objects[i].GetComponent<Wall>().wb; if(wb!=null) wb.reset(); }
            GameObject.Destroy(objects[i]);
        }
        objects.Clear();
        animals.Clear();
        rollerCoasters.Clear();
        walls.Clear();
        Data.activeObj = 0;
        Data.startingPointIndex = -1;
        GameObject.Destroy(landscape);
    }

    public static void loadFiles()
    {
        RCFile[] list;
        Debug.Log("loading files...");
        list = data.animals;
        for (int i = 0; i < list.Length; i++)
            files.Add(list[i]);

        list = data.trains;
        for (int i = 0; i < list.Length; i++)
            files.Add(list[i]);

        list = data.human;
        for (int i = 0; i < list.Length; i++)
            files.Add(list[i]);

        list = data.walls;
        for (int i = 0; i < list.Length; i++)
            files.Add(list[i]);

        list = data.monsters;
        for (int i = 0; i < list.Length; i++)
            files.Add(list[i]);

        list = data.architecture;
        for (int i = 0; i < list.Length; i++)
            files.Add(list[i]);

        list = data.misc;
        for (int i = 0; i < list.Length; i++)
            files.Add(list[i]);

        list = data.transport;
        for (int i = 0; i < list.Length; i++)
            files.Add(list[i]);

        list = data.plant;
        for (int i = 0; i < list.Length; i++)
            files.Add(list[i]);

        list = data.flower;
        for (int i = 0; i < list.Length; i++)
            files.Add(list[i]);

        list = data.trees;
        for (int i = 0; i < list.Length; i++)
            files.Add(list[i]);

        list = data.birds;
        for (int i = 0; i < list.Length; i++)
            files.Add(list[i]);

        list = data.reptiles;
        for (int i = 0; i < list.Length; i++)
            files.Add(list[i]);

        list = data.Robot;
        for (int i = 0; i < list.Length; i++)
            files.Add(list[i]);

        list = data.balloons;
        for (int i = 0; i < list.Length; i++)
            files.Add(list[i]);

        list = data.robotpuppies;
        for (int i = 0; i < list.Length; i++)
            files.Add(list[i]);

        list = data.zombie;
        for (int i = 0; i < list.Length; i++)
            files.Add(list[i]);

        list = data.snowman;
        for (int i = 0; i < list.Length; i++)
            files.Add(list[i]);

        list = data.houses;
        for (int i = 0; i < list.Length; i++)
            files.Add(list[i]);
        //Debug.Log("**********files List*********");
        //for(int i = 0; i < files.Count; i++) {
        //    Debug.Log(i + ":" + files[i].rcname);
        //}
        //Debug.Log("**********end of files List*********");
    }

    public static void loadDir() {
        //loading texxt asset from resources.//
        string file = (UnityEngine.Resources.Load("directory") as TextAsset).text;
        //string file = FileManager.loadFile(filePath);
        file = file.Replace("\r", "");
        string[] dir_l = file.Split('\n');
        Debug.Log("loading directories...");

        //create new directory with specified names.//
        for (int i = 0; i < dir_l.Length; i++)
        {
            string[] tmp = dir_l[i].Split(':');
            RCDirectory tmp_d = new RCDirectory();
            tmp_d.dirs = new List<RCDirectory>();
            tmp_d.files = new List<RCFile>();
            tmp_d.rcname = tmp[0];
            dirs.Add(tmp_d);
        }

        //update all directory links inside a directory.//
        for (int i = 0; i < dirs.Count; i++)
        {
            string loc = dir_l[i].Split(':')[1];
            if (loc == "") continue;
            string[] tmp = loc.Split(','); RCDirectory tmp_d;
            //Debug.Log(dir_l[i].Split(':')[0]);
            for(int k = 0; k < tmp.Length; k++)
            {
                for (int j = 0; j < dirs.Count; j++)
                    if (dirs[j].rcname == tmp[k] && i!=j)
                    { dirs[i].dirs.Add(dirs[j]);
                    //    Debug.Log(dirs[j].rcname);
                    }
            }
        }

        //update all file links inside directory.//
        for (int i = 0; i < dirs.Count; i++)
        {
            string loc = dir_l[i].Split(':')[2];
            if (loc == "") continue;
            string[] tmp = loc.Split(',');
            for (int k = 0; k < tmp.Length; k++)
            {
                for (int j = 0; j < files.Count; j++)
                    if (files[j].rcname == tmp[k])
                    { dirs[i].files.Add(files[j]);
                    //    Debug.Log(files[j].rcname);
                    }
            
            }
        }
    }

    public static int getFileIndex(string model)
    {
        int m = 0;
        for (int i = 0; i < files.Count; i++)
        {
            if(files[i].rcname == model) { m = i; break; }
        }
        return m;
    }

    public static void setAnimalsOnGround()
    {
        for (int i = 0; i < Data.objects.Count; i++)
        {
            GameObject obj = Data.objects[i];
            if (Data.objects[i].tag == "rollercoaster" || obj.tag == "generic" || obj.tag == "reflector") continue;

            RaycastHit hit = new RaycastHit();
            Ray ray = new Ray(obj.transform.position, new Vector3(0, -1, 0));
            Ray ray2 = new Ray(obj.transform.position, new Vector3(0, 1, 0));
            if (Physics.Raycast(ray, out hit))
            {
                setPosition(obj, hit.point);
            }
            else if (Physics.Raycast(ray2, out hit))
            {
                setPosition(obj, hit.point);
            }
        }
    }

    private static void setPosition(GameObject gameObject, Vector3 point) {
        MainObject obj;
        obj = gameObject.GetComponent<MainObject>();

        gameObject.transform.position = point;
        obj.position = point;
    }

    public static void loadScene(string sceneName)
    {
        RCLog.append("-----------------------------***-------------------------------");
        sceneName = sceneName.Replace("\"", "");
        int index = -1;
        UnityEngine.Debug.Log(sceneName);
        string[] gameFiles = FileManager.getAllScenes();
        //foreach (string s in gameFiles) UnityEngine.Debug.Log(s);
        for (int i = 0; i < gameFiles.Length; i++)
        {
            if (gameFiles[i] == sceneName) { index = i; break; }
        }
        if (index < 0) return;
        RCLog.append("checkpoint1");
        Data.reset();
        Data.sceneName = sceneName;
        UnityEngine.GameObject loader = UnityEngine.GameObject.Find("Loader");
        UnityEngine.GameObject gui = new UnityEngine.GameObject("GUI");
        ObjectInterface oi = gui.AddComponent<ObjectInterface>();
        FileManager.loadScene();
        UnityEngine.GameObject.Destroy(gui);
        RCLog.append("checkpoint2");
        loader.GetComponent<Loader>().reload();
        RCLog.append("checkpoint3");

    }
}