using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Main_AI : MonoBehaviour
{
    // 攻撃選択のランダム性
    [SerializeField, Range(0, 100)]
    int randomAttack = 50;

    // 検知距離(これ以上近づいたら攻撃)
    [SerializeField, Range(0, 100)]
    int distance = 4;

    Main_Map map;

    public void Initialize(Main_Map map)
    {
        this.map = map;
    }

    public void Move()
    {
        StartCoroutine(MoveCoroutine());
    }

    IEnumerator MoveCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        // 行動可能なユニット取得
        //var ownUnits = 
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
