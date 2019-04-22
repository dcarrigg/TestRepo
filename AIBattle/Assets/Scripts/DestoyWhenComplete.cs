using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoyWhenComplete : MonoBehaviour {

    public AudioSource source;
	// Use this for initialization
	void Start () {
        source.pitch = Random.Range(0.7f, 1.3f);
    }
	
	// Update is called once per frame
	void Update () {
		if (!source.isPlaying)
        {
            Destroy(gameObject);
        }
	}
}
