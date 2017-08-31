using System;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject default_pos;

    public float speed = 25f;

    public float zoomSpeed = 100f;

    private float minX = -360f;

    private float maxX = 360f;

    private float minY = -90f;

    private float maxY = 90f;

    public float sensX = 600f;

    public float sensY = 600f;

    private float rotationY;

    private float rotationX;

    public bool rotatable = true;
    float multiplier = 1.5f;

    void Start()
    {
        transform.position = Data.edit_cam_pos;
        transform.rotation = Data.edit_cam_rot;
    }

    private void Update()
    {
        if(Input.GetKeyDown ( KeyCode.LeftShift)  || Input.GetKeyDown (KeyCode.RightShift)) { multiplier = 5; }
        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift)) { multiplier = 1.5f; }
        float axis = Input.GetAxis("Mouse ScrollWheel");
        if (Input.mousePosition.x < Screen.width-Data._inspectorWidth && !Data._menuOpen) {
            if (stayAboveGround())
                base.transform.position += base.transform.forward * this.zoomSpeed * axis * Time.deltaTime * multiplier;
            updatePos();
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            base.transform.position += base.transform.right * this.speed * Time.deltaTime * multiplier;
            updatePos();
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            base.transform.position -= base.transform.right * this.speed * Time.deltaTime * multiplier;
            updatePos();
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            base.transform.position += base.transform.up * this.speed * Time.deltaTime * multiplier;
            updatePos();
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            if(stayAboveGround())
            base.transform.position -= base.transform.up * this.speed * Time.deltaTime * multiplier;
            updatePos();
        }
        if (Input.GetKey(KeyCode.Q))
        {
            base.transform.position += base.transform.forward * this.speed * Time.deltaTime * multiplier;
            updatePos();
        }
        if (Input.GetKey(KeyCode.E))
        {
            base.transform.position -= base.transform.forward * this.speed * Time.deltaTime * multiplier;
            updatePos();
        }
        if (Input.GetKey(KeyCode.R))
        {
            if (this.default_pos != null)
            {
                base.transform.position = this.default_pos.transform.position;
                base.transform.rotation = this.default_pos.transform.rotation;
            }
            else
            {
                base.transform.position = Vector3.zero;
                base.transform.rotation = Quaternion.identity;
            }
        }
        if (this.rotatable && Input.GetMouseButton(1))
        {
            this.rotationX += Input.GetAxis("Mouse X") * this.sensX * Time.deltaTime * multiplier;
            this.rotationY += Input.GetAxis("Mouse Y") * this.sensY * Time.deltaTime * multiplier;
            this.rotationY = Mathf.Clamp(this.rotationY, this.minY, this.maxY);
            base.transform.localEulerAngles = new Vector3(-this.rotationY, this.rotationX, 0f);
            updatePos();
        }

        //editor rotate active object. **tmp code//
        if (Input.GetKey(KeyCode.R)&&Data.activeObj>=0)
        {
            GameObject obj = Data.objects[Data.activeObj];
            MainObject mo = obj.GetComponent<MainObject>();
            //obj.transform.rotation = Quaternion.Euler( obj.transform.rotation.eulerAngles + new) 
        }
    }

    void updatePos()
    {
        Data.edit_cam_pos = transform.position;
        Data.edit_cam_rot = transform.rotation;
    }

    bool stayAboveGround()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out hit))
        {
            float dist = Vector3.Distance(transform.position, hit.point);
            if (dist > 1)
            {
                return true;
           }
        }
        return false;
    }
}
