using UnityEngine;

public class HorseRiderClayTest : MonoBehaviour
{
    public static HorseRiderClayTest Instance { get; private set; }

    [Header("Anomaly ID Config")]
    [Tooltip("GameManager에서 판정할 이 기믹의 고유 고유 ID")]
    public int anomalyID = 1; // 기마인물형 토기 ID: 1번

    [Header("References")]
    public Transform playerTransform;
    public Transform exhibitCenter;

    [Header("Audio Sources")]
    public AudioSource hoofbeatAudioSource;
    public AudioSource suddenStopAudioSource;

    [Header("Distance Settings")]
    public float maxDistance = 15f;
    public float triggerDistance = 2f;

    [Header("Audio Settings")]
    [Range(0f, 1f)] public float maxHoofbeatVolume = 1f;

    private bool isAnomalyTriggered = false;
    private bool isClimaxPlayed = false;

    private bool wasInDistanceZone = false;
    private float lastLoggedVolume = -1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (hoofbeatAudioSource == null || suddenStopAudioSource == null)
        {
            Debug.LogError("[오류] 오디오 소스가 인스펙터에 할당되지 않았습니다!");
        }

        if (playerTransform == null)
        {
            playerTransform = Camera.main?.transform;
        }

        // 매니저 연동을 위해 초기 사운드 세팅은 OnEnable로 이동합니다.
    }

    // 매니저에 의해 오브젝트가 활성화될 때마다 상태와 사운드 초기화
    private void OnEnable()
    {
        ResetTest();
    }

    // 오브젝트가 비활성화되면 재생 중이던 사운드를 완전히 정지
    private void OnDisable()
    {
        if (hoofbeatAudioSource != null) hoofbeatAudioSource.Stop();
        if (suddenStopAudioSource != null) suddenStopAudioSource.Stop();
    }

    private void Update()
    {
        // 🚨 [GameManager 연동 핵심 코드]
        // GameManager가 없거나, 현재 활성화된 이상현상 ID가 '1'이 아니라면 로직을 실행하지 않고 리턴합니다.
        if (GameManager.Instance == null || GameManager.Instance.currentAnomalyID != anomalyID)
        {
            // 혹시 사운드가 흐르고 있다면 즉시 뮤트/정지 처리
            if (hoofbeatAudioSource != null && hoofbeatAudioSource.isPlaying)
            {
                hoofbeatAudioSource.volume = 0f;
            }
            return;
        }

        if (playerTransform == null || exhibitCenter == null) return;

        // 1. 플레이어와 전시구역 간의 거리 계산
        float distance = Vector3.Distance(playerTransform.position, transform.position);

        // 2. 클라이맥스 연출이 아직 안 끝났을 때의 로직
        if (!isClimaxPlayed)
        {
            HandleHoofbeatVolume(distance);
            CheckClimaxCondition(distance);
        }
    }

    private void HandleHoofbeatVolume(float distance)
    {
        if (distance <= maxDistance && distance > triggerDistance)
        {
            float t = 1f - ((distance - triggerDistance) / (maxDistance - triggerDistance));
            float targetVolume = Mathf.Clamp(t, 0f, 1f) * maxHoofbeatVolume;

            // 말발굽 소리가 정지 상태였다면 재생 시작
            if (hoofbeatAudioSource != null && !hoofbeatAudioSource.isPlaying)
            {
                hoofbeatAudioSource.Play();
            }

            hoofbeatAudioSource.volume = targetVolume;

            if (Mathf.Abs(targetVolume - lastLoggedVolume) > 0.15f)
            {
                Debug.Log($"[소리 변화] 현재 거리: {distance:F1}m | 말발굽 사운드 볼륨: {(targetVolume * 100):F0}%");
                lastLoggedVolume = targetVolume;
            }
            wasInDistanceZone = true;
        }
        else if (distance > maxDistance)
        {
            if (hoofbeatAudioSource != null) hoofbeatAudioSource.volume = 0f;

            if (wasInDistanceZone)
            {
                Debug.Log($"[범위 벗어남] 현재 거리: {distance:F1}m -> 말발굽 소리 음소거.");
                wasInDistanceZone = false;
                lastLoggedVolume = -1f;
            }
        }
    }

    private void CheckClimaxCondition(float distance)
    {
        if (!isAnomalyTriggered && distance <= triggerDistance)
        {
            isAnomalyTriggered = true;
            Debug.LogWarning($"🟢 [이벤트 진입] 토기 앞 {distance:F1}m 지점 진입 완료!");
        }

        if (isAnomalyTriggered && !isClimaxPlayed)
        {
            Vector3 dirToExhibit = (exhibitCenter.position - playerTransform.position).normalized;
            float dotProduct = Vector3.Dot(playerTransform.forward, dirToExhibit);

            if (dotProduct < -0.2f)
            {
                Debug.LogWarning($"↩️ [조건 충족] 플레이어가 뒤를 돌아보았습니다! -> 클라이맥스 발동!");
                TriggerClimax();
            }
        }
    }

    private void TriggerClimax()
    {
        isClimaxPlayed = true;

        if (hoofbeatAudioSource != null)
        {
            hoofbeatAudioSource.Stop();
        }

        if (suddenStopAudioSource != null)
        {
            Vector3 backPosition = playerTransform.position - (playerTransform.forward * 2f);
            suddenStopAudioSource.transform.position = backPosition;
            suddenStopAudioSource.Play();
            Debug.LogError("🚨 [클라이맥스 실행] 플레이어 바로 뒤에서 말이 급정거했습니다!");
        }
    }

    [ContextMenu("Reset Anomaly Test")]
    public void ResetTest()
    {
        isAnomalyTriggered = false;
        isClimaxPlayed = false;
        wasInDistanceZone = false;
        lastLoggedVolume = -1f;

        if (hoofbeatAudioSource != null)
        {
            hoofbeatAudioSource.loop = true;
            hoofbeatAudioSource.volume = 0f;
        }

        if (suddenStopAudioSource != null)
        {
            suddenStopAudioSource.loop = false;
        }
    }
}