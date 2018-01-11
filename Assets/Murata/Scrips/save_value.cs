using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class save_value : MonoBehaviour
{
    public Slider slider_bgm;
    public static float value_bgm;//static変数に値を格納
    // Use this for initialization
    void Start()
    {
        value_bgm = slider_bgm.value;
        //スライダーを探す
       // slider_bgm = GameObject.Find("bgm_volume").GetComponent<Slider>();
    }
    void Update()
    {//スライダーの音量(value)を取得して保存
        value_bgm = slider_bgm.value;
    }
}