using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_SceneController : MonoBehaviour
{
    [SerializeField]
    Main_Map map;
    [SerializeField]
    Map_Unit unitPrefab;
    [SerializeField]
    Map_Unit unitPrefab_S;
    [SerializeField]
    Map_Unit unitPrefab_P;
    
    // ヒットしたオブジェクト
    GameObject hitObj;

    // マウスの位置
    public Vector3 mousePos;
    // 移動判定
    public static bool rayHit = false;

    // Use this for initialization
    IEnumerator Start ()
    {
        //unitPrefab.gameObject.SetActive(false);

        unitPrefab.gameObject.SetActive(false);
        unitPrefab_S.gameObject.SetActive(false);
        unitPrefab_P.gameObject.SetActive(false);

        // マップ生成
        map.Generate(9, 9);
        // GridLayoutによる自動レイアウトで、マスの座標が確定するのをまつ
        yield return null;
        // ユニット配置
        map.PutUnit(2, 0, unitPrefab);
        map.PutUnit(4, 0, unitPrefab_S);
        map.PutUnit(6, 0, unitPrefab_P);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetMouseButtonDown(0))
        {
            rayHit = false;

            // ワールド座標をスクリーン座標に変換
            mousePos = Vector3.zero;

            // マウスの位置からRayを飛ばす
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit))
            {
                // Rayが当たったオブジェクトの座標を取得
                mousePos = hit.collider.gameObject.transform.position;

                hitObj = hit.collider.gameObject;

                rayHit = true;
            }
            if (hitObj.tag == "Player")
            {
                hitObj.GetComponent<Map_Unit>().OnClick();
            }
        }
    }
}
