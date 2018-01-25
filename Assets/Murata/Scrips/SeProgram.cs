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
        ButtonSE,

        //[1]
        StartSE,

        //[2]
        Turn_ChengeSE,

        //[3]
        Mikoshi_WalkSE,

        //[4]
        Mikoshi_AttackSE,

        //[5]
        Mikoshi_DamegeSE,

        //[6]
        Unit_WalkSE,

        //[7]
        Unit_AttackSE,

        //[8]
        Unit_DamegeSE,

        //[9]
        Item_GetSE,

        //[10]
        Gage_UpSE,

        //[11]
        Game_SetSE
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
