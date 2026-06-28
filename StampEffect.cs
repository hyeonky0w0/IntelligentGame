using UnityEngine;
using UnityEngine.UI; // UI 이미지를 사용할 경우를 위해 추가

public class StampEffect : MonoBehaviour
{
    [Header("[사운드 설정]")]
    public AudioSource audioSource;    // 오디오 소스 컴포넌트
    public AudioClip stampSound;       // 도장 찍히는 소리 (.mp3 또는 .wav)

    [Header("[애니메이션 설정]")]
    public float StartScale = 5f;      // 시작 크기 (기본 크기의 5배로 거대하게 시작)
    public float TargetScale = 1f;     // 목표 크기 (원래 크기)
    public float stampSpeed = 15f;     // 도장이 찍히는 속도 (높을수록 빠름)

    private Image uiImage;
    private SpriteRenderer spriteRenderer;
    private bool isSoundPlayed = false;

    void Start()
    {
        // 1. UI 이미지 컴포넌트인지, 일반 스프라이트인지 자동으로 찾기
        uiImage = GetComponent<Image>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 2. 시작할 때 크기를 거대하게 키우고, 투명도를 0(안보임)으로 설정
        transform.localScale = Vector3.one * StartScale;
        SetAlpha(0f);
    }

    void Update()
    {
        // 현재 크기에서 목표 크기(1,1,1)로 부드럽고 빠르게 축소 (타격감 중심)
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * TargetScale, Time.deltaTime * stampSpeed);

        // 크기가 줄어들면서 점점 선명해지도록 서서히 등장
        float currentAlpha = Mathf.InverseLerp(StartScale, TargetScale, transform.localScale.x);
        SetAlpha(currentAlpha);

        // 거의 다 찍혔을 때 (목표 크기에 근접했을 때) 도장 소리 1회 재생
        if (!isSoundPlayed && transform.localScale.x <= TargetScale + 0.1f)
        {
            PlayStampSound();
            isSoundPlayed = true;
        }
    }

    // UI 이미지와 일반 스프라이트 모두 호환되는 투명도 조절 함수
    void SetAlpha(float alpha)
    {
        if (uiImage != null)
        {
            Color color = uiImage.color;
            color.a = alpha;
            uiImage.color = color;
        }
        else if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
    }

    // 소리 재생 함수
    void PlayStampSound()
    {
        if (audioSource != null && stampSound != null)
        {
            audioSource.PlayOneShot(stampSound);
        }
    }
}