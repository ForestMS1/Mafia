using Photon.Pun;
using UnityEngine;

public class NightPhaseManager : MonoBehaviourPunCallbacks
{
    public static NightPhaseManager instance;
    
    //각 역할이 선택한 대상(초기값 -1: 선택되지 않음)
    public int mafiaTargetActor = -1;
    public int doctorTargetActor = -1;
    public int policeTargetActor = -1;

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

    public void SetMafiaTarget(int targetActor)
    {
        mafiaTargetActor = targetActor;
        Debug.Log("마피아가 선택한 대상 : " + targetActor);
    }

    public void SetDoctorTarget(int targetActor)
    {
        doctorTargetActor = targetActor;
        Debug.Log("의사가 선택한 대상 : " + targetActor);
    }

    public void SetPoliceTarget(int targetActor)
    {
        policeTargetActor = targetActor;
        Debug.Log("경찰이 선택한 대상 : " + targetActor);
        //경찰은 바로 조사 결과를 받아볼 수 있도록 요청한다.
        GameManager.instance.RequestPoliceInvestigation(targetActor, PhotonNetwork.LocalPlayer.ActorNumber);
    }
    
    //밤 단계 종료 시 호출하여 결과를 처리
    public void ResolveNightPhase()
    {
        int killedActor = -1; //기본적으로 아무도 죽지 않는 상태
        
        //만약 마피아가 선택했고, 그 대상이 의사가 보호한 대상과 다르면 죽음 처리
        if (mafiaTargetActor != -1 && mafiaTargetActor != doctorTargetActor)
        {
            killedActor = mafiaTargetActor;
        }
        
        //결과를 GameManager에 전달하여 네트워크 동기화 및 UI 업데이트를 처리
        GameManager.instance.photonView.RPC("RPC_SetNightOutcome", RpcTarget.All, killedActor);
    }
}
