//using UnityEngine;
//using System.Collections;
//using System.IO;
//using System;
//using System.Collections.Generic;

//public class PauseMenu : MonoBehaviour
//{
//    bool showmenu = false;
//    static int width = 200;
//    static int height = 200;
//    Rect pause_menu = new Rect((Screen.width - width) / 2, (Screen.height - height) / 2, width, height);
//    private bool savegame = false;
//    private bool loadgame = false;
//    private bool newgame = false;
//    private bool deletegame = false;
//    private string path;
//    private string filename;
//    private Vector2 scrollPos;
//    InputData data;
//    string current_game = "";
//    //------------------------editable variable--------------------------//
//    //all types of file extentions//
//    string[] extentions = { "_rc", "_wall", "_ani", "_log" };

//    void OnGUI()
//    {
//        if (this.showmenu)
//        {
//            GUI.Window(0, this.pause_menu, new GUI.WindowFunction(this.pauseMenu), "Pause Menu");
//        }
//        if (this.savegame)
//        {
//            GUI.Window(0, new Rect((float)((Screen.width - PauseMenu.width) / 2), (float)((Screen.height - PauseMenu.height) / 2), (float)PauseMenu.width, (float)PauseMenu.height), new GUI.WindowFunction(this.saveGame), "Game Save");
//        }
//        if (this.loadgame)
//        {
//            GUI.Window(0, new Rect((float)((Screen.width - PauseMenu.width) / 2), (float)((Screen.height - PauseMenu.height) / 2), (float)PauseMenu.width, (float)PauseMenu.height), new GUI.WindowFunction(this.loadGame), "Game Load");
//        }
//        if (this.newgame)
//        {
//            GUI.Window(0, new Rect((float)((Screen.width - PauseMenu.width) / 2), (float)((Screen.height - PauseMenu.height) / 2), (float)PauseMenu.width, (float)PauseMenu.height), new GUI.WindowFunction(this.newGame), "New Game");
//        }
//        if (this.deletegame)
//        {
//            GUI.Window(0, new Rect((float)((Screen.width - PauseMenu.width) / 2), (float)((Screen.height - PauseMenu.height) / 2), (float)PauseMenu.width, (float)PauseMenu.height), new GUI.WindowFunction(this.deleteGame), "Delete Game");
//        }
//        if (Input.GetKeyDown(KeyCode.Escape) && (this.savegame || this.loadgame || this.newgame || this.deletegame))
//        {
//            this.savegame = false;
//            this.loadgame = false;
//            this.newgame = false;
//            this.deletegame = false;
//            this.showmenu = true;
//        }
//    }

//    // Use this for initialization
//    void Start()
//    {
//        this.path = "Resources\\VirtualCodeCoaster";
//        Directory.CreateDirectory(this.path);
//        string[] files = Directory.GetFiles(this.path);
//        int num = files.Length;
//        while (File.Exists(string.Concat(new object[]
//        {
//            this.path,
//            "\\coaster",
//            num,
//            ".txt"
//        })))
//        {
//            num++;
//        }
//        this.filename = "coaster" + num + ".txt";
//        data = base.gameObject.GetComponent<InputData>();

//        if (File.Exists(path + "\\game_log.txt"))
//        {
//            string file = File.ReadAllText(path + "\\game_log.txt");
//            Debug.Log(file);
//            load(file);
//        }
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.P) || data.getGameState() == 0)
//        {
//            showmenu = !showmenu;
//            if (showmenu)
//            {
//                //Debug.Log("gameObject paused.");
//                Time.timeScale = 0;
//            }
//            else Time.timeScale = 1;
//        }
//    }

//    void OnApplicationQuit()
//    {

//        save();
//        File.WriteAllText(this.path + "//game_log.txt", current_game);

//        Debug.Log("exit");
//    }

//    void pauseMenu(int win_id)
//    {
//        if (GUILayout.Button("Resume"))
//        {
//            showmenu = false;
//            Time.timeScale = 1;
//            //Debug.Log("game resumed.");
//        }
//        if (GUILayout.Button("New Game", new GUILayoutOption[0]))
//        {
//            filename = "new game";
//            String _tmp = filename;
//            for (int i = 2; Directory.Exists(this.path + "\\" + filename); i++) filename = _tmp + " " + i;
//            save();
//            data.flag_list.Clear();
//            Debug.Log("new file creation");
//            showmenu = false;
//            newgame = true;
//        }
//        if (GUILayout.Button("Save", new GUILayoutOption[0]))
//        {
//            save();
//        }
//        if (GUILayout.Button("Save As", new GUILayoutOption[0]))
//        {
//            this.showmenu = false;
//            this.savegame = true;
//            //Debug.Log("Saving game...");
//        }
//        if (GUILayout.Button("Load", new GUILayoutOption[0]))
//        {
//            this.showmenu = false;
//            this.loadgame = true;
//            //Debug.Log("Loading game...");
//        }
//        if (GUILayout.Button("Delete", new GUILayoutOption[0]))
//        {
//            save();
//            this.showmenu = false;
//            this.deletegame = true;
//            //Debug.Log("Saving game...");
//        }
//        if (GUILayout.Button("Settings"))
//        {
//            //Debug.Log("opening settings.");
//        }

//        if (GUILayout.Button("Quit"))
//        {
//            Application.Quit();
//        }
//    }

//    private void saveGame(int win_id)
//    {
//        filename = GUILayout.TextField(filename);
//        GUILayout.Label(warning_label);
//        if (GUILayout.Button("create"))
//        {
//            if (createNewGame())
//            {
//                String sourcePath = this.path + "\\" + current_game;
//                String targetPath = this.path + "\\" + filename;

//                string[] files = System.IO.Directory.GetFiles(sourcePath);
//                // Copy the files and overwrite destination files if they already exist.
//                foreach (string s in files)
//                {
//                    // Use static Path methods to extract only the file name from the path.
//                    String _fileName = System.IO.Path.GetFileName(s);
//                    String _destFile = System.IO.Path.Combine(targetPath, _fileName);
//                    System.IO.File.Copy(s, _destFile, true);
//                }
//                load(filename);
//                savegame = false;
//                showmenu = true;
//            }
//        }
//        if (GUILayout.Button("back")) { savegame = false; showmenu = true; }
//    }

//    private void loadGame(int id)
//    {
//        this.displayFiles(false);
//        if (GUILayout.Button("back", new GUILayoutOption[0]))
//        {
//            this.loadgame = false;
//            this.showmenu = true;
//        }
//    }

//    String warning_label;
//    private void newGame(int id)
//    {
//        Debug.Log("new game");
//        filename = GUILayout.TextField(filename);
//        GUILayout.Label(warning_label);
//        if (GUILayout.Button("create"))
//        {
//            if (createNewGame())
//            {
//                load(filename);
//                newgame = false;
//                showmenu = true;
//            }
//        }
//        if (GUILayout.Button("back")) { newgame = false; showmenu = true; }
//    }

//    private void deleteGame(int id)
//    {
//        displayFiles(true);
//        if (GUILayout.Button("back")) { deletegame = false; showmenu = true; }
//    }

//    private void displayFiles(bool isSave)
//    {
//        string[] files = Directory.GetDirectories(path);
//        scrollPos = GUILayout.BeginScrollView(this.scrollPos, new GUILayoutOption[0]);
//        if (files.Length > 0)
//        {
//            for (int i = 0; i < files.Length; i++)
//            {

//                //display only game files (hides sub game files.)//
//                bool continue_flag = false;
//                for (int j = 0; j < extentions.Length; j++)
//                {
//                    if (files[i].Contains(extentions[j])) continue_flag = true;
//                }
//                if (continue_flag) continue;

//                string text = files[i].Replace(path + "\\", "");
//                //text = text.Replace(".txt", "");
//                //text = text.Replace("\\", "");
//                if (GUILayout.Button(text, new GUILayoutOption[0]))
//                {
//                    if (isSave)
//                    {
//                        deleteDir(text);
//                    }
//                    else
//                    {
//                        closeGame();
//                        load(text);
//                    }
//                    this.showmenu = true;
//                }
//            }
//        }
//        GUILayout.EndScrollView();
//    }

//    public bool createNewGame()
//    {
//        if (Directory.Exists(this.path + "\\" + filename))
//        {
//            warning_label = "Alert: Game File already exists.";
//            return false;
//        }
//        Directory.CreateDirectory(this.path + "\\" + filename);
//        File.WriteAllText(this.path + "\\" + filename + "\\_game.txt", "");
//        warning_label = "";
//        return true;
//    }

//    public void deleteDir(string dir)
//    {
//        if (!dir.Contains(current_game))
//        {
//            string[] files = Directory.GetFiles(this.path + "\\" + dir);
//            foreach (string file in files)
//            {
//                File.Delete(file);
//            }
//            Directory.Delete(this.path + "\\" + dir);
//        }
//    }

//    public void save()
//    {
//        if (current_game == null && current_game == "none" && current_game == "") return;
//        string save_string = "";
//        string path = this.path + "\\" + current_game + "\\_game.txt";
//        for (int j = 0; j < data.flag_list.Count; j++)
//        {
//            if (j != 0) save_string += "~";
//            save_string += data.flag_list[j].name;
//            save_string += "$" + data.flag_list[j].flag.transform.position.x;
//            save_string += "$" + data.flag_list[j].flag.transform.position.y;
//            save_string += "$" + data.flag_list[j].flag.transform.position.z;
//            save_string += "$" + data.flag_list[j].type;
//            save_string += "$" + data.flag_list[j].model;
//            //          save_string += "$" + data.flag_list[j].script;
//        }
//        File.WriteAllText(path, save_string);
//        this.savegame = false;
//    }

//    //public void load(string file, InputData id) { data = id; load(file); }
//    public void load(string file)
//    {
//        Debug.Log("loading file " + file);
//        string path = this.path + "\\" + file;
//        //Debug.Log(path);
//        string load_string = File.ReadAllText(path + "\\_game.txt");
//        current_game = file;
//        //clear the current data in game//
//        //incase the file has nothing to load,return//
//        if (load_string == "")
//        {
//            data.setCurrentFlag(-1);
//            return;
//        }

//        string[] flags = load_string.Split('~');
//        for (int j = 0; j < flags.Length; j++)
//        {
//            string[] tokens = flags[j].Split('$');
//            string name = tokens[0];
//            float x = float.Parse(tokens[1]);
//            float y = float.Parse(tokens[2]);
//            float z = float.Parse(tokens[3]);
//            int type = int.Parse(tokens[4]);
//            int model = int.Parse(tokens[5]);

//            GameObject flag = Instantiate(data.flag_prefab);
//            flag.transform.position = new Vector3(x, y, z);

//            FlagData fd = new FlagData(flag, name);
//            fd.type = type;
//            fd.model = model;
//            data.flag_list.Add(fd);
//            data.setCurrentFlag(j);
//            fd.script = File.ReadAllText(path + "\\" + fd.name); //path is local path.//
//            if (type == 1 || type == 2)
//            {
//                data.trackSetup(fd);
//                data.runCode2(type);
//            }
//            else if (type == 3) { data.animalSetup(); }
//        }
//        //base.gameObject.GetComponent<InputData>().game_files;
//        this.loadgame = false;
//        current_game = file;
//        Debug.Log("load successfull.");
//        //Debug.Log(current_game);
//    }

//    public string getCurrentGame() { return current_game; }
//    public void saveFile(FlagData fd)
//    {
//        if (!fd.name.Contains(".txt")) fd.name += ".txt";
//        string path = this.path + "\\" + current_game + "\\" + fd.name;
//        File.WriteAllText(path, fd.script);
//    }
//    public void deleteFile(FlagData fd)
//    {
//        string path = this.path + "\\" + current_game + "\\" + fd.name;
//        File.Delete(path);

//    }
//    public string readFile(FlagData fd)
//    {
//        string path = this.path + "\\" + current_game + "\\" + fd.name;
//        return File.ReadAllText(path);

//    }

//    public void closeGame()
//    {
//        List<FlagData> flags = data.flag_list;
//        int size = flags.Count;
//        for (int i = 0; i < size; i++)
//        {
//            FlagData flag = flags[i];

//            if (flag.type == 3)
//            {
//                Destroy(flag.animal);
//            }
//            if (flag.type == 1 || flag.type == 2)
//            {
//                flag.gt.reset();
//                Destroy(flag.trigger);
//            }
//            Destroy(flag.flag);
//        }
//        flags.Clear();
//        Debug.Log("closed game.");
//    }
//}