using UnityEngine;

public class CeladonDollMovement : MonoBehaviour
{
    [Header("Anomaly ID Config")]
    public int anomalyID = 32;

    public Transform dollTransform;
    public Transform glassTargetPosition;
    public float moveSpeed = 2f;
    public float triggerDistance = 15f;

    [Header("오디오 설정")]
    public AudioSource audioSource;   // 이동할 때 소리 낼 오디오 소스
    public AudioClip dollMoveSound;   // 인형 이동 효과음 (루프 음원 권장)

    private Transform player;
    private Transform mainCamera;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isClimax = false;

    private void Awake()
    {
        if (dollTransform != null)
        {
            originalPosition = dollTransform.localPosition;
            originalRotation = dollTransform.localRotation;
        }
        else
        {
            Debug.LogError($"[CeladonDoll] {gameObject.name}에 DollTransform이 비어있습니다!");
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // 🔊 오디오 원활한 반복 재생을 위한 초기 세팅
        if (audioSource != null && dollMoveSound != null)
        {
            audioSource.clip = dollMoveSound;
            audioSource.loop = true; // 다가오는 동안 계속 나야 하므로 루프를 켭니다.
            audioSource.playOnAwake = false;
        }
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("[CeladonDoll] 'Player' 태그를 가진 오브젝트를 찾을 수 없습니다!");
        }

        if (Camera.main != null)
        {
            mainCamera = Camera.main.transform;
        }
    }

    private void OnEnable()
    {
        if (originalPosition != Vector3.zero || originalRotation != Quaternion.identity)
        {
            ResetAnomaly();
        }
    }

    private void OnDisable()
    {
        ResetAnomaly();
    }

    private void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.currentAnomalyID != anomalyID) return;
        if (player == null || dollTransform == null || glassTargetPosition == null) return;

        float distance = Vector3.Distance(dollTransform.position, player.position);

        // 뼈대 체크: 사운드를 꺼야 하는 기본 상태
        bool isMovingThisFrame = false;

        if (distance <= triggerDistance && !isClimax)
        {
            Vector3 lookDirection = mainCamera != null ? mainCamera.forward : player.forward;
            Vector3 dirToDoll = (dollTransform.position - (mainCamera != null ? mainCamera.position : player.position)).normalized;

            float dot = Vector3.Dot(lookDirection, dirToDoll);
            bool isPlayerLooking = (dot > 0.4f);

            // 플레이어가 안 보고 있을 때만 이동 수행
            if (!isPlayerLooking)
            {
                Debug.Log("[CeladonDoll] 플레이어가 안 보고 있음! 인형 이동 중...");
                isMovingThisFrame = true; // 이번 프레임에 이동 중임을 표시

                dollTransform.localPosition = Vector3.MoveTowards(
                    dollTransform.localPosition,
                    glassTargetPosition.localPosition,
                    moveSpeed * Time.deltaTime
                );

                if (Vector3.Distance(dollTransform.localPosition, glassTargetPosition.localPosition) < 0.05f)
                {
                    TriggerClimax();
                    isMovingThisFrame = false; // 다 도착했으므로 사운드 정지 유도
                }
            }
        }

        // 🔊 실시간 사운드 재생/정지 제어
        ControlMoveSound(isMovingThisFrame);
    }

    private void ControlMoveSound(bool shouldPlay)
    {
        if (audioSource == null || dollMoveSound == null) return;

        if (shouldPlay)
        {
            // 재생 중이 아닐 때만 재생 시작
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            // 이동이 멈추면 사운드도 즉시 정지
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    private void TriggerClimax()
    {
        isClimax = true;

        // 도착 순간 혹시 남아있을 사운드 완전 차단
        if (audioSource != null) audioSource.Stop();

        Vector3 targetDir = player.position - dollTransform.position;
        targetDir.y = 0;

        if (targetDir != Vector3.zero)
        {
            dollTransform.rotation = Quaternion.LookRotation(targetDir);
        }

        Debug.LogWarning("🚨 [이상현상 발동] 청자 인형 클라이맥스! 유리에 밀착 완료");
    }

    public void ResetAnomaly()
    {
        isClimax = false;

        if (audioSource != null)
        {
            audioSource.Stop();
        }

        if (dollTransform != null)
        {
            dollTransform.localPosition = originalPosition;
            dollTransform.localRotation = originalRotation;
            Debug.Log("[CeladonDoll] 인형 위치 및 사운드가 초기화되었습니다.");
        }
    }
}