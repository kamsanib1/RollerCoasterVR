using UnityEngine;
using System.Collections;

public class InputPC : InputManager {
    
    private int LEFT_CLICK = 0;
	// Use this for initialization
	void Start () {
        base.gun = Camera.main.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(LEFT_CLICK)) {
            base.triggerDown = true;
        }
        if (Input.GetMouseButtonUp(LEFT_CLICK))
        {
            base.triggerDown = false;
        }

        RCInput.palleteUpL      = Input.GetKey(KeyCode.W);
        RCInput.palleteRightL   = Input.GetKey(KeyCode.D);
        RCInput.palleteDownL    = Input.GetKey(KeyCode.S);
        RCInput.palleteLeftL    = Input.GetKey(KeyCode.A);
        RCInput.palleteUpR      = Input.GetKey(KeyCode.UpArrow);
        RCInput.palleteRightR   = Input.GetKey(KeyCode.RightArrow);
        RCInput.palleteDownR    = Input.GetKey(KeyCode.DownArrow);
        RCInput.palleteLeftR    = Input.GetKey(KeyCode.LeftArrow);

        base.update();
    }

}
