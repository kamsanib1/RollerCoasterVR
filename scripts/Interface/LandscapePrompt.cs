using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandscapePrompt : MonoBehaviour {

    int _menuHeight = 30;
    int _objectHeight = 90;
    Texture _backgroundImg;
    GUIStyle _edidorBg = new GUIStyle();
    string currentCode = "", output = "";
    public GUISkin _btnSkin;
    private Display _display = Display.HOME;
    private string msg = "All the objects placed on this landscape will be lost. Are you sure you want to delete it?";
    public int pos=0;

    private int _curScrWidth = 0;
    private int _curScrHeight = 0;
    private int _buttonWidth = 150;
    GUIStyle objectStyle;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
        if (Screen.width != _curScrWidth || Screen.height != _curScrHeight)
        {
            _curScrHeight = Screen.height;
            _curScrWidth = Screen.width;
            //_objectHeight = Screen.height / 5;
            objectStyle = new GUIStyle();
            objectStyle.normal.background = MakeTex(Screen.width - Data._inspectorWidth, _objectHeight, Color.grey);
        }
        //GUILayout.BeginArea(new Rect(new Vector2(0, Screen.height - _objectHeight), new Vector2(Screen.width - Data._inspectorWidth, _objectHeight)), objectStyle);
        //GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(new Vector2(Screen.width / 8, Screen.height / 8), new Vector2((Screen.width - Data._inspectorWidth) * 2 / 3, Screen.height / 4)), objectStyle);
        menu();
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

    void menu()
    {
        GUILayout.BeginVertical();
        GUILayout.Label(msg);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("yes")) {
            //Destroy(Data.landscape);
            Data.reset();
            Data.landscape = Instantiate(Data.files[pos].file);
            DontDestroyOnLoad(Data.landscape);
            Data.landscape.tag = "ground";
            Data.landscape.GetComponent<PlayerPosition>().index = Data.files[pos].rcname;
            Data.setAnimalsOnGround();

            Destroy(gameObject.GetComponent<LandscapePrompt>());
        }
        if (GUILayout.Button("no")) { Destroy(gameObject.GetComponent<LandscapePrompt>()); }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    public void setPosition(int _pos) { pos = _pos; }
}
