using UnityEngine;
using System.Collections;

public class DestroyScript : MonoBehaviour {
    public float time=1;
    float cur_time=0;
    public bool destroyFlag = true;
	// Use this for initialization
	void Start () {
        cur_time = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if (cur_time > time)
        {
            if (destroyFlag) Destroy(gameObject);
            else {
                gameObject.SetActive(false);
                Destroy(gameObject.GetComponent<DestroyScript>());
            }
        }
        else cur_time += Time.deltaTime;
	}

    public void setTime(float destroy_time) { time = destroy_time; }
    public void setInactivateFlag() { destroyFlag = false; }
}
