using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTesting : MonoBehaviour {

    public int animCount;
    int state = 0;
    public Animator anim;

	// Use this for initialization
	void Start () {
        if (anim == null) anim = GetComponent<Animator>();
        if (anim == null) anim = GetComponentInChildren<Animator>();

    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            state = (state + 1) % animCount;
            anim.SetInteger("state", state);
            anim.SetTrigger("makeTransition");
        }
    }
}
