using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TimeScript : MonoBehaviour {
    //時間のテキスト
    public float Time = 60;
    //次のシーンへ
    public GameObject ExchangeButton;
    //ゲームオーバーのテキスト
    public GameObject GameOverText;
     

    void Start()
    {
        //消しておく
        GameOverText.SetActive(false);
        //消しておく
        ExchangeButton.SetActive(false);

        GetComponent<Text>().text = ((int)Time).ToString();
    }

    void Update()
    {
        //カウントダウン
        Time -= UnityEngine.Time.deltaTime;
        //0以下になっら
        if (Time < 0)
        {
            //文字表示
            StartCoroutine("GameOver");
        }

        if (Time < 0) Time = 0;
        GetComponent<Text>().text = ((int)Time).ToString();
    }
    IEnumerator GameOver()
    {
        //出現
        ExchangeButton.SetActive(true);
        //出現
        GameOverText.SetActive(true);
        ExchangeButton.GetComponent<Button>().interactable = false;
        yield return new WaitForSeconds(2.0f);
        //ボタンを押されたらシーンに移動
        if (Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene("Title");
        }
    }
}
