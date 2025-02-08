using Photon.Pun;
using TMPro;
using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameStateText;
    [SerializeField] private TextMeshProUGUI PlayerRoleText;
    
    // 게임 상태와 역할 정보(예시로 GameManager나 PlayerInfo에서 가져올 수 있음)
    // 이 예제에서는 간단히 문자열을 사용합니다.
    private string currentGameState = "Waiting";
    private string currentPlayerRole = "Not decided";

    public Canvas outComeUI;
    public Canvas policeOutComeUI;

    private void Awake()
    {
        if (outComeUI != null)
        {
            outComeUI.gameObject.SetActive(false);
        }

        if (policeOutComeUI != null)
        {
            policeOutComeUI.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        // 초기 UI 업데이트
        UpdateGameStateUI(currentGameState);
        UpdatePlayerRoleUI(currentPlayerRole);
    }

    public void UpdateGameStateUI(string newState)
    {
        currentGameState = newState;
        if (gameStateText != null)
        {
            gameStateText.text = "Current status : " + currentGameState;
        }
    }
    
    public void UpdatePlayerRoleUI(string newRole)
    {   
        currentPlayerRole = newRole;
        if (PlayerRoleText != null)
        {
            PlayerRoleText.text = "Current Job : " + currentPlayerRole;
        }
    }
    
    // 결과창을 켜고 텍스트를 업데이트하는 메서드
    public void UpdateNightOutcomeUI(string outcome)
    {
        if (outComeUI != null)
        {
            outComeUI.gameObject.SetActive(true);
            Transform whoKilledTextTransform = outComeUI.transform.Find("whoKilledTXT");
            TextMeshProUGUI whoKilledText = whoKilledTextTransform.GetComponent<TextMeshProUGUI>();
            whoKilledText.text = outcome;
            
            Transform survivorsTextTransform = outComeUI.transform.Find("survivorsTXT");
            TextMeshProUGUI survivorsText = survivorsTextTransform.GetComponent<TextMeshProUGUI>();
            survivorsText.text = PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;

            StartCoroutine("CloseOutComeUI");
        }
    }
    
    //일정 시간 후 결과창 끄기 (코루틴)
    IEnumerator CloseOutComeUI()
    {
        yield return new WaitForSeconds(7.0f);
        outComeUI.gameObject.SetActive(false);
    }
    
    //경찰의 조사결과 UI를 켜는 메서드
    public void ShowPoliceInvestigationResult(bool isMafia)
    {
        if (policeOutComeUI != null)
            policeOutComeUI.gameObject.SetActive(true);
        Transform mafiaResultTransform = policeOutComeUI.transform.Find("mafiaResultTXT");
        TextMeshProUGUI mafiaResultText = mafiaResultTransform.GetComponent<TextMeshProUGUI>();
        mafiaResultText.text = "This is Mafia? : " + isMafia;
        StartCoroutine("ClosePoliceOutComeUI");
        //policeOutComeUI.GetComponentInChildren<TextMeshProUGUI>.text = "경찰 조사 결과: " + role;
    }
    
    //일정 시간 후 경찰 결과창 끄기 (코루틴)
    IEnumerator ClosePoliceOutComeUI()
    {
        yield return new WaitForSeconds(5.0f);
        policeOutComeUI.gameObject.SetActive(false);
    }

    public void ShowSelectionUI(bool selected)
    {
        //selected가 true면 UI를 켠다
        if (selected)
        {
            //현재 생존해있는 player목록을 나열한다.
        }
        else
        {
            //UI를 끈다.
        }
    }
    
}
