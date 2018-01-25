using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSelectProgram : MonoBehaviour
{
    TutorialSlide ts;//スライドスクリプトの読み込み
    SceneFader sf;//フェードスクリプトの読み込み
    public Image[] ButtonImage;//ボタンのイメージを保存する
    Color unselectedColor = Color.white;//非選択色
    Color selectColor = new Color(255.0f / 255.0f, 127 / 255.0f, 127 / 255.0f, 255 / 255.0f);//選択色

    bool isButtonChange;//選択ボタンの変更中かどうか

    public float cursorPosition;//選択中のボタンの位置
    float speed = 1;//コントローラーのアナログスティックの入力速度
    int buttonNum;//選択中のボタンが何番目なのか保存する
    int oldButtonNum;//ひとつ前に選択していたボタンを保存する

    public string thisSceneName;

    public SeProgram seProgram;//SEの設定

    void Start()
    {
        ts = FindObjectOfType<TutorialSlide>();//スライドプログラムのメソッドを読み込む
        sf = FindObjectOfType<SceneFader>();//フェードプログラムのメソッドを読み込む
        ButtonImage[0].color = selectColor;//最初に選択しているボタンの色を変える
        isButtonChange = false;//ボタンの変更後に設定する
    }

    void Update()
    {
        if (!isButtonChange)//変更中でないなら
        {
            cursorPosition = Input.GetAxis("Horizontal") * speed;//アナログスティックの入力をfloat型に代入

            //右に入力があったなら
            if (cursorPosition > 0)
            {
                //現在選択中のボタンを保存しておく
                oldButtonNum = buttonNum;
                //右のボタンを選択中のボタンに設定
                buttonNum++;
                //保存してあるボタンの数を超えたら
                if (buttonNum == ButtonImage.Length)
                {
                    buttonNum = 0;//初めのボタンに戻る
                }
                //ボタンの色を変更するコルーチンの呼び出し
                StartCoroutine(ButtoneEffect());
                //アナログスティックの入力を消す
                speed = 0;
            }
            else if (cursorPosition < 0)
            {
                oldButtonNum = buttonNum;
                buttonNum--;
                if (buttonNum < 0)
                {
                    buttonNum = ButtonImage.Length - 1;
                }
                StartCoroutine(ButtoneEffect());
                speed = 0;
            }
            //アナログスティックの入力がないならspeedを戻す
            if (Input.GetAxis("Horizontal") == 0) { speed = 1; }
        }
        //選択しているボタンを決定する
        if (Input.GetKeyDown(KeyCode.P)) { ButtonSelector(); }
    }
    /// <summary>
    /// ボタンの色を調整する
    /// </summary>
    /// <returns></returns>
    public IEnumerator ButtoneEffect()
    {
        isButtonChange = true;//ボタンの色を変更開始
        float seconds = 0.25f;//0.25秒で色を変える
        float t = 0;//時間をリセット

        //設定した時間になるまで色を変化させる
        while (t < 1)
        {
            //次のボタンの色を選択色にする
            ButtonImage[buttonNum].color = Color.Lerp(unselectedColor, selectColor, t);
            //前のボタンを非選択色にする
            ButtonImage[oldButtonNum].color = Color.Lerp(selectColor, unselectedColor, t);
            //時間経過
            t += Time.deltaTime / seconds;

            yield return null;
        }
        //ボタンを変更完了
        isButtonChange = false;
    }
    /// <summary>
    /// 選択中のボタンを変更する
    /// </summary>
    void ButtonSelector()
    {
        seProgram.SEPlay(0);//太鼓
        switch (thisSceneName)
        {
            case "Title":
                switch (buttonNum)
                {
                    case 0:
                        sf.StageSelect("Tutorial");//フェードメソッドの呼び出し
                        break;
                    case 1:
                        sf.StageSelect("Tutorial");//フェードメソッドの呼び出し
                        break;
                    case 2:
                        sf.StageSelect("Game");//フェードメソッドの呼び出し
                        break;
                    case 3:
                        sf.StageSelect("Game");//フェードメソッドの呼び出し
                        break;
                }
                break;
            case "Tutorial":
                switch (buttonNum)
                {
                    case 0:
                        sf.StageSelect("Title");//フェードメソッドの呼び出し
                        break;
                    case 1:
                        ts.Slide(true);//簡易スライドメソッドの呼び出し
                        break;
                    case 2:
                        ts.Slide(false);
                        break;
                    case 3:
                        sf.StageSelect("Game");//フェードメソッドの呼び出し
                        break;
                }
                break;
            case "Game":
                switch (buttonNum)
                {
                    case 0:
                        sf.StageSelect("Game");//フェードメソッドの呼び出し
                        break;
                    case 1:
                        sf.StageSelect("Title");//フェードメソッドの呼び出し
                        break;
                }
                break;
        }

    }
}