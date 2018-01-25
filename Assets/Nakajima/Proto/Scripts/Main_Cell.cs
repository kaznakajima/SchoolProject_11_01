using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Main_Cell : MonoBehaviour
{
    [SerializeField]
    Main_Map map;     // マップの参照
    [SerializeField]
    public int cost;     // マスの移動コスト
    [SerializeField]
    Material movableColor;
    [SerializeField]
    Material attackableColor;
    [SerializeField]
    MeshRenderer rangeMesh;

    public GameObject highlight;
    public GameObject Liquor;
    public GameObject Gallary;

    int x;
    int z;

    // マウスの位置
    public Vector3 mousePos;

    /// <summary>
    /// 移動可能なマスかどうか
    /// </summary>
    /// <value><c>true</c> if this instance is movable; otherwise, <c>false</c>.</value>
    public bool IsMovable
    {
        set
        {
            rangeMesh = highlight.GetComponent<MeshRenderer>();
            //rangeMesh.material = movableColor;
            highlight.SetActive(value);
            highlight.tag = "canMove";
        }
        get { return highlight.activeSelf; }
    }

    /// <summary>
    /// 攻撃可能なマスかどうか
    /// </summary>
    /// /// <value><c>true</c> if this instance is attackale; otherwise, <c>false</c>.</value>
    public bool IsAttackable
    {
        set
        {
            ////rangeMesh = highlight.GetComponent<MeshRenderer>();
            //rangeMesh.material = attackableColor;
            highlight.SetActive(value);
            highlight.tag = "Untagged";
            if (!IsAttackable)
            {
                //rangeMesh.material = movableColor;
                highlight.tag = "canMove";
            }
        }
        get { return highlight.activeSelf;}
    }

    // お酒があるかどうか
    public bool IsLiquor
    {
        set
        {
            Liquor.SetActive(value);
        }
        get
        {
            return Liquor.activeSelf;
        }
    }

    // ギャラリーがいるかどうか
    public bool IsGallary
    {
        set
        {
            Gallary.SetActive(value);
        }
        get
        {
            return Gallary.activeSelf;
        }
    }

    // マスの移動コスト
    public int Cost
    {
        get { return cost; }
    }

    public int X
    {
        get { return x; }
    }

    public int Z
    {
        get { return z; }
    }

    public Map_Unit Unit
    {
        get { return map.GetUnit(x, z); }
    }

    void Start()
    {
        //rangeMesh = highlight.GetComponent<MeshRenderer>();
    }

    void Update()
    {
        //// 通り抜け防止(味方は可能)
        //if(Unit != null)
        //{
        //    if(Input_Controller.myTurn == true)
        //    {
        //        if (Unit.tag == "Untagged")
        //            cost = 5;
        //        else
        //            cost = 1;
        //    }
        //    else
        //    {
        //        if (Unit.tag == "Player")
        //            cost = 5;
        //        else
        //            cost = 1;
        //    }
        //}

        //if (Input.GetMouseButtonDown(0))
        //{
        //    // ワールド座標をスクリーン座標に変換
        //    mousePos = Vector3.zero;

        //    // マウスの位置からRayを飛ばす
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit = new RaycastHit();
        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        // Rayが当たったオブジェクトの座標を取得
        //        mousePos = hit.collider.gameObject.transform.position;
        //    }
        //    if (mousePos == highlight.transform.position && highlight.tag == "canMove")
        //    {
        //        OnClick();
        //    }
        //}
    }

    /// <summary>
    /// 座標をセット
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    public void SetCoordinate(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public void OnClick()
    {
        if (IsMovable)
        {
            map.MoveTo(map.ActiveUnit, this);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "GameController")
        {
            cost = 5;
        }
    }
}

