using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_SceneController : MonoBehaviour
{
    [SerializeField]
    Main_Map map;
    [SerializeField]
    Map_Unit unitPrefab_S;
    [SerializeField]
    Map_Unit unitPrefab_M;
    [SerializeField]
    Map_Unit unitPrefab_T;
    [SerializeField]
    Main_AI enemyAI;

    // マップサイズ
    public static int mapSizeX = 20;
    public static int mapSizeZ = 27;
    
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

        unitPrefab_S.gameObject.SetActive(false);
        unitPrefab_M.gameObject.SetActive(false);
        unitPrefab_T.gameObject.SetActive(false);

        // マップ生成
        map.Generate(mapSizeZ,mapSizeX);
        // GridLayoutによる自動レイアウトで、マスの座標が確定するのをまつ
        yield return null;
        // ユニット配置
        map.PutUnit(2, 2, unitPrefab_M,Map_Unit.Team.Player1);
        map.PutUnit(4, 2, unitPrefab_S, Map_Unit.Team.Player1);
        map.PutUnit(6, 2, unitPrefab_T, Map_Unit.Team.Player1);
        map.PutUnit(2, 6, unitPrefab_S, Map_Unit.Team.Player2);
        map.PutUnit(4, 6, unitPrefab_M, Map_Unit.Team.Player2);
        map.PutUnit(6, 6, unitPrefab_T, Map_Unit.Team.Player2);

        // AI設定
        //map.SetAI(Map_Unit.Team.Player2, enemyAI);

        // ターン開始
        map.StartTurn(Map_Unit.Team.Player1);
    }
	
	// Update is called once per frame
	void Update ()
    {
        
    }
}
