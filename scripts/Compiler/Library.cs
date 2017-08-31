using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


static class Library
{
    private static string[,] _mathFuncs = new string[,] {
        {"rand","int:int","float" },
        {"sin","float","float" },
        {"cos","float","float" },
        {"tan","float","float" },
        {"inversetan","float","float" },
        {"inversesin","float","float" },
        {"inversecos","float","float" },
        {"exp","float","float" },
        {"abs","float","float" },
        {"sigmf","float","float" },
        {"sum","float","float" },
        {"round","float","float" },
    };

    private static string[,] _stdFuncs = new string[,] {
        {"random","var:float:float" },
        {"display","var" },
        {"print","string2" },
        {"load","string2:array2df" },
        {"collision","var"},
        {"loadscene","string2" },
        {"placeobject","string2:string2:float:float:float" },
        {"playsound","string2" },
        {"gotshot","var" },
        {"destroy","string2"},
        {"getglobal","string2:var" },
        {"setglobal","string2:var" },
        {"debug","var" }
    };
    private static string[,] _trackFuncs = new string[,]{
        { "track","int:direction:int:int:int" },
        { "brake","int" },
        { "boost","int" },
        {"tunnel","string" },
        //prop functions//
        { "goblin","int" },
        { "elf","int" },
        { "bear","int" },
        { "pumpkin","int" },
        { "deer","int" },
        { "wolf","int" },
        { "zombie","int" },
        { "nudewitch","int" },
        { "spider","int" },
        { "trex","int" },
        //audio functions//
        { "audio_goblin",   "int"},
        {"audio_elf",       "int"},
        {"audio_bear",      "int"},
        {"audio_pumpkin",   "int"},
        {"audio_deer",      "int"},
        {"audio_wolf",      "int"},
        {"audio_zombie",    "int"},
        {"audio_nudewitch", "int"},
        {"audio_spider",    "int"},
        {"audio_trex",      "int"}
   };
    private static string[,] _wallFuncs = new string[,] {
        {"wall","int:direction:int:int" }
    };
    private static string[,] _animalFuncs = new string[,] {
        {"walk","float:direction:float" },
        {"run","float:direction:float" },
        {"move","float:direction:float" },
        {"eat","int" },
        {"idle","int" },
        {"attack","int" },
        {"sleep","int" },
        {"roar","int" },
        {"follow","" },
        {"setspeed","float" },
        //transform data functions//
        {"getposition","string2:var:var:var" },
        {"getangle","string2:var" },
        {"setposition","string2:float:float:float" },
        {"anglebtw2obj","string2:var"},
        //bird functions//
        {"fly","float:direction:float" },
        {"glide","float:direction:float" },
        {"land","" },
        {"takeoff","" },
        //boy animation//
        {"b_ahoy"         ,"int"},
        {"b_aid"          ,"int"},
        {"b_defend"       ,"int"},
        {"b_crouch1"      ,"int"},
        {"b_crouch2"      ,"int"},
        {"b_sitt_g_start" ,"int"},
        {"b_sitt_g_loop"  ,"int"},
        {"b_sitt_g_end"   ,"int"},
        {"b_sitt_s_start" ,"int"},
        {"b_sitt_s_loop"  ,"int"},
        {"b_sitt_s_end"   ,"int"},
        {"b_spider_walk"  ,"int"},
        {"b_sidewalk_right" ,"int" },
        {"b_sidewalk_left"  ,"int"},
        {"b_talk2"        ,"int"},
        {"b_zombie_run"   ,"int"},
        {"b_dance_happy"  ,"int"},
        {"b_rock_dance"   ,"int"},
        {"b_claps"        ,"int"},
        {"b_leg_movement" ,"int"},
        {"b_attack_hand2" ,"int"},
        {"b_kick"         ,"int"},

        /*princess animation*/
        {"p_step_right","int" },
        {"p_step_left","int" },
        {"p_turn_right","int" },
        {"p_turn_left","int" },
        {"p_swim","int" },
        {"p_dive","int" },
        {"p_twirl","int" },
        {"p_dance_01","int" },
        {"p_dance_02","int" },
        {"p_dance_03","int" },
        //{"p_climb_up","int" },
        //{"p_climb_down","int" },
        //{"p_sit_down","int" },
        //{"p_sit_idle","int" },
        //{"p_sit_end","int" },
        {"p_pick_from_tree","int" },
        {"p_pick_from_table","int" },
        {"p_pick_from_floor","int" },
        {"p_throw","int" },
        {"p_hugging","int" },
        {"p_waving","int" },
        {"p_joking","int" },

        //robo animations//
        {"r_bake","int"},
        {"r_carrying","int"},
        {"r_climb_ladder","int"},
        {"r_cackle","int" },
        {"r_climp_stairs_up","int" },
        {"r_climb_stairs_down" ,"int"},
        {"r_climb_ladder_down" ,"int"},
        {"r_eat","int"},
        {"r_guitar","int"},
        {"r_push","int"},
        {"r_injection","int"},
        {"r_take","int"},
        {"r_build","int"},
        {"r_cooking","int"},
        {"r_aggressive","int"},
        {"r_cheer","int"},
        {"r_deactivation","int"},
        {"r_dancing1","int"},
        {"r_getup","int"},
        {"r_joy1","int"},
        {"r_evil","int"},
        {"r_joking","int"},
        {"r_mimic","int"},
        {"r_action1","int"},
        {"r_clearing","int"},
        {"r_applaud","int"},
        {"r_fall","int"},
        {"r_duck","int"},
        {"r_drilling","int"},
        {"r_empty","int"},
        {"r_pick","int"},
        {"r_talk","int"},
        {"r_hitfall","int"},
        {"r_attack_kick","int"},
        {"r_attack_punch","int"},
        {"r_singing","int"},
        {"r_seperating","int"},
        {"r_eating","int"},
        {"r_rolling","int"},
        {"r_trip","int"},
        {"r_gym","int"},
        {"r_dodge_left","int"},
        {"r_drinking","int"},
        {"r_drop_object","int"},
        { "r_evil2","int"},
        {"r_painting","int"},
        {"r_hit1","int"},
        {"r_jump","int"},
        //snowman functions
        {"s_idle","int"},
        {"s_walk","int"},
        {"s_run","int"},
        {"s_death","int"},
        {"s_walk_back","int"},
        {"s_eat","int"},
        {"s_jumpy","int"},
        {"s_hit_01","int"},
        {"s_runjump","int"},
        {"s_emotion_01","int"},
        {"s_emotion_02","int"},
        {"s_emotion_03","int"},
        {"s_emotion_04","int"},
        {"s_emotion_05","int"},
        {"s_listen","int"},
        {"s_punch","int"},
        {"s_scared","int"},
        {"s_stendup","int"},
        {"s_dance_1A","int"},
        {"s_dance_1J","int"},
        {"s_dance_2A","int"},

        //Zombie Functions
        {"z_idel","int"},
        {"z_walk","int"},
        {"z_run","int"},
        {"z_dead","int"},
        {"z_attack","int"},
        {"z_get_hit","int"},
        {"z_attack2","int"},
        {"z_attack3","int"},
        {"z_shout","int"}
    };

    private static string[,] _sensingLib = new string[,]
    {
        {"settype","int" },
        {"gettype","string2:var" },
        //raycast functions
        {"raycast","float:float:var" },
        {"collidercoords","var:var:var" },
        {"collidertype","var" },
        {"colliderangle","var" },
        {"releasecollider","" },
        {"setsound","string2" }
    };

    private static string[,] _movementLib = new string[,]
    {
        {"move2","float:direction:float:float" },
        {"movearch","float:direction:float:float" },
        {"rotate","float:float" },
        {"destroy","" },
        {"movement","string2" }
    };

    private static string[,] _graphFuncs = new string[,] {
           {"place","string:float:float:float" },
           {"drawline","float:float:float:float:float" },
           {"setposition","float:float:float" },
           {"wall","int:direction:int:int" }

    };
    private static string _args;
    private static string _returnType;

    public static bool isFunc(string __func)
    {
        if (isStdFunc(__func)) return true;
        for (int i = 0; i < _trackFuncs.Length / 2; i++)
        {
            if (_trackFuncs[i, 0] == __func)
            {
                _args = _trackFuncs[i, 1];
                return true;
            }
        }
        for (int i = 0; i < _wallFuncs.Length / 2; i++)
        {
            if (_wallFuncs[i, 0] == __func)
            {
                _args = _wallFuncs[i, 1];
                return true;
            }
        }
        for (int i = 0; i < _animalFuncs.Length / 2; i++)
        {
            if (_animalFuncs[i, 0] == __func)
            {
                _args = _animalFuncs[i, 1];
                return true;
            }
        }
        for (int i = 0; i < _graphFuncs.Length / 2; i++)
        {
            if (_graphFuncs[i, 0] == __func)
            {
                _args = _graphFuncs[i, 1];
                return true;
            }
        }
        for (int i = 0; i < _sensingLib.Length / 2; i++)
        {
            if (_sensingLib[i, 0] == __func)
            {
                _args = _sensingLib[i, 1];
                return true;
            }
        }
        for (int i = 0; i < _movementLib.Length / 2; i++)
        {
            if (_movementLib[i, 0] == __func)
            {
                _args = _movementLib[i, 1];
                return true;
            }
        }
        return false;
    }

    public static bool isStdFunc(string __func)
    {
        for (int i = 0; i < _stdFuncs.Length / 2; i++)
        {
            if (_stdFuncs[i, 0] == __func)
            {
                _args = _stdFuncs[i, 1];
                return true;
            }
        }

        return false;
    }
    public static bool isMathFunc(string __func)
    {
        for (int i = 0; i < _mathFuncs.Length / 3; i++)
        {
            if (_mathFuncs[i, 0] == __func)
            {
                _args = _mathFuncs[i, 1];
                _returnType = _mathFuncs[i, 2];
                return true;
            }
        }

        return false;
    }
    public static bool isSensingLibFunc(string __func)
    {
        for (int i = 0; i < _sensingLib.Length / 3; i++)
        {
            if (_sensingLib[i, 0] == __func)
            {
                _args = _sensingLib[i, 1];
                //_returnType = _mathFuncs[i, 2];
                return true;
            }
        }

        return false;
    }
    public static bool isMovementLibFunc(string __func)
    {
        for (int i = 0; i < _movementLib.Length / 3; i++)
        {
            if (_movementLib[i, 0] == __func)
            {
                _args = _movementLib[i, 1];
                //_returnType = _mathFuncs[i, 2];
                return true;
            }
        }

        return false;
    }

    public static string getArgs()
    {
        return _args;
    }
    public static string getArgs(string func)
    {
        string args = null;
        bool flag = false;
        for(int i = 0; i < _animalFuncs.Length/2; i++)
        {
            if(_animalFuncs[i,0] == func) {
                args = _animalFuncs[i, 1];flag = true; break; }
        }
        for (int i = 0; !flag && i < _graphFuncs.Length/2; i++)
        {
            if (_graphFuncs[i, 0] == func) { args = _graphFuncs[i, 1]; flag = true; break; }
        }
        for (int i = 0; !flag && i < _wallFuncs.Length/2; i++)
        {
            if (_wallFuncs[i, 0] == func) { args = _wallFuncs[i, 1]; flag = true; break; }
        }
        for (int i = 0; !flag && i < _trackFuncs.Length/2; i++)
        {
            if (_trackFuncs[i, 0] == func) { args = _trackFuncs[i, 1]; flag = true; break; }
        }
        for (int i = 0; !flag && i < _sensingLib.Length/2; i++)
        {
            if (_sensingLib[i, 0] == func) { args = _sensingLib[i, 1]; flag = true; break; }
        }
        for (int i = 0; !flag && i < _movementLib.Length/2; i++)
        {
            if (_movementLib[i, 0] == func) { args = _movementLib[i, 1]; flag = true; break; }
        }
        return args;
    }

    public static string getReturnType()
    {
        return _returnType;
    }

    public static double runFunction(string __func, double __val)
    {
        double ret = 0;
        //Console.Write("debug:val({0})", __val);
        if (__func == "sin")
        {
            __val = Math.PI * __val / 180;
            ret = Math.Sin(__val);
        }
        else if (__func == "cos")
        {
            __val = Math.PI * __val / 180;
            ret = Math.Cos(__val);
        }
        else if (__func == "cos")
        {
            __val = Math.PI * __val / 180;
            ret = Math.Tan(__val);
        }
        else if (__func == "inversetan")
        {
            ret = Math.Atan(__val);
            ret = 180 * ret / Math.PI;
            if (ret < 0) ret = 180 + ret;
        }
        else if (__func == "exp") { ret = Math.Exp(__val); }
        else if (__func == "abs") { ret = Math.Abs(__val); }
        else if (__func == "sigmf")
        {
            ret = 1 / (1 + Math.Exp(-(__val)));
        }
        else if (__func == "round")
        {
            UnityEngine.Debug.Log(__val);
            ret = Math.Round(__val);
        }
        //Console.WriteLine("debug:{0}({1}):{2}", __func, __val, ret);
        return ret;
    }
}
