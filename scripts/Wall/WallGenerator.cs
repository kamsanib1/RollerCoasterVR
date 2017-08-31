using System;
using UnityEngine;
using System.Collections.Generic;

public class WallGenerator
{
    public static float MAX = 60000f;
    public InputData data;
    public WallBuilder gt;
    
    private List<GameObject> tracks = new List<GameObject>();
    
    private GameObject[][] mesh_rails;
    private GameObject wall;
    private GameObject track_main;
    private GameObject railings;

    public WallGenerator(WallBuilder tmp_gt)
    {
        gt = tmp_gt;
        
        wall = new GameObject();
        track_main = new GameObject("poles");
        railings = new GameObject("planks");

        track_main.transform.parent = wall.transform;
        railings.transform.parent = wall.transform;
    }

    //generates tracks and hoardings//
    public void track_generator(GameObject g_obj)
    {
        //Debug.Log("Laying down tracks and hoardings.");
        for (int i = 0; i < (gt.points.Count); i++)
        {
            GameObject gameObject = gt.points[i];
            GameObject gameObject2 = UnityEngine.Object.Instantiate(g_obj, gameObject.transform.position, gameObject.transform.rotation) as GameObject;
            gameObject2.name = "pole" + (i + 1);
            gameObject2.transform.parent = track_main.transform;
            tracks.Add(gameObject2);
        }
        //-Debug.Log("railing generation started.");
    }

  
    public void wallGenerate()
    {
        wall.name = "wall";
        track_main = new GameObject("wall");
        RailMesh railMesh = track_main.AddComponent<RailMesh>();
        railMesh.skip_tracks = 0;
        railMesh.start = 0;
        railMesh.end = (int)(gt.points.Count );
        railMesh.object_mgr = gt.points;
        railMesh.two_sided = true;
        railMesh.collider_flag = true;
        railMesh.width = .1f;
        railMesh.height = 1f;
        railMesh.spacing = 0f;
        railMesh.height_relative = 0f;
        railMesh.mat = Data.data.wall_mat;
        railMesh.generateMesh();

    }

    public void fenceWall()
    {
        wall.name = "fence wall";
        track_generator(Data.data.fence_pole);
        int num = 2;
        float total_length = (gt.points.Count );
        int submesh_count = (int)Mathf.Ceil(total_length * 4f / RailGenerator.MAX);
        mesh_rails = new GameObject[num][];
        for (int i = 0; i < num; i++)
        {
            mesh_rails[i] = new GameObject[submesh_count];
        }
        int start = 0;
        int end;
        if (submesh_count == 1)
        {
            end = (int)total_length;
        }
        else
        {
            end = (int)(RailGenerator.MAX / 4f);
        }
        //Debug.Log("track count:" + total_length + "\t" + (int)(gt.points.getCount()/gt.speed_points));
        for (int j = 0; j < submesh_count; j++)
        {
            for (int k = 0; k < num; k++)
            {
                mesh_rails[k][j] = new GameObject("rail" + j + "." + k);
                mesh_rails[k][j].transform.parent = railings.transform;
                RailMesh railMesh = mesh_rails[k][j].AddComponent<RailMesh>();
                railMesh.skip_tracks = 0;
                railMesh.start = start;
                railMesh.end = end;
                railMesh.object_mgr = gt.points;
                railMesh.two_sided = true;
                railMesh.collider_flag = true;
                railMesh.width = .1f;
                railMesh.height = .2f;
                railMesh.spacing = -0.1f;
                if (k == 0) railMesh.height_relative = .3f;
                else railMesh.height_relative = 1f;
                railMesh.mat = Data.data.fence_mat;
                Debug.Log("generating mesh::start:"+start+"::end:"+end);
                railMesh.generateMesh();
            }
            start = end;
            if (j == submesh_count - 2)
            {
                end = (int)(total_length * 4f % RailGenerator.MAX);
            }
            else
            {
                end = (int)(total_length * (float)(j + 2));
            }
        }
    }

    //delete all data in the current object//
    public void flush()
    {
        if (gt.model == (int)WallModel.FENCE)
        {
            tracks.Clear();
            for (int i = 0; i < mesh_rails.Length; i++)
            {
                for (int j = 0; j < mesh_rails[i].Length; j++)
                {
                    UnityEngine.Object.Destroy(mesh_rails[i][j]);
                }
            }
        }
        
        UnityEngine.Object.Destroy(track_main);
   }

    public void setGenerateWall(WallBuilder g) { gt = g; }
    public void setInputData(InputData id) { data = id; }
    public void setPath(GameObject path)
    {
        path.transform.parent = wall.transform;
    }
    public GameObject getMainObject() { return wall; }
}
