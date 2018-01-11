using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

    Vector3 ImageRotate;//秒針のRotate
    Vector3 startRotate;
    Vector3 endRotate = new Vector3(0,0,-360);
    public Image timerImage;//秒針のImage
    bool isCountDown;//計測中:true 計測終了:false


    // Use this for initialization
    void Start() {
        timerImage.transform.eulerAngles = startRotate; //回転を0にする
    }
	
	// Update is called once per frame
	void Update ()
    {
        //デバッグ用
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isCountDown)
            {
                isCountDown = true;
                StartCoroutine(TimeCounter(30.0f));
            }
        }
    }


    public IEnumerator TimeCounter(float second)
    {
        Debug.Log("計測開始");
        timerImage.rectTransform.eulerAngles= startRotate;
        float time = 0;
        while (time < 1.0f)
        {
            time += Time.deltaTime/second;

            timerImage.rectTransform.eulerAngles = Vector3.Lerp(startRotate, endRotate, time);
            yield return null;
        }
        isCountDown = false;
        Debug.Log("計測終了");
    }
}
