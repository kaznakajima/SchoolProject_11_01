using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnShuffler : MonoBehaviour
{

    public Image choutinImage;//色を変化させるImage画像
    int ran;//ランダム用の変数
    bool turnFlg;//trueが赤：falseが青
    bool shuffleFlg;//trueがシャッフルし続ける：falseがシャッフルを中断する
    bool isShuffle;//trueなら
    Color finishColor;//最終的な色
    public float second = 0.15f;//色を何秒で変化させるか
    public float finishTime = 5.0f;//何秒間ターンをシャッフルするか
    int shuffleCounter = 0;//シャッフルが何回行われたかカウントする

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            
            StartCoroutine(TurnShuffle(second,finishTime));
        }

    }
    /// <summary>
    /// ターンをランダムに選ぶ(シャッフルのスタート)
    /// </summary>
    /// <param name="second">色を何秒で変化するのか</param>
    /// <param name="finishTime">何秒間色をシャッフルさせ続けるのか</param>
    /// <returns></returns>
    public IEnumerator TurnShuffle(float second,float finishTime)
    {

        if (!isShuffle)//シャッフル中でないならシャッフルをスタートする
        {
            Debug.Log("シャッフルスタート");
            shuffleFlg = true;//シャッフルし続けるように設定する
            isShuffle = true;//シャッフル中状態に設定する
            ran = Random.Range(1, 100);//ランダムでターンを設定する
            if (ran % 2 == 0)//偶数であれば開始ターンを赤プレイヤーにする
            {
                finishColor = new Color(255, 0, 0, 1);//最終的な色を赤色にする
                turnFlg = true;//開始ターンを赤プレイヤーにする
                Debug.Log("赤");
            }
            else//奇数であれば開始ターンを青プレイヤーにする
            {
                finishColor = new Color(0, 0, 255, 1);//最終的な色を青色にする
                turnFlg = false;//開始ターンを青プレイヤーにする
                Debug.Log("青");
            }
            StartCoroutine(ColorFade(second));//シャッフルを開始する
            　　　　　　　　　　　　　　　　//(開始色を変えないことで開始ターンがわからないようにするため)
            yield return new WaitForSeconds(finishTime);//設定秒後にシャッフルを中断する
            shuffleFlg = false;//シャッフルを中断する
            isShuffle = false;//シャッフル終了状態に設定する
        }
    }
    /// <summary>
    /// 赤色をフェードさせる
    /// </summary>
    /// <param name="second">色を何秒でフェードさせるのか</param>
    /// <returns></returns>
    public IEnumerator ColorFade(float second)
    {
        Color startColor;
        Color endColor;
        float t = 0;//時間をリセット
        if (shuffleCounter % 2 == 0)
        {
            startColor = new Color(255, 0, 0, 0.4f);//開始色
            endColor = new Color(255, 0, 0, 1);//目的色
        }
        else
        {
            startColor = new Color(0, 0, 255, 0.4f);//開始色
            endColor = new Color(0, 0, 255, 1);//目的色
        }
        while (t < 1)
        {
            t += Time.deltaTime / second;//経過時間の計算
            choutinImage.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }
        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / second;
            choutinImage.color = Color.Lerp(endColor, startColor, t);
            yield return null;
        }
        shuffleCounter++;
        if (shuffleFlg) StartCoroutine(ColorFade(second));
        else {
            t = 0;
            startColor = Color.white;
            while (t < 1)
            {
                t += Time.deltaTime / second;
                choutinImage.color = Color.Lerp(startColor,finishColor, t);
                yield return null;
            }
        }
    }
}
