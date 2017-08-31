using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphPlotter {
    GameObject origin;
    List<GameObject> points;
    GameObject line;
    WallBuilder wb;
    GameObject obj;

    string srcCode;
    Interpretor _ip;

    bool errorFlag = false;
    bool wallFlag = false;
    string wallScript = "";

    public GraphPlotter(GameObject origin) {
        this.origin = origin;
        points = new List<GameObject>();
    }
    
    public void generate(string code)
    {
        string[] func; //track variable information 

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

        //each track instruction is provided 
        while ((func = _ip.nextIns()) != null)
        {
            string __st = func[0];
            if (errorFlag) { break; }
            else if (__st == Compiler.EOP) { if(wallFlag)executeWall(); break; }
            else if (__st == "place") { placeObject(func); }
            else if (__st == "drawline") { drawLine(func); }
            else if (__st == "setposition") { setPosition(func); }
            else if (__st == "wall") { wallFlag = true;addWall(func); }
         }
    }

    private void placeObject(string[] func) {
        GameObject[] objects = Data.data.graphObjects;
        bool runtimeErrorFlag = true;
        for (int i = 0; i < objects.Length; i++)
        {
            if(objects[i].name == func[1])
            {
                //the point is created local to that object.//
                GameObject obj = GameObject.Instantiate(objects[i]);
                obj.transform.parent = origin.transform;

                //read the coordinates from function and move to that position.//
                //note:this position is relative to origin.//
                float[] coords = new float[3];
                for (int j = 0; j < 3; j++) coords[j] = float.Parse(func[j + 2]);
                obj.transform.localPosition = new Vector3(coords[0], coords[2], coords[1]);

                points.Add(obj);
                runtimeErrorFlag = false;
                break;
            }
        }
        if(runtimeErrorFlag) { errorFlag = true; }
    }

    private void drawLine(string[] func) {

        string script;
        float angle = float.Parse(func[4]) - 90;
        float length = float.Parse(func[5]);

        setPosition(func);

        script = "wall 1 left " + angle + " 0;";
        script += "wall " + length + " forward 0 0;";
        Debug.Log("line script:"+script);
        if (wb != null) wb.reset();
        wb = new WallBuilder(obj);
        wb.setModel("fence");
        wb.generate(script);
        //Object.Destroy(obj);
    }
    private void setPosition(string[] func) {
        obj = new GameObject("line");
        obj.transform.parent = origin.transform;

        float x = float.Parse(func[1]);
        float y = float.Parse(func[2]);
        float z = float.Parse(func[3]);
        
        obj.transform.localPosition = new Vector3(x, z, y);
    }
    private void addWall(string[] func) {
        for(int i=0;i<func.Length;i++) {
            wallScript += func[i];
            if (i != func.Length - 1) wallScript += " ";
        }
        wallScript += ";";
    }
    private void executeWall() {
        Debug.Log("line script:" + wallScript);
        if (wb != null) wb.reset();
        wb = new WallBuilder(obj);
        wb.setModel("fence");
        wb.generate(wallScript);
    }

    public void reset() {
        for (int i = 0; i < points.Count; i++) Object.Destroy(points[i]);
        points.Clear();
        if(wb!=null) wb.reset();
        Object.Destroy(obj);
        Object.Destroy(line);
        
    }
}
