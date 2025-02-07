using System.Collections.Generic;
using Cainos.PixelArtTopDown_Basic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;
    
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
                playerRoles[players[i].ActorNumber] = "마피아";
            }
            else if(i == 1)
            {
                playerRoles[players[i].ActorNumber] = "경찰";
            }
            else if (i == 2)
            {
                playerRoles[players[i].ActorNumber] = "의사";
            }
            else
            {
                playerRoles[players[i].ActorNumber] = "시민";
            }
        }
    }

    [PunRPC]
    public void RPC_SetGameState(int state)
    {
        currentState = (GameState)state;
        // 상태에 따라 UI 및 로직을 초기화하는 추가 작업을 실행
        Debug.Log("Game state changed to: " + currentState);
    }
}
