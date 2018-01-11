using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSlide : MonoBehaviour {

    public Image firstTutorialImage;//一枚目のスライド最初に表示される
    public Image secondTutorialImage;//切り替え用のスライド(スライドするときだけ表示される)

    public Vector2 slideImageSize;//スライドのサイズ
    public bool slideSizeIsScreenSize;//スライドを画面サイズに合わせるのかどうか(trueなら画面サイズに合わせる。)

    public Vector2 slidePosition;//スライドのポジション
    public bool slidePositionIsCenter;//スライドの位置を画面中央に合わせるのかどうか(trueなら自動で画面中央に移動する)

    public Sprite[] slideImageSprite;//スライドに使いたい画像を保存する(０から順番に表示される)
    
    int spriteNum;//表示するスプライトを管理する
    int secondSpriteNum;//二枚目の表示するスプライトを管理する

    bool isSlide;//true：スライド中・false：スライド終了
    // Use this for initialization
    void Start () {
        firstTutorialImage.sprite = slideImageSprite[0];//初期画像を一枚目に設定する
        if (slideSizeIsScreenSize) { slideImageSize = new Vector2(Screen.width, Screen.height); }//画面サイズに合わせる
        if (slidePositionIsCenter) { slidePosition = Vector2.zero; }//移動位置を画面中央に設定
        firstTutorialImage.rectTransform.localPosition = slidePosition;//設定された位置に移動する
        firstTutorialImage.rectTransform.sizeDelta = slideImageSize;//設定されたスライドのサイズに調節する
        secondTutorialImage.rectTransform.sizeDelta = slideImageSize;//二枚目も同様にスライドのサイズを調整する
        isSlide = false;//スライドしていない状態にセットする
    }

    // Update is called once per frame
    void Update () {

        //if (Input.GetKeyDown(KeyCode.Space)) { Debug.Log("呼んだ"); Slide(false); }
        //if (Input.GetKeyDown(KeyCode.LeftArrow)) Slide(false);
        //if (Input.GetKeyDown(KeyCode.RightArrow)) Slide(true);
    }
    /// <summary>
    /// コルーチン呼び出し簡易メソッド
    /// </summary>
    /// <param name="isSecondSlide">次のスライドにするかどうか(前のスライドに戻す場合はfalseに設定する)</param>
    public void Slide(bool isSecondSlide)
    {
        StartCoroutine(FirstPaperSlide(0.5f, isSecondSlide));//スライドコルーチンの呼び出し
    }
    /// <summary>
    /// スライドコルーチン
    /// </summary>
    /// <param name="seconds">何秒でスライドするのか</param>
    /// <param name="isSecondSlide">次のスライドにするかどうか(前のスライドに戻す場合はfalseに設定する)</param>
    /// <returns></returns>
    public IEnumerator FirstPaperSlide(float seconds, bool isSecondSlide)
    {
        //スライドの初期位置と移動後の座標を設定
        Vector2 startSlidePosition = slidePosition;
        Vector2 rightSlidePosition = new Vector2(Screen.width, slidePosition.y);
        Vector2 leftSlidePosition = new Vector2(-Screen.width, slidePosition.y);

        float t = 0;//時間経過

        if (isSlide) yield break;//スライド中であれば中断する
        isSlide = true;//スライド中に設定する

        //Debug.Log("呼んだ");

        if (isSecondSlide)//次のスライドにするなら
        {   
            //次の画像に設定する
            secondSpriteNum = spriteNum + 1;
            //用意した画像より大きくなったら最初の画像に設定する
            if (secondSpriteNum == slideImageSprite.Length) { secondSpriteNum = 0; }
            //画像を変更する
            secondTutorialImage.sprite = slideImageSprite[secondSpriteNum];
        }
        else
        {
            //前の画像に設定する
            secondSpriteNum = spriteNum - 1;
            //0より小さくなったら最後の画像を設定する
            if (secondSpriteNum < 0) { secondSpriteNum = slideImageSprite.Length - 1; }
            //画像を変更する
            secondTutorialImage.sprite = slideImageSprite[secondSpriteNum];
        }
        //スライドのサイズと位置を調整
        firstTutorialImage.rectTransform.sizeDelta = slideImageSize;
        firstTutorialImage.rectTransform.localPosition = new Vector2(0, 0);
        secondTutorialImage.rectTransform.sizeDelta = slideImageSize;

        //Debug.Log("１回目"+isRightSlide);

        while (t < 1)
        {
            t += Time.deltaTime / seconds;//時間経過

            //次のスライドにするのか前のスライドにするのか
            if (isSecondSlide)
            {
                //Lerpでスライドする
                firstTutorialImage.rectTransform.localPosition = Vector2.Lerp(startSlidePosition, leftSlidePosition, t);
                secondTutorialImage.rectTransform.localPosition = Vector2.Lerp(rightSlidePosition, startSlidePosition, t);
            }
            else
            {
                firstTutorialImage.rectTransform.localPosition = Vector2.Lerp(startSlidePosition, rightSlidePosition, t);
                secondTutorialImage.rectTransform.localPosition = Vector2.Lerp(leftSlidePosition, startSlidePosition, t);
            }

            yield return null;
        }

        //Debug.Log("２回目"+isRightSlide);//スペースキーで呼び出すとバグが出る(一回しか呼び出していないはずなのに2回表示される)

        //一枚目のスライドの画像を切り替える
        if (isSecondSlide)
        {
            spriteNum ++;
            if (spriteNum == slideImageSprite.Length) spriteNum = 0;
            firstTutorialImage.sprite = slideImageSprite[spriteNum];
        }
        else
        {
            spriteNum--;
            if (spriteNum < 0) spriteNum = slideImageSprite.Length-1;
            firstTutorialImage.sprite = slideImageSprite[spriteNum];
        }

        //Debug.Log(spriteNum);

        //最終位置を調整する
        firstTutorialImage.rectTransform.localPosition = startSlidePosition;
        secondTutorialImage.rectTransform.localPosition = rightSlidePosition;

        //スライド終了に設定する
        isSlide = false;
    }
}
