using UnityEngine;
using System.Collections;

public static class FileManager  {
    private const string path = "Resources\\VirtualCodeCoaster";
    
    public static void initGameData()
    {
        if (!System.IO.Directory.Exists(path)) { System.IO.Directory.CreateDirectory(path); }
        if(!System.IO.File.Exists(path + "\\game")) {
            System.IO.Directory.CreateDirectory(path + "\\default\\scenes");

            string filePath = path + "\\default\\" + "_game";
            //scene
            System.IO.File.WriteAllText(path + "\\default\\scenes\\default", "");
            //game
            System.IO.File.WriteAllText(filePath, "default");
            //current game
            System.IO.File.WriteAllText(path + "\\game", "default");
        }
        string gameName = System.IO.File.ReadAllText(path+"\\game");
        if (gameName != null || gameName != "") { Data.gameName = gameName; }
        else { Data.gameName = "default"; }
        loadGame();
    }

    public static void saveGame(){
        saveScene();
    }
    public static void loadGame() {
        string filename = Data.gameName;
        string dir = path + "\\" + filename;
        string load_string = "";
        string filepath = dir+"\\_game";

        //Debug.Log("loading file " + filepath);
        load_string = System.IO.File.ReadAllText(filepath);

        //clear the current data in game//
        //incase the file has nothing to load,return//
        if (load_string == "")
        {
            return;
        }
        Data.sceneName = load_string;
        loadScene();
    }
    public static string[] getAllGames() {
        string[] gamefiles;
        gamefiles = System.IO.Directory.GetDirectories(path+"\\");
        for(int i = 0; i < gamefiles.Length; i++)
        {
            gamefiles[i] = gamefiles[i].Replace(path + "\\", "");
        }
        return gamefiles;
    }

    public static void updateGameFile()
    {
        System.IO.File.WriteAllText(path + "//game", Data.gameName);
    }
    public static void createNewGame(string gameName)
    {
        string[] gameFiles = getAllGames();
        int addon = 1;
        string suffix = "";

        for(int i = 0; i < gameFiles.Length; i++)
        {
            if(gameFiles[i] == gameName + suffix)
            {
                addon++;
                suffix = "_" + addon;
                i = 0;
            }
        }

        System.IO.Directory.CreateDirectory(path +"\\"+ gameName + suffix);
        System.IO.Directory.CreateDirectory(path + "\\" + gameName + suffix + "\\" + "scenes");
        string filepath = path + "\\" + gameName + suffix + "\\_game";
        string scenePath = path + "\\" + gameName + suffix + "\\scenes\\default";
        System.IO.File.WriteAllText(filepath, "default");
        System.IO.File.WriteAllText(scenePath, "");

        Data.sceneName = "default";
    }
    
    //scene related functions//
    public static void createNewScene(string sceneName) {
        string[] gameFiles = getAllGames();
        int addon = 1;
        string suffix = "";

        for (int i = 0; i < gameFiles.Length; i++)
        {
            if (gameFiles[i] == sceneName + suffix)
            {
                addon++;
                suffix = "_" + addon;
                i = 0;
            }
        }

        string filepath = path + "\\" + Data.gameName + suffix + "\\_game";
        string scenePath = path + "\\" + Data.gameName + "\\scenes\\"+ sceneName + suffix;
        System.IO.File.WriteAllText(filepath, sceneName);
        System.IO.File.WriteAllText(scenePath, "");

        Data.sceneName = sceneName;
    }
    public static void saveScene()
    {
        string filename = Data.gameName;
        string dir = path + "\\" + filename;
        string save_string = "";
        string filepath;

        if (Data.landscape == null) { save_string = "#$0"; }
        else save_string += "#$" + Data.landscape.GetComponent<PlayerPosition>().index;

        for (int i = 0; i < Data.objects.Count; i++)
        {
            string objectType = "animal";
            MainObject obj = Data.objects[i].GetComponent<MainObject>();
            objectType = obj.tag;

            filepath = dir + "\\" + obj.scriptRef + ".txt";
            System.IO.File.WriteAllText(filepath, obj.script);

            save_string += "\r\n";
            save_string += obj.nameO;
            save_string += "$" + obj.scriptRef;
            save_string += "$" + objectType;
            save_string += "$" + obj.model;
            save_string += "$" + obj.position.x;
            save_string += "$" + obj.position.y;
            save_string += "$" + obj.position.z;
            save_string += "$" + obj.rotation.x;
            save_string += "$" + obj.rotation.y;
            save_string += "$" + obj.rotation.z;
            save_string += "$" + obj.rotation.w;
            save_string += "$" + obj.scale.x;
            save_string += "$" + obj.scale.y;
            save_string += "$" + obj.scale.z;
        }
        filepath = dir + "\\" + "scenes\\"+Data.sceneName;
        System.IO.File.WriteAllText(filepath, save_string);
        updateGameFile();
    }
    public static void loadScene()
    {
        string filename = Data.gameName;
        string dir = path + "\\" + filename;
        string load_string = "";
        string filepath;
        int landscape_index = 0;

        filepath = dir + "\\scenes\\"+Data.sceneName;
        Debug.Log("loading file " + filepath);
        load_string = System.IO.File.ReadAllText(filepath);

        //clear the current data in game//
        //incase the file has nothing to load,return//
        if (load_string == "")
        {
            return;
        }

        ObjectInterface objInterface = GameObject.Find("GUI").GetComponent<ObjectInterface>();

        load_string = load_string.Replace("\r", "");
        string[] flags = load_string.Split('\n');
        if (flags.Length == 0) return;
        for (int j = 0; j < flags.Length; j++)
        {
            string[] tokens = flags[j].Split('$');
            if (tokens[0] == "#")
            {
                if (tokens[1] == "0") continue;
                landscape_index = Data.getFileIndex(tokens[1]);
                objInterface.placeLandscape(landscape_index);
                continue;
            }
            //retrieve the data and store in respective formats.//
            string name = tokens[0];
            string scriptRef = tokens[1];
            string objectType = tokens[2];
            string model_name = tokens[3];
            float posX = float.Parse(tokens[4]);
            float posY = float.Parse(tokens[5]);
            float posZ = float.Parse(tokens[6]);
            float rotX = float.Parse(tokens[7]);
            float rotY = float.Parse(tokens[8]);
            float rotZ = float.Parse(tokens[9]);
            float rotW = float.Parse(tokens[10]);
            float sclx = float.Parse(tokens[11]);
            float scly = float.Parse(tokens[12]);
            float sclz = float.Parse(tokens[13]);
            int model = Data.getFileIndex(model_name);

            //creating transform data from retrieved data.//
            Vector3 position = new Vector3(posX, posY, posZ);
            Quaternion rotation = new Quaternion(rotX, rotY, rotZ, rotW);
            Vector3 scale = new Vector3(sclx, scly, sclz);

            //pulling the script data from respective script files.//
            string script = System.IO.File.ReadAllText(dir + "\\" + scriptRef + ".txt");
            
            //get objecttype and place the animal using object interface script.//
            ObjectType type = getObjectType(objectType);
            objInterface.placeObject(model, position, type);

            //setup rest of the data to initialize the object.//
            MainObject mainObject = Data.objects[Data.objects.Count - 1].GetComponent<MainObject>();
            mainObject.script = script;
            mainObject.scriptRef = scriptRef;
            mainObject.nameO = name;
            mainObject.position = position;
            mainObject.rotation = rotation;
            mainObject.scale = scale;

            //set gameobjects parameters.//
            Data.objects[Data.objects.Count - 1].transform.position = position;
            Data.objects[Data.objects.Count - 1].transform.rotation = rotation;
            Data.objects[Data.objects.Count - 1].transform.localScale = scale;
        }
        Data.setAnimalsOnGround();
        Data.activeObj = Data.objects.Count - 1;

        //updates the scene in game file
        string gameFilePath = dir + "\\_game";
        System.IO.File.WriteAllText(gameFilePath, Data.sceneName);
    }
    public static string[] getAllScenes() {
        string[] gamefiles;
        string filesPath = path + "\\" + Data.gameName + "\\scenes\\";
        gamefiles = System.IO.Directory.GetFiles(filesPath);
        for (int i = 0; i < gamefiles.Length; i++)
        {
            gamefiles[i] = gamefiles[i].Replace(filesPath, "");
        }
        return gamefiles;
    }
    static ObjectType getObjectType(string objectType)
    {
        ObjectType type = ObjectType.ANIMAL;
        if (objectType == "animal") type = ObjectType.ANIMAL;
        else if (objectType == "train") type = ObjectType.TRAIN;
        else if (objectType == "wall") type = ObjectType.WALL;
        else if (objectType == "graph") type = ObjectType.GRAPH;
        else if (objectType == "Respawn") type = ObjectType.RESPAWN;
        else if (objectType == "generic") type = ObjectType.PLANT;
        else if (objectType == "vehicle") type = ObjectType.DRIVING;
        else if (objectType == "bird") type = ObjectType.BIRD;
        else if (objectType == "reflector") type = ObjectType.REFLECTOR;

        return type;
    }

    //********file operation functions*********//
    //copies files from a source directory to destination directory including directories.//
    //works only for this game file saving format.//
    public static void copyFiles(string src, string dest) {
        string[] files = System.IO.Directory.GetFiles(path + "\\" + src);
        foreach(string s in files)
        {
            string filename = System.IO.Path.GetFileName(s);
            string destPath = System.IO.Path.Combine(path + "\\" + dest,filename);
            System.IO.File.Copy(s, destPath,true);
        }

        //copies only files in scenes directory.//
        files = System.IO.Directory.GetFiles(path + "\\" + src+"\\scenes");
        foreach (string s in files)
        {
            string filename = System.IO.Path.GetFileName(s);
            string destPath = System.IO.Path.Combine(path + "\\" + dest+"\\scenes", filename);
            System.IO.File.Copy(s, destPath, true);
        }
    }
    public static string getFilename(string name)
    {
        string suffix = "";
        string extension = ".txt";
        int index = 1;
        string dir = path + "\\" + Data.gameName+"\\";

        name = name.Replace(".txt", "");
        Debug.Log(dir + name + suffix + "");
        while (System.IO.File.Exists(dir + name + suffix+extension)) {
            suffix ="" + index;
            index++;
        }
        name += suffix;
        //Debug.Log(name);
        return name;
    }
    public static void createNewFile(string filename) { createNewFile(filename,""); }
    public static void createNewFile(string filename, string content) {
        //if (!filename.Contains("")) { filename += ""; }
        string dir = path + "\\" + Data.gameName + "\\";

        System.IO.File.WriteAllText(dir + filename+".txt", content);
    }
    public static void deleteFile(string filename)
    {
        if (!filename.Contains(".txt")) { filename += ".txt"; }
        string dir = path + "\\" + Data.gameName + "\\";
        Debug.Log("deleting file:" + dir + filename);
        System.IO.File.Delete(dir + filename);
    }
    public static string updateFileName(string oldName,string newName) {
        string dir = path + "\\" + Data.gameName + "\\";

        string name = oldName.Replace(".txt", "");
        string name2 = newName.Replace(".txt", "");
        if (name == name2) return oldName;

        newName = getFilename(newName);
        string content = System.IO.File.ReadAllText(dir + oldName+".txt");
        Debug.Log("new name:"+newName+"::oldname:"+oldName);

        createNewFile(newName, content);
        deleteFile(oldName);
        return newName;
    }
    public static string loadFile(string path)
    {
        string content;
        content = System.IO.File.ReadAllText(path);
        return content;
    }
}
