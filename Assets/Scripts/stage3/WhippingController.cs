using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WhippingController : MonoBehaviour
{
    public float orbitRadiusX = 180f;
    public float orbitRadiusY = 80f;

    public float centerOffsetX = 0f;
    public float centerOffsetY = -30f;

    public float startAngle = 180f;

    public float swingAngle = 45f;
    public float swingForward = 0.08f;
    public float swingBack = 0.08f;

    public float hitScalePeak = 1.2f;

    public Sprite spriteNormal;
    public Sprite spriteDough;

    public AudioClip stirSound;

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

        if (this.audioSource == null)
            this.audioSource = gameObject.AddComponent<AudioSource>();

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
    }

    void UpdatePosition()
    {
        float rad = this.orbitAngle * Mathf.Deg2Rad;

        float x = this.bowlCenter.x + Mathf.Cos(rad) * this.orbitRadiusX;
        float y = this.bowlCenter.y + Mathf.Sin(rad) * this.orbitRadiusY;
        this.rt.anchoredPosition = new Vector2(x, y);

        float tx = -Mathf.Sin(rad) * this.orbitRadiusX;
        float ty = Mathf.Cos(rad) * this.orbitRadiusY;
        float tangentAngle = Mathf.Atan2(ty, tx) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, tangentAngle);
    }

    public void OnHit(string judgement)
    {
        if (this.swingRoutine != null) StopCoroutine(this.swingRoutine);

        if (judgement == "PERFECT!")
        {
            PlayStirSound();
            this.swingRoutine = StartCoroutine(SwingAndReturn(this.swingAngle, this.swingForward * 0.7f, this.swingBack * 0.7f));
        }
        else if (judgement == "GOOD")
        {
            PlayStirSound();
            this.swingRoutine = StartCoroutine(SwingAndReturn(this.swingAngle, this.swingForward, this.swingBack));
        }
        else
        {
            this.swingRoutine = StartCoroutine(SwingAndReturn(this.swingAngle * 0.3f, this.swingForward, this.swingBack));
        }
    }

    IEnumerator SwingAndReturn(float angle, float forwardDuration, float backDuration)
    {
        this.isSwinging = true;

        float baseAngle = this.orbitAngle;
        float targetAngle = baseAngle + angle;
        float elapsed = 0f;

        ShowDoughSprite();

        while (elapsed < forwardDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / forwardDuration);

            float eased = 1f - Mathf.Pow(1f - t, 2f);

            this.orbitAngle = Mathf.Lerp(baseAngle, targetAngle, eased);
            UpdatePosition();

            float scaleT = Mathf.Sin(t * Mathf.PI * 0.5f);
            float s = 1f + (this.hitScalePeak - 1f) * scaleT;
            transform.localScale = this.originScale * s;

            yield return null;
        }

        this.orbitAngle = targetAngle;
        UpdatePosition();

        elapsed = 0f;

        while (elapsed < backDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / backDuration);

            float eased = t < 0.5f
                ? 2f * t * t
                : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;

            this.orbitAngle = Mathf.Lerp(targetAngle, baseAngle, eased);
            UpdatePosition();

            float scaleT = 1f - t;
            float s = 1f + (this.hitScalePeak - 1f) * scaleT * 0.5f;
            transform.localScale = this.originScale * s;

            yield return null;
        }

        this.orbitAngle = baseAngle;
        UpdatePosition();

        transform.localScale = this.originScale;
        this.isSwinging = false;
    }

    void PlayStirSound()
    {
        if (this.audioSource == null) return;
        if (this.stirSound == null) return;
        this.audioSource.PlayOneShot(this.stirSound);
    }

    void ShowDoughSprite()
    {
        if (this.img == null || this.spriteDough == null) return;
        if (this.doughRoutine != null) StopCoroutine(this.doughRoutine);
        this.doughRoutine = StartCoroutine(DoughSpriteRoutine());
    }

    IEnumerator DoughSpriteRoutine()
    {
        this.img.sprite = this.spriteDough;
        yield return new WaitForSeconds(this.swingForward + this.swingBack);
        if (this.spriteNormal != null)
            this.img.sprite = this.spriteNormal;
    }
}