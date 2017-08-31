using UnityEngine;
using System.Collections;
public class SendBreakeMenssage : MonoBehaviour {

	public HairyCopter helicopterScript;
	public AudioSource audSource;

	void OnTriggerEnter (Collider col)
	{
		helicopterScript.Breaked ();
		if(audSource.isPlaying == false)
		{
			audSource.Play ();
		}
	}
}
