using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeController : MonoBehaviour {

    //BGM・SE
    private AudioSource[] sources;//AudioSourceの配列

	void Start ()
    {//start時に、gameObject[bgm_se]のコンポネント(AudioSource3つ)を取って来る
        //BGM
        sources = gameObject.GetComponents<AudioSource>();
	}

	void Update () {
        //配列渡す
        if (Input.GetKey(KeyCode.Space))
        {
            sources[0].Play();
        }
        if (Input.GetKey(KeyCode.Space))
        {
            sources[1].Play();
        }
        if (Input.GetKey(KeyCode.Space))
        {
            sources[2].Play();
        }
    }
}
