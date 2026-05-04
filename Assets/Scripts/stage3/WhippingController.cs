using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WhippingController : MonoBehaviour
{
    // ── 타원 궤도 설정 ──
    public float orbitRadiusX = 180f;  // 타원 가로 반지름
    public float orbitRadiusY = 80f;   // 타원 세로 반지름 (작을수록 납작)

    // ── 궤도 중심 오프셋 (bowl 기준) ──
    public float centerOffsetX = 0f;
    public float centerOffsetY = -30f;

    // ── 시작 각도 (0=오른쪽, 90=위, 180=왼쪽) ──
    public float startAngle = 180f;

    // ── 왕복 설정 ──
    // 한 번 누를 때 전진 각도 (180 = 반원)
    public float swingAngle = 45f;   // 1/8원
    // 전진 시간 / 복귀 시간
    public float swingForward = 0.08f; // 전진 시간
    public float swingBack = 0.08f; // 복귀 시간

    // ── 스케일 이펙트 ──
    public float hitScalePeak = 1.2f;

    // ── 스프라이트 ──
    public Sprite spriteNormal;
    public Sprite spriteDough;

    // ── 사운드 (Inspector에서 연결) ──
    public AudioClip stirSound;      // 반죽 젓는 소리

    // ── 내부 변수 ──
    float orbitAngle = 0f;
    Vector2 bowlCenter;
    Vector3 originScale;
    RectTransform rt;
    Image img;
    AudioSource audioSource;
    bool isSwinging = false;
    Coroutine swingRoutine;
    Coroutine doughRoutine;

    void Start()
    {
        this.rt = GetComponent<RectTransform>();
        this.img = GetComponent<Image>();
        this.audioSource = GetComponent<AudioSource>();
        this.originScale = transform.localScale;

        // AudioSource가 없으면 자동으로 추가
        if (this.audioSource == null)
            this.audioSource = gameObject.AddComponent<AudioSource>();

        // bowl 중심 좌표
        GameObject bowl = GameObject.Find("bowl");
        if (bowl != null)
        {
            RectTransform bowlRt = bowl.GetComponent<RectTransform>();
            this.bowlCenter = new Vector2(
                bowlRt.anchoredPosition.x + this.centerOffsetX,
                bowlRt.anchoredPosition.y + this.centerOffsetY
            );
        }
        else
        {
            this.bowlCenter = new Vector2(this.centerOffsetX, this.centerOffsetY);
        }

        this.orbitAngle = this.startAngle;
        UpdatePosition();
    }

    void Update()
    {
        // 움직임은 OnHit()에서만 발생
    }

    // ──────────────────────────────────────────
    // 타원 궤도 위치·회전 갱신
    // ──────────────────────────────────────────
    void UpdatePosition()
    {
        float rad = this.orbitAngle * Mathf.Deg2Rad;

        float x = this.bowlCenter.x + Mathf.Cos(rad) * this.orbitRadiusX;
        float y = this.bowlCenter.y + Mathf.Sin(rad) * this.orbitRadiusY;
        this.rt.anchoredPosition = new Vector2(x, y);

        // 타원 접선 방향으로 자전
        float tx = -Mathf.Sin(rad) * this.orbitRadiusX;
        float ty = Mathf.Cos(rad) * this.orbitRadiusY;
        float tangentAngle = Mathf.Atan2(ty, tx) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, tangentAngle);
    }

    // ──────────────────────────────────────────
    // 판정 반응 (Stage3Director에서 호출)
    // judgement : "PERFECT!" / "GOOD" / "MISS"
    // ──────────────────────────────────────────
    public void OnHit(string judgement)
    {
        if (this.swingRoutine != null) StopCoroutine(this.swingRoutine);

        if (judgement == "PERFECT!")
        {
            // 소리 재생 + 빠른 왕복
            PlayStirSound();
            this.swingRoutine = StartCoroutine(SwingAndReturn(this.swingAngle, this.swingForward * 0.7f, this.swingBack * 0.7f));
        }
        else if (judgement == "GOOD")
        {
            // 소리 재생 + 보통 왕복
            PlayStirSound();
            this.swingRoutine = StartCoroutine(SwingAndReturn(this.swingAngle, this.swingForward, this.swingBack));
        }
        else
        {
            // MISS : 소리 없음 + 조금만 갔다가 복귀
            this.swingRoutine = StartCoroutine(SwingAndReturn(this.swingAngle * 0.3f, this.swingForward, this.swingBack));
        }
    }

    // ──────────────────────────────────────────
    // 왕복 코루틴
    // 전진(swingAngle만큼) → 복귀(원래 위치로)
    // Translation + Rotation + Scaling 복합 제어
    // ──────────────────────────────────────────
    IEnumerator SwingAndReturn(float angle, float forwardDuration, float backDuration)
    {
        this.isSwinging = true;

        float baseAngle = this.orbitAngle;  // 출발 각도
        float targetAngle = baseAngle + angle; // 전진 목표 각도
        float elapsed = 0f;

        ShowDoughSprite();

        // ── 1. 전진 ──
        while (elapsed < forwardDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / forwardDuration);

            // EaseOut : 빠르게 출발 → 천천히 도착
            float eased = 1f - Mathf.Pow(1f - t, 2f);

            // Translation + Rotation
            this.orbitAngle = Mathf.Lerp(baseAngle, targetAngle, eased);
            UpdatePosition();

            // Scaling : 전진 중 살짝 커짐
            float scaleT = Mathf.Sin(t * Mathf.PI * 0.5f); // 0→1
            float s = 1f + (this.hitScalePeak - 1f) * scaleT;
            transform.localScale = this.originScale * s;

            yield return null;
        }

        // 전진 끝 위치 고정
        this.orbitAngle = targetAngle;
        UpdatePosition();

        elapsed = 0f;

        // ── 2. 복귀 ──
        while (elapsed < backDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / backDuration);

            // EaseInOut : 부드럽게 출발 → 부드럽게 도착
            float eased = t < 0.5f
                ? 2f * t * t
                : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;

            // Translation + Rotation
            this.orbitAngle = Mathf.Lerp(targetAngle, baseAngle, eased);
            UpdatePosition();

            // Scaling : 복귀 중 원래 크기로
            float scaleT = 1f - t; // 1→0
            float s = 1f + (this.hitScalePeak - 1f) * scaleT * 0.5f;
            transform.localScale = this.originScale * s;

            yield return null;
        }

        // 완전히 원래 위치로 복귀
        this.orbitAngle = baseAngle;
        UpdatePosition();

        transform.localScale = this.originScale;
        this.isSwinging = false;
    }

    // ──────────────────────────────────────────
    // 반죽 젓는 소리 재생
    // ──────────────────────────────────────────
    void PlayStirSound()
    {
        if (this.audioSource == null) return;
        if (this.stirSound == null) return;
        this.audioSource.PlayOneShot(this.stirSound);
    }

    // ──────────────────────────────────────────
    // 반죽 묻은 스프라이트 잠깐 표시
    // ──────────────────────────────────────────
    void ShowDoughSprite()
    {
        if (this.img == null || this.spriteDough == null) return;
        if (this.doughRoutine != null) StopCoroutine(this.doughRoutine);
        this.doughRoutine = StartCoroutine(DoughSpriteRoutine());
    }

    IEnumerator DoughSpriteRoutine()
    {
        this.img.sprite = this.spriteDough;
        // 전진 + 복귀 시간 동안 유지
        yield return new WaitForSeconds(this.swingForward + this.swingBack);
        if (this.spriteNormal != null)
            this.img.sprite = this.spriteNormal;
    }
}