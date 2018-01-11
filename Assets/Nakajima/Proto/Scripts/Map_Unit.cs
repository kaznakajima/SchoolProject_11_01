using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[AddComponentMenu("Scripts//Main_Unit")]
public class Map_Unit : MonoBehaviour
{

    public enum Team
    {
        Player1,
        Player2
    }

    public enum UnitType
    {
        Speed,
        Mikoshi,
        Technique
    }

    public int x;
    public int z;
    public int moveAmount = 2;
    public int attackRangeMin;
    public int attackRangeMax;
    public Team team;
    public int attackPowerBase;
    public int unitAtk;

    [SerializeField]
    int lifeMax;
    [SerializeField]
    Main_Map map;

    // ユニットのタイプ
    public UnitType unitType;
    // ユニットの数
    public int myTeamUnit;

    int life;
    bool isFocused = false;
    bool isMoved = false;

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
                gameObject.tag = "Untagged";
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
            Destroy(gameObject);
        });
    }
}
