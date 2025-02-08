using System.Collections;
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
    
    // 각 상태의 지속 시간 (예시: 초 단위)
    public float nightDuration = 60f;
    public float dayDuration = 90f;
    public float votingDuration = 60f;
    
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
            // 밤 단계로 전환
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
            int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
            if (playerRoles.ContainsKey(actorNumber))
            {
                uiManager.UpdatePlayerRoleUI(playerRoles[actorNumber]);
            }
        }
    }
    
    // 경찰의 조사 요청을 처리하는 함수
    // policeActor: 조사를 요청한 경찰의 ActorNumber
    public void RequestPoliceInvestigation(int targetActor, int policeActor)
    {
        string role = "";
        bool isMafia = false;
        if (playerRoles.TryGetValue(targetActor, out role))
        {
            //특정 경찰 클라이언트에게 조사 결과를 보내기 위해 RPC호출 (대상 클라이언트에게만 전송)
            Player policePlayer = null;
            foreach (var p in PhotonNetwork.PlayerList)
            {
                if (p.ActorNumber == policeActor)
                {
                    policePlayer = p;
                    break;
                }
            }

            if (role == "Mafia") isMafia = true;
            
            if (policePlayer != null)
            {
                photonView.RPC("RPC_SendPoliceResult", policePlayer, isMafia);
            }
        }
    }

    [PunRPC]
    public void RPC_SendPoliceResult(bool isMafia)
    {
        //이 RPC는 경찰 역할을 가진 플레이어 클라이언트에서 호출된다.
        Debug.Log("As a result of the police investigation, the subjects were Mafia: " + isMafia);
        // uiManager 또는 별도의 경찰 전용 UI를 통해 결과를 표시한다.
        if (uiManager != null)
        {
            uiManager.ShowPoliceInvestigationResult(isMafia);
        }
    }

    [PunRPC]
    public void RPC_SetNightOutcome(int killedActor)
    {
        if (killedActor != -1)
        {
            Debug.Log("밤에 죽은 플레이어: " + killedActor);
            if (uiManager != null)
            {
                uiManager.UpdateNightOutcomeUI("밤에 죽은 플레이어: " + killedActor);
            }
            // 추가: 죽은 플레이어의 상태 변경(예: 움직임 중지, 색상 변경 등)
        }
        else
        {
            Debug.Log("밤에 아무도 죽지 않았습니다.");
            if (uiManager != null)
            {
                uiManager.UpdateNightOutcomeUI("밤에 아무도 죽지 않았습니다.");
            }
        }
    }
}
