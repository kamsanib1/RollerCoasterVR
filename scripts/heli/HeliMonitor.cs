using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeliMonitor : MonoBehaviour {
    public bool isAuto = true;
    public GameObject Camera;
    public GameObject VRCamera;
    public bool active = false;
    public Animator animController;
    public MyHeliController heliController;
    public carrunning carController;
    public Collider collider;
    GameObject player;
    public bool isHeli = true;

    float timer;
    
	// Use this for initialization
	void Start () {
        active = false;
        Camera.SetActive(false);
        VRCamera.SetActive(false);
        if (isHeli)
        {
            
            heliController =  GetComponent<MyHeliController>();
            heliController.Stop();
            heliController.enabled = false;
            animController.SetInteger("state", 0);
            collider.enabled = true;
        }
        else {
            carController = GetComponent<carrunning>();
            carController.stopCar();
            carController.enabled = false;
        }

        //player.GetComponent<InputPC>().manageDrive();
        //Destroy(GetComponent<InputPC>());
        //Destroy(GetComponent<ViveRightInput>());
        MainObject mo = GetComponent<MainObject>();
        if(mo.script == "" || mo.script == null) { isAuto = false; }
        else { gameObject.GetComponent<HeliMonitor>().enabled = false; }        
    }
	
	// Update is called once per frame
	void Update () {
        
        if (isAuto) return;
        if (RCInput.triggerR && timer >= 5) {
            if (heliController!=null && heliController.canGetOut())
                endRide();
            if (carController != null)
                endRide();
        }
        if (timer >= 1) timer += Time.deltaTime;
        //Debug.Log("active:" + active);
	}

    public void startRide(GameObject _player)
    {
        player = _player;
        active = true;
        if (isAuto) { endRide(); return; }
        timer = 1;
        Debug.Log("starting heli ride");
        if (Data.vrEnabled) VRCamera.SetActive(true);
        else Camera.SetActive(true);
        if (isHeli)
        {
            heliController.enabled = true; 
            animController.SetInteger("state", 1);
            collider.enabled = false;
        }
        else {
            carController.enabled = true; ;
            carController.startCar();
        }
        //gameObject.AddComponent<InputPC>();
        //gameObject.AddComponent<ViveRightInput>();
    }

    public void endRide()
    {
        if (!active) return;
        timer = 0;
        Debug.Log(gameObject.name + ":end ride : " + active);
        active = false;
        Debug.Log(gameObject.name + ":end ride : " + active);
        Camera.SetActive(false);
        VRCamera.SetActive(false);
        if(player == null && !isHeli) { Debug.Log(gameObject.name + ":player null!!!"); return; }
        if (Data.vrEnabled)
            player.GetComponentInChildren<ViveRightInput>().manageDrive();
        else
            player.GetComponent<InputPC>().manageDrive();
        if (isHeli)
        {
            heliController.Stop();
            heliController.enabled = false;
            animController.SetInteger("state", 0);
            collider.enabled = true;
        }
        else {
            carController.stopCar();
            carController.enabled = false;
        }
    }
}
