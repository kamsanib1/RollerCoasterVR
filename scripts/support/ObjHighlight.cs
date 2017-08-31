using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjHighlight : MonoBehaviour {
    public GameObject lightPrefab;
    GameObject light;
	// Use this for initialization
	void Start () {
        light = Instantiate(lightPrefab);
        light.transform.position = new Vector3(-1000, -1000, -1000);
	}
	
	// Update is called once per frame
	void Update () {
        if (Data.objects.Count > 0)
        {
            light.transform.position = Data.objects[Data.activeObj].transform.position + new Vector3(0, 1, 0);
        }
	}
}
