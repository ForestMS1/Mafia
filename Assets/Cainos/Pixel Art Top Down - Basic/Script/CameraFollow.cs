using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Cainos.PixelArtTopDown_Basic
{
    //let camera follow target
    public class CameraFollow : MonoBehaviour
    {
        public Transform target;
        public float lerpSpeed = 1.0f;

        private Vector3 offset;

        private Vector3 targetPos;
        private PhotonView targetPhotonView;

        private void Start()
        {
            if (target == null) return;
            
            targetPhotonView = target.GetComponent<PhotonView>();
            
            // 화면 중앙에 플레이어를 두기 위해 X, Y 오프셋은 0으로, Z는 카메라와 플레이어의 거리로 설정
            offset = new Vector3(0, 0, transform.position.z - target.position.z);
        }

        private void Update()
        {
            if (target == null) return;

            if (targetPhotonView == null || !targetPhotonView.IsMine) return;
            
            targetPos = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);
        }

    }
}