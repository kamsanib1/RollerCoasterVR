using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour {

    public CharacterController controller;
    public float walk_speed = 5;
    public float run_speed = 20;
    public float tx_speed = 2f;
    public float ty_speed = 2f;
    public float ymin = -90;
    public float ymax = 90;

    public GameObject gun_object;
    public GameObject gun_end;
    public GameObject bullet;
    LineRenderer laserLine;

    private Vector3 moveDirection = Vector3.zero;
    private float speed;
    private float x;
    private float y;
    // Use this for initialization
    void Start () {
        if (controller == null) controller = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetAxis("Vertical") != 0)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                 speed = run_speed;
            }
            else {
                speed = walk_speed;
            }
            //Debug.Log("walking");
        }

        //movement allocation and calculation//
        if (controller.isGrounded)
        {
            moveDirection = transform.forward * Input.GetAxis("Vertical") * speed;
            moveDirection += transform.right * Input.GetAxis("Horizontal") * speed;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                moveDirection += transform.up * 10;
            }
        }
        if (Data.movementFlag) { controller.Move(moveDirection * Time.deltaTime); }
        //Debug.Log("movement:"+Data.movementFlag);
        moveDirection.y -= 20 * Time.deltaTime;
        
        if (true) {
            //turning allocation and calculation//
            x += Input.GetAxis("Mouse X") * tx_speed;// * 0.02f;
            y += Input.GetAxis("Mouse Y") * ty_speed;// * 0.02f;

            y = Mathf.Clamp(y, ymin, ymax);
            transform.eulerAngles = new Vector3(-y, x, 0);
        }
        //shooting();
    }

    float time = 0;
    float interval = .5f;
    private void shooting()
    {

        if (Input.GetMouseButton(0) && time > interval)
        {
            RaycastHit hit;
            //gun_end = transform.position + transform.forward * 1.2f;
            Debug.Log(gun_object.transform.forward);
            if (Physics.Raycast(gun_end.transform.position, this.gun_end.transform.forward, out hit, 500))
            {
                Debug.Log("raycast hit:" + hit.collider.gameObject + ":" + hit.collider.tag);
                //Debug.DrawRay(gun_end.transform.position, gun_end.transform.forward*gun.range,Color.red);
                GameObject b = Instantiate(bullet);
                b.transform.position = hit.point;

                //laserLine.SetPosition(0, gun_end.transform.position);
                //laserLine.SetPosition(1, hit.point);
                if (hit.collider.gameObject.tag == "animal")
                {
                    Debug.Log("shot the animal.");
                    hit.collider.gameObject.GetComponent<AnimalAnimation>().gotShot();
                }
            }
            time = 0;
        }
        else time += Time.deltaTime;
    }
}
