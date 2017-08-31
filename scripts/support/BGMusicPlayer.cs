using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMusicPlayer : MonoBehaviour {

    AudioSource src;
    int music = -1;

	// Use this for initialization
	void Start () {
        src = gameObject.AddComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Data.music < 0) { src.Stop();music = -1; }
        else if (Data.music != music)
        {
            src.clip = Data.data.backgroundThemes[Data.music];
            music = Data.music;
            src.loop = true;
            src.Play();
        }
	}
}
