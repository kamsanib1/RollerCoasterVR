using UnityEngine;
using System.Collections;
using Valve.VR;

public class ViveMenu : MonoBehaviour
{
    public GameObject player;

    SteamVR_Controller.Device device;
    SteamVR_TrackedObject controller;

    Vector2 touchpad;

    private float sensitivityX = 1.5F;
    private Vector3 playerPos;

    void Start()
    {
        controller = gameObject.GetComponent<SteamVR_TrackedObject>();
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}