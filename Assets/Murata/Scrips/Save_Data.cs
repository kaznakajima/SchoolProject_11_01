using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Save_Data : MonoBehaviour {

    //現在存在しているオブジェクト実体の記憶領域
    static Save_Data _instance = null;

    //オブジェクト実体の参照（初期参照時、実体の登録も行う）
    static Save_Data instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<Save_Data>()); }
    }

    private void Awake()
    {
        //※オブジェクトが重複していたらここで破壊される

        //自身がインスタンスでなければ自滅
        if(this != instance)
        {
            Destroy(gameObject);
            return;
        }

        //以降破棄しない
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        //※破棄時に、登録した実体の解除を行っている

        //自身がインスタンスなら登録を解除
        if (this == instance) _instance = null;
    }
}
