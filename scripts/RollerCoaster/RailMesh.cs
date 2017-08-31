using System;
using System.Collections.Generic;
using UnityEngine;

public class RailMesh : MonoBehaviour
{
    public Material mat;
    public float spacing = 3f;
    public bool collider_flag = true;
    public bool render_flag = true;
    public float height = 0.08f;
    public float width = 0.05f;
    public float height_relative;
    public float skip_tracks = 1f;
    public PhysicMaterial phy_mat;
    public InputData data;
    public int start;
    public int end;
    public List<GameObject> object_mgr;
    public bool two_sided;

    private Mesh mesh;
    private MeshCollider meshCollider;
    private string name;

    private void Start()
    {
        mesh = new Mesh();
    }

    public void generateMesh() {
        //-Debug.Log("mesh generation started.");
        base.gameObject.AddComponent<MeshFilter>();
        base.GetComponent<MeshFilter>().mesh = (this.mesh = new Mesh());
        this.mesh.name = base.gameObject + "mesh";
        base.gameObject.AddComponent<MeshRenderer>();
        if (this.collider_flag)
        {
            this.meshCollider = base.gameObject.AddComponent<MeshCollider>();
            this.meshCollider.material = this.phy_mat;
        }
        base.gameObject.GetComponent<MeshRenderer>().material = this.mat;
        if (!this.render_flag)
        {
            base.GetComponent<MeshRenderer>().enabled = false;
        }
        this.height /= 2f;
        this.width /= 2f;
        this.height_relative += this.height + 0.05f;
        this.skip_tracks = (float)((int)this.skip_tracks);
        if (this.skip_tracks == 0f)
        {
            this.skip_tracks = 1f;
        }
        //Debug.Log("points:");
        this.railMesh();
    }

    private void railMesh()
    {
        Vector3 position = base.gameObject.transform.position;
        Vector3[] array = this.vertexCalculator();
        int num = array.Length;
        int[] array2;
        if (this.two_sided)
        {
            array2 = new int[2 * (num - 4) * 3 * 2];
        }
        else
        {
            array2 = new int[2 * (num - 4) * 3];
        }
        int i = -1;
        int num2 = -1;
        while (i < num - 5)
        {
            array2[++num2] = i + 8;
            array2[++num2] = i + 4;
            array2[++num2] = i + 1;
            array2[++num2] = i + 1;
            array2[++num2] = i + 5;
            array2[++num2] = i + 8;
            array2[++num2] = i + 5;
            array2[++num2] = i + 1;
            array2[++num2] = i + 2;
            array2[++num2] = i + 2;
            array2[++num2] = i + 6;
            array2[++num2] = i + 5;
            array2[++num2] = i + 7;
            array2[++num2] = i + 6;
            array2[++num2] = i + 2;
            array2[++num2] = i + 2;
            array2[++num2] = i + 3;
            array2[++num2] = i + 7;
            array2[++num2] = i + 8;
            array2[++num2] = i + 7;
            array2[++num2] = i + 3;
            array2[++num2] = i + 3;
            array2[++num2] = i + 4;
            array2[++num2] = i + 8;
            if (this.two_sided)
            {
                array2[++num2] = i + 8;
                array2[++num2] = i + 5;
                array2[++num2] = i + 1;
                array2[++num2] = i + 1;
                array2[++num2] = i + 4;
                array2[++num2] = i + 8;
                array2[++num2] = i + 5;
                array2[++num2] = i + 6;
                array2[++num2] = i + 2;
                array2[++num2] = i + 2;
                array2[++num2] = i + 1;
                array2[++num2] = i + 5;
                array2[++num2] = i + 7;
                array2[++num2] = i + 3;
                array2[++num2] = i + 2;
                array2[++num2] = i + 2;
                array2[++num2] = i + 6;
                array2[++num2] = i + 7;
                array2[++num2] = i + 8;
                array2[++num2] = i + 4;
                array2[++num2] = i + 3;
                array2[++num2] = i + 3;
                array2[++num2] = i + 7;
                array2[++num2] = i + 8;
            }
            i += 4;
        }
        this.mesh.vertices = array;
        this.mesh.triangles = array2;
        this.mesh.RecalculateBounds();
        this.mesh.RecalculateNormals();
        if (this.collider_flag)
        {
            this.meshCollider.sharedMesh = this.mesh;
        }
        //-Debug.Log("-------rail generation finished-------------");
    }

    private Vector3[] vertexCalculator()
    {
        int num = 0;
        GameObject obj = new GameObject("tmp1");
        GameObject gameObject = new GameObject("tmp2");
        Vector3[] array = new Vector3[(this.end - this.start) * 4];
        //Debug.Log("count:" + object_mgr.Count + "::end:" + end*skip_tracks);
        for (int i = this.start; i < this.end; i++)
        {
            GameObject gameObject2 = this.object_mgr[(int)(i*skip_tracks)];
            int num2 = 1;
            int num3 = 1;
            for (int j = 0; j < 4; j++)
            {
                gameObject.transform.parent = gameObject2.transform;
                gameObject.transform.localPosition = new Vector3(this.spacing + this.width * (float)num2, this.height_relative + this.height * (float)num3, 0f);
                array[num++] = gameObject.transform.position;
                if (j == 1)
                {
                    num3 *= -1;
                }
                else
                {
                    num2 *= -1;
                }
            }
        }
        UnityEngine.Object.Destroy(obj);
        UnityEngine.Object.Destroy(gameObject);
        return array;
    }

    public void setMaterial(Material m) { mat = m; }
    public void setSpacing(float s) { spacing = s; }
    //public void setCollider(bool c) { collider_flag = true; }
    public void setRender(bool r) { render_flag = r; }
    public void setHeight(float h) { height = h; }
    public void setWidth(float w) { width = w; }
    public void setRelativeHeight(float rh) { height_relative = rh; }
    public void setSkipTracks(float st) { skip_tracks = st; }
    public void setPhysicMaterial(PhysicMaterial pm) { phy_mat = pm; }
    public void setInputData(InputData d) { data = d; }
    public void setStart(int s) { start = s; }
    public void setEnd(int e) { end = e; }
    public void setObjectManager(List<GameObject> om) { object_mgr = om; }
    public void setTwoSided(bool ts) { two_sided = ts; }
}
