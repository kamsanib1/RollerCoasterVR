using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuInterface : MonoBehaviour {
    //enums local to this script//
    private enum Menu { NONE, FILE, SCENE, MUSIC, OBJECTS, HELP }
    private enum FileMenu { NONE, NEW, LOAD, SAVE, SAVE_AS, QUIT }
    private enum SceneMenu { NONE, NEW, LOAD, SAVE, SAVE_AS, DELETE }

    GUIStyle menuStyle;
    int _menuBtnWidth = 100;
    int _menuOptionWidth = 200; 
    int _menuBtnHeight = 30;
    int _spacing = 0;

    bool _showFileMenu = false;
    bool _showObjectMenu = false;
    bool _showMusicMenu = false;
    bool  _showHelpMenu = false;
    Menu menu = Menu.NONE;

    private int _curScrWidth = 0;
    private int _curScrHeight = 0;

    void Start()
    {
    }

    void OnGUI() {
        if (Screen.width != _curScrWidth || Screen.height != _curScrHeight)
        {
            _curScrHeight = Screen.height;
            _curScrWidth = Screen.width;
            menuStyle = new GUIStyle();
            menuStyle.normal.background = MakeTex(Screen.width, Data._menuHeight, Color.grey);
        }

        GUILayout.BeginArea(new Rect(new Vector2(0, 0), new Vector2(Screen.width, Data._menuHeight)), menuStyle);
        GUILayout.EndArea();

        GUI.BeginGroup(new Rect(new Vector2(0, 0), new Vector2(Screen.width , Screen.height )));
        menuBar();
        GUI.EndGroup();
    }
    void menuBar()
    {
        int __space = _spacing;
        GUI.BeginGroup(new Rect(0, 0, Screen.width / 2, Screen.height / 2));
        if (GUI.Button(new Rect(0, 0, _menuBtnWidth, _menuBtnHeight), "File")) {if (menu == Menu.FILE) menu = Menu.NONE; else menu = Menu.FILE;  }
        if (_showFileMenu) { closeObjectMenu(); closeMusicMenu(); closeSceneMenu(); fileoptions(0); }
        GUI.EndGroup();

        __space += _menuBtnWidth + _spacing;
        GUI.BeginGroup(new Rect(__space, 0, Screen.width / 2, Screen.height / 2));
        if (GUI.Button(new Rect(0, 0, _menuBtnWidth, _menuBtnHeight), "Scene")) { if (menu == Menu.SCENE) menu = Menu.NONE; else menu = Menu.SCENE; }
        if (_showObjectMenu) { closeFileMenu(); closeObjectMenu(); closeMusicMenu(); sceneMenu(1); }
        GUI.EndGroup();

        __space += _menuBtnWidth + _spacing;
        GUI.BeginGroup(new Rect(__space, 0, Screen.width / 2, Screen.height / 2));
        if (GUI.Button(new Rect(0, 0, _menuBtnWidth, _menuBtnHeight), "Objects")) { if (menu == Menu.OBJECTS) menu = Menu.NONE; else menu = Menu.OBJECTS; }
        if (_showObjectMenu) { closeFileMenu(); closeMusicMenu(); closeSceneMenu(); objectsOptions(2); }
        GUI.EndGroup();

        __space += _menuBtnWidth + _spacing;
        GUI.BeginGroup(new Rect(__space, 0, Screen.width / 2, Screen.height / 2));
        if (GUI.Button(new Rect(0, 0, _menuBtnWidth, _menuBtnHeight), "Music")) { if (menu == Menu.NONE) menu = Menu.MUSIC; else menu = Menu.MUSIC; }
        if (_showMusicMenu) { closeFileMenu(); closeObjectMenu(); closeSceneMenu(); musicOptions(3); }
        GUI.EndGroup();

        __space += _menuBtnWidth + _spacing;
        if (GUI.Button(new Rect(__space, 0, _menuBtnWidth, _menuBtnHeight), "play 3D")) {
            FileManager.saveGame();
            Data.vrEnabled = false;
            SceneManager.LoadScene("game");
        }
        
        __space += _menuBtnWidth + _spacing;
        if (GUI.Button(new Rect(__space, 0, _menuBtnWidth, _menuBtnHeight), "play VR"))
        {
            FileManager.saveGame();
            Data.vrEnabled = true;
            SceneManager.LoadScene("game");
        }

        __space += _menuBtnWidth + _spacing;
        GUI.BeginGroup(new Rect(__space, 0, Screen.width / 2, Screen.height / 2));
        if (GUI.Button(new Rect(0, 0, _menuBtnWidth, _menuBtnHeight), "Help"))
        {
            _showHelpMenu = !_showHelpMenu;
            menu = Menu.HELP;
            if (_showHelpMenu) { closeFileMenu(); closeObjectMenu(); closeSceneMenu(); closeMusicMenu(); gameObject.AddComponent<HelpMenu>(); }
            else { Destroy(gameObject.GetComponent<HelpMenu>()); }
        }
        GUI.EndGroup();

        __space += _menuBtnWidth + _spacing;
        GUI.Label(new Rect(__space, 0, _menuBtnWidth * 2, _menuBtnHeight), " Game: "+Data.gameName);

        __space += _menuBtnWidth + _spacing;
        GUI.Label(new Rect(__space, 0, _menuBtnWidth * 2, _menuBtnHeight), " Scene: " + Data.sceneName);

        switch (menu)
        {
            case Menu.NONE:
                closeMenus();break;
            case Menu.FILE:
                fileoptions(0);break;
            case Menu.SCENE:
                sceneMenu(1);break;
            case Menu.OBJECTS:
                objectsOptions(2);break;
            case Menu.MUSIC:
                musicOptions(3);break;
            case Menu.HELP:
                break;
        }
    }
    //file menu//
    bool _loadGame = false;
    bool _newGame = false;
    bool _saveAs = false;
    void fileoptions(int offset)
    {
        int startX = offset * (_menuBtnWidth + _spacing);
        GUI.BeginGroup(new Rect(startX, _menuBtnHeight + _spacing, Screen.width, Screen.height));
        if (GUI.Button(new Rect(0, 0, _menuBtnWidth, _menuBtnHeight), "new")) { _newGame = !_newGame; }
        if (_newGame) { newGame(0); _loadGame = false; _saveAs = false; }
        if (GUI.Button(new Rect(0, (_menuBtnHeight + _spacing), _menuBtnWidth, _menuBtnHeight), "load")) { _loadGame = !_loadGame; }
        if (_loadGame) { loadGame(1); _newGame = false; _saveAs = false; }
        if (GUI.Button(new Rect(0, (_menuBtnHeight + _spacing) * 2, _menuBtnWidth, _menuBtnHeight), "save")) { saveGame(); }
        if (GUI.Button(new Rect(0, (_menuBtnHeight + _spacing) * 3, _menuBtnWidth, _menuBtnHeight), "save as")) { _saveAs = !_saveAs; }
        if (_saveAs) { saveAs(3); _newGame = false; _loadGame = false; }
        if (GUI.Button(new Rect(0, (_menuBtnHeight + _spacing) * 4, _menuBtnWidth, _menuBtnHeight), "quit")) { }
        GUI.EndGroup();
    }
    void sceneMenu(int offset)
    {
        int startX = offset * (_menuBtnWidth + _spacing);
        GUI.BeginGroup(new Rect(startX, _menuBtnHeight + _spacing, Screen.width, Screen.height));
        if (GUI.Button(new Rect(0, 0, _menuBtnWidth, _menuBtnHeight), "new scene")) { _newGame = !_newGame; }
        if (_newGame) { newScene(0); _loadGame = false; _saveAs = false; }
        if (GUI.Button(new Rect(0, (_menuBtnHeight + _spacing), _menuBtnWidth, _menuBtnHeight), "load scene")) { _loadGame = !_loadGame; }
        if (_loadGame) { loadScene(1); _newGame = false; _saveAs = false; }
        if (GUI.Button(new Rect(0, (_menuBtnHeight + _spacing) * 2, _menuBtnWidth, _menuBtnHeight), "save scene")) { saveGame(); }
        if (GUI.Button(new Rect(0, (_menuBtnHeight + _spacing) * 3, _menuBtnWidth, _menuBtnHeight), "save scene as")) { _saveAs = !_saveAs; }
        if (_saveAs) { saveAsScene(3); _newGame = false; _loadGame = false; }
        if (GUI.Button(new Rect(0, (_menuBtnHeight + _spacing) * 4, _menuBtnWidth, _menuBtnHeight), "delete scene")) { }
        GUI.EndGroup();

    }
    //objects menu//
    bool _showOptions = false;
    ObjectType displayOPtion;

    Vector2 _scrollPos = new Vector2(0,0);
    float heightS = Screen.width / 6;
    float widthS = 100;
    int optionsIndex = 0;
    void objectsOptions(int offset)
    {
        int startX = offset * (_menuBtnWidth + _spacing);
        GUI.BeginGroup(new Rect(startX, _menuBtnHeight + _spacing, Screen.width, Screen.height));
        if (GUI.Button(new Rect(0, 0, _menuBtnWidth, _menuBtnHeight), "all")) { _showOptions = !_showOptions; displayOPtion = ObjectType.DEFAULT; optionsIndex = 0;}
        if (GUI.Button(new Rect(0, (_menuBtnHeight + _spacing) *1, _menuBtnWidth, _menuBtnHeight), "trains")) { _showOptions = !_showOptions; displayOPtion = ObjectType.TRAIN; optionsIndex = 1;}
        if (GUI.Button(new Rect(0, (_menuBtnHeight + _spacing) *2, _menuBtnWidth, _menuBtnHeight), "animals")) { _showOptions = !_showOptions; displayOPtion = ObjectType.ANIMAL; optionsIndex = 2;}
        if (GUI.Button(new Rect(0, (_menuBtnHeight + _spacing) *3, _menuBtnWidth, _menuBtnHeight), "monsters")) { _showOptions = !_showOptions; displayOPtion = ObjectType.MONSTER; optionsIndex = 3;}
        if (GUI.Button(new Rect(0, (_menuBtnHeight + _spacing) *4, _menuBtnWidth, _menuBtnHeight), "human")) { _showOptions = !_showOptions; displayOPtion = ObjectType.HUMAN; optionsIndex = 4;}
        if (GUI.Button(new Rect(0, (_menuBtnHeight + _spacing) *5, _menuBtnWidth, _menuBtnHeight), "wall")) { _showOptions = !_showOptions; displayOPtion = ObjectType.WALL; optionsIndex = 5;}
        if (GUI.Button(new Rect(0, (_menuBtnHeight + _spacing) *6, _menuBtnWidth, _menuBtnHeight), "transport")) { _showOptions = !_showOptions; displayOPtion = ObjectType.DRIVING; optionsIndex = 6;}
        if (GUI.Button(new Rect(0, (_menuBtnHeight + _spacing) *7, _menuBtnWidth, _menuBtnHeight), "graph")) { _showOptions = !_showOptions; displayOPtion = ObjectType.GRAPH; optionsIndex = 7;}
        if (GUI.Button(new Rect(0, (_menuBtnHeight + _spacing) *8, _menuBtnWidth, _menuBtnHeight), "misc")) { _showOptions = !_showOptions; displayOPtion = ObjectType.RESPAWN; optionsIndex = 8; }
        if (_showOptions) { animalOptions(optionsIndex); }
        //if (GUI.Button(new Rect(0, (_menuBtnHeight + _spacing), _menuBtnWidth, _menuBtnHeight), "wall")) { _showWallOptions = !_showWallOptions; }
        //if (_showWallOptions) { _showOptions = false; _showAnimalOptions = false; wallOptions(1); }
        //if (GUI.Button(new Rect(0, (_menuBtnHeight + _spacing) * 2, _menuBtnWidth, _menuBtnHeight), "animals")) { _showAnimalOptions = !_showAnimalOptions; }
        //if (_showAnimalOptions) { _showWallOptions = false; _showOptions = false; animalOptions(2); }
        GUI.EndGroup();
    }
    //void rcOptions(int offset)
    //{
    //    float heightC = (_menuBtnHeight + _spacing * 2) * Data.rollerCoasters.Count;
    //    _scrollPos = GUI.BeginScrollView(new Rect((_menuBtnWidth + _spacing), (_menuBtnHeight + _spacing) * offset + _spacing, widthS, heightS), _scrollPos, new Rect(0, 0, _menuBtnWidth, heightC));
    //    for (int i = 0, index = 0; i < Data.objects.Count; i++)
    //    {
    //        RollerCoaster train = Data.objects[i].GetComponent<RollerCoaster>();
    //        if (Data.objects[i].tag == "train")
    //        {
    //           if (GUI.Button(new Rect(0, (_menuBtnHeight + _spacing) * index, _menuBtnWidth, _menuBtnHeight), train.name)) { Data.activeObj = i;setCamera(); closeObjectMenu(); }
    //            index++;
    //        }
    //    }
    //    GUI.EndScrollView();
    //}
    void animalOptions(int offset)
    {
        float heightC = (_menuBtnHeight + _spacing * 2) * Data.objects.Count;
        _scrollPos = GUI.BeginScrollView(new Rect((_menuBtnWidth + _spacing), (_menuBtnHeight + _spacing) * offset + _spacing, _menuOptionWidth+20, heightS), _scrollPos, new Rect(0, 0, _menuOptionWidth, heightC));
        for (int i = 0, index = 0; i < Data.objects.Count; i++)
        {
            bool flag = false;
            MainObject obj = Data.objects[i].GetComponent<MainObject>();

            if (displayOPtion == obj.type || displayOPtion == ObjectType.DEFAULT)
            {
                if (GUI.Button(new Rect(0, (_menuBtnHeight + _spacing) * index, _menuOptionWidth, _menuBtnHeight), obj.nameO)) { Data.activeObj = i; setCamera(); menu = Menu.NONE;  }
                index++;
            }
        }
        GUI.EndScrollView();
        
    }
    //void wallOptions(int offset)
    //{
    //    float heightC = (_menuBtnHeight + _spacing * 2) * Data.walls.Count;
    //    _scrollPos = GUI.BeginScrollView(new Rect((_menuBtnWidth + _spacing), (_menuBtnHeight + _spacing) * offset + _spacing, widthS, heightS), _scrollPos, new Rect(0, 0, _menuBtnWidth, heightC));
    //    for (int i = 0, index = 0; i < Data.objects.Count; i++)
    //    {
    //        Wall wall = Data.objects[i].GetComponent<Wall>();
    //        if (Data.objects[i].tag == "wall")
    //        {
    //            if (GUI.Button(new Rect(0, (_menuBtnHeight + _spacing) * index, _menuBtnWidth, _menuBtnHeight), wall.name)) { Data.activeObj = i; setCamera(); closeObjectMenu(); }
    //            index++;
    //        }
    //    }
    //    GUI.EndScrollView();
    //}

    void musicOptions(int offset) {
        int startX = offset * (_menuBtnWidth + _spacing);
        float heightC = (_menuBtnHeight + _spacing * 2) * Data.data.backgroundThemes.Length;
        _scrollPos = GUI.BeginScrollView(new Rect(startX, _menuBtnHeight + _spacing, _menuOptionWidth+20, heightS),_scrollPos, new Rect(0, 0, _menuOptionWidth, heightC));
        if(GUI.Button(new Rect(0, 0, _menuOptionWidth, _menuBtnHeight), "none")){ Data.music = -1; closeMusicMenu(); }
        for (int i = 0; i < Data.data.backgroundThemes.Length; i++)
        {
            if(GUI.Button(new Rect(0, (_menuBtnHeight + _spacing)*(i+1), _menuOptionWidth, _menuBtnHeight), Data.data.backgroundThemes[i].name)) {
                if(Data.music == i) { Data.music = -1; }
                else Data.music = i;
                menu = Menu.NONE;
            }
        }
        
        GUI.EndScrollView();
    }

    //void appOptions() {
    //    GUI.BeginGroup(new Rect(0, _menuBtnHeight + _spacing, Screen.width, Screen.height));
    //    if (GUI.Button(new Rect(0, 0, _menuBtnWidth, _menuBtnHeight), "Core")) { Data.gameType = GameType.CORE; }
    //    //if (_showOptions) { _showWallOptions = false; _showAnimalOptions = false; rcOptions(0); }
    //    if (GUI.Button(new Rect(0, (_menuBtnHeight + _spacing), _menuBtnWidth, _menuBtnHeight), "Machine Learning")) { Data.gameType = GameType.MACHINE_LEARNING; }
    //    //if (_showWallOptions) { _showOptions = false; _showAnimalOptions = false; wallOptions(1); }
    //    GUI.EndGroup();
    //}

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

    //file menu functions//
    void saveGame() {
        FileManager.saveGame();
        menu = Menu.NONE;
    }
    void loadGame(int offset)
    {
     
        string[] gameFiles = FileManager.getAllGames();
        float heightC = (_menuBtnHeight + _spacing * 2) * gameFiles.Length;
        _scrollPos = GUI.BeginScrollView(new Rect((_menuBtnWidth+_spacing), (_menuBtnHeight + _spacing) * offset, _menuOptionWidth+20, heightS),_scrollPos, new Rect(0, 0, _menuOptionWidth, heightC));
        for(int i = 0; i < gameFiles.Length; i++)
        {
            //Debug.Log(gameFiles[i]);
            if (GUI.Button(new Rect(0, (_menuBtnHeight + _spacing) * i, _menuOptionWidth, _menuBtnHeight), gameFiles[i])) {
                FileManager.saveGame();
                Data.reset();
                Data.gameName = gameFiles[i];
                FileManager.loadGame();
                menu = Menu.NONE;
            }
        }
        GUI.EndScrollView();
    }
    string _gameName = "new game";
    void newGame(int offset)
    {
        GUI.BeginGroup(new Rect((_menuBtnWidth + _spacing), (_menuBtnHeight + _spacing) * offset, Screen.width, Screen.height));
        _gameName = GUI.TextField(new Rect(0, 0 , _menuOptionWidth, _menuBtnHeight), _gameName);
        if (GUI.Button(new Rect(0, (_menuBtnHeight + _spacing), _menuBtnWidth, _menuBtnHeight), "create"))
        {
            FileManager.saveGame();
            Data.reset();
            FileManager.createNewGame(_gameName);
            Data.gameName = _gameName;
            menu = Menu.NONE;
            gameObject.AddComponent<LandscapeMenu>();
        }
        GUI.EndGroup();
    }
    void saveAs(int offset)
    {
        GUI.BeginGroup(new Rect((_menuBtnWidth + _spacing), (_menuBtnHeight + _spacing)* offset, Screen.width, Screen.height));
        _gameName = GUI.TextField(new Rect(0, 0, _menuOptionWidth, _menuBtnHeight), _gameName);
        if (GUI.Button(new Rect(0, (_menuBtnHeight + _spacing) * 1, _menuBtnWidth, _menuBtnHeight), "save as"))
        {
            FileManager.saveGame();
            FileManager.createNewGame(_gameName);
            FileManager.copyFiles(Data.gameName, _gameName);
            Data.gameName = _gameName;
            menu = Menu.NONE;
        }
        GUI.EndGroup();
    }

    //SCENE MENU FUNCTIONS//
    string _sceneName = "new scene";
    void newScene(int offset)
    {
        GUI.BeginGroup(new Rect((_menuBtnWidth + _spacing), (_menuBtnHeight + _spacing) * offset, Screen.width, Screen.height));
        _sceneName = GUI.TextField(new Rect(0, 0, _menuOptionWidth, _menuBtnHeight), _sceneName);
        if (GUI.Button(new Rect(0, (_menuBtnHeight + _spacing), _menuBtnWidth, _menuBtnHeight), "create"))
        {
            FileManager.saveScene();
            Data.reset();
            FileManager.createNewScene(_sceneName);
            Data.sceneName = _sceneName;
            menu = Menu.NONE;
            gameObject.AddComponent<LandscapeMenu>();
        }
        GUI.EndGroup();
    }
    void loadScene(int offset)
    {
        string[] gameFiles = FileManager.getAllScenes();
        float heightC = (_menuBtnHeight + _spacing * 2) * gameFiles.Length;
        _scrollPos = GUI.BeginScrollView(new Rect((_menuBtnWidth + _spacing), (_menuBtnHeight + _spacing) * offset, _menuOptionWidth + 20, heightS), _scrollPos, new Rect(0, 0, _menuOptionWidth, heightC));
        for (int i = 0; i < gameFiles.Length; i++)
        {
            //Debug.Log(gameFiles[i]);
            if (GUI.Button(new Rect(0, (_menuBtnHeight + _spacing) * i, _menuOptionWidth, _menuBtnHeight), gameFiles[i]))
            {
                FileManager.saveScene();
                Data.reset();
                Data.sceneName = gameFiles[i];
                FileManager.loadScene();
                menu = Menu.NONE;
            }
        }
        GUI.EndScrollView();
    }
    void saveScene()
    {
        FileManager.saveScene();
        menu = Menu.NONE;
    }
    void saveAsScene(int offset)
    {
        GUI.BeginGroup(new Rect((_menuBtnWidth + _spacing), (_menuBtnHeight + _spacing) * offset, Screen.width, Screen.height));
        _sceneName = GUI.TextField(new Rect(0, 0, _menuOptionWidth, _menuBtnHeight), _sceneName);
        if (GUI.Button(new Rect(0, (_menuBtnHeight + _spacing), _menuBtnWidth, _menuBtnHeight), "create"))
        {
            FileManager.saveScene();
            FileManager.createNewScene(_sceneName);
            Data.sceneName = _sceneName;
            FileManager.saveScene();
            menu = Menu.NONE;
        }
        GUI.EndGroup();
    }
    void deleteScene(int offset) { }

    //menu closing functions//
    void closeMenus()
    {
        closeFileMenu();
        closeSceneMenu();
        closeObjectMenu();
        closeMusicMenu();
        Destroy(GetComponent<HelpMenu>());
    }
    void closeFileMenu()
    {
        _loadGame = false;
        _saveAs = false;
        _newGame = false;
        _showFileMenu = false;
    }
    void closeSceneMenu()
    {
        _loadGame = false;
        _saveAs = false;
        _newGame = false;
        _showFileMenu = false;
    }
    void closeObjectMenu()
    {
        _showOptions = false;
        _showObjectMenu = false;
    }
    void closeMusicMenu()
    {
        _showMusicMenu = false;
    }
    void OnApplicationQuit()
    {
        //FileManager.saveGame();
    }

    void setCamera() {
        GameObject obj = Data.objects[Data.activeObj];
        GameObject cam = GameObject.Find("Main Camera");
        cam.transform.position = obj.transform.position + new Vector3(0, 3, -4);
        cam.transform.LookAt(obj.transform);
    }
}
