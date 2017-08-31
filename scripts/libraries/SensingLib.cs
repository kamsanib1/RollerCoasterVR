using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensingLib : MonoBehaviour {
    public Interpretor _ip;
    GameObject collider;
    string[] typeMap = { "animal", "ground", "bird", "train", "wall" };

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void execute(string[] ins)
    {
        if(ins[0] == "gettype") { getType(ins); }
        else if(ins[0] == "settype") { setType(ins); }
        else if (ins[0] == "raycast") { raycast(ins);                       }
        else if (ins[0] == "collidercoords") { getColliderLocation(ins);    }
        else if (ins[0] == "collidertype") { getColliderType(ins);          }
        else if (ins[0] == "colliderangle") { getColliderAngle(ins);         }
        else if (ins[0] == "releasecollider") { releaseCollider();          }
        else if(ins[0] == "setsound") { setAudio(ins[1]); }
    }

    public void raycast(string[] ins)
    {
        float dist = float.Parse(ins[1]);
        float angle = float.Parse(ins[2]);
        Vector3 dir = gameObject.transform.forward + new Vector3(0, 0, angle);
        double val = 0;

        Ray ray = new Ray(transform.position + new Vector3(0, .5f, 0.2f), dir);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, dist))
        {
            val = 1;
            if (hit.collider.name == gameObject.name)
            {
                ray = new Ray(hit.point + transform.forward * 0.5f, dir);
                if (Physics.Raycast(ray, out hit, dist))
                {
                    collider = hit.collider.gameObject;
                    Debug.Log("raycast value:" + val + ":" + ins[3] + ":tag:" + hit.collider.tag + ":name:" + hit.collider.name);
                }
            }
        }
        _ip.setValue(ins[3], val);
    }

    public void getColliderLocation(string[] ins)
    {
        float x = 0;
        float y = 0;
        float z = 0;
        Debug.Log("collider location:" + ins[1]);
        if (collider != null)
        {
            Debug.Log("collider is working." + collider.name);
            x = collider.transform.position.x;
            y = collider.transform.position.y;
            z = collider.transform.position.z;
        }
        _ip.setValue(ins[1], x);
        _ip.setValue(ins[2], y);
        _ip.setValue(ins[3], z);
    }
    public void getColliderType(string[] ins)
    {
        int type = -1;

        if (collider != null)
        {
            for (int i = 0; i < typeMap.Length; i++)
            {
                if (collider.tag == typeMap[i]) { type = i; break; }
            }
            Debug.Log("collider type is working." + collider.name);
        }

        _ip.setValue(ins[1], type);
    }
    public void getColliderAngle(string[] ins)
    {
        float angle = 0;
        if (collider != null)
        {
            angle = collider.transform.rotation.eulerAngles.y;
            Debug.Log("collider angle is working:" + angle + "::name:" + collider.name);
        }
        _ip.setValue(ins[1], angle);
    }
    public void releaseCollider()
    {
        collider = null;
    }


    public void getType(string[] ins) {
        string name = ins[1].Replace("\"","");
        string var = ins[2];
        //Debug.Log("name: "+name);
        for (int i = 0; i < Data.objects.Count; i++)
        {
            MainObject mo = Data.objects[i].GetComponent<MainObject>();
            if (name == mo.scriptRef)
            {
                _ip.setValue(var, mo.userType);
//                Debug.Log("value :" + _ip.getValue(var)+" ::var:"+var+"::val:"+mo.userType+"::script ref:"+mo.scriptRef);
                break;
            }
        }
    }

    public void setType(string[] ins)
    {
        int type = int.Parse(ins[1]);
        gameObject.GetComponent<MainObject>().userType = type;
    }
    public void setAudio(string name) {
        int index = -1;
        name = name.Replace("\"", "");
        for (int i = 0; i < Data.data.sounds.Length; i++) if (Data.data.sounds[i].rcname == name) { index = i; break; }
        if (index < 0) { UnityEngine.Debug.Log(name + " audio clip not found!!!"); return; }
        if (Data.objects.Count > 0)
        {
            for (int i = 0; i < Data.objects.Count; i++)
            {
                AudioSource audio = gameObject.AddComponent<AudioSource>();
                audio.clip = Data.data.sounds[index].clip; ;
                audio.Play();
                audio.spatialBlend = 1;
                audio.loop = true;
            }

        }
    }    
    public void setInterpretor(Interpretor interpretor)
    {
        _ip = interpretor;
    }
}
