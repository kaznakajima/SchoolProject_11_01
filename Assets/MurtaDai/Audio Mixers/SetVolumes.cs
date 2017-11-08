using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetVolumes : MonoBehaviour {

    [SerializeField]
    AudioClip[] Clip;//オーディオデータ
    public AudioSource Se;

	public enum SeType
    {
        //[0]
        ClickSe,//クリックSe
        //[1]
        AttacSe,//攻撃Se
        //[2]
        ExplosionSe//爆発Se
    }
    private void Awake()
    {
        Se = GameObject.Find("SE").GetComponent<AudioSource>();//名前を探し取得
    }

    public void SePlay(int SeNo)
    {
        Se.PlayOneShot(Clip[SeNo]);//再生されているクリップ
    }
}