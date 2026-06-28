using UnityEngine;

public class Anomaly_JewelryAudio : MonoBehaviour
{
    [Header("Anomaly ID Config")]
    [Tooltip("GameManager에서 판정할 이 기믹의 고유 ID")]
    public int anomalyID = 3; // 💡 여기에 변수가 없어서 에러가 났던 것입니다! 3번으로 설정합니다.

    public AudioSource audioSource; // 2D 세팅 필수 (Spatial Blend = 0)
    private bool isPlaying = false;

    // 매니저가 새로 순찰을 돌리거나 오브젝트를 껐다 켤 때 상태 리셋
    private void OnEnable()
    {
        isPlaying = false;
    }

    private void OnDisable()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // [테스트용 로그]
        Debug.Log($"장신구 구역 진입 완료: {other.name}");

        // 🚨 [GameManager 체크 및 중복 조건 제거]
        // 매니저의 당첨 번호가 내 번호(3번)가 아니거나, 이미 재생 중이라면 리턴합니다.
        if (GameManager.Instance == null || GameManager.Instance.currentAnomalyID != anomalyID || isPlaying)
            return;

        if (other.CompareTag("Player"))
        {
            isPlaying = true;
            if (audioSource != null)
            {
                audioSource.Play();
            }
            Debug.Log("<color=#FFFF33><b>[장신구]</b></color> 근원 없는 루프 사운드 재생 시작!");
        }
    }
}