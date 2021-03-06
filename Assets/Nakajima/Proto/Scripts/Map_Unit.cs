﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Map_Unit : MonoBehaviour
{

    // チーム分け
    public enum Team
    {
        Player1,
        Player2
    }

    // ユニットの種類
    public enum UnitType
    {
        Speed,
        Mikoshi,
        Technique
    }

    // ユニットの座標
    public int x;
    public int z;
    
    // ユニットの移動距離
    public int moveAmount = 2;
    
    // ユニットの攻撃範囲
    public int attackRangeMin;
    public int attackRangeMax;
    
    // ユニットのチーム
    public Team team;

    // ユニットの攻撃力
    public int attackPowerBase;
    public int unitAtk;

    // ユニットの色分け
    [SerializeField]
    SkinnedMeshRenderer unitRen;
    [SerializeField]
    Material M_unit;

    // ユニット攻撃の目印
    public GameObject attackPoint;

    // 最大HP
    [SerializeField]
    int lifeMax;
    [SerializeField]
    Main_Map map;

    // ユニットのタイプ
    public UnitType unitType;
    // ユニットの数
    public int myTeamUnit;

    // 現在のHP
    int life;

    // ユニットの選択中かの判断
    bool isFocused = false;
    // ユニットが動き終わったかどうか
    public bool isMoved = false;

    public int LifeMax { get { return lifeMax; } }

    public int Life { get { return life; } }

    public int AttackPower { get { return Mathf.RoundToInt(attackPowerBase + unitAtk * (Mathf.Ceil((float)life / (float)lifeMax * 10.0f) / 10.0f)); } }

    // ユニットが選択状態かどうか
    public bool IsFocused { get { return isFocused;} set { isFocused = value; } }

    // ユニットのいるマスの座標
    public Main_Cell Cell { get { return map.GetCell(x, z); } }

    // ユニットが動いたかどうか
    public bool IsMoved
    {
        get { return isMoved; }
        set
        {
            isMoved = value;
            if (isMoved && IsFocused)
            {
                OnClick();
            }
        }
    }

    void Start()
    {
        // タグ設定
        switch (team)
        {
            case Team.Player1:
                gameObject.tag = "Player";
                break;
            case Team.Player2:
                if(unitType != UnitType.Mikoshi)
                {
                    unitRen.material = M_unit;
                }
                break;
        }

        life = lifeMax;
    }

    void Update()
    {
        
    }

    public void OnClick()
    {

        //攻撃対象の選択中なら攻撃実行
        if (map.GetCell(x, z).IsAttackable)
        {
            map.AttackTo(map.ActiveUnit, this);

            return;
        }


        if (isMoved == false)
        {

            // 自分以外が選択状態なら解除
            if (map.ActiveUnit != null && map.ActiveUnit != this)
            {
                map.ActiveUnit.isFocused = false;
                map.ClearHighLight();
            }

            isFocused = !isFocused;
            if (isFocused)
            {
                map.HighlightMovableCells(x, z, moveAmount);
                map.HighlightAttackableCells(x, z, attackRangeMin, attackRangeMax);
            }
            else
            {
                //map.ResetMovableCells();
                map.ClearHighLight();
            }
        }
    }

    public void Cancel()
    {
        isFocused = false;
        map.ClearHighLight();
    }

    public void Damage(Map_Unit attacker,Map_Unit defender)
    {
        // 三つ巴ダメージ計算
        var unitTypeBonus = new float[] { 1.0f, 2.0f, 0.5f }[(((int)attacker.unitType - (int)unitType) + 3) % 3];
        int damage = Mathf.RoundToInt(attacker.AttackPower * unitTypeBonus);
        life = Mathf.Max(0, life - damage);

        Debug.LogFormat("攻撃" + attacker.unitType + " 防御"+ defender.unitType + "," + "ダメージ" + damage);
    }

    public int CalcurateDamageValue(Map_Unit attacker)
    {
        // 三つ巴ダメージ実装
        var unitTypeBouns = new float[] { 1.0f, 2.0f, 0.5f }[(((int)attacker.unitType - (int)unitType) + 3) % 3];
        int damage = Mathf.RoundToInt(attacker.AttackPower * unitTypeBouns);
        return damage;
    }

    public void DestroyAnimate()
    {
        transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
        {
            switch (team)
            {
                case Team.Player1:
                    map.myUnitOriginNum--;
                    break;
                case Team.Player2:
                    map.eUnitOriginNum--;
                    break;
            }

            map.GetCell(x, z).cost = 1;
            map.GetCell(x, z).IsAttackable = false;

            Destroy(gameObject);
        });
    }
}
