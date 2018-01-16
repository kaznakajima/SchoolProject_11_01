using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hide : MonoBehaviour {
    bool hideFlg = true;
    public GameObject[] hideObject;
    public enum hideObjectName
    {
        SV
    }

    // Use this for initialization
    void Start() {
        

    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Hide(hideObject[(int)hideObjectName.SV],false);
        }
        //if (Input.GetKeyDown(KeyCode.I))
        //{
        //    Hide("SV2");
        //}

    }
    public void Hide(GameObject hideObject,bool hide) {
        hideFlg = hide;
        hideObject.SetActive(hideFlg);
    }
            
}
