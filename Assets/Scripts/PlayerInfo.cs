using Cainos.PixelArtTopDown_Basic;
using Photon.Pun;
using UnityEngine;

public class PlayerInfo : MonoBehaviourPunCallbacks
{
    public bool isAlive;
    
    
    // 플레이어가 죽었을 때 호출
    public void Die()
    {
        if (!isAlive) return;  // 이미 죽은 상태면 무시

        isAlive = false;
        Debug.Log($"플레이어 {photonView.Owner.ActorNumber}가 죽었습니다.");

        // 예: 이동 컴포넌트를 비활성화하거나, 애니메이션 재생, 색상 변경 등
        TopDownCharacterController movement = GetComponent<TopDownCharacterController>();
        if (movement != null)
        {
            movement.enabled = false;
        }
        // 추가로, 죽은 플레이어 오브젝트에 대해 투명도 변경 또는 다른 시각적 효과 적용 가능

        // 게임매니저에게 알림 (예: 사망한 플레이어를 기록)
        GameManager.instance.OnPlayerDied(photonView.Owner.ActorNumber);
    }
}
