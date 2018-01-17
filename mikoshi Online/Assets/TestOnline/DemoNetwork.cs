using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoNetwork : Photon.MonoBehaviour
{

    void Start()
    {
        //Photonの入るバージョンの設定。nullがバージョン
        PhotonNetwork.ConnectUsingSettings(null);
    }

    // Update is called once per frame
    void Update()
    {

    }

    //  ランダムでルームを選び入る
    void OnJoinedLobby()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    //  JoinRandomRoom()が失敗した(false)時に呼ばれる
    void OnPhotonRandomJoinFailed()
    {
        //  部屋に入れなかったので自分で作る
        PhotonNetwork.CreateRoom(null);
        Debug.Log("作ったよ");
    }

    //  ルームに入れた時に呼ばれる（自分の作ったルームでも）
    void OnJoinedRoom()
    {
        Debug.Log("入ったよ");
        //  ルームに入っている全員の画面にPlayerを生成する
        //PhotonNetwork.Instantiate("プレイヤーのオブジェクト", this.transform.position, this.transform.rotation, 0);
        GameObject player = PhotonNetwork.Instantiate("Cube", this.transform.position, this.transform.rotation, 0);
        //  自分が生成したPlayerを移動可能にする
        player.GetComponent<MOVE>().enabled = true;
    }
}
