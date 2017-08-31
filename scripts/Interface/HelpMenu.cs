using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class HelpMenu : MonoBehaviour
{

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
    GUIStyle style = new GUIStyle();


    bool showTrainContent = false;
    string searchString = "";
    string displayContent = "";

    Vector2 menuScrollPos = new Vector2(0, 0);
    Vector2 contentScrollPos = new Vector2(0, 0);

    // Use this for initialization
    void Start()
    {
        Data._menuOpen = true;
        //resize();
    }

    void OnGUI()
    {
        if (Screen.width != _curScrWidth || Screen.height != _curScrHeight)
        {
            //resize();
        }
        GUILayout.BeginArea(new Rect(posX, posY, menuWidth, menuHeight), menuStyle);
        helpMenu();
        GUILayout.EndArea();
    }
    void helpMenu()
    {
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal(GUILayout.MaxHeight(45));
        menuScrollPos = GUILayout.BeginScrollView(menuScrollPos);
        GUILayout.BeginHorizontal(GUILayout.MinWidth(menuBarWidth));
        menuBar();
        GUILayout.EndHorizontal();
        GUILayout.EndScrollView();
        GUILayout.BeginHorizontal();
        searchBar();
        GUILayout.EndHorizontal();
        GUILayout.EndHorizontal();
        contentScrollPos = GUILayout.BeginScrollView(contentScrollPos);
        GUILayout.BeginHorizontal();
        //content();
        GUILayout.EndHorizontal();
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private void menuBar()
    {
        if (GUILayout.Button("Train", GUILayout.MaxWidth(btnWidth)))
        {
            displayContent = (Resources.Load("help\\train") as TextAsset).text;
        }

        if (GUILayout.Button("Animals", GUILayout.MaxWidth(btnWidth)))
        {
            displayContent = (Resources.Load("help\\animals") as TextAsset).text;
        }

        if (GUILayout.Button("Monsters", GUILayout.MaxWidth(btnWidth)))
        {
            displayContent = (Resources.Load("help\\monsters") as TextAsset).text;
        }

        if (GUILayout.Button("Princess", GUILayout.MaxWidth(btnWidth)))
        {
            displayContent = (Resources.Load("help\\princess") as TextAsset).text;
        }

        if (GUILayout.Button("Boys", GUILayout.MaxWidth(btnWidth)))
        {
            displayContent = (Resources.Load("help\\boys") as TextAsset).text;
        }

        if (GUILayout.Button("Heli", GUILayout.MaxWidth(btnWidth)))
        {
            displayContent = (Resources.Load("help\\heli") as TextAsset).text;
        }

        if (GUILayout.Button("Car", GUILayout.MaxWidth(btnWidth)))
        {
            displayContent = (Resources.Load("help\\car") as TextAsset).text;
        }

        if (GUILayout.Button("Environment", GUILayout.MaxWidth(btnWidth)))
        {
            displayContent = (Resources.Load("help\\environment") as TextAsset).text;
        }
    }

    private void searchBar()
    {
        style.richText = true;
        searchString = GUILayout.TextField(searchString, GUILayout.MaxWidth(menuWidth - menuBarWidth - 50));
        if (GUILayout.Button("Go", GUILayout.MaxWidth(searchBtnWidth)))
        {
            displayContent = "";
            Debug.Log("string to be searched:" + searchString);
            if (File.ReadAllText(@"Assets\\Resources\\help\\train.txt").ToLower().Contains(searchString.ToLower()))
            {
                displayContent += (Resources.Load("help\\train") as TextAsset).text.ToString().Replace(searchString.ToLower(), "<size=20><color=yellow>" + searchString.ToLower() + "</color></size>").Replace(searchString.ToLower().First().ToString().ToUpper() + searchString.Substring(1), "<size=20><color=yellow>" + searchString + "</color></size>");
                displayContent += "<color=white>==============================================================================================</color>";
            }

            if (File.ReadAllText(@"Assets\\Resources\\help\\animals.txt").ToLower().Contains(searchString.ToLower()))
            {
            }
        }
    }
}