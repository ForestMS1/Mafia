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
    //각 플레이어 생존 정보를 저장할 딕셔너리
    public Dictionary<int, bool> playerAliveStatus = new Dictionary<int, bool>();

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
    
    //플레이어에게 직업할당하는 메서드
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
            RegisterPlayer(players[i].ActorNumber);
        }
    }
    // 플레이어 생존상태 초기화 메서드
    public void RegisterPlayer(int actorNumber)
    {
        if (!playerAliveStatus.ContainsKey(actorNumber))
        {
            playerAliveStatus.Add(actorNumber, true);
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

        StartCoroutine("NightPhaseRoutine");
    }
    // 밤 단계 루틴: 마피아, 의사, 경찰 선택 단계를 순차적으로 진행
    private IEnumerator NightPhaseRoutine()
    {
        // 1. 마피아 선택 단계
        Debug.Log("마피아 선택 단계 시작");
        if (uiManager != null)
        {
            uiManager.ShowSelectionUI(true);
        }
        // 마피아가 선택할 수 있도록 nightDuration 동안 대기
        yield return new WaitForSeconds(nightDuration);
        if (uiManager != null)
        {
            uiManager.ShowSelectionUI(false);
        }
        int mafiaTarget = NightPhaseManager.instance.mafiaTargetActor; // 예: -1이면 미선택

        // 2. 의사 선택 단계
        Debug.Log("의사 선택 단계 시작");
        if (uiManager != null)
        {
            uiManager.ShowSelectionUI(true);
        }
        yield return new WaitForSeconds(nightDuration);
        if (uiManager != null)
        {
            uiManager.ShowSelectionUI(false);
        }
        int doctorTarget = NightPhaseManager.instance.doctorTargetActor;

        // 3. 경찰 선택 단계
        Debug.Log("경찰 선택 단계 시작");
        if (uiManager != null)
        {
            uiManager.ShowSelectionUI(true);
        }
        yield return new WaitForSeconds(nightDuration);
        if (uiManager != null)
        {
            uiManager.ShowSelectionUI(false);
        }
        int policeTarget = NightPhaseManager.instance.policeTargetActor;
        // 경찰은 선택과 동시에 조사 기능이 실행될 수 있도록 별도로 처리(여기서는 이미 NightPhaseManager 또는 GameManager의 다른 함수에서 처리)
        int policeActor = -1;
        foreach (var p in playerRoles)
        {
            if (p.Value == "Police")
            {
                policeActor = p.Key;
            }
        }

        if (policeActor != -1)
        {
            RequestPoliceInvestigation(policeTarget, policeActor);
        }
        // 밤 단계 종료 → 결과 계산
        NightPhaseManager.ResolveNightPhase(mafiaTarget, doctorTarget);

        // 밤이 끝나고 낮으로 전환
        photonView.RPC("RPC_SetGameState", RpcTarget.All, (int)GameState.Day);
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
    
    // PlayerInfo의 Die() 함수에서 호출하여 상태 업데이트
    public void OnPlayerDied(int actorNumber)
    {
        if (playerAliveStatus.ContainsKey(actorNumber))
        {
            playerAliveStatus[actorNumber] = false;
            // 추가로 UI 업데이트, 게임 로직 처리(예: 승리 조건 체크) 등 수행
            Debug.Log($"GameManager: 플레이어 {actorNumber}의 상태를 죽음으로 업데이트했습니다.");
        }
    }
}
