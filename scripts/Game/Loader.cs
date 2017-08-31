using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour {
    public GameObject player;
    public GameObject VRPlayer;
    GameObject spawnObj ;
    string initialScene;
	// Use this for initialization
	void OnEnable () {
        initialScene = Data.sceneName;
        Cursor.visible = false;
        load();
    }
    void load()
    {
        Data.gameState = GameState.ROAM;
        int size = Data.objects.Count;
        playerInit();
        for (int index = 0; index < size; index++)
        {
            GameObject obj = Data.objects[index];
            obj.GetComponent<Drag>().enabled = false;
            //obj.transform.position = Data.animals[index].flag;
            if (obj.tag == "animal") { loadAnimal(index); }
            else if (obj.tag == "train") { loadTrain(index); }
            else if (obj.tag == "wall") { loadWall(index); }
            else if (obj.tag == "graph") { loadGraph(index); }
            else if (obj.tag == "Respawn") { resetSpawnPoint(index); }
            else if (obj.tag == "bird") { loadBird(index); }
            else if(obj.tag == "vehicle") { loadAnimal(index); }
            else if (obj.tag == "reflector") { loadReflector(index); }
            else { loadGeneric(index); }
        }
        Debug.Log("object count:" + size);
        //loadPlayer();      
    }
    // Update is called once per frame
    //editor aunch funcs//
    void Update () {
        if (Input.GetKeyDown(KeyCode.Escape)) { editorLaunch(); }
        if (Input.GetKeyDown(KeyCode.Backspace)) { Cursor.visible = !Cursor.visible; }
	}
    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 60, 30), "editor"))
        {
            editorLaunch();
        }
    }
    //end of editor launch functions//
    void loadAnimal(int index)
    {
        GameObject obj = Data.objects[index];
        obj.AddComponent<AnimalAnimation>();
        AnimalStats aStats = obj.AddComponent<AnimalStats>();
        aStats.setType(obj.GetComponent<MainObject>().model);
    }
    void loadBird(int index)
    {
        GameObject obj = Data.objects[index];
        obj.AddComponent<BirdAnim>();
        AnimalStats aStats = obj.AddComponent<AnimalStats>();
        aStats.setType(obj.GetComponent<MainObject>().model);
    }
    void loadTrain(int index)
    {
        GameObject obj = Data.objects[index];
        TrackBuilder tb;
        MainObject mo = obj.GetComponent<MainObject>();
        RollerCoaster rc = obj.GetComponent<RollerCoaster>();
        if (rc.tb != null)
        {
            rc.tb.reset();
            tb = rc.tb;
        }
        tb = new TrackBuilder(obj);
        tb.setModel(mo.model);
        tb.setType(0);
        tb.generate(mo.script);
        rc.tb = tb;
        
        //obj.AddComponent<AnimationTrigger>();
    }
    void loadWall(int index)
    {
        GameObject obj = Data.objects[index];
        WallBuilder wb;
        Wall wall = obj.GetComponent<Wall>();
        MainObject mo = obj.GetComponent<MainObject>();
        if (wall.wb != null)
        {
            wall.wb.reset();
            wb = wall.wb;
        }
        wb = new WallBuilder(obj);
        wb.setModel(mo.model);
        wb.generate(mo.script);
        wall.wb = wb;

    }
    void loadGraph(int index)
    {
        GameObject obj = Data.objects[index];
        GraphPlotter gp;
        Graph graph = obj.GetComponent<Graph>();
        MainObject mo = obj.GetComponent<MainObject>();
        if (graph.gp != null)
        {
            graph.gp.reset();
            //tb = rc.tb;
        }
        gp = new GraphPlotter(obj);
        gp.generate(mo.script);
        graph.gp = gp;
    }
    void loadReflector(int index)
    {
        GameObject obj = Data.objects[index];
        Collider col = obj.GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }
    void loadGeneric(int index)
    {
        GameObject obj = Data.objects[index];
        obj.AddComponent<MovementLib>();
    }
    void resetSpawnPoint(int index)
    {
        spawnObj = Data.objects[index];
        string code = null;
        if (code == null)
            code = spawnObj.GetComponent<MainObject>().script;

        Compiler.setCode(code);
        Compiler.compile();

        Interpretor _ip = new Interpretor();
        _ip.setICode(Compiler.getICode());
        _ip.init();
        string[] ins = _ip.nextIns();
        while ((ins) != null && ins[0] != Compiler.EOP) {
            if(ins[0] == "movement")
            {
                if(ins[1].Replace("\"","") == "false") { Data.movementFlag = false; }
            }
            ins = _ip.nextIns();
            RCLog.append(ins[0]);
        }
        Vector3 distAboveGround = new Vector3(0, 3, 0);
        if (Data.vrEnabled)
        {
            VRPlayer.transform.position = spawnObj.transform.position + distAboveGround;
        }
        else {
            player.transform.position = spawnObj.transform.position + distAboveGround;
        }
        spawnObj.SetActive(false);
    }
    void unload()
    {
        Data.movementFlag = true;
        if (spawnObj != null) spawnObj.SetActive(true);
        Data.gameState = GameState.EDIT;
        for(int i = 0; i < Data.objects.Count; i++)
        {
            GameObject obj = Data.objects[i];
            obj.GetComponent<Drag>().enabled = true;

            if (obj.tag == "animal" || obj.tag == "bird" || obj.tag == "vehicle")
            {
                Destroy(obj.GetComponent<AnimalStats>());
                Destroy(obj.GetComponent<AnimalAnimation>());
                Destroy(obj.GetComponent<VisionScript2>());
                Destroy(obj.GetComponent<BirdAnim>());
            }
            else if(obj.tag == "generic") {
                obj.SetActive(true);
                Destroy(obj.GetComponent<MovementLib>());
            }
            else if(obj.tag == "reflector")
            {
                obj.GetComponent<Collider>().enabled = true;
            }       
            obj.transform.position = obj.GetComponent<MainObject>().position;
            obj.transform.rotation = obj.GetComponent<MainObject>().rotation;
        }

        Data.dObjects.Clear();
        Data.globals.Clear();
        Cursor.visible = true;
    }
    void playerInit()
    {
        if (Data.vrEnabled) {
            player.SetActive(false);
            VRPlayer.transform.position = Data.landscape.GetComponent<PlayerPosition>().position.transform.position;
        }
        else {
            VRPlayer.SetActive(false);
            player.transform.position = Data.landscape.GetComponent<PlayerPosition>().position.transform.position;
        }
    }

    void editorLaunch()
    {
        Data.loadScene(initialScene);
        RCLog.append("checkpoint4");
        unload();
        RCLog.append("checkpoint5");
        SceneManager.LoadScene("editor");
        RCLog.append("checkpoint6");
    }
    public void reload()
    {
        unload();
        load();
    }
}
