using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandscapeMenu : MonoBehaviour {
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

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
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

        GUILayout.BeginArea(new Rect(new Vector2(Screen.width/8, Screen.height/8), new Vector2((Screen.width - Data._inspectorWidth)*2/3, Screen.height/4)), objectStyle);
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
    Vector2 _scrollPos = Vector2.zero;
    void menu()
    {
        int landscapeIndex =0;
        for(int i = 0; i < Data.files.Count; i++,landscapeIndex++) { if (Data.files[i].type == ObjectType.LANDSCAPE) break; }
        int indexStart = landscapeIndex;
        _scrollPos = GUILayout.BeginScrollView(_scrollPos);
        for (int i=0; i < Data.data.architecture.Length; i++)
        {
            GUILayout.BeginHorizontal();
            for (int j = 0; j < 4 && landscapeIndex - indexStart < Data.data.architecture.Length; j++, landscapeIndex++)
            {
                if (GUILayout.Button(Data.files[landscapeIndex].icon))
                {
                    initLandscape(landscapeIndex);
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
    }

    void initLandscape(int index)
    {
        Debug.Log("placing landscape...");
        GameObject landscape;
        if (Data.landscape != null) Destroy(Data.landscape);
        if (Data.files[index].file == null) landscape = Resources.Load("Landscapes\\" + Data.files[index].rcname) as GameObject;
        else landscape = Data.files[index].file;
        Data.landscape = Instantiate(landscape);
        DontDestroyOnLoad(Data.landscape);
        Data.landscape.tag = "ground";
        Data.landscape.GetComponent<PlayerPosition>().index = Data.files[index].rcname;
        Data.setAnimalsOnGround();
        Destroy(this.GetComponent<LandscapeMenu>());
    }
}
