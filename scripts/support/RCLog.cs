using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RCLog  {
    static string dirPath = "Resources\\VirtualCodeCoaster\\log.txt";
    static string defaultFilename = "log.txt";
    static string filename = "log.txt";
    static string path = dirPath + defaultFilename;

    public static void append(string msg)
    {
        msg = msg + "\r\n";
        System.IO.File.AppendAllText(path, msg);
    }
    public static void putTimeStamp() {

    }
    public static void clearAll() {
        System.IO.File.WriteAllText(path, "");
    }
    public static void useFile(string name)
    {
        filename = name;
        path = dirPath + filename;
    }
}
