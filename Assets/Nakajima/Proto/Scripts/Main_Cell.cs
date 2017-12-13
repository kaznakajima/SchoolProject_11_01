using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Main_Cell : MonoBehaviour
{
    [SerializeField]
    Main_Map map;
    [SerializeField]
    int cost;
    [SerializeField]
    Material movableColor;
    [SerializeField]
    Material attackableColor;
    [SerializeField]
    MeshRenderer rangeMesh;

    public GameObject highlight;

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
            //rangeMesh = highlight.GetComponent<MeshRenderer>();
            rangeMesh.material = movableColor;
            highlight.SetActive(value);
            highlight.tag = "canMove";
        }
        get { return highlight.activeSelf; }
    }

    public bool IsAttackable
    {
        set
        {
            //rangeMesh = highlight.GetComponent<MeshRenderer>();
            rangeMesh.material = attackableColor;
            highlight.SetActive(value);
            highlight.tag = "GameController";
            if (!IsAttackable)
            {
                rangeMesh.material = movableColor;
                highlight.tag = "canMove";
            }

        }
        get { return highlight.activeSelf; }
    }

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
        rangeMesh = highlight.GetComponent<MeshRenderer>();
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            // ワールド座標をスクリーン座標に変換
            mousePos = Vector3.zero;

            // マウスの位置からRayを飛ばす
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit))
            {
                // Rayが当たったオブジェクトの座標を取得
                mousePos = hit.collider.gameObject.transform.position;
            }
            if (mousePos == highlight.transform.position && highlight.tag == "canMove")
            {
                OnClick();
            }
        }
    }

    /// <summary>
    /// 座標をセットします
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

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Untagged")
        {
            cost = 5;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Untagged")
        {
            cost = 1;
        }
    }
}

