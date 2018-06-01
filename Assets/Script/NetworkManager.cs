using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour {

    public Text text;
    public GameObject loginUI;
    public Dropdown roomList;
    public InputField roomName;
    public GameObject logoutUI;     //　ログアウトボタン

    // Use this for initialization
    void Start () {
        PhotonNetwork.logLevel = PhotonLogLevel.Full; //ログをすべて表示する。
        PhotonNetwork.autoJoinLobby = true; //ロビー自動で入る。
        PhotonNetwork.ConnectUsingSettings("0.1"); //ゲームのバージョン設定。

	}
	
    void OnConnectedToMaster(){ Debug.Log("マスターサーバに接続");} //マスターサーバに接続されたときに呼ばれる。
    void OnJoinedLobby()
    {
        Debug.Log("ロビーに入る。");
        loginUI.SetActive(true);
    }

    public void LoginGame()
    {
        //　ルームオプションを設定
        RoomOptions ro = new RoomOptions();
        //　ルームを見えるようにする
        ro.IsVisible = true;
        //　部屋の入室最大人数
        ro.MaxPlayers = 10;

        if (roomName.text != "")
        {
            //　部屋がない場合は作って入室
            PhotonNetwork.JoinOrCreateRoom(roomName.text, ro, TypedLobby.Default);
        }
        else
        {
            //　部屋が存在すれば
            if (roomList.options.Count != 0)
            {
                Debug.Log(roomList.options[roomList.value].text);
                PhotonNetwork.JoinRoom(roomList.options[roomList.value].text);
                //　部屋が存在しなければDefaultRoomという名前で部屋を作成
            }
            else
            {
                PhotonNetwork.JoinOrCreateRoom("DefaultRoom", ro, TypedLobby.Default);
            }
        }
    }


    //　部屋が更新された時の処理
    void OnReceivedRoomListUpdate()
    {
        Debug.Log("部屋更新");

        //　部屋情報を取得する
        RoomInfo[] rooms = PhotonNetwork.GetRoomList();

        //　ドロップダウンリストに追加する文字列用のリストを作成
        List<string> list = new List<string>();

        //　部屋情報を部屋リストに表示
        foreach (RoomInfo room in rooms)
        {
            //　部屋が満員でなければ追加
            if (room.PlayerCount < room.MaxPlayers)
            {
                list.Add(room.Name);
            }
        }

        //　ドロップダウンリストをリセット
        roomList.ClearOptions();

        //　部屋が１つでもあればドロップダウンリストに追加
        if (list.Count != 0)
        {
            roomList.AddOptions(list);
        }
    }


    //　部屋に入室した時に呼ばれるメソッド
    void OnJoinedRoom()
    {
        loginUI.SetActive(false);
        logoutUI.SetActive(true);
        Debug.Log("入室");

        //　InputFieldに入力した名前を設定
        //PhotonNetwork.player.NickName = playerName.text;
    }

    //　部屋の入室に失敗した
    void OnPhotonJoinRoomFailed()
    {
        Debug.Log("入室に失敗");

        //　ルームオプションを設定
        RoomOptions ro = new RoomOptions();
        //　ルームを見えるようにする
        ro.IsVisible = true;
        //　部屋の入室最大人数
        ro.MaxPlayers = 10;
        //　入室に失敗したらDefaultRoomを作成し入室
        PhotonNetwork.JoinOrCreateRoom("DefaultRoom", ro, TypedLobby.Default);
    }

    //　ログアウトボタンを押した時の処理
    public void LogoutGame()
    {
        PhotonNetwork.LeaveRoom();
    }

    //　部屋を退室した時の処理
    void OnLeftRoom()
    {
        Debug.Log("退室");
        logoutUI.SetActive(false);
    }


    // Update is called once per frame
    void Update () {
        text.text = PhotonNetwork.connectionStateDetailed.ToString();
	}

    

}
