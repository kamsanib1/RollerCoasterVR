using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heliTest : MonoBehaviour {
    GameObject go;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        go = GameObject.FindGameObjectWithTag("go");
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * 5 * Time.deltaTime;
            go.transform.localRotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(90, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z) , Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= transform.right * 5 * Time.deltaTime;
            go.transform.localRotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.y, 90), Time.deltaTime*1.2f* 0.8f );
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * 5 * Time.deltaTime;
            go.transform.localRotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles.y , transform.rotation.eulerAngles.y, -90), Time.deltaTime * 1.2f * 0.8f);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward * 5 * Time.deltaTime;
            go.transform.localRotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(-90, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z), Time.deltaTime);
        }
    }
}
