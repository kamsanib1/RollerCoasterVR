using System;
using UnityEngine;
using System.Collections.Generic;

public class RailGenerator
{
    public static float MAX = 60000f;
    public InputData data;
    public TrackBuilder gt;
    public WallBuilder wb;

    private List<GameObject> tracks = new List<GameObject>();
    private List<GameObject> hoardings = new List<GameObject>();

    private GameObject[][] mesh_rails;
    private GameObject rollercoaster;
    private GameObject track_main;
    private GameObject railings;
    private GameObject hoarding_main;

    private GameObject[] tunnels;

    private GameObject tunnels_main;

    public RailGenerator(TrackBuilder tmp_gt)
    {
        gt = tmp_gt;
        data = Data.data;

        rollercoaster = new GameObject();
        track_main = new GameObject("tracks");
        hoarding_main = new GameObject("hoardings");
        tunnels_main = new GameObject("tunnels");
        railings = new GameObject("railings");

        track_main.transform.parent = rollercoaster.transform;
        hoarding_main.transform.parent = rollercoaster.transform;
        tunnels_main.transform.parent = rollercoaster.transform;
        railings.transform.parent = rollercoaster.transform;
    }

    //generates tracks and hoardings//
    public void track_generator(GameObject g_obj)
    {
        //Debug.Log("Laying down tracks and hoardings.");
        bool flag = false;
        int num = 0;
        for (int i = 0; i < (int)(gt.points.Count/Data.speed_points); i++)
        {
            GameObject gameObject = gt.points[(int)(i*Data.speed_points)];
            GameObject gameObject2 = UnityEngine.Object.Instantiate( g_obj, gameObject.transform.position, gameObject.transform.rotation) as GameObject;
            gameObject2.name = "track" + (i + 1);
            gameObject2.transform.parent = track_main.transform;
            tracks.Add(gameObject2);
            if (gt.type == 1) {
                if (i == 10)
                {
                    num = 0;
                    flag = true;
                }
                else if (i == 40)
                {
                    num = 1;
                    flag = true;
                }
                else if (i > 99 && i % Data.data.hoarding_gap == 0)
                {
                    num = UnityEngine.Random.Range(0, 100) % Data.data.hoarding_mats.Length;
                    flag = true;
                }
                if (flag)
                {
                    flag = false;
                    GameObject gameObject3 = UnityEngine.Object.Instantiate(Data.data.hoarding, gameObject.transform.position, gameObject.transform.rotation) as GameObject;
                    gameObject3.transform.parent = hoarding_main.transform;
                    gameObject3.GetComponentInChildren<Renderer>().material = Data.data.hoarding_mats[num];
                    hoardings.Add(gameObject3);
                }
            }
        }
        generateTunnels();
        //-Debug.Log("railing generation started.");
    }

    //generates tunnels for all kinds of tracks//
    private void generateTunnels()
    {
        if (gt.tunnels.Count <= 0)
        {
            return;
        }
        //Debug.Log("tunnel count" + gt.tunnels.Count);
        tunnels = new GameObject[gt.tunnels.Count];
        for (int i = 0; i < gt.tunnels.Count; i++)
        {
            tunnels[i] = new GameObject("tunnel" + (i + 1));
            RailMesh railMesh = tunnels[i].AddComponent<RailMesh>();
            railMesh.skip_tracks = Data.speed_points;
            railMesh.start = gt.tunnels[i].start/(int)Data.data.speed_points;
            railMesh.end = gt.tunnels[i].end/(int)Data.data.speed_points;
            railMesh.object_mgr = gt.points;
            railMesh.two_sided = true;
            railMesh.collider_flag = true;
            railMesh.width = 8f;
            railMesh.height = 6f;
            railMesh.spacing = 0f;
            railMesh.height_relative = -1f;
            railMesh.mat = Data.data.tunnel_mat;
            tunnels[i].transform.parent = tunnels_main.transform;
            railMesh.generateMesh();
        }
     }

    //generates a wooden rollercoaster track//
    public void woodCoaster()
    {
        rollercoaster.name = "wooden coaster";
        track_generator(Data.data.bar);
        int num = 7;
        float total_length = (gt.points.Count / Data.speed_points);
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
            Debug.Log("something is wrong");
            end = (int)(RailGenerator.MAX / 4f);
        }
        Debug.Log("track count:" + total_length+"\t"+(int)(gt.points.Count/Data.speed_points));
        for (int j = 0; j < submesh_count; j++)
        {
            for (int k = 0; k < num; k++)
            {
                mesh_rails[k][j] = new GameObject("rail"+j+"."+k);
                mesh_rails[k][j].transform.parent = railings.transform;
                RailMesh railMesh = mesh_rails[k][j].AddComponent<RailMesh>();
                railMesh.skip_tracks = Data.speed_points;
                railMesh.start = start;
                railMesh.end = end;
                railMesh.object_mgr = gt.points;
                railMesh.collider_flag = true;
                if (k == 0)
                {
                    railMesh.mat = Data.data.motor_mat;
                    railMesh.start = 0;
                    railMesh.end = end;
                    Debug.Log("top point : " + railMesh.end);
                    railMesh.spacing = 0f;
                }
                else if (k < 3)
                {
                    railMesh.mat = Data.data.railing_mat;
                    railMesh.spacing = 0.35f;
                }
                else
                {
                    railMesh.mat = Data.data.railing_mat;
                    railMesh.spacing = 0.78f;
                    if (k <= 4)
                    {
                        railMesh.height_relative = 0.45f;
                    }
                    else
                    {
                        railMesh.height_relative = 0.18f;
                    }
                    railMesh.width = 0.05f;
                    railMesh.height = 0.05f;
                }
                if (k % 2 == 0)
                {
                    railMesh.spacing *= -1f;
                }
                Debug.Log("setup finished for railings. Generation in progress...");
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

    //generates a steel rollercoaster track//
    public void steelCoaster()
    {
        rollercoaster.name = "steel coaster";
        track_generator(Data.data.steel_bar);
        int num = 4;
        float total_length = (gt.points.Count/Data.speed_points);
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
        Debug.Log("sub mesh count:" + submesh_count);
        for (int j = 0; j < submesh_count; j++)
        {
            for (int k = 0; k < num; k++)
            {
                mesh_rails[k][j] = new GameObject(string.Concat(new object[]
                {
                    "rail ",
                    k,
                    ".",
                    j
                }));
                RailMesh railMesh = mesh_rails[k][j].AddComponent<RailMesh>();
                mesh_rails[k][j].transform.parent = railings.transform;
                railMesh.skip_tracks = Data.speed_points;
                railMesh.start = start;
                railMesh.end = end;
                railMesh.object_mgr = gt.points;
                railMesh.collider_flag = true;
                if (k == 0)
                {
                    railMesh.mat = Data.data.motor_mat;
                    railMesh.start = 0;
                    railMesh.end = end;
                    //Debug.Log("top point : " + railMesh.end);
                    railMesh.spacing = 0f;
                }
                else if (k == 1)
                {
                    railMesh.mat = Data.data.steel_coaster_mat;
                    railMesh.spacing = 0f;
                    railMesh.height_relative = -0.3f;
                    railMesh.width = 0.2f;
                    railMesh.height = 0.2f;
                }
                else
                {
                    railMesh.mat = Data.data.steel_coaster_mat;
                    railMesh.spacing = 0.5f;
                    railMesh.width = 0.1f;
                    railMesh.height = 0.1f;
                }
                if (k % 2 == 0)
                {
                    railMesh.spacing *= -1f;
                }
                Debug.Log("j:" + j + "::start:" + (start * Data.speed_points) + "::end:" + end * Data.speed_points);
                railMesh.generateMesh();
            }
            start = end;
            if (j == submesh_count - 2)
            {
                end = (int)(total_length );
            }
            else
            {
                end = (int)(total_length * (float)(j + 2));
            }
            
        }
    }

    public void wall()
    {
        rollercoaster.name = "wall";
        track_main = new GameObject("wall");
        RailMesh railMesh = track_main.AddComponent<RailMesh>();
        railMesh.skip_tracks = Data.speed_points;
        railMesh.start = 0;
        railMesh.end = (int)(gt.points.Count/Data.speed_points);
        railMesh.object_mgr = gt.points;
        railMesh.two_sided = false;
        railMesh.collider_flag = true;
        railMesh.width = 1f;
        railMesh.height = 6f;
        railMesh.spacing = 0f;
        railMesh.height_relative = -1f;
        railMesh.mat = Data.data.wall_mat;
        railMesh.generateMesh();

    }

    public void fenceWall()
    {
        rollercoaster.name = "fence wall";
        track_generator(Data.data.fence_pole);
        int num = 2;
        float total_length = (gt.points.Count/Data.speed_points);
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
                railMesh.skip_tracks = Data.speed_points;
                railMesh.start = start;
                railMesh.end = end;
                railMesh.object_mgr = gt.points;
                railMesh.two_sided = false;
                railMesh.collider_flag = true;
                railMesh.width = .1f;
                railMesh.height = .2f;
                railMesh.spacing = -0.1f;
                if (k == 0) railMesh.height_relative = .3f;
                else railMesh.height_relative = 1f;
                railMesh.mat = Data.data.fence_mat;
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
        UnityEngine.Object.Destroy(rollercoaster);
        if (gt.type == 1)
        {
            tracks.Clear();
            hoardings.Clear();
            for (int i = 0; i < mesh_rails.Length; i++)
            {
                for (int j = 0; j < mesh_rails[i].Length; j++)
                {
                    UnityEngine.Object.Destroy(mesh_rails[i][j]);
                }
            }
        }
        else if (gt.type == 2)
        {
            if (gt.model == 2) {
                tracks.Clear();
                for (int i = 0; i < mesh_rails.Length; i++)
                {
                    for (int j = 0; j < mesh_rails[i].Length; j++)
                    {
                        UnityEngine.Object.Destroy(mesh_rails[i][j]);
                    }
                }
            }
        }
        UnityEngine.Object.Destroy(track_main);
        UnityEngine.Object.Destroy(hoarding_main);
        UnityEngine.Object.Destroy(tunnels_main);
    }

    public void setGenerateTrack(TrackBuilder g) { gt = g; }
    public void setInputData(InputData id) { data = id; }
    public void setPath(GameObject path)
    {
        path.transform.parent = rollercoaster.transform;
    }
    public GameObject getMainObject() { return rollercoaster; }
}
