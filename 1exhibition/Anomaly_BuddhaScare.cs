using System.Collections;
using UnityEngine;

public class Anomaly_BuddhaScare : MonoBehaviour
{
    [Header("Anomaly ID Config")]
    [Tooltip("GameManager에서 판정할 이 기믹의 고유 ID")]
    public int anomalyID = 2; // 💡 불두 ID: 2번으로 지정

    public Light zoneLight;
    public GameObject scareObject; // 카메라 바로 앞에 매달아둔 불두 오브젝트
    public AudioSource scareAudio;
    private bool isTriggered = false;

    private void Start()
    {
        if (scareObject != null)
        {
            scareObject.SetActive(false);
        }
    }

    // 매니저가 껐다 켤 때 상태 자동 리셋
    private void OnEnable()
    {
        isTriggered = false;
        if (scareObject != null) scareObject.SetActive(false);
        if (zoneLight != null) zoneLight.enabled = true;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        if (scareObject != null) scareObject.SetActive(false);
        if (zoneLight != null) zoneLight.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // [테스트용 로그] 트리거 작동 확인
        Debug.Log($"불두 퇴장 구역 진입 완료: {other.name}");

        // 🚨 [ID 일치 체크 수정] 하드코딩된 '2' 대신 'anomalyID' 변수 비교
        if (GameManager.Instance == null || GameManager.Instance.currentAnomalyID != anomalyID || isTriggered)
            return;

        isTriggered = true;
        StartCoroutine(PlayBuddhaEvent());
    }

    IEnumerator PlayBuddhaEvent()
    {
        if (zoneLight == null || scareObject == null || scareAudio == null)
        {
            Debug.LogError("인스펙터 창에서 zoneLight, scareObject, scareAudio 중 비어있는 칸이 있습니다!");
            yield break;
        }

        // 1. 조명 깜빡이기
        for (int i = 0; i < 3; i++)
        {
            zoneLight.enabled = false;
            yield return new WaitForSeconds(0.2f);
            zoneLight.enabled = true;
            yield return new WaitForSeconds(0.3f);
        }

        // 2. 클라이맥스 점프스케어
        zoneLight.enabled = false;
        yield return new WaitForSeconds(0.1f);

        scareObject.SetActive(true);
        scareAudio.Play();

        yield return new WaitForSeconds(1.5f);
        scareObject.SetActive(false);
        zoneLight.enabled = true;
    }
}