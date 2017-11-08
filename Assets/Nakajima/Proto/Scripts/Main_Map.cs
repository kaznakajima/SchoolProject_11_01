using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using DG.Tweening;

public class Main_Map : MonoBehaviour
{
    [SerializeField]
    Main_Cell cellFieldPrefab;
    [SerializeField]
    Main_Cell cellForestPrefab;
    [SerializeField]
    Main_Cell cellRockPrefab;
    [SerializeField]
    Transform unitContainer;
    [SerializeField]
    GameObject Battle;

    List<Main_Cell> cells = new List<Main_Cell>();

    /// <summary>
    /// 選択中のユニットを返します
    /// </summary>
    /// <value>The active unit.</value>
    public Map_Unit ActiveUnit
    {
        get { return unitContainer.GetComponentsInChildren<Map_Unit>().FirstOrDefault(x => x.IsFocused); }
    }

    void Start()
    {
        foreach (var prefab in new Main_Cell[] { cellFieldPrefab, cellForestPrefab, cellRockPrefab })
        {
            prefab.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// マップを生成します
    /// </summary>
    /// <param name="width">Width.</param>
    /// <param name="height">Height.</param>
    public void Generate(int width, int height)
    {
        //foreach (var cell in cells)
        //{
        //    Destroy(cell.gameObject);
        //}

        Main_Cell cell;

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                int rand = UnityEngine.Random.Range(0, 10);
                if (rand == 0)
                {
                    cell = Instantiate(cellRockPrefab);
                }
                else if (rand <= 2)
                {
                    cell = Instantiate(cellForestPrefab);
                }
                else
                {
                    cell = Instantiate(cellFieldPrefab);
                }
                cell.gameObject.SetActive(true);
                cell.transform.SetParent(transform);
                cell.gameObject.transform.position = new Vector3(x, -0.5f, z);
                cell.SetCoordinate(x,z);
                cells.Add(cell);

            }
        }
    }

    /// <summary>
    /// 任意のマスの取得
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public Main_Cell GetCell(int x,int z)
    {
        return cells.First(c => c.X == x && c.Z == z);
    }

    /// <summary>
    /// 移動可能なマスをハイライトします
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="moveAmount">Move amount.</param>
    public void HighlightMovableCells(int x, int z, int moveAmount)
    {
        //ResetMovableCells();
        var startCell = cells.First(c => c.X == x && c.Z == z);
        foreach (var info in GetRemainingMoveAmountInfos(startCell, moveAmount))
        {
            var cell = cells.First(c => c.X == info.coordinate.x && c.Z == info.coordinate.z);
            if (cell.Unit == null)
            {
                cells.First(c => c.X == info.coordinate.x && c.Z == info.coordinate.z).IsMovable = true;
            }
        }
    }

    /// <summary>
    /// 攻撃可能なマスをハイライトする
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="attackRangeMin"></param>
    /// <param name="attackRangeMax"></param>
    /// <returns></returns>
    public bool HighlightAttackableCells(int x,int z,int attackRangeMin,int attackRangeMax)
    {
        var startCell = cells.First(c => c.X == x && c.Z == z);
        var targetinfo = GetRemainingAccountRangeInfos(startCell, attackRangeMin, attackRangeMax).Where(i =>
          {
              var cell = cells.First(c => i.coordinate.x == c.X && i.coordinate.z == c.Z);
              return null != cell.Unit;
          });
        foreach(var info in targetinfo)
        {
            var cell = cells.First(c => c.X == info.coordinate.x && c.Z == info.coordinate.z);
            cell.IsAttackable = true;
        }
        return targetinfo.Count() > 0;
    }

    /// <summary>
    /// ハイライトのリセット
    /// </summary>
    public void ClearHighLight()
    {
        foreach (var cell in cells)
        {
            if (cell.IsAttackable)
            {
                
            }
            cell.IsMovable = false;
        }
    }

    /// <summary>
    /// 移動可能なマスのハイライトを消します
    /// </summary>
    public void ResetMovableCells()
    {
        foreach (var cell in cells.Where(c => c.IsMovable))
        {
            cell.IsMovable = false;
        }
    }



    /// <summary>
    /// 移動経路となるマスを返す
    /// </summary>
    /// <returns>The route cells.</returns>
    /// <param name="startCell">Start cell.</param>
    /// <param name="moveAmount">Move amount.</param>
    /// <param name="endCell">End cell.</param>
    public Main_Cell[] CalculateRouteCells(int x, int z, int moveAmount, Main_Cell endCell)
    {
        var startCell = cells.First(c => c.X == x && c.Z == z);
        var infos = GetRemainingMoveAmountInfos(startCell, moveAmount);
        if (!infos.Any(info => info.coordinate.x == endCell.X && info.coordinate.z == endCell.Z))
        {
            throw new ArgumentException(string.Format("endCell(x:{0}, y:{1}) is not movable.", endCell.X, endCell.Z));
        }

        //var endCellMoveAmountInfo = infos.First(info => info.coordinate.x == endCell.X && info.coordinate.z == endCell.Z);
        var routeCells = new List<Main_Cell>();
        routeCells.Add(endCell);
        while (true)
        {
            var currentCellInfo = infos.First(info => info.coordinate.x == routeCells[routeCells.Count - 1].X && info.coordinate.z == routeCells[routeCells.Count - 1].Z);
            var currentCell = cells.First(cell => cell.X == currentCellInfo.coordinate.x && cell.Z == currentCellInfo.coordinate.z);
            var previousMoveAmount = currentCellInfo.amount + currentCell.Cost;
            var previousCellInfo = infos.FirstOrDefault(info => (Mathf.Abs(info.coordinate.x - currentCell.X) + Mathf.Abs(info.coordinate.z - currentCell.Z)) == 1 && info.amount == previousMoveAmount);
            if (null == previousCellInfo)
            {
                break;
            }
            routeCells.Add(cells.First(c => c.X == previousCellInfo.coordinate.x && c.Z == previousCellInfo.coordinate.z));
        }
        routeCells.Reverse();
        return routeCells.ToArray();
    }

    /// <summary>
    /// 指定座標にユニットを配置します
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="unitPrefab">Unit prefab.</param>
    public void PutUnit(int x, int z, Map_Unit unitPrefab)
    {
        var unit = Instantiate(unitPrefab);
        unit.gameObject.SetActive(true);
        unit.transform.SetParent(unitContainer);
        //unit.transform.position = cells.First(c => c.X == x && c.Y == y).transform.position;
        unit.transform.position = new Vector3(x,0.5f,z);
        unit.x = x;
        unit.z = z;
    }

    /// <summary>
    /// ユニットを対象のマスに移動させます
    /// </summary>
    /// <param name="cell">Cell.</param>
    public void MoveTo(Map_Unit unit,Main_Cell cell)
    {
        //ResetMovableCells();
        ClearHighLight();
        var routeCells = CalculateRouteCells(unit.x, unit.z, unit.moveAmount, cell);
        var sequence = DOTween.Sequence();
        for (int i = 1; i < routeCells.Length; i++)
        {
            var routeCell = routeCells[i];
            sequence.Append(unit.transform.DOMove(new Vector3(routeCell.transform.position.x, 0.5f, routeCell.transform.position.z), 0.1f).SetEase(Ease.Linear));
        }
        sequence.OnComplete(() =>
        {
            unit.x = routeCells[routeCells.Length - 1].X;
            unit.z = routeCells[routeCells.Length - 1].Z;
            bool isAttackable = HighlightAttackableCells(unit.x, unit.z, unit.attackRangeMin, unit.attackRangeMax);
            if (!isAttackable)
                unit.IsFocused = false;
        });
    }

    public void AttackTo(Map_Unit fromUnit,Map_Unit toUnit)
    {
        BattleController.attacker = fromUnit;
        BattleController.defender = toUnit;
        Instantiate(Battle);
        ClearHighLight();
        ActiveUnit.IsFocused = false;
    }

    /// <summary>
    /// 移動力を元に移動可能範囲の計算を行います
    /// </summary>
    /// <returns>The remaining move amount infos.</returns>
    /// <param name="startCell">Start cell.</param>
    /// <param name="moveAmount">Move amount.</param>
    MoveAmountInfo[] GetRemainingMoveAmountInfos(Main_Cell startCell, int moveAmount)
    {
        var infos = new List<MoveAmountInfo>();
        infos.Add(new MoveAmountInfo(startCell.X, startCell.Z, moveAmount));
        for (int i = moveAmount; i >= 0; i--)
        {
            var appendInfos = new List<MoveAmountInfo>();
            foreach (var calcTargetInfo in infos.Where(info => info.amount == i))
            {
                // 四方のマスの座標配列を作成
                var calcTargetCoordinate = calcTargetInfo.coordinate;
                var aroundCellCoordinates = new Coordinate[]
                {
                    new Coordinate(calcTargetCoordinate.x - 1, calcTargetCoordinate.z),
                    new Coordinate(calcTargetCoordinate.x + 1, calcTargetCoordinate.z),
                    new Coordinate(calcTargetCoordinate.x, calcTargetCoordinate.z - 1),
                    new Coordinate(calcTargetCoordinate.x, calcTargetCoordinate.z + 1),
                };
                // 四方のマスの残移動力を計算
                foreach (var aroundCellCoordinate in aroundCellCoordinates)
                {
                    var targetCell = cells.FirstOrDefault(c => c.X == aroundCellCoordinate.x && c.Z == aroundCellCoordinate.z);
                    if (null == targetCell ||
                        infos.Any(info => info.coordinate.x == aroundCellCoordinate.x && info.coordinate.z == aroundCellCoordinate.z) ||
                        appendInfos.Any(info => info.coordinate.x == aroundCellCoordinate.x && info.coordinate.z == aroundCellCoordinate.z))
                    {
                        // マップに存在しない、または既に計算済みの座標はスルー
                        continue;
                    }
                    var remainingMoveAmount = i - targetCell.Cost;
                    appendInfos.Add(new MoveAmountInfo(aroundCellCoordinate.x, aroundCellCoordinate.z, remainingMoveAmount));
                }
            }
            infos.AddRange(appendInfos);
        }
        // 残移動力が0以上（移動可能）なマスの情報だけを返す
        return infos.Where(x => x.amount >= 0).ToArray();
    }

    /// <summary>
    /// 攻撃可能範囲の計算
    /// </summary>
    /// <param name="startCell"></param>
    /// <param name="attackRangeMin"></param>
    /// <param name="attackRangeMax"></param>
    /// <returns></returns>
    MoveAmountInfo[] GetRemainingAccountRangeInfos(Main_Cell startCell,int attackRangeMin,int attackRangeMax)
    {
        var infos = new List<MoveAmountInfo>();
        infos.Add(new MoveAmountInfo(startCell.X, startCell.Z, attackRangeMax));
        for (int i = attackRangeMax; i >= 0; i--)
        {
            var appendInfos = new List<MoveAmountInfo>();
            foreach(var calcTargetInfo in infos.Where(info => info.amount == i))
            {
                // 四方のマスの座標配列を作成
                var calcTargetCordinate = calcTargetInfo.coordinate;
                var aroundCellCoordinates = new Coordinate[]
                {
                    new Coordinate(calcTargetCordinate.x - 1,calcTargetCordinate.z),
                    new Coordinate(calcTargetCordinate.x + 1,calcTargetCordinate.z),
                    new Coordinate(calcTargetCordinate.x,calcTargetCordinate.z - 1),
                    new Coordinate(calcTargetCordinate.x,calcTargetCordinate.z + 1),
                };
                // 四方のマスの残攻撃範囲を計算
                foreach(var aroundCellCoordinate in aroundCellCoordinates)
                {
                    var targetCell = cells.FirstOrDefault(c => c.X == aroundCellCoordinate.x && c.Z == aroundCellCoordinate.z);
                    if(null == targetCell ||
                        infos.Any(info => info.coordinate.x == aroundCellCoordinate.x && info.coordinate.z == aroundCellCoordinate.z) ||
                        appendInfos.Any(info => info.coordinate.x == aroundCellCoordinate.x && info.coordinate.z == aroundCellCoordinate.z))
                    {
                        // マップに存在しない、または既に計算済みの座標はスルー
                        continue;
                    }
                    int remainingMoveAmont = i - 1;
                    appendInfos.Add(new MoveAmountInfo(aroundCellCoordinate.x, aroundCellCoordinate.z, remainingMoveAmont));
                }
            }
            infos.AddRange(appendInfos);
        }
        // 攻撃範囲内のマスの情報だけ返す
        return infos.Where(x => 0 <= x.amount && x.amount <= (attackRangeMax - attackRangeMin)).ToArray();
    }

    /// <summary>
    /// 任意の座標にいるユニットの取得
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public Map_Unit GetUnit(int x,int z)
    {
        return unitContainer.GetComponentsInChildren<Map_Unit>().FirstOrDefault(u => u.x == x && u.z == z);
    }

    /// <summary>
    /// 残移動力情報クラス
    /// </summary>
    class MoveAmountInfo
    {
        public readonly Coordinate coordinate;
        public readonly int amount;

        public MoveAmountInfo(int x, int z, int amount)
        {
            this.coordinate = new Coordinate(x, z);
            this.amount = amount;
        }
    }

    /// <summary>
    /// 座標クラス
    /// </summary>
    class Coordinate
    {
        public readonly int x;
        public readonly int z;

        public Coordinate(int x, int z)
        {
            this.x = x;
            this.z = z;
        }
    }
}
