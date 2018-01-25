using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Audio_Data : MonoBehaviour {

    [SerializeField]
    Slider Volume_Slider;//スライダー本体

    Slider Volume;

    static Slider Volume_Mixer;//音量

    public static float AudioSoundData=1;//初期値１

    public AudioSource Mixer;//BGMのAudio

    public AudioSource MixerSE;//SE

    bool sliderOn;

    float span;

    float speed = 1;

    public float volumePoint = 0.1f;
    void Start()
    {
        GetComponent<VolumeProgram>().Position();
        //読み取り
        PlayerPrefs.GetFloat("key", AudioSoundData);
        Volume_Slider.value = AudioSoundData;
        
        //変更を伝える
        this.GetComponent<Slider>().onValueChanged.AddListener(value => this.Mixer.volume = value);
        this.GetComponent<Slider>().onValueChanged.AddListener(value => this.MixerSE.volume = value);

        Volume_Mixer = Volume_Slider;

        sliderOn = false;

        Volume = GetComponent<Slider>();

    }
	void AudioSave()
    {
        //保存
        PlayerPrefs.SetFloat("key",AudioSoundData);
        
    }

    void OnApplicationQuit()
    {
        //ゲームを閉じた時
        AudioSave();
        
    }

    void Update()
    {
        GetComponent<VolumeProgram>().MoveZ();
        sliderOn = !sliderOn;
        if (sliderOn)
        {
            span = Input.GetAxis("Horizontal") * speed;

            if (span > 0)
            {
                speed = 0f;
                Volume.value += volumePoint;
            }
            else if (span < 0)
            {
                speed = 0f;
                Volume.value -= volumePoint;
            }

            if (Input.GetAxis("Horizontal") == 0)
            {
                speed = 1f;
            }
            AudioSoundData = Volume_Mixer.value;
            //保存
            PlayerPrefs.SetFloat("key", AudioSoundData);
            PlayerPrefs.Save();
        }
    }

}
