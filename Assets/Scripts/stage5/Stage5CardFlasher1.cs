using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage5CardFlasher : MonoBehaviour
{
    [Header("카드 이미지")]
    public Image cardImage;
    public Sprite card1;
    public Sprite card2;

    [Header("카드2 유지 시간 (초)")]
    public float flashDuration = 0.1f;

    private readonly List<float> beatTimes = new List<float>
    {
        89.290f, 89.495f, 89.700f, 89.910f,
        90.142f, 90.350f, 90.557f, 90.767f,
        90.993f, 91.430f, 91.845f
    };

    private int _nextIndex = 0;
    private Coroutine _flashRoutine = null;

    void Start()
    {
        if (cardImage != null && card1 != null)
            cardImage.sprite = card1;
    }

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

    void TriggerFlash()
    {
        if (_flashRoutine != null)
            StopCoroutine(_flashRoutine);
        _flashRoutine = StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        if (cardImage != null && card2 != null)
            cardImage.sprite = card2;

        yield return new WaitForSeconds(flashDuration);

        if (cardImage != null && card1 != null)
            cardImage.sprite = card1;

        _flashRoutine = null;
    }
}