using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrbSpawner : MonoBehaviour
{
    [Header("── 필수 연결 ──────────────────")]
    public GameObject orbPrefab;
    public Transform statueTransform;
    public Transform playerTransform;
    public LayerMask orbLayerMask;

    [Header("── 이펙트 ──────────────────────")]
    [Tooltip("던질 때 석상에서 발생할 파티클 프리팹 (없으면 스킵)")]
    public GameObject throwEffectPrefab;

    [Tooltip("이펙트 발생 위치 오프셋 (석상 기준)")]
    public Vector3 effectOffset = new Vector3(0f, 2f, 0.5f);

    [Tooltip("이펙트 자동 제거 시간 (초)")]
    public float effectLifetime = 1.5f;

    [Header("── 웨이브 설정 ─────────────────")]
    public int totalWaves = 5;
    public float waveDuration = 8f;

    [Header("── 발사 설정 ──────────────────")]
    [Tooltip("발사 간격 (초)")]
    public float spawnInterval = 1.8f;

    [Tooltip("수평 속도 (m/s)")]
    public float horizontalSpeed = 8f;

    [Tooltip("추가 위쪽 속도 (m/s)")]
    public float arcUpSpeed = 5f;

    [Tooltip("웨이브당 발사 간격 감소량")]
    public float intervalDecay = 0.15f;

    [Tooltip("웨이브당 수평 속도 증가량")]
    public float speedGrowth = 0.5f;

    [Header("── 발사 위치 ──────────────────")]
    [Tooltip("석상 발 기준 Y 오프셋. 석상이 떠있으면 이 값을 줄이세요.")]
    public float spawnHeightOffset = 2f;

    [Header("── 던지기 연출 ────────────────")]
    public float throwTiltAngle = 20f;
    public float throwTiltDuration = 0.2f;

    // ── 내부 상태 ──
    int _currentWave;
    bool _gameRunning;
    float _currentInterval;
    float _currentSpeed;
    Quaternion _statueOriginalRot;

    // ── 석상 발(foot) 위치 캐싱 ──
    // 석상 피벗이 중심점이면 실제 발 Y를 직접 지정할 수 있도록
    [Header("── 석상 발 보정 ─────────────────")]
    [Tooltip("true면 statueFootY를 직접 사용. false면 statueTransform.position.y 사용.")]
    public bool overrideStatueFootY = false;
    [Tooltip("석상 발 바닥의 월드 Y 좌표 (Ground Y와 동일하게)")]
    public float statueFootY = 0f;


    void Awake()
    {
        _currentWave = 0;
        _currentInterval = spawnInterval;
        _currentSpeed = horizontalSpeed;

        if (statueTransform != null)
            _statueOriginalRot = statueTransform.rotation;
    }

    void OnEnable()
    {
        _gameRunning = true;
        StartCoroutine(WaveLoop());
    }

    void Start() // Start에서 하던 초기화는 Awake로 이동했으므로 제거 또는 Cursor 설정만 남김
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    IEnumerator WaveLoop()
    {
        // ✅ Start()가 먼저 실행되도록 한 프레임 대기
        yield return null;

        while (_currentWave < totalWaves)
        {
            _currentWave++;
            MiniGameUIManager.Instance?.UpdateWave(_currentWave, totalWaves);

            float elapsed = 0f;
            while (elapsed < waveDuration)
            {
                if (!_gameRunning) yield break;
                float before = Time.time;
                yield return StartCoroutine(ThrowOrb());
                elapsed += Time.time - before; // ✅ 실제 경과 시간으로 측정
            }

            _currentInterval = Mathf.Max(0.4f, _currentInterval - intervalDecay);
            _currentSpeed += speedGrowth;

            yield return new WaitForSeconds(0.5f);
        }

        StartCoroutine(SuccessSequence());
    }

    IEnumerator ThrowOrb()
    {
        // ✅ throwTiltDuration이 너무 작으면 최소값 보장
        float tiltDur = Mathf.Max(throwTiltDuration, 0.05f);

        // 1) 석상 기울기
        if (statueTransform != null)
        {
            Vector3 toPlayer = (playerTransform.position - statueTransform.position).normalized;
            // ✅ toPlayer가 zero벡터일 경우 방어
            if (toPlayer == Vector3.zero) toPlayer = statueTransform.forward;

            Quaternion tiltRot = Quaternion.LookRotation(toPlayer) *
                                 Quaternion.Euler(throwTiltAngle, 0, 0);
            float t = 0f;
            while (t < tiltDur)
            {
                t += Time.deltaTime;
                statueTransform.rotation = Quaternion.Slerp(
                    _statueOriginalRot, tiltRot, t / tiltDur);
                yield return null;
            }
            statueTransform.rotation = tiltRot; // ✅ 정확히 목표 회전으로 고정
        }

        // 2) 이펙트 + 오브 생성
        PlayThrowEffect();
        SpawnOrb();

        // 3) 석상 복귀
        if (statueTransform != null)
        {
            float t = 0f;
            Quaternion fromRot = statueTransform.rotation;
            while (t < tiltDur)
            {
                t += Time.deltaTime;
                statueTransform.rotation = Quaternion.Slerp(
                    fromRot, _statueOriginalRot, t / tiltDur);
                yield return null;
            }
            statueTransform.rotation = _statueOriginalRot;
        }

        // ✅ 남은 대기시간이 양수일 때만 대기
        float waitTime = _currentInterval - tiltDur * 2f;
        if (waitTime > 0f)
            yield return new WaitForSeconds(waitTime);
    }

    /// <summary>
    /// 던지기 이펙트를 석상 위치에 생성하고 일정 시간 후 자동 제거합니다.
    /// Inspector에서 throwEffectPrefab을 연결하지 않으면 조용히 스킵됩니다.
    /// </summary>
    void PlayThrowEffect()
    {
        if (throwEffectPrefab == null || statueTransform == null) return;

        Vector3 effectPos = statueTransform.position
                          + statueTransform.TransformDirection(effectOffset);

        GameObject fx = Instantiate(throwEffectPrefab, effectPos,
                                    statueTransform.rotation);

        // 파티클 시스템이 있으면 한 번만 재생
        ParticleSystem ps = fx.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Stop();
            ps.Play();
        }

        Destroy(fx, effectLifetime);
    }

    /// <summary>
    /// 오브를 생성하고 발사합니다.
    /// overrideStatueFootY가 true이면 석상 피벗 오프셋을 무시하고
    /// statueFootY + spawnHeightOffset 위치에서 발사합니다.
    /// </summary>
    void SpawnOrb()
    {
        if (orbPrefab == null || statueTransform == null || playerTransform == null)
        {
            Debug.LogWarning("[OrbSpawner] 필수 참조가 비어있습니다.");
            return;
        }

        // ── 발사 위치 계산 (불상 떠있음 버그 보정) ──
        Vector3 basePos = statueTransform.position;
        if (overrideStatueFootY)
            basePos.y = statueFootY;   // 피벗 오프셋 무시, 발 기준 사용

        Vector3 spawnPos = basePos + Vector3.up * spawnHeightOffset;

        Vector3 flatDir = playerTransform.position - spawnPos;
        flatDir.y = 0f;
        flatDir = flatDir.normalized;

        Vector3 velocity = flatDir * _currentSpeed
                         + Vector3.up * arcUpSpeed;

        velocity += new Vector3(
            Random.Range(-0.5f, 0.5f),
            0f,
            Random.Range(-0.5f, 0.5f)
        );

        GameObject orb = Instantiate(orbPrefab, spawnPos, Quaternion.identity);
        orb.GetComponent<KarmaOrbController>()?.Shoot(velocity);
    }

    void Update()
    {
        if (!_gameRunning) return;
        if (!Input.GetMouseButtonDown(0)) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);
        foreach (RaycastHit hit in hits)
        {
            KarmaOrbController orb = hit.collider.GetComponent<KarmaOrbController>();
            if (orb != null)
            {
                orb.Deflect();
                break;
            }
        }
    }

    IEnumerator SuccessSequence()
    {
        _gameRunning = false;
        MiniGameUIManager.Instance?.ShowResult(true);
        yield return new WaitForSeconds(2f);
        LifeManager.Instance?.TriggerSuccess();
    }

    public void StopGame()
    {
        _gameRunning = false;
        StopAllCoroutines();
    }
}