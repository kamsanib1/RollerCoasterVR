using UnityEngine;
using System.Collections;
using System;

public class AnimalStats:MonoBehaviour  {

    private string[] animal_names = { "monster","giraffe","bear","elephant","gassele","jaguar","rooster","Lion" };
    public int type;
    
    public float health;
    public float max_health;
    public float health_regen;
    public float stamina;
    public float run_speed;
    public float walk_speed;
    public float turn_speed;
    public float jump;
    public float apetite;
    public float thirst;

    private float regen_time = 3;
    private float time = 0;

    void Update()
    {
        time += Time.deltaTime;
        if (time >= regen_time)
        {
            time = 0;
            regenHealth();
        }
    }

    public void setType(int t) { type = t; loadStats(); }
    public void setType(string model)
    {
        string fcontent = (Resources.Load("animalStats") as TextAsset).text;
        fcontent = fcontent.Replace("\r", "");
        string[] animals = fcontent.Split('\n');
        for(int i = 0; i < animals.Length; i++)
        {
            string[] stats = animals[i].Split(':');
            if(stats[0] == model)
            {
                health       = float.Parse(stats[1]);
                max_health   = float.Parse(stats[2]);
                health_regen = float.Parse(stats[3]);
                stamina      = float.Parse(stats[4]);
                run_speed    = float.Parse(stats[5]);
                walk_speed   = float.Parse(stats[6]);
                turn_speed   = float.Parse(stats[7]);
                jump         = float.Parse(stats[8]);
                apetite      = float.Parse(stats[9]);
                thirst       = float.Parse(stats[10]);
            }
        }
    }
    public bool setAnimal(string name) {
        bool flag = true;
        for (int i = 1; i < animal_names.Length; i++)
        {
            if (animal_names[i] == name)
            {
                flag = false;
                setType(i);
                break;
            }
        }
        if (flag) return false;
        return true;
    }
    public void regenHealth()
    {
        health += health_regen;
        if (health > max_health) health = max_health;
    }
    private void loadStats()
    {
        //Debug.Log("Stats type:" + type);
        switch (type) {
            //bear//
            case 0:
                setHealth(100, 3);
                setSpeed(7, 1.8f, 60, 30, 200);
                break;
            //elephant//
            case 1:
                setHealth(100, 3);
                setSpeed(5, 1, 60, 30, 200); 
                break;
            //gasselle//
            case 2:
                setHealth(100, 3);
                setSpeed(15, 2f, 60, 30, 200);
                break;
            //girraffe//
            case 3:
                setHealth(100, 3);
                setSpeed(8, 2.2f, 60, 30, 200);
                break;
            //jaguar//
            case 4:
                setHealth(100, 3);
                setSpeed(20, 2.3f, 60, 30, 200);
                break;
            //lion//
            case 5:
                setHealth(100, 3);
                setSpeed(15.5f, 2.9f, 60, 30, 200);
                break;
            //rooster//
            case 6:
                setHealth(100, 3);
                setSpeed(1.5f, .5f, 60, 30, 200);
                break;
            //monsters 7-16//
            case 7:
                setHealth(100, 3);
                setSpeed(15.5f, 2.9f, 60, 30, 200);
                break;
            case 8:
                setHealth(100, 3);
                setSpeed(15.5f, 2.9f, 60, 30, 200);
                break;
            case 9:
                setHealth(100, 3);
                setSpeed(15.5f, 2.9f, 60, 30, 200);
                break;
            case 10:
                setHealth(100, 3);
                setSpeed(15.5f, 2.9f, 60, 30, 200);
                break;
            case 11:
                setHealth(100, 3);
                setSpeed(15.5f, 2.9f, 60, 30, 200);
                break;
            case 12:
                setHealth(100, 3);
                setSpeed(15.5f, 2.9f, 60, 30, 200);
                break;
            case 13:
                setHealth(100, 3);
                setSpeed(15.5f, 2.9f, 60, 30, 200);
                break;
            case 14:
                setHealth(100, 3);
                setSpeed(15.5f, 2.9f, 60, 30, 200);
                break;
            case 15:
                setHealth(100, 3);
                setSpeed(15.5f, 2.9f, 60, 30, 200);
                break;
            case 16:
                setHealth(100, 3);
                setSpeed(15.5f, 2.9f, 60, 30, 200);
                break;
            //case 17:
            //    setHealth(100, 3);
            //    setSpeed(15.5f, 2.9f, 60, 30, 200);
            //    break;
            //case 18:
            //    setHealth(100, 3);
            //    setSpeed(15.5f, 2.9f, 60, 30, 200);
            //    break;
            //case 19:
            //    setHealth(100, 3);
            //    setSpeed(15.5f, 2.9f, 60, 30, 200);
            //    break;
            //case 20:
            //    setHealth(100, 3);
            //    setSpeed(15.5f, 2.9f, 60, 30, 200);
            //    break;
        }
        //helicopter//
        if(type == 17) {
            setHealth(100, 3);
            setSpeed(13f, 7f, 80, 30, 200);
        }
        //boy//
        if(type>=18 && type <= 27) {
            setHealth(100, 3);
            setSpeed(10f, 1f, 80, 30, 200);
        }
        //princess//
        else if (type >= 28 && type <= 35)
        {
            setHealth(100, 3);
            setSpeed(7f, 2f, 80, 30, 200);
        }
    }

    private void setHealth(int health,int regen) {
        this.health = health;
        this.max_health = health;
        this.health_regen = regen;
    }

    private void setSpeed(float run,float walk,float turn,float jump,float stamina)
    {
        run_speed = run;
        walk_speed = walk;
        turn_speed = turn;
        this.jump = jump;
        this.stamina = stamina;
    }
}

