using UnityEngine;
using System.Collections;

public class Drag : MonoBehaviour {
    bool _drag = false;
    MainObject obj;
    // Use this for initialization
    void Start () {
        obj = gameObject.GetComponent<MainObject>();
       
    }
	
	// Update is called once per frame
	void Update () {
        if (_drag)
        {
            RaycastHit hit = new RaycastHit();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit) && hit.collider.tag == "ground")
            {
                transform.position = hit.point + new Vector3(0, 0, 0);

                obj.position = hit.point;
               
            }
        }
    }

    void OnMouseUp()
    {
        //Debug.Log("UP");
        _drag = false; 
    }
    void OnMouseDown()
    {
        _drag = true;
        //Debug.Log("down");
        if (obj == null) return;
        Data.activeObj = obj.id;
    }

    
}
