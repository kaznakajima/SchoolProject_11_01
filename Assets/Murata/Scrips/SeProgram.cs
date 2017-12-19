using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeProgram : MonoBehaviour {

    [SerializeField]
    AudioClip[] clip;

    public AudioSource SE;
    


    public enum SEType
    {
        //[0]
        ClickSE,

        //[1]
        FillSE,

        //[3]
        StartSE
    }
    void Awake()
    {
       SE = GameObject.Find("SE").GetComponent<AudioSource>();
    }

    public void SEPlay(int seNo)
    {
        SE.PlayOneShot(clip[seNo]);
    }
}
