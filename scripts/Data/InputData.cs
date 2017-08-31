using UnityEngine;
using System.Collections;

public class InputData : MonoBehaviour {
    [Header("GameObjects")]
    public GameObject bar;                  //track prefab.
    public GameObject steel_bar;                  //track prefab.
    public GameObject fence_pole;
    public GameObject railing;              //rail prefab.*no longer used.
    public GameObject train_normal;         //trin runs without unity physics.
    public GameObject train_cam;
    public GameObject train_vr;             //train prefab runs with unity physics.
    public GameObject hoarding;
    public GameObject flag_prefab;
    public GameObject trigger_prefab;
    public GameObject[] objects;
    public RCFile[] animals;
    public RCFile[] trains;
    public RCFile[] walls;
    public RCFile[] misc;
    public RCFile[] monsters;
    public RCFile[] human;
    public RCFile[] architecture;
    public RCFile[] transport;
    public RCFile[] plant;
    public RCFile[] flower;
    public RCFile[] trees;
    public RCFile[] houses;
    public RCFile[] birds;
    public RCFile[] reptiles;
    public RCFile[] Robot;
    public RCFile[] balloons;
    public RCFile[] robotpuppies;
    public RCFile[] zombie;
    public RCFile[] snowman;
    //public File[] 
    public GameObject[] graphObjects;
    public GameObject[] inventory;
    [Header("Material")]
    public Material rail_material;          //material used for rails.
    public Material railing_mat;
    public Material steel_coaster_mat;
    public Material motor_mat;
    public Material tunnel_mat;
    public Material[] hoarding_mats;
    public PhysicMaterial phy_mat;          //physics material for rail mesh on whick track runs.
    public Material wall_mat;
    public Material fence_mat;
    [Header("audio clips")]
    public AudioObject[] sounds;
    public AudioClip clip;                  //train background audio clip.
    public AudioClip air_background;
    public AudioClip[] backgroundThemes;
    public AudioClip[] object_voice;
    public AudioClip[] gun_sound;
    public GameObject[] test;
    [Header("track settings")]
    public int speed_points = 24;           //number of points between two tracks.
    public float points_per_track = 24;     //number of points between two tracks to generate rail mesh.
    public float spacing = .5f;             //space between the tracks.
    public int hoarding_gap = 10;
    [Header("GUI Images")]
    public Texture2D[] animal_icons;
    public Texture backgroundImg;
    [Header("animation settings")]
    public float train_velocity = 100;      //initial velocity of train.
    public float vel_multiplier = 32f;       //velocity multiplier
    public float factor = 0;
    public float friction = 0.0005f;
    public float mass = 4500;
    public float drop_vel = 5;
    public bool physics_animator = false;   //flag to decide the type of train animator. true - uses unity physics
    public float thrust = 20;               //speed in forward direction for force script.
    public bool coaster;
    public Light main_light;
    [Header("unnecessary")]
    //public Animator_b animator;
    public string output = "";
    //public PauseMenu pmenu;

    // Use this for initialization
    void Start () {
        Data.data = this;
        Data.init();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
