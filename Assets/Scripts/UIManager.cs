using Photon.Pun;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameStateText;
    [SerializeField] private TextMeshProUGUI PlayerRoleText;
    
    // 게임 상태와 역할 정보(예시로 GameManager나 PlayerInfo에서 가져올 수 있음)
    // 이 예제에서는 간단히 문자열을 사용합니다.
    private string currentGameState = "Waiting";
    private string currentPlayerRole = "Not decided";

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
}
