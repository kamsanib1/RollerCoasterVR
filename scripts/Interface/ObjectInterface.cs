using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Display { HOME,ANIMALS,RC,WALL,GRAPH,EXTRA};
public class ObjectInterface : MonoBehaviour {

    int _menuHeight = 30;
    int _objectHeight = 90;
    Texture _backgroundImg;
    GUIStyle _edidorBg = new GUIStyle();
    string currentCode = "", output = "";
    public GUISkin _btnSkin;
    private Display _display = Display.HOME;

    private int _curScrWidth = 0;
    private int _curScrHeight = 0;
    private int _buttonWidth = 150;
    GUIStyle objectStyle;

    List<RCDirectory> _dirPath;
    RCDirectory _curDir;

    void Start() {
       //add home directory
        Data.init();
        _dirPath = new List<RCDirectory>();
        _dirPath.Add(Data.dirs.Find(x => x.rcname == "home"));
        _curDir = _dirPath[0];

        Data.setAnimalsOnGround();
    }

    void OnGUI()
    {
        if (Screen.width != _curScrWidth || Screen.height != _curScrHeight)
        {
            _curScrHeight = Screen.height;
            _curScrWidth = Screen.width;
            objectStyle = new GUIStyle();
            objectStyle.normal.background = MakeTex(Screen.width - Data._inspectorWidth, _objectHeight, Color.grey);
        }
        
        GUILayout.BeginArea(new Rect(new Vector2(0, Screen.height - _objectHeight), new Vector2(Screen.width - Data._inspectorWidth, _objectHeight)), objectStyle);
        objectBar();
        GUILayout.EndArea();
    }
    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];

        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }
    void objectBar()
    {
        GUILayout.BeginVertical();
        path();
        objects();
        GUILayout.EndVertical();
    }
    void path() {
        GUILayout.BeginHorizontal();
        for(int i = 0; i < _dirPath.Count; i++)
        {
            if (GUILayout.Button(_dirPath[i].rcname, GUILayout.MaxWidth(_buttonWidth))) {
                for (int j = _dirPath.Count - 1; _dirPath[i].rcname != _dirPath[j].rcname; j--)
                    _dirPath.RemoveAt(_dirPath.Count - 1);
                _curDir = _dirPath[_dirPath.Count - 1];
            } 
        }
        GUILayout.EndHorizontal();
    }
    void objects() {
        _scrollPos = GUILayout.BeginScrollView(_scrollPos);
        GUILayout.BeginHorizontal();
        //display directories//
        for (int i = 0; i < _curDir.dirs.Count; i++) {
            if (GUILayout.Button(_curDir.dirs[i].rcname, GUILayout.MaxWidth(_buttonWidth))) {
                _dirPath.Add(_curDir.dirs[i]);
                _curDir = _curDir.dirs[i];
            }
        }
        //display files//
        for (int i = 0; i < _curDir.files.Count; i++)
        {
            if (_curDir.files[i].icon != null) {
                if (GUILayout.Button(_curDir.files[i].icon, GUILayout.MaxWidth(_buttonWidth), GUILayout.MaxHeight(60)))
                {
                    for (int j = 0; j < Data.files.Count; j++)
                        if (_curDir.files[i].rcname == Data.files[j].rcname)
                        { raycastPosition(j); }
                }
            }
            else {
                if (GUILayout.Button(_curDir.files[i].rcname, GUILayout.MaxWidth(_buttonWidth), GUILayout.MaxHeight (60)))
                {
                    for (int j = 0; j < Data.files.Count; j++)
                        if (_curDir.files[i].rcname == Data.files[j].rcname)
                        { raycastPosition(j); }
                }
            }
            
        }
        GUILayout.EndHorizontal();
        GUILayout.EndScrollView();
    }

    Vector2 _scrollPos =Vector2.zero;
    bool _drag = false;
    GameObject _animal,_ext;
    void raycastPosition(int pos) {
        RCFile file = Data.files[pos];

        if(file.type == ObjectType.LANDSCAPE) { placeLandscape(pos); return; }

        RaycastHit hit = new RaycastHit();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //Debug.Log(Input.mousePosition);
        Ray ray2 = new Ray(Camera.main.gameObject.transform.position, Camera.main.gameObject.transform.forward);
        if(Physics.Raycast(ray2,out hit))
        {
            Debug.Log("placing object:" + file.type);
            //objects cannot be placed without landscape.//
            if (file.type != ObjectType.LANDSCAPE && Data.landscape == null) return;
            placeObject(pos, hit.point, file.type);
            scriptSetup(Data.objects[Data.objects.Count - 1].GetComponent<MainObject>());
        }
    }

    public void placeObject(int pos, Vector3 point, ObjectType type)
    {
        switch (type)
        {
            case ObjectType.SNOWMAN:
            case ObjectType.ROBOT:
            case ObjectType.ROBOTPUPPIES:
            case ObjectType.ZOMBIE:
            case ObjectType.MONSTER:
            case ObjectType.HUMAN:
            case ObjectType.ANIMAL:
            case ObjectType.FLOWER:
                placeAnimal(pos, point);
                break;
            case ObjectType.TRAIN:
                placeTrain(pos, point);
                break;
            case ObjectType.WALL:
                placeWall(pos, point);
                break;
            case ObjectType.GRAPH:
                placeGraph(pos, point);
                break;
            case ObjectType.LANDSCAPE:
                placeLandscape(pos);
                break;
            case ObjectType.RESPAWN:
                setStartingPoint(pos, point);
                break;
            case ObjectType.DRIVING:
                placeDriving(pos, point);
                break;
            case ObjectType.BIRD:
                placeBird(pos, point);
                break;
            case ObjectType.BALLOON:
            case ObjectType.PLANT:
                placeGeneric(pos, point);
                break;
            case ObjectType.REFLECTOR:
                placeReflector(pos,point);
                break;
        }
        Data.activeObj = Data.objects.Count - 1;
    }

    public void placeAnimal(int pos,Vector3 point) {
        mainObjectSetup(pos, point);
        GameObject _animal = Data.objects[Data.objects.Count - 1];
        _animal.tag = "animal";
        _animal.AddComponent<ShootingHandler>();
        Animal animalObj = _animal.AddComponent<Animal>();
        Data.animals.Add(animalObj);
    }
    public void placeTrain(int pos,Vector3 point) {
        mainObjectSetup(pos, point);
        GameObject _train = Data.objects[Data.objects.Count - 1];
        _train.tag = "train";
        RollerCoaster trainObj = _train.AddComponent<RollerCoaster>();
        Data.rollerCoasters.Add(trainObj);
    }
    public void placeDriving(int pos,Vector3 point) {
        mainObjectSetup(pos, point);
        GameObject _vehicle = Data.objects[Data.objects.Count - 1];
        _vehicle.tag = "vehicle";
        //_vehicle.AddComponent<ShootingHandler>();
        Animal animalObj = _vehicle.AddComponent<Animal>();
    }
    public void placeWall(int pos,Vector3 point) {
        mainObjectSetup(pos, point);
        GameObject _wall = Data.objects[Data.objects.Count - 1];
        _wall.tag = "wall";
        Wall wall = _wall.AddComponent<Wall>();
        Data.walls.Add(wall);
    }
    public void placeGraph(int pos, Vector3 point)
    {
        mainObjectSetup(pos, point);
        GameObject _graph = Data.objects[Data.objects.Count - 1];
        _graph.tag = "graph";
        Graph graph = _graph.AddComponent<Graph>();
        Data.graphs.Add(graph);
    }
    public void placeGeneric(int pos,Vector3 point)
    {
        mainObjectSetup(pos, point);
        GameObject _generic = Data.objects[Data.objects.Count - 1];
        //_generic.AddComponent<MovementLib>();
        _generic.AddComponent<ShootingHandler>();
        _generic.tag = "generic";
    }
    public void placeReflector(int pos, Vector3 point)
    {
        mainObjectSetup(pos, point);
        //GameObject _generic = Data.objects[Data.objects.Count - 1];
        //_generic.AddComponent<MovementLib>();
        //_generic.AddComponent<ShootingHandler>();
        //_generic.tag = "generic";
    }
    public void placeBird(int pos, Vector3 point)
    {
        mainObjectSetup(pos, point);
        GameObject _bird = Data.objects[Data.objects.Count - 1];
        _bird.tag = "bird";
        Animal animalObj = _bird.AddComponent<Animal>();
        _bird.AddComponent<ShootingHandler>();
        Data.animals.Add(animalObj);
    }
    bool _displace = false;
    void Update()
    {
        
    }

    public void placeLandscape(int pos)
    {
        Debug.Log("placing landscape...");
        if (Data.landscape != null)
        {
            if (Data.objects.Count > 0) {
                LandscapePrompt lp = gameObject.AddComponent<LandscapePrompt>();
                lp.setPosition(pos);
                return;
            }
            else Destroy(Data.landscape);
        }
        GameObject landscape;
        if (Data.files[pos].file == null) landscape = Resources.Load("Landscapes\\" + Data.files[pos].rcname) as GameObject;
        else landscape = Data.files[pos].file;
        Data.landscape = Instantiate(landscape);
        DontDestroyOnLoad(Data.landscape);
        Data.landscape.tag = "ground";
        Data.landscape.GetComponent<PlayerPosition>().index = Data.files[pos].rcname;
        Data.setAnimalsOnGround();
       
    }
    
    public void setStartingPoint(int pos,Vector3 point) {
        
        GameObject _graph = GameObject.Instantiate(Data.files[pos].file);
        Object.DontDestroyOnLoad(_graph);
        _graph.transform.position = point + new Vector3(0, 0.5f, 0);
        _drag = true;
        _graph.tag = "Respawn";
        _graph.AddComponent<Drag>();
        MainObject graph = _graph.AddComponent<MainObject>();

        //create wall object and store.
        graph.position = _graph.transform.position;
        graph.rotation = _graph.transform.rotation;
        graph.scale = _graph.transform.localScale;
        graph.nameO = Data.files[pos].rcname;
        graph.model = Data.files[pos].rcname;
        graph.scriptRef = FileManager.getFilename( _graph.name );
        graph.script = "";
        graph.icode = null;
        graph.id = Data.objects.Count;
        //Data.graphs.Add(graph);
        if (Data.startingPointIndex >= 0) {
            GameObject gameObject = Data.objects[Data.startingPointIndex];
            Data.objects[Data.startingPointIndex] = _graph;
            graph.id = Data.startingPointIndex;
            Destroy(gameObject);
        }
        else { Data.startingPointIndex = Data.objects.Count; Data.objects.Add(_graph); }
    }
    void mainObjectSetup(int pos,Vector3 point)
    {
        //retrive the prefab file.//
        RCFile file = Data.files[pos];
        
        //initialize game object and add scripts to it.//
        GameObject gameObject = GameObject.Instantiate(file.file);
        Object.DontDestroyOnLoad(gameObject);
        gameObject.transform.position = point + new Vector3(0, 0.5f, 0);
        _drag = true;
        gameObject.AddComponent<Drag>();
        MainObject obj = gameObject.AddComponent<MainObject>();
        
        //create obj object and store.
        obj.position = gameObject.transform.position;
        obj.rotation = gameObject.transform.rotation;
        obj.scale = gameObject.transform.localScale;
        obj.nameO = file.rcname;
        obj.model = file.rcname;
        Debug.Log("file type of "+file.rcname+":"+file.type);
        obj.type = file.type;
        obj.script = "";
        obj.icode = null;
        obj.id = Data.objects.Count;
        Data.objects.Add(gameObject);

    }

    void scriptSetup(MainObject obj)
    {
        //setup script reference name. creates a new name if name already exists.//
        string scriptRef = FileManager.getFilename(obj.nameO);
        FileManager.createNewFile(scriptRef);
        obj.scriptRef = scriptRef;

    }
}
