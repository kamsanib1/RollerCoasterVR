using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
0-99 : single point triggers.       //activation triggers.
100-999 : start and end triggers.   //animation trigger
1000-9999 : inactive triggers.      //inactive triggers
*/

public struct trigger
{
    public int type;
    public string param;
    public int start;
    public int end;
    public AudioClip voice;

    public trigger(int s, int t) { type = t; start = end = s; param = ""; voice = null; }
    public trigger(int s, int t, string p) { type = t; start =  end = s; param = p; voice = null; }
    public trigger(int s, int t, AudioClip v) { type = t; start = end = s; param = ""; voice = v; }
    public trigger(int s, int t, string p, int e) { type = t; start = s; end = e; param = p; voice = null; }
    public trigger(int s, int t, string p, int e, AudioClip v) { type = t; start = s; end = e; param = p; voice = v; }
}

public struct trigger_mapping
{
    public string name;
    public int id;

    public trigger_mapping(string n, int i) { name = n; id = i; }
}

public static class TriggerLibrary {
    private static bool initFlag = false;

    private static List<trigger_mapping> trigger_library = new List<trigger_mapping>();
    
    //-----------------constructors---------------------------------//
    public static void init() {
        if (!initFlag)
        {         //all trigger functions.
            addTrigger("brake", 1);
            addTrigger("boost", 2);

            //object triggers
            addTrigger("goblin", 100);
            addTrigger("elf", 101);
            addTrigger("bear", 102);
            addTrigger("pumpkin", 103);
            addTrigger("deer", 104);
            addTrigger("wolf", 105);
            addTrigger("zombie", 106);
            addTrigger("nudewitch", 107);
            addTrigger("spider", 108);
            addTrigger("trex", 109);

            //sound triggers
            addTrigger("audio_goblin", 1000);
            addTrigger("audio_elf", 1001);
            addTrigger("audio_bear", 1002);
            addTrigger("audio_pumpkin", 1003);
            addTrigger("audio_deer", 1004);
            addTrigger("audio_wolf", 1005);
            addTrigger("audio_zombie", 1006);
            addTrigger("audio_nudewitch", 1007);
            addTrigger("audio_spider", 1008);
            addTrigger("audio_trex", 1009);

            initFlag = true;
        }
    }
    
    static void addTrigger(string name,int id) {
        trigger_library.Add(new trigger_mapping(name, id));
    }

    public static int Exists(string name) {
        int index;
        if ((index = trigger_library.FindIndex(x => x.name == name)) >= 0) return trigger_library[index].id;
        return -1;
    }

    public static bool isAnimationTrigger(int type)
    {
        if (type > 99 && type < 1000) return true;
        return false;
    }
}
