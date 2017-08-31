using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCInput : MonoBehaviour {
    
    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        RCInput.triggerR = Input.GetKey(KeyCode.Mouse0);
               
        RCInput.palleteUpL = Input.GetKey(KeyCode.W);
        RCInput.palleteRightL = Input.GetKey(KeyCode.D);
        RCInput.palleteDownL = Input.GetKey(KeyCode.S);
        RCInput.palleteLeftL = Input.GetKey(KeyCode.A);

        RCInput.palleteUpR = Input.GetKey(KeyCode.UpArrow);
        RCInput.palleteRightR = Input.GetKey(KeyCode.RightArrow);
        RCInput.palleteDownR = Input.GetKey(KeyCode.DownArrow);
        RCInput.palleteLeftR = Input.GetKey(KeyCode.LeftArrow);

    }

}