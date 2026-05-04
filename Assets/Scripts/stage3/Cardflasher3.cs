using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardFlasher3 : MonoBehaviour
{
    [Header("카드 이미지")]
    public Image cardImage;
    public Sprite card1;   // 기본 이미지
    public Sprite card2;   // 박자에 맞춰 잠깐 보일 이미지

    [Header("카드2 유지 시간 (초)")]
    public float flashDuration = 0.1f;

    [Header("가이드 타이밍 목록 (BGM 기준 초)")]
    public List<float> beatTimes = new List<float>
    {
        // 55초 따~단 단 따라따~ 단
        55.114f, 55.519f, 55.967f,
        56.880f, 56.967f, 57.170f, 57.687f,
        // 1분 1초 따~단 따~단 따라따라단
        61.983f, 62.377f, 62.752f, 63.158f,
        63.703f, 63.746f, 64.012f, 64.109f, 64.455f,
        // 1분 8초 딴딴딴딴 따안~ 따라라라라
        68.732f, 69.174f, 69.604f, 70.028f, 70.452f,
        // 1분 15초 딴딴딴 틱
        75.691f, 76.037f, 76.461f, 76.982f
    };

    private BGMManager _bgm;
    private int _nextIndex = 0;
    private Coroutine _flashRoutine;

    void Start()
    {
        this._bgm = BGMManager.Instance;

        // 시작 시 card1로 초기화
        if (this.cardImage != null && this.card1 != null)
            this.cardImage.sprite = this.card1;
    }

    void Update()
    {
        if (this._bgm == null) return;
        if (this._nextIndex >= this.beatTimes.Count) return;

        float current = this._bgm.GetCurrentTime();

        if (current >= this.beatTimes[this._nextIndex])
        {
            TriggerFlash();
            this._nextIndex++;
        }
    }

    void TriggerFlash()
    {
        if (this._flashRoutine != null)
            StopCoroutine(this._flashRoutine);
        this._flashRoutine = StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        // card2로 전환
        if (this.cardImage != null && this.card2 != null)
            this.cardImage.sprite = this.card2;

        yield return new WaitForSeconds(this.flashDuration);

        // card1로 복귀
        if (this.cardImage != null && this.card1 != null)
            this.cardImage.sprite = this.card1;

        this._flashRoutine = null;
    }
}