using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {
    Camera cam;
	// Use this for initialization
	void Start () {
        cam = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
        if (Data.gameState == GameState.TRAIN)
        {
            cam.enabled = false;
        }
        else cam.enabled = true;
	}
}
