using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class BehaviourInterfaceImg : MonoBehaviour
{
    string currentCode = "", output = "";

    private int _labelWidth = 80;
    private int _changeBtnWidth = 60;
    private int _curScrWidth = 0;
    private int _curScrHeight = 0;
    GUIStyle inspectorStyle;
    private int turnAngle = 17;
    private float _sclObj;

    public Texture grw = null;
    public Texture shrnk = null;
    public Texture rotRight = null;
    public Texture rotLeft = null;
    public Texture rst = null;
    public Texture rotateUL = null;
    public Texture rotateUR = null;
    public Texture rotateDL = null;
    public Texture rotateDR = null;
    private float scalefactor = 0.01f;
    private float rotFactor = 0.5f;
    private string s1;
    private int _rotVal = 0;

    MainObject obj;
    int _activeObj = -1;

    Vector2 _insScrollPos;
    Vector2 _codeScrollPos;
    Vector2 _outputScrollPos;


    void Start()
    {
        Data.output = "";
        _insScrollPos = Vector2.zero;
        _codeScrollPos = Vector2.zero;
        _outputScrollPos = Vector2.zero;
    }

    void OnGUI()
    {
        GUI.backgroundColor = Color.clear;
        if (Screen.width != _curScrWidth || Screen.height != _curScrHeight)
        {
            _curScrHeight = Screen.height;
            _curScrWidth = Screen.width;
            inspectorStyle = new GUIStyle();
            inspectorStyle.normal.background = MakeTex(Screen.width - Data._inspectorWidth, Screen.height, Color.clear);
        }
        //Debug.Log(Screen.width - 1.5f * Data._inspectorWidth);
        inspector();
    }

    void inspector()
    {
        if (Data.objects.Count == 0) return;
        if (_activeObj != Data.activeObj)
        {
            obj = Data.objects[Data.activeObj].GetComponent<MainObject>();
        }

        if (obj == null) return;
        //GUILayout.BeginArea(new Rect(new Vector2(Screen.width - 1.5f * Data._inspectorWidth, 30), new Vector2(Screen.width - Data._inspectorWidth, Screen.height - 90)), inspectorStyle);
        GUILayout.BeginArea(new Rect(new Vector2(Screen.width - 1.3f * Data._inspectorWidth, 30), new Vector2(Screen.width - Data._inspectorWidth, Screen.height - Screen.height * 4 / 5)), inspectorStyle);
        rotateButtons();
        GUILayout.EndArea();
        GUILayout.BeginArea(new Rect(new Vector2(Screen.width - 1.3f * Data._inspectorWidth, Screen.height - 200), new Vector2(Screen.width - Data._inspectorWidth, Screen.height - 90)), inspectorStyle);
        scaleButtons();
        GUILayout.EndArea();
        //GUILayout.EndArea();
    }
    private void rotateButtons()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("", GUILayout.MaxWidth(10));
        GUILayout.BeginVertical();
        GUILayout.Label("   Rotate");
        GUILayout.BeginHorizontal();
        if (GUILayout.RepeatButton(rotateUL, GUILayout.MaxWidth(25), GUILayout.MaxHeight(25)))
        {
            rotateUpLeft();
        }
        if (GUILayout.RepeatButton("", GUILayout.MaxWidth(25), GUILayout.MaxHeight(25)))
        {
            //rotateUp();
        }
        if (GUILayout.RepeatButton(rotateUR, GUILayout.MaxWidth(25), GUILayout.MaxHeight(25)))
        {
            //rotateUpRight();
            rotateUp();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.RepeatButton(rotLeft, GUILayout.MaxWidth(25), GUILayout.MaxHeight(25)))
        {
            rotateLeft();
        }
        if (GUILayout.RepeatButton(rst, GUILayout.MaxWidth(25), GUILayout.MaxHeight(25)))
        {
            resetRotation();
        }
        if (GUILayout.RepeatButton(rotRight, GUILayout.MaxWidth(25), GUILayout.MaxHeight(25)))
        {
            rotateRight();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.RepeatButton(rotateDL, GUILayout.MaxWidth(25), GUILayout.MaxHeight(25)))
        {
            //rotateDownLeft();
            rotateDown();
        }
        if (GUILayout.RepeatButton("", GUILayout.MaxWidth(25), GUILayout.MaxHeight(25)))
        {
            //rotateDown();
        }
        if (GUILayout.RepeatButton(rotateDR, GUILayout.MaxWidth(25), GUILayout.MaxHeight(25)))
        {
            rotateDownRight();
        }
        GUILayout.EndHorizontal();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }
    private void scaleButtons()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("", GUILayout.MaxWidth(10));
        GUILayout.BeginVertical();
        GUILayout.Label("    Scale");
        GUILayout.BeginHorizontal();
        if (GUILayout.RepeatButton(rotateUL,  GUILayout.MaxWidth(25), GUILayout.MaxHeight(25)))
        {
            scaleUpLeft();
        }
        if (GUILayout.RepeatButton(grw,  GUILayout.MaxWidth(25), GUILayout.MaxHeight(25)))
        {
            scaleUpDefault();
        }
        if (GUILayout.RepeatButton(rotateUR,  GUILayout.MaxWidth(25), GUILayout.MaxHeight(25)))
        {
            scaleUpRight();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.RepeatButton(rotLeft,  GUILayout.MaxWidth(25), GUILayout.MaxHeight(25)))
        {
            scaleLeft();
        }
        if (GUILayout.RepeatButton(rst,  GUILayout.MaxWidth(25), GUILayout.MaxHeight(25)))
        {
            resetScale();
        }
        if (GUILayout.RepeatButton(rotRight,  GUILayout.MaxWidth(25), GUILayout.MaxHeight(25)))
        {
            scaleRight();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.RepeatButton(rotateDL,  GUILayout.MaxWidth(25), GUILayout.MaxHeight(25)))
        {
            scaleDownLeft();
        }
        if (GUILayout.RepeatButton(shrnk,  GUILayout.MaxWidth(25), GUILayout.MaxHeight(25)))
        {
            scaleDownDefault();
        }
        if (GUILayout.RepeatButton(rotateDR,  GUILayout.MaxWidth(25), GUILayout.MaxHeight(25)))
        {
            scaleDownRight();
        }
        GUILayout.EndHorizontal();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    void rotateRight()
    {
        GameObject objt = Data.objects[Data.activeObj];
        objt.transform.Rotate(Vector3.up * rotFactor);
        Debug.Log("Object:" + obj);
        obj.rotation = objt.transform.rotation;
        //obj.transform.rotation = Quaternion.Euler(objt.transform.rotation.x, objt.transform.rotation.y, objt.transform.rotation.z);
    }
    void rotateLeft()
    {
        GameObject objt = Data.objects[Data.activeObj];
        objt.transform.Rotate(Vector3.down * rotFactor);
        Debug.Log("Object:" + obj);
        //obj.transform.rotation = Quaternion.Euler(objt.transform.rotation.x, objt.transform.rotation.y, objt.transform.rotation.z);
        obj.rotation = objt.transform.rotation;
    }
    void rotateUp()
    {
        GameObject objt = Data.objects[Data.activeObj];
        objt.transform.Rotate(Vector3.right * rotFactor);
        Debug.Log("Object:" + objt);
        //obj.transform.rotation = Quaternion.Euler(objt.transform.rotation.x, objt.transform.rotation.y, objt.transform.rotation.z);
        obj.rotation = objt.transform.rotation;
    }
    void rotateDown()
    {
        GameObject objt = Data.objects[Data.activeObj];
        objt.transform.Rotate(Vector3.left * rotFactor);
        Debug.Log("Object:" + obj);
        //obj.transform.rotation = Quaternion.Euler(objt.transform.rotation.x, objt.transform.rotation.y, objt.transform.rotation.z);
        obj.rotation = objt.transform.rotation;
    }
    void rotateUpLeft()
    {
        GameObject objt = Data.objects[Data.activeObj];
        objt.transform.Rotate(Vector3.forward * rotFactor);
        Debug.Log("Object:" + obj);
        //obj.transform.rotation = Quaternion.Euler(objt.transform.rotation.x, objt.transform.rotation.y, objt.transform.rotation.z);
        obj.rotation = objt.transform.rotation;
    }
    void rotateUpRight()
    {
        GameObject objt = Data.objects[Data.activeObj];
        objt.transform.Rotate(new Vector3(1, 0, -1) * rotFactor);
        Debug.Log("Object:" + obj);
        //obj.transform.rotation = Quaternion.Euler(objt.transform.rotation.x, objt.transform.rotation.y, objt.transform.rotation.z);
        obj.rotation = objt.transform.rotation;
    }
    void rotateDownLeft()
    {
        GameObject objt = Data.objects[Data.activeObj];
        objt.transform.Rotate(new Vector3(-1, 0, 1) * rotFactor);
        Debug.Log("Object:" + obj);
        //obj.transform.rotation = Quaternion.Euler(objt.transform.rotation.x, objt.transform.rotation.y, objt.transform.rotation.z);
        obj.rotation = objt.transform.rotation;
    }
    void rotateDownRight()
    {
        GameObject objt = Data.objects[Data.activeObj];
        objt.transform.Rotate(Vector3.back * rotFactor);
        Debug.Log("Object:" + obj);
        //obj.transform.rotation = Quaternion.Euler(objt.transform.rotation.x, objt.transform.rotation.y, objt.transform.rotation.z);
        obj.rotation = objt.transform.rotation;
    }

    void scaleRight()
    {
        GameObject objt = Data.objects[Data.activeObj];
        objt.transform.localScale += Vector3.right * scalefactor;
        obj.scale = objt.transform.localScale;
        Debug.Log("Object:" + obj);
    }
    void scaleLeft()
    {
        GameObject objt = Data.objects[Data.activeObj];
        objt.transform.localScale += Vector3.left * scalefactor;
        obj.scale = objt.transform.localScale;
        Debug.Log("Object:" + Data.activeObj);
    }
    void scaleUpDefault()
    {
        GameObject objt = Data.objects[Data.activeObj];
        //obj.transform.localScale += new Vector3((float)0.1, (float)0.1, (float)0.1);
        objt.transform.localScale += new Vector3(1, 1, 1) * scalefactor;
        obj.scale = objt.transform.localScale;
        Debug.Log("Object:" + Data.activeObj);
    }
    void scaleDownDefault()
    {
        GameObject objt = Data.objects[Data.activeObj];
        //obj.transform.localScale -= new Vector3((float)0.1, (float)0.1, (float)0.1);
        objt.transform.localScale += new Vector3(-1, -1, -1) * scalefactor;
        obj.scale = objt.transform.localScale;
        Debug.Log("Object:" + obj);
    }
    void scaleUpLeft()
    {
        GameObject objt = Data.objects[Data.activeObj];
        //obj.transform.Rotate(new Vector3(1, 0, 1) * 11, Space.Self);
        objt.transform.localScale += Vector3.forward * scalefactor;
        obj.scale = objt.transform.localScale;
        Debug.Log("Object:" + obj);
    }
    void scaleUpRight()
    {
        GameObject objt = Data.objects[Data.activeObj];
        objt.transform.localScale += Vector3.up * scalefactor;
        obj.scale = objt.transform.localScale;
        Debug.Log("Object:" + obj);
    }
    void scaleDownLeft()
    {
        GameObject objt = Data.objects[Data.activeObj];
        objt.transform.localScale += Vector3.down * scalefactor;
        obj.scale = objt.transform.localScale;
        Debug.Log("Object:" + obj);
    }
    void scaleDownRight()
    {
        GameObject objt = Data.objects[Data.activeObj];
        objt.transform.localScale += Vector3.back * scalefactor;
        obj.scale = objt.transform.localScale;
        Debug.Log("Object:" + obj);
    }
    void resetScale()
    {
        GameObject objt = Data.objects[Data.activeObj];
        objt.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        obj.scale = objt.transform.localScale;
    }
    void resetRotation()
    {
        GameObject objt = Data.objects[Data.activeObj];
        objt.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        obj.rotation = objt.transform.rotation;
    }
    void updateId()
    {
        for (int i = 0; i < Data.objects.Count; i++)
        {
            MainObject obj = Data.objects[i].GetComponent<MainObject>();
            if (Data.objects[i].tag == "Respawn") { Data.startingPointIndex--; }

            //if (obj == null) Debug.Log("no obj detected at(" + i + "):" + Data.objects[i].tag);
            if (obj != null) obj.id = i;
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

    }
}