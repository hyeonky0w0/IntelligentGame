using UnityEngine;

public class KillerEscapeZone : MonoBehaviour
{
    [Tooltip("이 탈출 구역이 속한 층 번호 (기본값 2층)")]
    public int floorNumber = 2;

    private bool isZoneActive = false;

    /// <summary>
    /// 킬러가 출현했을 때 추격 전력질주 시퀀스에서 이 탈출구를 활성화하기 위해 호출합니다.
    /// </summary>
    public void SetEscapeZoneActive(bool active)
    {
        isZoneActive = active;
        Debug.Log($"🚪 [탈출구 상태 변경] {floorNumber}층 탈출구 감지 센서 작동 여부: {active}");
    }
    private void OnTriggerEnter(Collider other)
    {
        // 활성화되지 않은 상태(평상시)라면 플레이어가 지나가도 무시합니다.
        if (!isZoneActive) return;

        // 플레이어가 아니면 무시
        if (!other.CompareTag("Player"))
        {
            Debug.Log($"[탈출구 접촉] 플레이어가 아닌 오브젝트({other.name})가 지나감 - 무시");
            return;
        }

        Debug.LogError($"🎉 [탈출 대성공] 플레이어가 {floorNumber}층 전시관 구역 이탈에 성공했습니다!");

        // 중복 탈출 연산 방지 가드
        isZoneActive = false;

        // 1. 숨어있던 킬러의 추격을 즉시 종료하고 은신시킵니다.
        if (KillerAI.Instance != null)
        {
            KillerAI.Instance.StopChase();
        }
        else
        {
            Debug.LogError("[오류] 씬에 KillerAI 인스턴스를 찾을 수 없어 추격을 멈추지 못했습니다.");
        }

        // 2. 게임 매니저와 연동하여 출구 문 상태를 정비합니다.
        if (GameManager.Instance != null)
        {
            // 🚨 [순서 교정 핵심 포인트]
            // isKillerEvent = false를 여기서 먼저 꺼버리면 매니저가 일반 채션을 하므로 지워버립니다!
            // 매니저 채점소 내부에서 연산이 끝난 후 알아서 꺼주도록 설계했으므로, 
            // 여기서는 정직하게 일지 통과 함수만 다이렉트로 기동합니다.
            GameManager.Instance.SubmitReport(false);
        }
    }
}