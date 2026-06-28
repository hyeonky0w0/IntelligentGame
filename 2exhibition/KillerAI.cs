using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
public class KillerAI : MonoBehaviour
{
    public static KillerAI Instance { get; private set; }

    [Header("추격 설정")]
    public float chaseSpeed = 4.5f;
    public float catchDistance = 1.5f;

    [Header("참조")]
    public Transform playerTransform;
    public Animator killerAnimator;

    [Header("탈출 구역")]
    public KillerEscapeZone[] escapeZones;

    [Header("사운드")]
    public AudioClip chaseStartSound;
    public AudioClip caughtSound;
    private AudioSource audioSource;

    [Header("연출")]
    public float chaseStartDelay = 1.5f;

    private NavMeshAgent agent;
    private bool isChasing = false;
    private float lastDistanceLogTime = 0f;

    private readonly string ANIM_SPEED = "Speed";
    private readonly string ANIM_CATCH = "Catch";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("[KillerAI] 싱글톤 인스턴스 등록 완료.");
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();

        if (killerAnimator == null)
        {
            killerAnimator = GetComponent<Animator>();
        }

        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
        }
    }

    private void Start()
    {
        if (agent != null)
        {
            agent.speed = chaseSpeed;
            agent.isStopped = true;
        }

        // 처음 시작할 때 킬러를 숨깁니다 (메모리는 유지, 그래픽과 물리만 차단)
        SetKillerVisibility(false);
    }

    private void Update()
    {
        if (!isChasing || playerTransform == null || agent == null || !agent.enabled) return;

        // 플레이어 위치 실시간 타겟팅
        agent.SetDestination(playerTransform.position);

        if (killerAnimator != null)
            killerAnimator.SetFloat(ANIM_SPEED, agent.velocity.magnitude);

        float dist = Vector3.Distance(transform.position, playerTransform.position);

        if (Time.time - lastDistanceLogTime > 1.0f)
        {
            Debug.Log($"🏃‍♂️ [추격 중] 플레이어와의 남은 거리: {dist:F1}m");
            lastDistanceLogTime = Time.time;
        }

        if (dist <= catchDistance)
        {
            CatchPlayer();
        }
    }

    public void StartChase()
    {
        // 추격 시작 시 그래픽과 인공지능 연산 안전 기동
        SetKillerVisibility(true);
        StartCoroutine(ChaseStartSequence());
    }

    public void StopChase()
    {
        isChasing = false;

        if (agent != null)
        {
            agent.ResetPath();
            agent.isStopped = true;
        }

        if (killerAnimator != null)
            killerAnimator.SetFloat(ANIM_SPEED, 0f);

        // 🎵 추격이 끝났으므로 오디오 재생을 즉시 중단합니다.
        if (audioSource != null)
        {
            audioSource.Stop();
        }

        // 🚨 모든 탈출구 기능을 안전하게 끕니다.
        foreach (var zone in escapeZones)
        {
            if (zone != null) zone.SetEscapeZoneActive(false);
        }

        Debug.LogWarning("🛑 [KillerAI] 추격 완료. 킬러가 은신 모드로 안전하게 복귀합니다.");

        // 다시 완벽하게 숨김 처리
        SetKillerVisibility(false);
    }

    private void SetKillerVisibility(bool isVisible)
    {
        // 1. 메쉬 렌더러만 골라서 온/오프
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (var render in renderers)
        {
            render.enabled = isVisible;
        }

        // 2. NavMeshAgent 컴포넌트 자체를 끄지 않고, 이동 및 경로 연산만 통제 (좌표 탈출 방지)
        if (agent != null)
        {
            if (isVisible)
            {
                agent.isStopped = false;
            }
            else
            {
                agent.ResetPath();
                agent.isStopped = true;
            }
        }

        // 3. 메인 루트의 충돌 콜라이더만 제어 (자식 뼈대 콜라이더 간섭 방지)
        Collider mainCollider = GetComponent<Collider>();
        if (mainCollider != null)
        {
            mainCollider.enabled = isVisible;
        }
    }

    private System.Collections.IEnumerator ChaseStartSequence()
    {
        Debug.LogWarning("💀 [이벤트 발생] 귀형문 킬러 출현! 연출 시퀀스 기동.");

        // 🎵 BGM 루프 세팅 후 완벽 재생
        if (audioSource != null && chaseStartSound != null)
        {
            audioSource.clip = chaseStartSound;
            audioSource.loop = true;
            audioSource.Play();
        }

        if (agent != null)
            agent.isStopped = true;

        yield return new WaitForSeconds(chaseStartDelay);

        isChasing = true;
        if (agent != null)
        {
            agent.isStopped = false;
            agent.speed = chaseSpeed;
        }

        int activeZonesCount = 0;
        foreach (var zone in escapeZones)
        {
            if (zone != null)
            {
                zone.SetEscapeZoneActive(true);
                activeZonesCount++;
            }
        }
        Debug.LogError($"🔥 [추격 기동 완료] 활성화된 탈출구 수: {activeZonesCount}");
    }

    private void CatchPlayer()
    {
        if (!isChasing) return;
        isChasing = false;

        if (agent != null)
            agent.isStopped = true;

        StartCoroutine(CaughtPlayerSequence());
    }

    private System.Collections.IEnumerator CaughtPlayerSequence()
    {
        // 🎵 잡혔을 때는 추격 BGM을 끄고 잡힌 단발성 사운드만 재생
        if (audioSource != null)
        {
            audioSource.Stop();
            if (caughtSound != null)
            {
                audioSource.loop = false;
                audioSource.PlayOneShot(caughtSound);
            }
        }

        if (killerAnimator != null)
            killerAnimator.SetTrigger(ANIM_CATCH);

        yield return new WaitForSeconds(1.5f);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver("귀형문 킬러에게 잡혔습니다.");
        }
    }

    [ContextMenu("Test: Start Chase")]
    public void TestStart() => StartChase();
}