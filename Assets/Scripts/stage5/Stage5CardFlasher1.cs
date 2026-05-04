using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage5CardFlasher : MonoBehaviour
{
    // ===================================================================
    // Inspector 연결
    // ===================================================================
    [Header("카드 이미지")]
    public Image cardImage;
    public Sprite card1;            // 기본 이미지
    public Sprite card2;            // 박자에 맞춰 잠깐 보일 이미지

    [Header("카드2 유지 시간 (초)")]
    public float flashDuration = 0.1f;

    // ===================================================================
    // 가이드 타이밍 (ms → 초 변환)
    // GUIDE_TIMES_MS = { 89290, 89495, 89700, 89910,
    //                    90142, 90350, 90557, 90767,
    //                    90993, 91430, 91845 }
    // ===================================================================
    private readonly List<float> beatTimes = new List<float>
    {
        89.290f, 89.495f, 89.700f, 89.910f,
        90.142f, 90.350f, 90.557f, 90.767f,
        90.993f, 91.430f, 91.845f
    };

    // ===================================================================
    // 내부 상태
    // ===================================================================
    private int _nextIndex = 0;
    private Coroutine _flashRoutine = null;

    // ===================================================================
    // Start
    // ===================================================================
    void Start()
    {
        if (cardImage != null && card1 != null)
            cardImage.sprite = card1;
    }

    // ===================================================================
    // Update
    // ===================================================================
    void Update()
    {
        if (_nextIndex >= beatTimes.Count) return;

        float current = BGMManager.Instance.GetCurrentTime();

        if (current >= beatTimes[_nextIndex])
        {
            TriggerFlash();
            _nextIndex++;
        }
    }

    // ===================================================================
    // 플래시 트리거
    // ===================================================================
    void TriggerFlash()
    {
        if (_flashRoutine != null)
            StopCoroutine(_flashRoutine);
        _flashRoutine = StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        // card1 → card2
        if (cardImage != null && card2 != null)
            cardImage.sprite = card2;

        yield return new WaitForSeconds(flashDuration);

        // card2 → card1
        if (cardImage != null && card1 != null)
            cardImage.sprite = card1;

        _flashRoutine = null;
    }
}