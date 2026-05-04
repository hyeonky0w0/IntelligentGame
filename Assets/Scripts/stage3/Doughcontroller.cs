using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoughController : MonoBehaviour
{
    // ── 평상시 흔들림 ──
    public float swaySpeed = 1.5f;
    public float swayAngle = 12.0f;
    public float squishAmt = 0.1f;
    public float squishSpeed = 2.0f;

    // ── 내부 변수 ──
    Vector3 originScale;
    bool isHit = false;
    Coroutine hitRoutine;

    void Start()
    {
        this.originScale = transform.localScale;
    }

    void Update()
    {
        if (this.isHit) return;

        // ── Rotation : sin 좌우 흔들림 ──
        float angle = Mathf.Sin(Time.time * this.swaySpeed) * this.swayAngle;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // ── Scaling : 젤리 출렁임 (X↔Y 반비례) ──
        float squishX = 1f + Mathf.Sin(Time.time * this.squishSpeed) * this.squishAmt;
        float squishY = 1f - Mathf.Sin(Time.time * this.squishSpeed) * this.squishAmt * 0.5f;
        transform.localScale = new Vector3(
            this.originScale.x * squishX,
            this.originScale.y * squishY,
            this.originScale.z
        );
    }

    // ──────────────────────────────────────────
    // 판정 반응 (Stage3Director에서 호출)
    // ──────────────────────────────────────────
    public void OnHit(string judgement)
    {
        if (this.hitRoutine != null) StopCoroutine(this.hitRoutine);

        if (judgement == "PERFECT!") this.hitRoutine = StartCoroutine(HitEffect(0.35f, 22.0f));
        else if (judgement == "GOOD") this.hitRoutine = StartCoroutine(HitEffect(0.18f, 12.0f));
        else this.hitRoutine = StartCoroutine(HitEffect(0.04f, 4.0f));
    }

    // Scaling(찌그러짐) + Rotation(틀어짐) 복합
    IEnumerator HitEffect(float squish, float spinDeg)
    {
        this.isHit = true;

        float duration = 0.35f;
        float elapsed = 0f;

        float targetX = this.originScale.x * (1f + squish);
        float targetY = this.originScale.y * (1f - squish * 0.6f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Scaling : EaseOut
            float easeOut = 1f - Mathf.Pow(1f - t, 3f);
            float currentX = Mathf.Lerp(targetX, this.originScale.x, easeOut);
            float currentY = Mathf.Lerp(targetY, this.originScale.y, easeOut);
            transform.localScale = new Vector3(currentX, currentY, this.originScale.z);

            // Rotation : 확 틀렸다가 복귀
            float spinT = Mathf.Sin(t * Mathf.PI);
            float currentZ = spinDeg * spinT;
            transform.rotation = Quaternion.Euler(0f, 0f, currentZ);

            yield return null;
        }

        transform.localScale = this.originScale;
        transform.rotation = Quaternion.identity;
        this.isHit = false;
    }
}