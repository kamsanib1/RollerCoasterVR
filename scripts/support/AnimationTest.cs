using UnityEngine;
using System.Collections;

public class AnimationTest : MonoBehaviour {
    Animator anim;
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.A)) { anim.SetTrigger("a"); }
	}
}
