using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeProgram : MonoBehaviour {
    public static bool SliderMoveStart = false;//スライダーの最初
    public static bool SlideMove = false;//スライダーの移動
   
    [SerializeField]
    Slider VolumeSlider;//音量のスライダー

    public static Slider Volume;//音量

    [SerializeField]
    GameObject VolumeSliderEnd;//スライダーの移動終了場所

    Vector3 StartPos;//最初の位置

    public static Vector3 startPos;//スライダーの位置

    Vector3 EndPos;//終了の位置

    public static Vector3 endPos;//スライダーの終了の位置

    public static bool BarMove = false;//音量のバー移動
    void Start()
    {
        StartPos = VolumeSlider.GetComponent<RectTransform>().anchoredPosition;//最初の位置を基点にして取得する
        EndPos = VolumeSliderEnd.GetComponent<RectTransform>().anchoredPosition;//最後の位置を基点にして取得する

        startPos = StartPos;//最初の位置に同じにする
        endPos = EndPos;//最後の位置に同じにする

        Volume = VolumeSlider;//変更点を同じにする

    }

    //ボタンを押されたら
    public void VolumeButtonClicked()
    {
        if (!SlideMove)
        {
            SliderMoveStart = true;//スライダー移動
            SlideMove = true;//スライダーが動く

            //移動用コルーチンを実行
            StartCoroutine(SlideMoving());
        }
        
    }

    //音を変える
    public static void VolumeChange()
    {
        if (!SlideMove)
        {
            SliderMoveStart = true;
            SlideMove = true;

        }
    }

    //スライダーが移動
    IEnumerator SlideMoving()
    {
        //移動した割合
        float MoveSpeed = 0.0f;

        if (SliderMoveStart)//最初の位置なら
        {
            
            //Debug.Log("DEBUG");
            RectTransform sliderTransform = VolumeSlider.GetComponent<RectTransform>();//音量スライダーを取得

            while (true)
            {
                //移動の割合を加算
                MoveSpeed += Time.deltaTime * 1.5f;

                if (BarMove)
                {
                    //RecTransformのPositionを移動
                    sliderTransform.anchoredPosition = Vector3.Lerp(EndPos, StartPos, MoveSpeed);

                    if (MoveSpeed > 1.0f)
                    {
                        Debug.Log("True");
                        BarMove = false;

                        SliderMoveStart = false;
                        SlideMove = false;

                        //コルーチンを終了
                        yield break;
                    }
                }
                else
                {
                    sliderTransform.anchoredPosition = Vector3.Lerp(StartPos, EndPos, MoveSpeed);

                    if (MoveSpeed>1.0f)
                    {
                        BarMove = true;

                        SliderMoveStart = false;
                        SlideMove = false;

                        yield break;
                    }
                }

                yield return null;              
            }
        }
    }

    public static IEnumerator SliderMove()
    {
        //移動した割合
        float MoveSpeed = 0.0f;

        if (SliderMoveStart)
        {
            Debug.Log("DEBUG");
            RectTransform sliderTransform = Volume.GetComponent<RectTransform>();

            while (true)
            {
                //移動の割合を加算
                MoveSpeed += Time.deltaTime * 1.5f;

                if (BarMove)
                {
                    //RecTransformのPositionを移動
                    sliderTransform.anchoredPosition = Vector3.Lerp(endPos, startPos, MoveSpeed);

                    if (MoveSpeed > 1.0f)
                    {
                        Debug.Log("True");
                        BarMove = false;

                        SliderMoveStart = false;
                        SlideMove = false;

                        //コルーチンを終了
                        yield break;
                    }
                }
                else
                {
                    sliderTransform.anchoredPosition = Vector3.Lerp(startPos, endPos, MoveSpeed);

                    if (MoveSpeed > 1.0f)
                    {
                        BarMove = true;

                        SliderMoveStart = false;
                        SlideMove = false;

                        yield break;//コルーチンを終了
                    }
                }

                yield return null;//処理を中断し、再開
            }
        }
    }
}
