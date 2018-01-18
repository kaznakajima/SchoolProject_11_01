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
    GameObject mikoshi_Action;
    [SerializeField]
    GameObject mikoshi_Damage;
    [SerializeField]
    GameObject Battle;

    // 自ユニット・敵ユニットの向き
    float unitDir;
    float e_unitDir;

    //自ユニット・敵ユニットの数
    public int myUnitOriginNum;
    public int eUnitOriginNum;
    int myUnitNum;
    int eUnitNum;

    // マスのリスト
    List<Main_Cell> cells = new List<Main_Cell>();
    // ユニットのリスト
    public List<Map_Unit> units = new List<Map_Unit>();

    Map_Unit.Team currentTeam;
    Dictionary<Map_Unit.Team, Main_AI> ais = new Dictionary<Map_Unit.Team, Main_AI>();

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

    void Update()
    {
        //if (Input.GetButtonDown("Click"))
        //{
        //    NextTurn();
        //}
    }

    public void SetAI(Map_Unit.Team team, Main_AI ai)
    {
        ai.Initialize(this);
        ais[team] = ai;
    }

    public void StartTurn(Map_Unit.Team team)
    {
        currentTeam = team;

        // 移動可能なユニットの計算
        switch (team)
        {
            case Map_Unit.Team.Player1:
                myUnitNum = myUnitOriginNum;
                break;
            case Map_Unit.Team.Player2:
                eUnitNum = eUnitOriginNum;
                break;
        }

        // 敵のすり抜け防止
        foreach(var cell in cells)
        {
            // 敵のいる位置のマスは移動不可能
            if(cell.Unit != null && cell.Unit.team != currentTeam)
            {
                cell.cost = 5;
            }
            // 敵じゃなければ移動可能
            else if(cell.Unit != null && cell.Unit.team == currentTeam)
            {
                cell.cost = 1;
            }
        }

        foreach(var unit in unitContainer.GetComponentsInChildren<Map_Unit>())
        {
            unit.IsMoved = team != unit.team;
        }

        // 敵の行動開始(ボタン)
        if (ais.ContainsKey(team))
        {
            var ai = ais[team];
            ai.Move();
        }
        else
        {
            Debug.LogFormat("敵ターン終了");
            Input_Controller.myTurn = true;
        }
    }

    public void NextTurn()
    {
        var nextTeam = currentTeam == Map_Unit.Team.Player1 ? Map_Unit.Team.Player2 : Map_Unit.Team.Player1;
        StartTurn(nextTeam);
    }

    /// <summary>
    /// マップを生成します
    /// </summary>
    /// <param name="width">Width.</param>
    /// <param name="height">Height.</param>
    public void Generate(int width, int height)
    {
        Main_Cell cell;

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                cell = Instantiate(cellFieldPrefab);
                cell.gameObject.SetActive(true);
                cell.transform.SetParent(transform);
                cell.gameObject.transform.position = new Vector3(x, -0.5f, z);
                cell.SetCoordinate(x, z);
                cells.Add(cell);
            }
        }
    }

    /// <summary>
    /// 指定座標から各マスまで、移動コストいくつで行けるかを計算
    /// </summary>
    /// <param name="from"></param>
    /// <returns>The move amount to cells.</returns>
    public List<MoveAmountInfo> GetMoveCostToAllCells(Main_Cell from)
    {
        var infos = new List<MoveAmountInfo>();
        infos.Add(new MoveAmountInfo(from.X, from.Z, 0));
        int countCell = 0;
        while (true)
        {
            var appendInfos = new List<MoveAmountInfo>();
            foreach(var calcTargetInfo in infos.Where(info => info.amount == countCell))
            {
                // 四方のマスの座標配列を作成
                var calcTargetCoordinate = calcTargetInfo.coordinate;
                var aroundCellCoordinates = new Coordinate[]
                {
                    new Coordinate(calcTargetCoordinate.x - 1,calcTargetCoordinate.z),
                    new Coordinate(calcTargetCoordinate.x + 1,calcTargetCoordinate.z),
                    new Coordinate(calcTargetCoordinate.x,calcTargetCoordinate.z - 1),
                    new Coordinate(calcTargetCoordinate.x,calcTargetCoordinate.z + 1),
                };
                // 四方のマスの残移動力を計算
                foreach(var aroundCellCoordinate in aroundCellCoordinates)
                {
                    var targetCell = cells.FirstOrDefault(c => c.X == aroundCellCoordinate.x && c.Z == aroundCellCoordinate.z);
                    if(targetCell == null ||
                        infos.Any(info => info.coordinate.x == aroundCellCoordinate.x && info.coordinate.z == aroundCellCoordinate.z) ||
                        appendInfos.Any(info => info.coordinate.x == aroundCellCoordinate.x && info.coordinate.z == aroundCellCoordinate.z))
                    {
                        // マップに存在しない、または既に計算済みの座標はスルー
                        continue;
                    }
                    int remainingMoveAmount = countCell + targetCell.Cost;
                    appendInfos.Add(new MoveAmountInfo(aroundCellCoordinate.x, aroundCellCoordinate.z, remainingMoveAmount));
                }
            }
            infos.AddRange(appendInfos);

            countCell++;
            if(countCell > infos.Max(x => x.amount < 999 ? x.amount : 0))
            {
                break;
            }
        }
        return infos.Where(x => x.amount < 999).ToList();
    }

    /// <summary>
    /// 指定位置までの移動ルートと移動コストを返す
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns>The route coordinates and move amount.</returns>
    public List<MoveAmountInfo> CalcurateRouteCoordnatesAndMoveAmount(Main_Cell from, Main_Cell to)
    {
        var costs = GetMoveCostToAllCells(from);
        if (!costs.Any(info => info.coordinate.x == to.X && info.coordinate.z == to.Z))
        {
            throw new ArgumentException(string.Format("x:{0}, y:{1} is not movable.", to.X, to.Z));
        }

        var toCost = costs.First(info => info.coordinate.x == to.X && info.coordinate.z == to.Z);
        var route = new List<MoveAmountInfo>();
        route.Add(toCost);
        while (true)
        {
            var currentCost = route.Last();
            var currentCell = cells.First(cell => cell.X == currentCost.coordinate.x && cell.Z == currentCost.coordinate.z);
            var prevMoveCost = currentCost.amount - currentCell.Cost;
            var previousCost = costs.FirstOrDefault(info => (Mathf.Abs(info.coordinate.x - currentCell.X) + Mathf.Abs(info.coordinate.z - currentCell.Z)) == 1 && info.amount == prevMoveCost);
            if(previousCost == null)
            {
                break;
            }
            route.Add(previousCost);
        }
        route.Reverse();
        return route.ToList();
    }

    public Main_Cell[] GetAttackableCells()
    {
        return cells.Where(x => x.IsAttackable).ToArray();
    }

    public Main_Cell[] GetMovableCells()
    {
        return cells.Where(x => x.IsMovable).ToArray();
    }

    /// <summary>
    /// 任意のマスの取得
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public Main_Cell GetCell(int x, int z)
    {
        return cells.First(c => c.X == x && c.Z == z);
    }

    public Main_Cell[] GetCellsByDistance(Main_Cell baseCell, int distanceMin, int distanceMax)
    {
        return cells.Where(x =>
        {
            var distance = Math.Abs(baseCell.X - x.X) + Math.Abs(baseCell.Z - x.Z);
            return distanceMin <= distance && distance <= distanceMax;
        }).ToArray();
    }

    /// <summary>
    /// 移動可能なマスをハイライトする
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="moveAmount">Move amount.</param>
    public void HighlightMovableCells(int x, int z, int moveAmount)
    { 
        var startCell = cells.First(c => c.X == x && c.Z == z);
        foreach (var info in GetRemainingMoveAmountInfos(startCell, moveAmount))
        {
            var cell = cells.First(c => c.X == info.coordinate.x && c.Z == info.coordinate.z);
            if(cell.Unit == null)
            {
                cells.First(c => c.X == info.coordinate.x && c.Z == info.coordinate.z).IsMovable = true;

                if(ActiveUnit.unitType == Map_Unit.UnitType.Mikoshi)
                {
                    // 移動先のまわりの座標
                    var mikoshiUnitCell = new Coordinate[]
                    {
                        new Coordinate(cell.X + 1,cell.Z),
                        new Coordinate(cell.X,cell.Z + 1),
                        new Coordinate(cell.X + 1,cell.Z - 1),
                        new Coordinate(cell.X + 1,cell.Z + 1),
                        new Coordinate(cell.X - 1,cell.Z),
                        new Coordinate(cell.X,cell.Z - 1),
                        new Coordinate(cell.X - 1,cell.Z + 1),
                        new Coordinate(cell.X - 1,cell.Z - 1)
                    };

                    // 神輿同士の移動制限
                    var _mikoshiUnitCell = new Coordinate[]
                    {
                        new Coordinate(cell.X + 1,cell.Z),
                        new Coordinate(cell.X,cell.Z + 1),
                        new Coordinate(cell.X + 1,cell.Z - 1),
                        new Coordinate(cell.X + 1,cell.Z + 1),
                        new Coordinate(cell.X - 1,cell.Z),
                        new Coordinate(cell.X,cell.Z - 1),
                        new Coordinate(cell.X - 1,cell.Z + 1),
                        new Coordinate(cell.X - 1,cell.Z - 1),
                        new Coordinate(cell.X + 2,cell.Z),
                        new Coordinate(cell.X,cell.Z + 2),
                        new Coordinate(cell.X + 1,cell.Z - 2),
                        new Coordinate(cell.X + 1,cell.Z + 2),
                        new Coordinate(cell.X - 2,cell.Z),
                        new Coordinate(cell.X,cell.Z - 2),
                        new Coordinate(cell.X - 1,cell.Z + 2),
                        new Coordinate(cell.X - 1,cell.Z - 2)
                    };

                    // 神輿以外のユニットが神輿に近づきすぎないように制限
                    foreach (var mikoshiaroundCell in mikoshiUnitCell)
                    {
                        var spaceCell = cells.FirstOrDefault(c => c.X == mikoshiaroundCell.x && c.Z == mikoshiaroundCell.z);

                        // マスにユニットがいる場合実行
                        if (spaceCell.Unit != null)
                        {
                            if (spaceCell.Unit.unitType != Map_Unit.UnitType.Mikoshi)
                            {
                                cells.First(c => c.X == info.coordinate.x && c.Z == info.coordinate.z).IsMovable = false;
                            } 
                        }
                        else if (spaceCell.Unit == null && spaceCell.Cost != 1)
                        {
                            cells.First(c => c.X == info.coordinate.x && c.Z == info.coordinate.z).IsMovable = false;
                        }
                    }

                    // 神輿同士が近づきすぎないように制限
                    foreach (var mikoshiaroundCell in _mikoshiUnitCell)
                    {
                        var spaceCell = units.FirstOrDefault(c => c.x == mikoshiaroundCell.x && c.z == mikoshiaroundCell.z);

                        // マスにユニットがいる場合に実行
                        if (spaceCell != null)
                        {
                            if (spaceCell != ActiveUnit && spaceCell.unitType == Map_Unit.UnitType.Mikoshi)
                            {
                                cells.First(c => c.X == info.coordinate.x && c.Z == info.coordinate.z).IsMovable = false;
                            }
                        }
                    }
                }
                else if(ActiveUnit.unitType != Map_Unit.UnitType.Mikoshi)
                {
                    // 移動先のまわりの座標
                    var nomalUnitCell = new Coordinate[]
                    {
                         new Coordinate(cell.X + 1,cell.Z),
                         new Coordinate(cell.X,cell.Z + 1),
                         new Coordinate(cell.X + 1,cell.Z - 1),
                         new Coordinate(cell.X + 1,cell.Z + 1),
                         new Coordinate(cell.X - 1,cell.Z),
                         new Coordinate(cell.X,cell.Z - 1),
                         new Coordinate(cell.X - 1,cell.Z + 1),
                         new Coordinate(cell.X - 1,cell.Z - 1)
                    };

                    // 神輿に近づきすぎないように制限
                    foreach (var mikoshiaroundCell in nomalUnitCell)
                    {
                        var mikoshiOnCell = units.FirstOrDefault(u => u.x == mikoshiaroundCell.x && u.z == mikoshiaroundCell.z);

                        // ユニットがいる場合実行
                        if (mikoshiOnCell != null && mikoshiOnCell.unitType == Map_Unit.UnitType.Mikoshi)
                        {
                            cells.First(c => c.X == info.coordinate.x && c.Z == info.coordinate.z).IsMovable = false;
                        }
                    }
                }
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
    public bool HighlightAttackableCells(int x, int z, int attackRangeMin, int attackRangeMax)
    {
        var startCell = cells.First(c => c.X == x && c.Z == z);

        // 神輿が攻撃範囲にいるかの確認
        if(startCell.Unit.unitType != Map_Unit.UnitType.Mikoshi)
        {
            // 2マス先のマス
            var unitaroundCell = new Coordinate[]
            {
                new Coordinate(startCell.X + 2,startCell.Z),
                new Coordinate(startCell.X - 2,startCell.Z),
                new Coordinate(startCell.X,startCell.Z + 2),
                new Coordinate(startCell.X,startCell.Z - 2),
                new Coordinate(startCell.X + 1,startCell.Z - 1),
                new Coordinate(startCell.X + 1,startCell.Z + 1),
                new Coordinate(startCell.X - 1,startCell.Z + 1),
                new Coordinate(startCell.X - 1,startCell.Z - 1)
            };

            foreach (var srantCell in unitaroundCell)
            {
                var unit = units.FirstOrDefault(u => u.x == srantCell.x && u.z == srantCell.z);
                // 神輿がいれば攻撃可能
                if(unit != null && unit.unitType != Map_Unit.UnitType.Mikoshi)
                {
                    attackRangeMax = 1;
                }
            }
        }

        var hasTarget = false;

        // 神輿同士の攻撃用
        if (startCell.Unit.unitType == Map_Unit.UnitType.Mikoshi)
        {
            var mikoshiaroundCell = new Coordinate[]
            {
                new Coordinate(startCell.X + 3,startCell.Z),
                new Coordinate(startCell.X - 3,startCell.Z),
                new Coordinate(startCell.X,startCell.Z + 3),
                new Coordinate(startCell.X,startCell.Z - 3)
            };

            foreach (var thirdfrontCell in mikoshiaroundCell)
            {
                var unit = units.FirstOrDefault(u => u.x == thirdfrontCell.x && u.z == thirdfrontCell.z);

                // 神輿がいれば攻撃可能
                if (unit != null && unit.unitType == Map_Unit.UnitType.Mikoshi && unit.team != currentTeam)
                {
                    hasTarget = true;
                    GetCell(unit.x, unit.z).IsAttackable = true;


                    return hasTarget;
                }
            }
        }

        // 攻撃範囲に敵がいるかの確認
        foreach (var cell in GetCellsByDistance(startCell, attackRangeMin, attackRangeMax))
        {
            if (cell.Unit != null && cell.Unit.team != currentTeam)
            {
                hasTarget = true;
                cell.IsAttackable = true;
            }
        }
        return hasTarget;
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
                cell.IsAttackable = false;
            }
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

        // 移動できないマスが選択されたら
        if (!infos.Any(info => info.coordinate.x == endCell.X && info.coordinate.z == endCell.Z))
        {
            throw new ArgumentException(string.Format("endCell(x:{0}, y:{1}) is not movable.", endCell.X, endCell.Z));
        }

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
    public void PutUnit(int x, int z, Map_Unit unitPrefab, Map_Unit.Team team)
    {
        var unit = Instantiate(unitPrefab);
        unit.team = team;
        switch (unit.team)
        {
            case Map_Unit.Team.Player1:
                myUnitOriginNum++;
                myUnitNum = myUnitOriginNum;
                break;
            case Map_Unit.Team.Player2:
                eUnitOriginNum++;
                eUnitNum = eUnitOriginNum;
                break;
        }
        unit.gameObject.SetActive(true);
        unit.transform.SetParent(unitContainer);
        unit.transform.position = new Vector3(x, 0.0f, z);
        unit.x = x;
        unit.z = z;
        units.Add(unit);
    }

    /// <summary>
    /// ユニットを対象のマスに移動させます
    /// </summary>
    /// <param name="cell">Cell.</param>
    public void MoveTo(Map_Unit unit, Main_Cell cell)
    {
        // ユニットの移動アニメーション
        Animator unitAnim;
        unitAnim = unit.GetComponent<Animator>();
        unitAnim.SetBool("Iswalk", true);

        ClearHighLight();
        var routeCells = CalculateRouteCells(unit.x, unit.z, unit.moveAmount, cell);
        var sequence = DOTween.Sequence();
        unitDir = unit.transform.rotation.y;

        for (int i = 1; i < routeCells.Length; i++)
        {
            // 現在の位置
            var routeCell = routeCells[i];

            // 前の移動位置
            var endCell = routeCells[i - 1];

            // -----------------------進行方向に向かせる----------------------------------------
            if (routeCell.transform.position.x < endCell.transform.position.x)
            {
                sequence.Append(unit.transform.DORotate(new Vector3(unitDir, -90), 0.1f));
            }
            else if (routeCell.transform.position.x > endCell.transform.position.x)
            {
                sequence.Append(unit.transform.DORotate(new Vector3(unitDir, 90), 0.1f));
            }

            unitDir = unit.transform.rotation.y;

            if (routeCell.transform.position.z < endCell.transform.position.z)
            {
                sequence.Append(unit.transform.DORotate(new Vector3(unitDir, 180), 0.1f));
            }
            else if (routeCell.transform.position.z > endCell.transform.position.z)
            {
                sequence.Append(unit.transform.DORotate(new Vector3(unitDir, 0), 0.1f));
            }

            unitDir = unit.transform.rotation.y;

            // ----------------------------------------------------------------------------------

            // 移動実行
            sequence.Append(unit.transform.DOMove(new Vector3(routeCell.transform.position.x, 0.0f, routeCell.transform.position.z), 0.3f).SetEase(Ease.Linear));
        }

        // 移動が終了した場合の処理
        sequence.OnComplete(() =>
        {
            unitAnim.SetBool("Iswalk", false);
            unit.x = routeCells[routeCells.Length - 1].X;
            unit.z = routeCells[routeCells.Length - 1].Z;

            bool isAttackable = HighlightAttackableCells(unit.x, unit.z, unit.attackRangeMin, unit.attackRangeMax);
            //if(cell.Cost == 1)
            //{
            //    PutUnit(unit.x + 1,unit.z,unit,currentTeam);
            //}

            // 攻撃ができなかったら行動終了
            if (!isAttackable)
            {
                unit.IsFocused = false;
                unit.IsMoved = true;
                switch (unit.team)
                {
                    case Map_Unit.Team.Player1:
                        myUnitNum--;
                        if(myUnitNum == 0)
                        {
                            NextTurn();
                        }
                        break;
                    //case Map_Unit.Team.Player2:
                    //    eUnitNum--;
                    //    if(eUnitNum == 0)
                    //    {
                    //        NextTurn();
                    //    }
                    //    break;
                }
            }
                
        });
    }

    public void AttackTo(Map_Unit fromUnit, Map_Unit toUnit)
    {
       
        if(fromUnit.unitType == Map_Unit.UnitType.Mikoshi)
        {
            // 神輿の攻撃アニメーション
            mikoshi_Action.transform.position = new Vector3(fromUnit.transform.position.x, -0.05f, fromUnit.transform.position.z);
            mikoshi_Action.SetActive(true);
        }
        else
        {
            // ユニットの攻撃アニメーション
            Animator unitAnim;
            unitAnim = fromUnit.GetComponent<Animator>();
            unitAnim.SetTrigger("Isattack");
        }

        if(toUnit.unitType == Map_Unit.UnitType.Mikoshi)
        {
            // 神輿のダメージアニメーション
            mikoshi_Damage.transform.position = new Vector3(toUnit.transform.position.x, -0.05f, toUnit.transform.position.z);
            mikoshi_Damage.SetActive(true);
        }
        else
        {
            // ユニットのダメージアニメーション
            Animator enemyAnim;
            enemyAnim = toUnit.GetComponent<Animator>();
            enemyAnim.SetTrigger("Isdamage");
        }

        // 向きの取得
        unitDir = fromUnit.transform.rotation.y;
        e_unitDir = toUnit.transform.rotation.y;

        // 攻撃と防御の取得
        BattleController.attacker = fromUnit;
        BattleController.defender = toUnit;
        BattleController.mikoshi_Action = mikoshi_Action;
        BattleController.mikoshi_Damage = mikoshi_Damage;
        var sequence = DOTween.Sequence();

        // 隣接しているユニットがいるかの確認用
        var unitCoordinates = new Coordinate[]
        {
            new Coordinate((int)fromUnit.transform.position.x + 1,(int)fromUnit.transform.position.z),
            new Coordinate((int)fromUnit.transform.position.x - 1,(int)fromUnit.transform.position.z),
            new Coordinate((int)fromUnit.transform.position.x,(int)fromUnit.transform.position.z + 1),
            new Coordinate((int)fromUnit.transform.position.x,(int)fromUnit.transform.position.z - 1)
        };

        // 自ユニットが隣接していたら攻撃力増加
        foreach(var aroundUnit in unitCoordinates)
        {
            var nextCell = units.FirstOrDefault(c => c.x == aroundUnit.x && c.z == aroundUnit.z);
            if(nextCell != null)
            {
                if(nextCell.team == fromUnit.team)
                {
                    fromUnit.unitAtk += 1;
                }
            }
        }

        // ------------------------------------------------向きの変更-------------------------------------------------------------------

        if(fromUnit.transform.position.x < toUnit.transform.position.x)
        {
            sequence.Append(fromUnit.transform.DORotate(new Vector3(unitDir, 90), 0.1f));
            sequence.Append(mikoshi_Damage.transform.DORotate(new Vector3(e_unitDir, -90), 0.1f));
            if (fromUnit.unitType == Map_Unit.UnitType.Mikoshi)
            {
                sequence.Append(mikoshi_Action.transform.DORotate(new Vector3(unitDir, 90), 0.1f));
            }
        }
        else if(fromUnit.transform.position.x > toUnit.transform.position.x)
        {
            sequence.Append(fromUnit.transform.DORotate(new Vector3(unitDir, -90),0.1f));
            sequence.Append(mikoshi_Damage.transform.DORotate(new Vector3(e_unitDir, 90), 0.1f));
            if (fromUnit.unitType == Map_Unit.UnitType.Mikoshi)
            {
                sequence.Append(mikoshi_Action.transform.DORotate(new Vector3(unitDir, -90), 0.1f));
            }
        }

        unitDir = fromUnit.transform.rotation.y;
        e_unitDir = toUnit.transform.rotation.y;

        if(fromUnit.transform.position.z < toUnit.transform.position.z)
        {
            sequence.Append(fromUnit.transform.DORotate(new Vector3(unitDir, 0), 0.1f));
            sequence.Append(mikoshi_Damage.transform.DORotate(new Vector3(e_unitDir, 180), 0.1f));
            if (fromUnit.unitType == Map_Unit.UnitType.Mikoshi)
            {
                sequence.Append(mikoshi_Action.transform.DORotate(new Vector3(unitDir, 0), 0.1f));
            }
        }
        else if(fromUnit.transform.position.z > toUnit.transform.position.z)
        {
            sequence.Append(fromUnit.transform.DORotate(new Vector3(unitDir, 180), 0.1f));
            sequence.Append(mikoshi_Damage.transform.DORotate(new Vector3(e_unitDir, 0), 0.1f));
            if (fromUnit.unitType == Map_Unit.UnitType.Mikoshi)
            {
                sequence.Append(mikoshi_Action.transform.DORotate(new Vector3(unitDir, 180), 0.1f));
            }
        }

        // ---------------------------------------------------------------------------------------------------------------------------------

        Instantiate(Battle);
        ClearHighLight();
        ActiveUnit.IsFocused = false;
        if (fromUnit.unitType == Map_Unit.UnitType.Mikoshi)
        {
            fromUnit.gameObject.SetActive(false);
        }
        if(toUnit.unitType == Map_Unit.UnitType.Mikoshi)
        {
            toUnit.gameObject.SetActive(false);
        }

        // 攻撃力のリセット
        fromUnit.unitAtk = 0;

        // 行動終了
        switch (fromUnit.team)
        {
            case Map_Unit.Team.Player1:
                myUnitNum--;
                // 移動可能なユニットがいなかったらターン変更
                if (myUnitNum == 0)
                {
                    NextTurn();
                }
                break;
            case Map_Unit.Team.Player2:
                eUnitNum--;
                // 移動可能なユニットがいなかったらターン変更
                if (eUnitNum == 0)
                {
                    NextTurn();
                }
                break;
        }
    }

    /// <summary>
    /// 自軍のユニットを取得します
    /// </summary>
    /// <returns>The own units.</returns>
    public Map_Unit[] GetOwnUnits()
    {
        return unitContainer.GetComponentsInChildren<Map_Unit>().Where(x => x.team == currentTeam).ToArray();
    }

    /// <summary>
    /// 敵軍のユニットを取得します
    /// </summary>
    /// <returns>The enemy units.</returns>
    public Map_Unit[] GetEnemyUnits()
    {
        return unitContainer.GetComponentsInChildren<Map_Unit>().Where(x => x.team != currentTeam).ToArray();
    }

    /// <summary>
    /// 任意の座標にいるユニットの取得
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public Map_Unit GetUnit(int x, int z)
    {
        return unitContainer.GetComponentsInChildren<Map_Unit>().FirstOrDefault(u => u.x == x && u.z == z);
    }

    /// <summary>
    /// 移動力を元に移動可能範囲の計算
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
    /// 残移動力情報クラス
    /// </summary>
    public class MoveAmountInfo
    {
        public readonly Coordinate coordinate;
        public readonly int amount;

        public MoveAmountInfo(int x, int z, int amount)
        {
            coordinate = new Coordinate(x, z);
            this.amount= amount;
        }
    }

    /// <summary>
    /// 座標クラス
    /// </summary>
    public class Coordinate
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
