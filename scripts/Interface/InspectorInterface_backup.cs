using UnityEngine;
using System.Collections;

public class InspectorInterface2 : MonoBehaviour {
    string currentCode = "", output = "";

    private int _labelWidth = 80;
    private int _changeBtnWidth = 60;
    private int _curScrWidth = 0;
    private int _curScrHeight = 0;
    GUIStyle inspectorStyle;

    MainObject obj;
    int _activeObj = -1;

    Vector2 _insScrollPos;
    Vector2 _codeScrollPos;
    Vector2 _outputScrollPos;

    void Start() {
        Data.output = "";
        _insScrollPos = Vector2.zero;
        _codeScrollPos = Vector2.zero;
        _outputScrollPos = Vector2.zero;
    }

    void OnGUI()
    {
        if (Screen.width != _curScrWidth || Screen.height != _curScrHeight)
        {
            _curScrHeight = Screen.height;
            _curScrWidth = Screen.width;
            inspectorStyle = new GUIStyle();
            inspectorStyle.normal.background = MakeTex(Screen.width - Data._inspectorWidth, Screen.height, Color.grey);
        }

        GUILayout.BeginArea(new Rect(new Vector2(Screen.width - Data._inspectorWidth, Data._menuHeight), new Vector2(Data._inspectorWidth, Screen.height-Data._menuHeight)), inspectorStyle);
        _insScrollPos = GUILayout.BeginScrollView(_insScrollPos);
        inspector();
        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }
    bool _nameFlag = false;
    bool _scriptFlag = false;
    string _scriptName = "";
    void inspector()
    {
        if (Data.objects.Count == 0) return;
        if (_activeObj != Data.activeObj)
        {
            obj = Data.objects[Data.activeObj].GetComponent<MainObject>();
        }
        if (obj == null) return;
        //labels displaying object name and filename.
        gameName();
        scriptName();

        //code input area and output display window. 
        codeWindow();
        outputWindow();

        //rest of interface.
        buttons();
    }
    void gameName() {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Object Name:", new GUILayoutOption[] { GUILayout.Width(_labelWidth) });
        if (_nameFlag) {
            obj.nameO = GUILayout.TextField(obj.nameO);
            if(Event.current.keyCode == KeyCode.Return) { _nameFlag = !_nameFlag; }
        }
        else GUILayout.Label(obj.nameO);
        if (GUILayout.Button("change", new GUILayoutOption[] { GUILayout.Width(_changeBtnWidth) })) { _nameFlag = !_nameFlag; }
        GUILayout.EndHorizontal();
    }
    void scriptName()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Script Name:", new GUILayoutOption[] { GUILayout.Width(_labelWidth) });
        if (_scriptFlag) {
            _scriptName = GUILayout.TextField(_scriptName);
            if (Event.current.keyCode == KeyCode.Return) { _scriptFlag = !_scriptFlag; }
        }
        else GUILayout.Label(obj.scriptRef);
        if (GUILayout.Button("change", new GUILayoutOption[] { GUILayout.Width(_changeBtnWidth) }))
        {
            if (_scriptFlag) { obj.scriptRef = FileManager.updateFileName(obj.scriptRef, _scriptName); }
            else { _scriptName = obj.scriptRef; }
            _scriptFlag = !_scriptFlag;
        }
        GUILayout.EndHorizontal();
    }
    void codeWindow() {
        currentCode = obj.script;
        GUILayout.Label("   Code Window:");
        _codeScrollPos = GUILayout.BeginScrollView(_codeScrollPos, new GUILayoutOption[] { GUILayout.Height(Screen.height / 2), GUILayout.ExpandHeight(false) });
        obj.script = GUILayout.TextArea(currentCode, new GUILayoutOption[] {  GUILayout.ExpandHeight(true) });
        GUILayout.EndScrollView();
        //Debug.Log(_codeScrollPos);
        //_codeScrollPos.y = 1;
        TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
        //Debug.Log(editor.cursorIndex);
        if (Event.current.type == EventType.KeyUp && !_nameFlag && !_scriptFlag)
        {
            if (Event.current.keyCode == KeyCode.UpArrow) { _codeScrollPos.y -= 16; }
            if (Event.current.keyCode == KeyCode.DownArrow || Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter) { _codeScrollPos.y += 16; }
        }
        //else
        //{
        //    if (Event.current.keyCode == KeyCode.UpArrow) { _codeScrollPos.y -= 12; Debug.Log("up arrow"); }
        //    if (Event.current.keyCode == KeyCode.DownArrow) { _codeScrollPos.y += 12; Debug.Log("down arrow"); }
        //}
    }
    void outputWindow() {
        GUILayout.Label("   Output Window:");
        _outputScrollPos = GUILayout.BeginScrollView(_outputScrollPos, new GUILayoutOption[] { GUILayout.Height(Screen.height / 6), GUILayout.ExpandHeight(false) });
         GUILayout.Label(Data.output, new GUILayoutOption[] { GUILayout.ExpandHeight(true) });
        GUILayout.EndScrollView();
    }
    private void buttons()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("compile"))
        {
            compile();
        }
        if (GUILayout.Button("delete")) {
            delete();
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("command help"))
        {
            CommandHelp ch = gameObject.AddComponent<CommandHelp>();
            //ch.setType();
        }
    }

    void compile() {
        Compiler.setCode(currentCode);
        Data.output = "";
        if (Compiler.compile())
        {
            Debug.Log("compilation succesfull");
            GameObject obj = Data.objects[Data.activeObj];
            if (obj.tag == "animal") { }
            else if (obj.tag == "train")
            {
                TrackBuilder tb;
                RollerCoaster rc = obj.GetComponent<RollerCoaster>();
                MainObject mo = obj.GetComponent<MainObject>();
                if (rc.tb != null)
                {
                    rc.tb.reset();
                    //tb = rc.tb;
                }
                tb = new TrackBuilder(obj); ;
                tb.setModel(mo.model);
                tb.setType(0);
                tb.generate(mo.script);
                rc.tb = tb;
            }
            else if (obj.tag == "wall")
            {
                WallBuilder wb;
                Wall wall = obj.GetComponent<Wall>();
                MainObject mo = obj.GetComponent<MainObject>();
                if (wall.wb != null)
                {
                    wall.wb.reset();
                    //tb = rc.tb;
                }
                wb = new WallBuilder(obj); 
                wb.setModel(mo.model);
                wb.generate(mo.script);
                wall.wb = wb;
            }
            else if (obj.tag == "graph")
            {
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
        }
        else {
            Debug.Log("compilation error:" + Compiler.getError());
            Data.output += Compiler.getError()+"\n";
        }
    }
    void delete() {
        Debug.Log("count:"+Data.objects.Count);
        MainObject obj = Data.objects[Data.activeObj].GetComponent<MainObject>();
        if(obj.tag == "Respawn") { Data.startingPointIndex = -1; }

        GameObject gameObject = Data.objects[Data.activeObj];
        if(obj.scriptRef!= null) FileManager.deleteFile(obj.scriptRef);
        Data.objects.RemoveAt(obj.id);
        Destroy(gameObject);
        if (Data.activeObj >= Data.objects.Count) Data.activeObj = Data.objects.Count - 1;
        Debug.Log("count:" + Data.objects.Count);
        
        updateId();
        Debug.Log("count:" + Data.objects.Count);
    }

    void updateId() {
        for(int i = 0; i < Data.objects.Count; i++)
        {
            MainObject obj = Data.objects[i].GetComponent<MainObject>();
            if(Data.objects[i].tag == "Respawn") { Data.startingPointIndex--; }
            
            //if (obj == null) Debug.Log("no obj detected at(" + i + "):" + Data.objects[i].tag);
            if(obj!=null) obj.id = i;

        }

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

    void Update()
    {
        //Debug.Log("update");
        if (Input.GetKey(KeyCode.A)) { Debug.Log("working"); }
        if (Input.GetKeyDown(KeyCode.KeypadEnter)) { _codeScrollPos.y += 12; Debug.Log("enter"); }
        if (Input.GetKeyDown(KeyCode.UpArrow)) { _codeScrollPos.y -= 12f; Debug.Log("up arrow"); }
        if (Input.GetKeyDown(KeyCode.DownArrow)) { _codeScrollPos.y += 12f; Debug.Log("down arrow"); }
        if (Input.GetKeyDown(KeyCode.Delete)) { if (Data.activeObj >= 0) delete(); }

     }
}
