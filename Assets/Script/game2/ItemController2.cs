using System.Collections;
using UnityEngine;

/// <summary>
/// 깨달음(빛의 구슬) / 번뇌(어둠의 불꽃) 아이템의 낙하 및 시각 연출
/// AppleCatch의 ItemController를 테마에 맞게 확장
/// </summary>
public class ItemController2 : MonoBehaviour
{
    [Header("=== 낙하 ===")]
    public float dropSpeed = -0.03f;

    [Header("=== 연출: 흔들림 ===")]
    [Tooltip("번뇌 아이템에만 ON — 좌우로 흔들리며 낙하")]
    public bool wobble = false;
    public float wobbleFrequency = 2.5f;
    public float wobbleAmplitude = 0.5f;

    [Header("=== 연출: 발광 (Light) ===")]
    public Light itemLight;             // 아이템에 붙은 Point Light (선택)

    // 내부
    private float startX;
    private float timeAlive = 0f;
    private bool isDestroying = false;

    void Start()
    {
        startX = transform.position.x;

        // 발광 연출 초기화
        if (itemLight != null)
            StartCoroutine(PulsateLight());
    }

    void Update()
    {
        if (isDestroying) return;

        timeAlive += Time.deltaTime;

        // 낙하
        transform.Translate(0, dropSpeed, 0);

        // 번뇌는 좌우 흔들림 추가
        if (wobble)
        {
            float offsetX = Mathf.Sin(timeAlive * wobbleFrequency) * wobbleAmplitude;
            Vector3 pos = transform.position;
            pos.x = startX + offsetX;
            transform.position = pos;
        }

        // 화면 아래로 벗어나면 제거
        if (transform.position.y < -1.5f)
        {
            DestroyItem();
        }
    }

    /// <summary>
    /// BasketController2 충돌 시 외부에서 호출 — Destroy 전 연출용
    /// </summary>
    public void OnCollected()
    {
        DestroyItem();
    }

    void DestroyItem()
    {
        if (isDestroying) return;
        isDestroying = true;
        Destroy(gameObject);
    }

    // 발광 맥동 연출 (깨달음 = 부드럽게 빛남, 번뇌 = 불규칙하게 깜빡)
    IEnumerator PulsateLight()
    {
        float baseIntensity = itemLight.intensity;
        while (true)
        {
            float pulse = wobble
                ? Mathf.Abs(Mathf.Sin(Time.time * 6f + Random.Range(-0.5f, 0.5f)))  // 번뇌: 불규칙
                : Mathf.Abs(Mathf.Sin(Time.time * 2f));                              // 깨달음: 부드럽게
            itemLight.intensity = baseIntensity * (0.6f + pulse * 0.4f);
            yield return null;
        }
    }
}
