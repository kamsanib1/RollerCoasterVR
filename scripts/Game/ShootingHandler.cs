using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void gotShot()
    {
        Debug.Log(gameObject.name + " shooting handler working.");
        MainObject mo = gameObject.GetComponent<MainObject>();
        if(gameObject.tag == "animal") { gameObject.GetComponent<AnimalAnimation>().gotShot(); }
        if(mo.type == ObjectType.BIRD) { gameObject.GetComponent<BirdAnim>().gotShot(); }
        if(mo.type == ObjectType.PLANT) { gameObject.GetComponent<MovementLib>().gotShot(); }
        if (mo.type == ObjectType.BALLOON) { gameObject.GetComponent<MovementLib>().gotShot(); }

    }
}
