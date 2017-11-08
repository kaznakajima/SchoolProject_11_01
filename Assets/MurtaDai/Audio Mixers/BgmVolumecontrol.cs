using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BgmVolumecontrol : MonoBehaviour {

    Slider Slider;
    [SerializeField]
    UnityEngine.Audio.AudioMixer mixer;

	void Start ()
    {
        GetComponent<SaveValue>();
        Slider = GetComponent<Slider>();
        Slider.value = SaveValue.ValueBgm;
	}
	
    public float BgmVolume
    {
        set { mixer.SetFloat("BgmVolume", value); }
    }
}
