using UnityEngine;
using System.Collections.Generic;

public class WhitePorcelainBlackout : MonoBehaviour
{
    [Header("Anomaly ID Config")]
    [Tooltip("GameManager에서 판정할 이 기믹의 고유 ID")]
    public int anomalyID = 31;

    [System.Serializable]
    public struct RelicData
    {
        public Transform relicTransform;
        [HideInInspector] public Vector3 originalPosition;
        public Vector3 changedPosition;
    }

    public List<RelicData> targetRelics;
    private bool isTriggered = false;
    private bool isInitialized = false; // Awake가 안전하게 실행되었는지 확인용

    private void Awake()
    {
        InitializeOriginalPositions();
    }

    private void InitializeOriginalPositions()
    {
        if (isInitialized) return;

        // 최초 1회만 원본 위치를 정확하게 기록
        for (int i = 0; i < targetRelics.Count; i++)
        {
            if (targetRelics[i].relicTransform != null)
            {
                var data = targetRelics[i];
                data.originalPosition = data.relicTransform.localPosition;
                targetRelics[i] = data;
            }
        }
        isInitialized = true;
    }

    private void OnEnable()
    {
        // 오브젝트가 켜질 때 Awake가 안 돌았을 수 있으므로 안전하게 한 번 더 체크
        InitializeOriginalPositions();
        ResetAnomaly();
    }

    private void OnDisable()
    {
        ResetAnomaly();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!enabled) return;

        if (!other.CompareTag("Player")) return;

        // 🚨 디버그 로그를 추가하여 조건 중 어디서 막히는지 확인하는 것이 좋습니다.
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager 인스턴스가 존재하지 않습니다.");
            return;
        }

        if (GameManager.Instance.currentAnomalyID != anomalyID)
        {
            // ID가 맞지 않아 스킵됨 (현재 매니저 ID와 내 ID를 로그로 비교)
            // Debug.Log($"ID 불일치 - 매니저: {GameManager.Instance.currentAnomalyID}, 내 ID: {anomalyID}");
            return;
        }

        if (isTriggered) return;

        isTriggered = true;

        Debug.LogWarning("🚨 [이상현상 발동] 3-1. 백자관 암전 및 유물 위치 변경 완료!");
        ChangeRelicPositions();
    }

    private void ChangeRelicPositions()
    {
        foreach (var relic in targetRelics)
        {
            if (relic.relicTransform != null)
                relic.relicTransform.localPosition = relic.changedPosition;
        }
    }

    public void ResetAnomaly()
    {
        isTriggered = false;

        if (targetRelics == null || !isInitialized) return;

        foreach (var relic in targetRelics)
        {
            if (relic.relicTransform != null)
                relic.relicTransform.localPosition = relic.originalPosition;
        }
    }
}