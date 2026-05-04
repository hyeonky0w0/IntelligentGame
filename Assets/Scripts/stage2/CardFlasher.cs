using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardFlasher : MonoBehaviour
{
    [Header("카드 이미지")]
    public Image cardImage;
    public Sprite card1;  
    public Sprite card2;  

    [Header("카드2 유지 시간 (초)")]
    public float flashDuration = 0.1f;

    [Header("가이드 타이밍 목록 (BGM 기준 초)")]
    public List<float> beatTimes = new List<float>
    {
        // 41초
        41.308f, 41.519f, 41.736f,
        42.159f, 42.386f, 42.600f,

        // 44초 따아~ 따 따아~ 따
        44.731f, 44.931f, 45.173f,
        45.607f, 45.820f, 46.025f,

        // 48초 (따~~묵음~~)따따 (따~~묵음~~)따따
        48.170f, 48.372f, 48.581f,
        49.013f, 49.250f, 49.438f
    };

    private BGMManager _bgm;
    private int _nextIndex = 0;
    private Coroutine _flashRoutine;

    void Start()
    {
        _bgm = BGMManager.Instance;

        if (cardImage != null && card1 != null)
            cardImage.sprite = card1;
    }

    void Update()
    {
        if (_bgm == null || _nextIndex >= beatTimes.Count) return;

        float current = _bgm.GetCurrentTime();

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
        // card2로 전환
        if (cardImage != null && card2 != null)
            cardImage.sprite = card2;

        yield return new WaitForSeconds(flashDuration);

        // card1로 복귀
        if (cardImage != null && card1 != null)
            cardImage.sprite = card1;

        _flashRoutine = null;
    }
}