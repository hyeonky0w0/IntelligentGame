using UnityEngine;

public class FloorZoneTrigger : MonoBehaviour
{
    [Header("층 설정")]
    [Tooltip("이 트리거를 지나면 바뀔 층 번호 (1~3)")]
    public int targetFloor = 1;

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어가 이 구역(전시실 입구/계단 끝)에 들어왔을 때만 실행
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                // GameManager에게 현재 플레이어가 물리적으로 몇 층에 진입했는지 실시간 통보
                GameManager.Instance.UpdateCurrentFloor(targetFloor);
            }
        }
    }
}