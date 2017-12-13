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
        var OwnUnits = map.GetOwnUnits().OrderByDescending(x => x.Life);
        var enemyUnits = map.GetEnemyUnits();
        if(OwnUnits.Min(owUnit => enemyUnits.Min(enUnit => Mathf.Abs(owUnit.x - enUnit.x) + Mathf.Abs(owUnit.z - enUnit.z))) <= distance)
        {
            // 敵ユニット(プレイヤー)が指定距離以内に入ったら行動開始
            foreach(var unit in OwnUnits)
            {
                yield return MoveAndAttackCoroutne(unit); 
            }
        }
        yield return new WaitForSeconds(0.5f);
        // 全ての動作が完了したらターン終了
        map.NextTurn();
    }

    IEnumerator MoveAndAttackCoroutne(Map_Unit unit)
    {
        // 移動可能な全てのマスまでの移動コストを取得
        var moveCosts = map.GetMoveCostToAllCells(unit.Cell);

        var attackBaseCells = GetAttackBaseCells(unit).ToList();
        if(attackBaseCells.Count() == 0)
        {
            // 攻撃可能なマスがないなら行動終了
            yield return new WaitForSeconds(0.5f);
            unit.IsMoved = true;
            yield return new WaitForSeconds(0.5f);
            yield break;
        }

        // 攻撃可能なマスのうち、1番近い場所を目標地点にする
        var TargetCell = attackBaseCells.OrderBy(cell => moveCosts.First(cost =>
        {
            return cost.coordinate.x == unit.Cell.X && cost.coordinate.z == unit.Cell.Z;
        }).amount).First();

        // ユニット選択
        unit.OnClick();

        var route = map.CalcurateRouteCoordnatesAndMoveAmount(unit.Cell, TargetCell);
        var movableCells = map.GetMovableCells().ToList();
        if(movableCells.Count == 0)
        {
            yield return AttackIfPossibleCoroutine(unit);
            if (!unit.IsMoved)
            {
                // 行動不能の場合は行動終了
                unit.OnClick();
                yield return new WaitForSeconds(0.5f);
                unit.IsMoved = true;
                yield return new WaitForSeconds(0.5f);
            }
        }
        else
        {
            // 自分のいるマスも移動先の選択肢に含める
            movableCells.Add(unit.Cell);

            var moveTargetCell = movableCells.OrderByDescending(_cell =>
            {
                var matchedRoute = route.FirstOrDefault(_route => _route.coordinate.x == _cell.X && _route.coordinate.z == _cell.Z);
                return (null != matchedRoute ? matchedRoute.amount : 0);
            }).First();

            if(moveTargetCell != unit.Cell)
            {
                yield return new WaitForSeconds(0.5f);
                moveTargetCell.OnClick();
                // 移動完了を待つ
                yield return WaitMoveCoroutine(unit, moveTargetCell);
            }
            yield return AttackIfPossibleCoroutine(unit);
        }
    }

    IEnumerator AttackIfPossibleCoroutine(Map_Unit unit)
    {
        var attackableCells = map.GetAttackableCells();
        if(0 < attackableCells.Length)
        {
            if(Random.Range(0,100) < randomAttack)
            {
                // ランダムで対象を選ぶ
                attackableCells[Random.Range(0, attackableCells.Length)].Unit.OnClick();
            }
            else
            {
                // 攻撃可能なマスのうち、できるだけ倒せる/大ダメージが与えられる/反撃が少ないマスに攻撃
                attackableCells.OrderByDescending(x =>
                {
                    var damageValue = x.Unit.CalcurateDamageValue(unit);
                    return damageValue * (x.Unit.Life <= damageValue ? 10 : 1) - unit.CalcurateDamageValue(x.Unit);
                }).First().Unit.OnClick();
            }
        }
        yield return WaitBattleCoroutine();
    }

    /// <summary>
    /// 移動終了を待つコルーチン
    /// </summary>
    /// <param name="unit">Unit.</param>
    /// <param name="cell">Cell.</param>
    /// <returns>The move croutine.</returns>
    IEnumerator WaitMoveCoroutine(Map_Unit unit,Main_Cell cell)
    {
        while (true)
        {
            if (cell.Unit == unit)
            {
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.5f);
    }

    /// <summary>
    /// Battleが終わるまで待つコルーチン
    /// </summary>
    /// <returns>The battle coroutine.</returns>
    IEnumerator WaitBattleCoroutine()
    {
        while (true)
        {
            // Battleが終わるまで待つ
            yield return new WaitForSeconds(0.1f);
            break;
        }
        yield return new WaitForSeconds(1.0f);
    }
    
    /// <summary>
    /// 敵ユニットに攻撃できるマスを取得させる
    /// </summary>
    /// <param name="unit">Unit.</param>
    /// <returns>The attack base cells.</returns>
    Main_Cell[] GetAttackBaseCells(Map_Unit unit)
    {
        var cells = new List<Main_Cell>();
        foreach(var enemyUnit in map.GetEnemyUnits())
        {
            cells.AddRange(map.GetCellsByDistance(enemyUnit.Cell, unit.attackRangeMin, unit.attackRangeMax).Where(c => c.Cost < 5));
        }
        return cells.Distinct().ToArray();
    }    
}
