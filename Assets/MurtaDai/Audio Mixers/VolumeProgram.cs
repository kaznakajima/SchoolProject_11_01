using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeProgram : MonoBehaviour
{
   // public static bool SliderMoveSteart = false;
    //public static bool SlideMove = false;

    [SerializeField]
    Slider VolumeSlider;

    public static Slider Volume;

    [SerializeField]
    GameObject VolumeSliderEnd;

    Vector3 StartPos;

    public static Vector3 startPos;

    Vector3 EndPos;

    public static Vector3 endPos;

    public static bool BarMove = false;

    void Start()
    {
        StartPos = VolumeSlider.GetComponent<RectTransform>().anchoredPosition;
        EndPos = VolumeSliderEnd.GetComponent<RectTransform>().anchoredPosition;

        startPos = StartPos;
        endPos = EndPos;
    }

    public void VolumeButtonClicked()
    {
        //if (!SlideMove)
        //{
        //    SliderMoveSteart = true;
        //    SlideMove = true;

        //    //移動用コルーチンを実行
        //    StartCoroutine(SlideMoving());
        //}

    }

    public static void VolumeChange()
    {
        //if (!SlideMove)
        //{
        //    SliderMoveSteart = true;
        //    SlideMove = true;

        //    //移動用コルーチンを実行
        //   // GameObject.Find("Map").GetComponent<BoardProgram>().StartCoroutine(SliderMove());
        //}
    }

    //IEnumerator SlideMoving()
    //{
    //    //移動した割合
    //    float MoveSpeed = 0.0f;

    //    if (SliderMoveSteart)
    //    {


    //        //Debug.Log("DEBUG");
    //        RectTransform sliderTransform = VolumeSlider.GetComponent<RectTransform>();

    //        while (true)
    //        {
    //            //移動の割合を加算
    //            MoveSpeed += Time.deltaTime * 1.5f;

    //            if (BarMove)
    //            {
    //                //RecTransformのPositionを移動
    //                sliderTransform.anchoredPosition = Vector3.Lerp(EndPos, StartPos, MoveSpeed);

    //                if (MoveSpeed > 1.0f)
    //                {
    //                    Debug.Log("True");
    //                    BarMove = false;

    //                    SliderMoveSteart = false;
    //                    SlideMove = false;

    //                    //コルーチンを終了
    //                    yield break;
    //                }
    //            }
    //            else
    //            {
    //                sliderTransform.anchoredPosition = Vector3.Lerp(StartPos, EndPos, MoveSpeed);

    //                if (MoveSpeed > 1.0f)
    //                {
    //                    BarMove = true;

    //                    SliderMoveSteart = false;
    //                    SlideMove = false;

    //                    yield break;
    //                }
    //            }

    //            yield return null;
    //        }
    //    }
    //}

    //public static IEnumerator SliderMove()
    //{
    //    //移動した割合
    //    float MoveSpeed = 0.0f;

    //    if (SliderMoveSteart)
    //    {


    //        Debug.Log("DEBUG");
    //        RectTransform sliderTransform = Volume.GetComponent<RectTransform>();

    //        while (true)
    //        {
    //            //移動の割合を加算
    //            MoveSpeed += Time.deltaTime * 1.5f;

    //            if (BarMove)
    //            {
    //                //RecTransformのPositionを移動
    //                sliderTransform.anchoredPosition = Vector3.Lerp(endPos, startPos, MoveSpeed);

    //                if (MoveSpeed > 1.0f)
    //                {
    //                    Debug.Log("True");
    //                    BarMove = false;

    //                    SliderMoveSteart = false;
    //                    SlideMove = false;

    //                //    Player1.Volumeflg = false;

    //                    //コルーチンを終了
    //                    yield break;
    //                }
    //            }
    //            else
    //            {
    //                sliderTransform.anchoredPosition = Vector3.Lerp(startPos, endPos, MoveSpeed);

    //                if (MoveSpeed > 1.0f)
    //                {
    //                    BarMove = true;

    //                    SliderMoveSteart = false;
    //                    SlideMove = false;

    //                //    Player1.Volumeflg = true;

    //                    yield break;
    //                }
    //            }

    //            yield return null;
    //        }
    //    }
    //}
}
