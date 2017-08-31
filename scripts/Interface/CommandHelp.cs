using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandHelp : MonoBehaviour {
    private ObjectType menu;
    private string content;
    private string file;

    private int _curScrWidth = 0;
    private int _curScrHeight = 0;

    GUIStyle menuStyle;

    int menuHeight;
    int menuWidth;
    int btnHeight = 30;
    int btnWidth = 150;
    int posX;
    int posY;

    int menuBarWidth;
    int searchBtnWidth = 30;

    bool showTrainContent = false;
    string searchString = "";
    string displayContent = "";

    Vector2 menuScrollPos = new Vector2(0, 0);
    Vector2 contentScrollPos = new Vector2(0, 0);
    
    // Use this for initialization
    void Start () {
        menu = ObjectType.LANDSCAPE;
        setType();      		
	}

    void OnGUI()
    {
        if (Screen.width != _curScrWidth || Screen.height != _curScrHeight)
        {
            resize();
        }
        GUILayout.BeginArea(new Rect(posX, posY, menuWidth, menuHeight), menuStyle);
        helpMenu();
        GUILayout.EndArea();
    }

    void helpMenu() {
        GUILayout.BeginVertical();
        GUILayout.Label(file);
        contentScrollPos = GUILayout.BeginScrollView(contentScrollPos);
        GUILayout.Label(content);
        GUILayout.EndScrollView();
        if (GUILayout.Button("close")) { Destroy(gameObject.GetComponent<CommandHelp>()); }
        GUILayout.EndVertical();
    }

    public void setType() {
        GameObject gObj = Data.objects[Data.activeObj];

        MainObject obj = gObj.GetComponent<MainObject>();

        menu = obj.type;

        file = "none";
        switch (menu)
        {
            case ObjectType.BIRD:
                file = "command help\\bird commands"; break;
            case ObjectType.ANIMAL:
                file = "command help\\animal commands"; break;
            case ObjectType.GRAPH:
                file = "command help\\animal commands"; break;
            case ObjectType.HUMAN:
                file = "command help\\human commands"; break;
            case ObjectType.MONSTER:
                file = "command help\\animal commands"; break;
            case ObjectType.TRAIN:
                file = "command help\\train commands"; break;
            case ObjectType.WALL:
                file = "command help\\wall commands"; break;
            case ObjectType.ROBOT:
                file = "command help\\robo commands"; break;
        }

        if (file == "none") content = "";
        else
        {
            content = Resources.Load<TextAsset>(file).text;
        }
}

    void resize()
    {
        posX = Screen.width / 9;
        posY = Screen.height * 5 / 36;
        _curScrHeight = Screen.height; ;
        _curScrWidth = Screen.width; ;
        menuWidth = Screen.width * 5 / 9;
        menuHeight = Screen.height * 6 / 9;
        menuBarWidth = menuWidth * 3 / 4;
        menuStyle = new GUIStyle();
        menuStyle.normal.background = MakeTex(menuWidth, menuHeight, Color.grey);

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
}
