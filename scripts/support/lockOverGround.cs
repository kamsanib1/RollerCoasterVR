using UnityEngine;
using System.Collections;

public class lockOverGround : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, -transform.up);
        if(Physics.Raycast(ray,out hit)) { }
	}
}
