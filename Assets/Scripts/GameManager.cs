using System.Collections.Generic;
using Cainos.PixelArtTopDown_Basic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;
    public UIManager uiManager;
    
    //게임 단계 상태 : 토론, 투표, 행동
    public enum GameState {Initialization, Night, Day, Voting, End}

    public GameState currentState = GameState.Initialization;
    public Transform spawnPosition;
    
    //각 플레이어 역할 정보를 저장할 딕셔너리
    public Dictionary<int, string> playerRoles = new Dictionary<int, string>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 플레이어 생성 후 카메라의 target을 설정하는 예시 (예: GameManager에서)
        GameObject localPlayer = PhotonNetwork.Instantiate("PF player", spawnPosition.position, Quaternion.identity, 0);
        Camera.main.GetComponent<CameraFollow>().target = localPlayer.transform;
        
        if (PhotonNetwork.IsMasterClient)
        {
            AssignRoles();
            // 모든 클라이언트에 역할 정보를 전달
            photonView.RPC("RPC_SetGameState", RpcTarget.All, (int)GameState.Night);
        }
    }

    public void AssignRoles()
    {
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            if (i == 0)
            {
                playerRoles[players[i].ActorNumber] = "Mafia";
            }
            else if(i == 1)
            {
                playerRoles[players[i].ActorNumber] = "Police";
            }
            else if (i == 2)
            {
                playerRoles[players[i].ActorNumber] = "Doctor";
            }
            else
            {
                playerRoles[players[i].ActorNumber] = "citizen";
            }
        }
    }
    
    [PunRPC]
    public void RPC_SetGameState(int state)
    {
        currentState = (GameState)state;
        Debug.Log("Game state changed to: " + currentState);

        // UIManager를 통해 게임 상태 텍스트 업데이트
        if (uiManager != null)
        {
            uiManager.UpdateGameStateUI(currentState.ToString());
        }
        
        // 로컬 플레이어의 역할 정보를 UI에 업데이트하기
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        if (playerRoles.ContainsKey(actorNumber) && uiManager != null)
        {
            uiManager.UpdatePlayerRoleUI(playerRoles[actorNumber]);
        }
    }
}
