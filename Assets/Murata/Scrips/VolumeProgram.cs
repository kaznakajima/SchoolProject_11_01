using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeProgram : MonoBehaviour {

    public static bool SliderMoveStart = false;//スライダーの最初
    public static bool SlideMove = false;//スライダーの移動

    [SerializeField]
    Slider VolumeSlider;//スライダー本体
    [SerializeField]
    GameObject VolumeSliderEnd;//スライダーの移動終了場所

    Vector3 StartPos;//最初の位置
    public static Vector3 startPos;//スライダーの位置
    Vector3 EndPos;//終了の位置
    public static Vector3 endPos;//スライダーの終了の位置
    public static bool BarMove = false;//音量のバー移動可能かどうか

    public void Position()
    {
        //最初の位置を基点にして取得する
        StartPos = VolumeSlider.GetComponent<RectTransform>().anchoredPosition;
        //最後の位置を基点にして取得する
        EndPos = VolumeSliderEnd.GetComponent<RectTransform>().anchoredPosition;
        startPos = StartPos;//最初の位置に同じにする
        endPos = EndPos;//最後の位置に同じにする

    }

    //ボタンを押されたら
    public void VolumeButtonClicked()
    {
        if (!SlideMove)
        {
            SliderMoveStart = true;//スライダー本体移動
            SlideMove = true;//スライダーも動く

            //移動用コルーチンを実行
            StartCoroutine(SlideMoving());
        }
        
    }
    
    //スライダー本体が移動
    IEnumerator SlideMoving()
    {
        //移動した割合
        float MoveSpeed = 0.0f;

        //最初の位置なら
        if (SliderMoveStart)
        {
            //スライダー本体を取得
            RectTransform sliderTransform = VolumeSlider.GetComponent<RectTransform>();
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


     public void MoveZ()
    {
        //zとボタンを押したらスライド本体が移動する
        if (Input.GetKeyDown(KeyCode.Z))
        {
            VolumeButtonClicked();
        }
    }

}
