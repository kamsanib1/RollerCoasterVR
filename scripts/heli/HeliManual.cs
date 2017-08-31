using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeliManual : MonoBehaviour
{
    public float moveSpeed = 10f;
    private Vector3 moveDirection = Vector3.zero;
    private CharacterController controller;
    public Rigidbody rb;
    // Update is called once per frame
    //void Update()
    //{
    // transform.Translate(Vector3.forward * Time.deltaTime * Input.GetAxis("Vertical") * moveSpeed);
    // transform.Translate(Vector3.right * Time.deltaTime * Input.GetAxis("Horizontal") * moveSpeed);
    //}

    //public float verticalSpeed=1;
    // public float amplitude=1;
    float speed = 0;
    
    void Start()
    {
        //tempPosition = transform.position;
        rb = GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        
        //tempPosition.y = Mathf.Sin(Time.realtimeSinceStartup * verticalSpeed) * amplitude;
        //transform.position = tempPosition;
        //if (gameObject.transform.position.y <= 23)
       // {
         //   rb.isKinematic = false;
        //    rb.detectCollisions = true;
        //    rb.useGravity = true;
            //transform.rotation = Quaternion.AngleAxis(90, Vector3.up);
    //    }
     //   if (gameObject.transform.position.y > 18)
      //  {
      //      rb.isKinematic = true;
      //      rb.useGravity = false;
            
     //   }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(Vector3.up * Time.deltaTime * moveSpeed);
        }
       if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(Vector3.down * Time.deltaTime * moveSpeed);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            GetComponent<Transform>().Rotate(Vector3.down * Time.deltaTime * moveSpeed*2);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            GetComponent<Transform>().Rotate(Vector3.up * Time.deltaTime * moveSpeed*2);
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * Time.deltaTime * moveSpeed);
        }
    }
}