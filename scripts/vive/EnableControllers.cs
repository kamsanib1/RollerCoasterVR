using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableControllers : MonoBehaviour {
    public GameObject[] controller;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (gameObject.active)
        {
            for(int i = 0; i < controller.Length; i++)
            {
                if(!controller[i].active) controller[i].active = true;
            }
        }
	}
}
