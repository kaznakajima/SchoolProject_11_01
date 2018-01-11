using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    /////////////////////////////////////////////////////////
    // このscriptはシーン上のCanvasにアタッチしてください。//
    /////////////////////////////////////////////////////////

    float red, green, blue; //RGBを操作するための変数
    public static string next_Scene;
    public static bool isFade = false;//フェード中かどうか

    public float fadeTime = 1;

    int fade;//FadeIn：１ FadeOut：０

    Color fadeColor;

    void Start()
    {
       fade = 0;
        StartCoroutine(SceneFade(fadeTime, false));
    }

    /// <summary>
    /// 引数のシーンにフェードして遷移する
    /// </summary>
    /// <param name="nextSceneName">遷移先のシーン名</param>
    public void StageSelect(string nextSceneName)
    {
        if (!isFade)
        {
            isFade = true;
            next_Scene = nextSceneName;
            fade = 1;
            StartCoroutine(SceneFade(fadeTime,true));
        }
    }

    /// <summary>
    /// フェードだけする
    /// </summary>
    public void OnFade(float time)
    {
        if (!isFade)
        {
            isFade = true;
            fade = 1;
            StartCoroutine(SceneFade(time, false));
        }
    }

    //フェード用のGUI
    void OnGUI()
    {   
        GUI.color = fadeColor;
        GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),Texture2D.whiteTexture);
    }

    /// <summary>
    /// フェード用コルーチン
    /// </summary>
    /// <param name="second">何秒かけてフェードさせるのか</param>
    /// <param name="sceneChange">シーン遷移させるのかどうか(true:させる　false:させない)</param>
    /// <returns></returns>
    public IEnumerator SceneFade(float second,bool sceneChange)
    {
        

        float t = 0.0f;
        switch (fade)
        {
            case 1:
                Debug.Log("フェードイン");
                while (t < 1.0f)
                {
                    t += Time.deltaTime / second;
                    fadeColor.a = Mathf.Lerp(0.0f, 1.0f, t);
                    yield return null;
                }
                if (sceneChange) { SceneManager.LoadScene(next_Scene); }
                else
                {
                    fade = 0;
                    StartCoroutine(SceneFade(1.5f, false));
                }
                break;
            case 0:
                while (t < 1.0f)
                {
                    t += Time.deltaTime / second;
                    fadeColor.a = Mathf.Lerp(1.0f, 0.0f, t);
                    yield return null;
                }
                isFade = false;
                break;
        }
    }
}