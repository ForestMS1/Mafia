using System;
using System.Net.Mime;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
   private string gameVersion = "1.0";
   
   //접속상태를 나타내는 텍스트
   public TextMeshProUGUI connectionInfoText;
   //마스터 서버 접속하는 버튼
   public Button joinButton;
   public Button createRoomButton;
   
   //게임 시작과 동시에 마스터 서버 접속 시도
   private void Start()
   {
       PhotonNetwork.GameVersion = gameVersion;
        
       //마스터 서버 접속
       PhotonNetwork.ConnectUsingSettings();
        
       //접속하는동안 접속버튼 비활성화
       joinButton.interactable = false;
       createRoomButton.interactable = false;
       connectionInfoText.text = "Connecting to master server...";
   }
    
   //마스터 서버 접속 성공시 자동 호출되는 메서드
   public override void OnConnectedToMaster()
   {
       joinButton.interactable = true;
       createRoomButton.interactable = true;
       connectionInfoText.text = "Master server connection successful!";
   }
    
   //마스터 서버 접속 실패시 자동 호출되는 메서드
   public override void OnDisconnected(DisconnectCause cause)
   {
       joinButton.interactable = false;
       createRoomButton.interactable = false;
       connectionInfoText.text = "Master server connection failed,'\nattempting to reconnect";

       PhotonNetwork.ConnectUsingSettings();
   }
    
   //JOIN 버튼 클릭시 룸 접속하는 메서드
   public void Connect()
   {
       //중복 접송 방지를 위해 버튼 비활성화
       joinButton.interactable = false;

       if (PhotonNetwork.IsConnected)
       {
           //마스터 서버와 접속했다면 룸 접속 시도
           connectionInfoText.text = "Looking for an empty room...";
           PhotonNetwork.JoinRandomRoom();
       }
       else
       {
           //마스터 서버 재접속
           connectionInfoText.text = "Master server connection failed,'\nattempting to reconnect";
           PhotonNetwork.ConnectUsingSettings();
       }
   }
   
   //CreateRoom 버튼 클릭시 룸 생성하는 메서드
   public void CreateRoom()
   {
       createRoomButton.interactable = false;

       if (PhotonNetwork.IsConnected)
       {
           //마스터 서버와 접속했다면 룸 생성 시도
           connectionInfoText.text = "create new room...";
           PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 6 });
       }
       else
       {
           //마스터 서버 재접속
           connectionInfoText.text = "Master server connection failed,'\nattempting to reconnect";
           PhotonNetwork.ConnectUsingSettings();
       }
   }
   
   

   public override void OnJoinRandomFailed(short returnCode, string message)
   {
       connectionInfoText.text = "No empty rooms, create new room...";
       
       //최대 6인 수용 가능한 방 생성
       PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 6 });
   }
   public override void OnJoinedRoom()
   {
       connectionInfoText.text = "Room participation successful!";
       
       //모든 룸 참가자가 씬을 로드하게 함
       PhotonNetwork.LoadLevel("WaitingRoom");
   }

   public void ExitGame()
   {
       if (UnityEditor.EditorApplication.isPlaying)
       {
           UnityEditor.EditorApplication.isPlaying = false;
       }
       else
       {
           Application.Quit();
       }
   }
}
