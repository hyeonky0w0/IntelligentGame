using System.Collections;
using UnityEngine;

public class Anomaly_ArahatGroup : MonoBehaviour
{
    [Header("Anomaly ID Config")]
    [Tooltip("GameManager에서 판정할 이 기믹의 고유 ID")]
    public int anomalyID = 22; // 나한상 그룹 ID: 22번

    [Header("참조 오브젝트")]
    public Transform playerTransform;
    public GameObject scareArahatAsset;
    public AudioSource scareAudio; // ★ 나한상 쾅! 소리용 오디오 소스

    [Header("나한상 구역 조명들 (4개 드래그)")]
    public Light[] zoneLights;

    [Header("전시된 나한상 에셋들 (4개 드래그)")]
    public Transform[] arahatAssets;

    private Camera playerCamera;
    private float gazeTimer = 0f;
    private bool isTriggered = false;
    private bool isGazing = false;

    // 원래 위치 복구를 위한 변수 저장
    private Vector3[] originalPositions;

    private void Awake()
    {
        // 다음 회차 리셋을 위해 원래 나한상들의 시작 위치를 기록해둡니다.
        if (arahatAssets != null && arahatAssets.Length > 0)
        {
            originalPositions = new Vector3[arahatAssets.Length];
            for (int i = 0; i < arahatAssets.Length; i++)
            {
                if (arahatAssets[i] != null)
                {
                    originalPositions[i] = arahatAssets[i].position;
                }
            }
        }
    }

    private void Start()
    {
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
        }

        if (playerTransform != null)
        {
            playerCamera = playerTransform.GetComponentInChildren<Camera>();
        }
    }

    // 매니저에 의해 오브젝트가 활성화될 때마다 상태 초기화
    private void OnEnable()
    {
        isTriggered = false;
        isGazing = false;
        gazeTimer = 0f;

        if (scareArahatAsset != null)
            scareArahatAsset.SetActive(false);
    }

    // 오브젝트가 비활성화되면 연출 Coroutine을 멈추고 원래 위치와 조명을 원상복구
    private void OnDisable()
    {
        StopAllCoroutines();
        ResetAnomalyState();
    }

    private void Update()
    {
        // 🚨 [GameManager 연동 핵심 코드]
        // 매니저가 고른 당첨 번호가 22번이 아니라면 시선 체크 및 연출 로직 전체를 차단합니다.
        if (GameManager.Instance == null || GameManager.Instance.currentAnomalyID != anomalyID) return;

        if (isTriggered) return;

        CheckIfPlayerLookingAnyArahat();

        if (isGazing)
        {
            gazeTimer += Time.deltaTime;

            foreach (Transform arahat in arahatAssets)
            {
                if (arahat == null) continue;
                Vector3 directionToPlayer = playerTransform.position - arahat.position;
                directionToPlayer.y = 0;
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                arahat.rotation = Quaternion.Slerp(arahat.rotation, targetRotation, Time.deltaTime * 0.2f);
            }

            if (gazeTimer >= 0.3f) // 조건 충족 시 발동
            {
                isTriggered = true;
                StartCoroutine(PlayArahatClimax());
            }
        }
        else
        {
            gazeTimer = Mathf.Max(0f, gazeTimer - Time.deltaTime);
        }
    }

    private void CheckIfPlayerLookingAnyArahat()
    {
        if (playerCamera == null) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 20f))
        {
            foreach (Transform arahat in arahatAssets)
            {
                if (arahat == null) continue;
                if (hit.transform == arahat || hit.transform.IsChildOf(arahat))
                {
                    isGazing = true;
                    return;
                }
            }
        }
        isGazing = false;
    }

    IEnumerator PlayArahatClimax()
    {
        for (int i = 0; i < 3; i++)
        {
            SetAllLights(false);
            yield return new WaitForSeconds(0.2f);
            SetAllLights(true);
            yield return new WaitForSeconds(0.3f);
        }

        SetAllLights(false);
        yield return new WaitForSeconds(0.4f);

        // 나한상 앞으로 전진 연출
        foreach (Transform arahat in arahatAssets)
        {
            if (arahat != null) arahat.position += arahat.forward * 1.5f;
        }

        if (scareArahatAsset != null && playerTransform != null)
        {
            Vector3 spawnPos = playerTransform.position - (playerTransform.forward * 1.5f);
            spawnPos.y = playerTransform.position.y;

            scareArahatAsset.transform.position = spawnPos;
            scareArahatAsset.transform.LookAt(playerTransform);
            scareArahatAsset.SetActive(true);

            if (scareAudio != null) scareAudio.Play();
        }

        yield return new WaitForSeconds(0.2f);
        SetAllLights(true);
    }

    private void SetAllLights(bool state)
    {
        if (zoneLights == null) return;
        foreach (Light light in zoneLights)
        {
            if (light != null) light.enabled = state;
        }
    }

    /// <summary>
    /// 기믹의 위치와 조명 상태를 원래대로 리셋하는 안전 함수
    /// </summary>
    private void ResetAnomalyState()
    {
        SetAllLights(true);
        if (scareAudio != null) scareAudio.Stop();

        if (arahatAssets == null || originalPositions == null) return;

        for (int i = 0; i < arahatAssets.Length; i++)
        {
            if (arahatAssets[i] != null && i < originalPositions.Length)
            {
                arahatAssets[i].position = originalPositions[i];
            }
        }
    }
}