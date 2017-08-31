using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserLight : MonoBehaviour {
    private GameObject holder;
    private GameObject pointer;
    private GameObject[] reflection_holder;
    private GameObject[] reflection_pointer;
    //private float dist;

    public Color color;
    public float thickness;
    public float maxDist = 100;

	// Use this for initialization
	void Start () {
        holder = new GameObject("laser");
        holder.transform.parent = this.transform;
        holder.transform.localPosition = Vector3.zero;
        holder.transform.localRotation = Quaternion.identity;

        pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pointer.transform.parent = holder.transform;
        pointer.transform.localScale = new Vector3(thickness, thickness, 100f);
        pointer.transform.localPosition = new Vector3(0f, 0f, 50f);
        pointer.transform.localRotation = Quaternion.identity;
        BoxCollider collider = pointer.GetComponent<BoxCollider>();
        Object.Destroy(collider);

        Material newMaterial = new Material(Shader.Find("Unlit/Color"));
        newMaterial.SetColor("_Color", color);
        pointer.GetComponent<MeshRenderer>().material = newMaterial;

    }

    // Update is called once per frame
    void Update () {
        Ray ray = new Ray(transform.localPosition, transform.forward);
        RaycastHit hit;
        float dist;
        if (Physics.Raycast(ray, out hit, maxDist))
        {
            dist = Vector3.Distance(transform.position, hit.point);
            if (hit.collider.tag == "reflector")
            {
                drawReflections(hit);
            }
        }
        else dist = maxDist;
	}

    void drawReflections(RaycastHit fhit)
    {
        RaycastHit hit;
        Vector3 dir = getReflection(fhit.transform.forward, fhit.collider.transform.forward);

        Ray ray = new Ray(fhit.point, transform.forward);
        float dist;
        //RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDist))
        {
            dist = Vector3.Distance(fhit.point, hit.point);
            if (hit.collider.tag == "reflector")
            {
                drawReflections(hit);
            }
        }
        //return hit;
    }

    Vector3 getReflection(Vector3 d, Vector3 n)
    {
        Vector3 r;
        r = d - 2 * (Vector3.Dot(d, n)) * n;
        return r;
    }

}
