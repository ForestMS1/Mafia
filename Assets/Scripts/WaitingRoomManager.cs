using System;
using Cainos.PixelArtTopDown_Basic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Photon.Realtime;

public class WaitingRoomManager : MonoBehaviourPunCallbacks
{
    public GameObject player;
    public TextMeshProUGUI textMesh;
    RoomOptions roomOptions;
    
    private void Awake()
    {
        // 방 옵션 설정
        roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 6;
        
        // UI 초기화
        UpdatePlayerCountUI();
    }


    private void Start()
    {
        PhotonNetwork.Instantiate("PF player", player.transform.position, Quaternion.identity, 0);
    }

    private void Update()
    {
        
    }
    
    //플레이어가 방에 들어올때 호출되는 콜백
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerCountUI();
        if (PhotonNetwork.CurrentRoom.PlayerCount == roomOptions.MaxPlayers)
        {
            GameStart();
        }
    }
    
    //플레이어가 방에 나갈때 호출되는 콜백
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerCountUI();
    }
    
    //참가자 수UI를 갱신하는 메서드
    public void UpdatePlayerCountUI()
    {
        textMesh.text = "Number of participants :" + PhotonNetwork.CurrentRoom.PlayerCount;
    }

    private void GameStart()
    {
        PhotonNetwork.LoadLevel("Main");
    }
}
