using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Input_Controller : MonoBehaviour
{
    // 動く距離
    [SerializeField]
    float moveSpeed;
    // Main_Mapの参照
    [SerializeField]
    Main_Map map;

    // ターン変更用
    public static bool myTurn;

    // 敵か味方かの判断
    bool isPlayer;

    // Use this for initialization
    void Start()
    {
        isPlayer = false;
        myTurn = true;
    }

    // Update is called once per frame
    void Update()
    {
        // 移動量の計算
        float moveX = Input.GetAxisRaw("Horizontal") * moveSpeed;
        float moveZ = Input.GetAxisRaw("Vertical") * moveSpeed;

        // 入力判定
        Vector3 inputVec;
        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.z = Input.GetAxisRaw("Vertical");

        if (moveX != 0)
        {
            moveZ = 0;
            // 右移動
            if (moveX > 0 && moveZ == 0)
            {
                if (transform.position.x + 1 < Main_SceneController.mapSizeX)
                {
                    transform.position += new Vector3(1, 0, 0);
                    moveSpeed = 0;
                }
            }
            // 左移動
            if (moveX < 0 && moveZ == 0)
            {
                if (transform.position.x > 0)
                {
                    transform.position += new Vector3(-1, 0, 0);
                    moveSpeed = 0;
                }
            }
        }
        if (moveZ != 0)
        {
            moveX = 0;

            // 上移動
            if (moveZ < 0 && moveX == 0)
            {
                if (transform.position.z + 1 < Main_SceneController.mapSizeZ)
                {
                    transform.position += new Vector3(0, 0, 1);
                    moveSpeed = 0;
                }
            }
            // 下移動
            if (moveZ > 0 && moveX == 0)
            {
                if (transform.position.z > 0)
                {
                    transform.position += new Vector3(0, 0, -1);
                    moveSpeed = 0;
                }
            }
        }
        // 連続入力防止
        if (inputVec.x == 0 && inputVec.z == 0)
        {
            moveSpeed = 1.0f;
        }

        // ユニットまたは移動先の選択
        if (Input.GetButtonDown("Click"))
        {
            if (myTurn == true)
            {
                MoveSelect();
            }
            else
            {
                return;
            }
        }
        // キャンセル
        if (Input.GetButtonDown("Cancel"))
        {
            if (map.ActiveUnit != null)
            {
                map.ActiveUnit.IsFocused = false;
                map.ClearHighLight();
            }
            else
            {
                return;
            }
        }
        // ターン変更
        if (Input.GetButtonDown("Change"))
        {
            TurnChange();
        }
    }

    // ユニット選択、移動先の選択メソッド
    void MoveSelect()
    {
        Main_Cell cell;
        Map_Unit unit;

        // コントローラーの位置からRayを飛ばす
        Ray ray = new Ray(transform.position, transform.forward);

        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(ray,out hit))
        {
            if(hit.collider.tag == "Player" || hit.collider.tag == "Untagged")
            {
                if (hit.collider.tag == "Player")
                {
                    isPlayer = true;
                }
                else if (hit.collider.tag == "Untagged")
                {
                    isPlayer = false;
                }
                unit = hit.collider.gameObject.GetComponent<Map_Unit>();
                unit.OnClick();
            }

            if (hit.collider.tag == "canMove")
            {
                if (isPlayer && map.ActiveUnit != null)
                {
                    cell = hit.collider.gameObject.GetComponent<Main_Cell>();
                    cell.OnClick();
                }
                else
                {
                    return;
                }
            }
        }
    }

    // ターン変更
    void TurnChange()
    {
        if (myTurn)
        {
            map.NextTurn();
            myTurn = false;
        }
        else
        {
            return;
        }
    }
}
