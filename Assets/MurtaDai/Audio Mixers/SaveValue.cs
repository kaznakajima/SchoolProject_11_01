using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveValue : MonoBehaviour {

    public Slider SliderBgm;
    public static float ValueBgm;

    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //スライダーの音量（value）を取得して保存
        ValueBgm = SliderBgm.value;
	}
}
