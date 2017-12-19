using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class bgm_volumecontrol : MonoBehaviour
{
    Slider b_slider;
    [SerializeField]
    UnityEngine.Audio.AudioMixer mixer;
    void Start()
    {//value_saverに保管されている値を持ってくる
        GetComponent<save_value>();
        b_slider = GetComponent<Slider>();
        b_slider.value = save_value.value_bgm;
    }

    public float masterBgmVolume
    {
        set { mixer.SetFloat("MasterBgmVolume", value); }
    }
}