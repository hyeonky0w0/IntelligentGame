using UnityEngine;

public class Anomaly_BuddhistPaintingGroup : MonoBehaviour
{
    [Header("Anomaly ID Config")]
    [Tooltip("GameManager에서 판정할 이 기믹의 고유 ID")]
    public int anomalyID = 21; // 불화 그룹 ID: 21번

    [Header("전시된 통짜 불화 에셋들 (7개 드래그)")]
    public Renderer[] paintingRenderers;
    public AudioSource glitchAudio; // ★ 불화 일그러질 때 날 소리용 오디오 소스

    [Header("일러그러짐 강도 및 속도")]
    public float distortionIntensity = 0.05f;
    public float distortionSpeed = 15f;

    private float zoneTimer = 0f;
    private bool isPlayerInZone = false;
    private bool isAnomalyTriggered = false;

    private Vector2[] originalOffsets;
    private Vector2[] originalTilings;
    private Color[] originalColors;

    private void Start()
    {
        if (paintingRenderers != null && paintingRenderers.Length > 0)
        {
            originalOffsets = new Vector2[paintingRenderers.Length];
            originalTilings = new Vector2[paintingRenderers.Length];
            originalColors = new Color[paintingRenderers.Length];

            for (int i = 0; i < paintingRenderers.Length; i++)
            {
                if (paintingRenderers[i] != null && paintingRenderers[i].material != null)
                {
                    originalOffsets[i] = paintingRenderers[i].material.mainTextureOffset;
                    originalTilings[i] = paintingRenderers[i].material.mainTextureScale;
                    originalColors[i] = paintingRenderers[i].material.color;
                }
            }
        }
    }

    // 매니저에 의해 활성화될 때 플래그 초기화
    private void OnEnable()
    {
        isPlayerInZone = false;
        zoneTimer = 0f;
        isAnomalyTriggered = false;
    }

    // 오브젝트가 꺼질 때는 확실하게 원상복구 시키기
    private void OnDisable()
    {
        ResetToOriginal();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // 🚨 [GameManager 체크] 현재 매니저가 선택한 ID가 내 번호가 아니면 리턴
        if (GameManager.Instance == null || GameManager.Instance.currentAnomalyID != anomalyID) return;

        isPlayerInZone = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ResetToOriginal();
        }
    }

    private void Update()
    {
        // 🚨 [GameManager 체크] 실시간으로도 내 번호가 유지되고 있는지 매 프레임 감시
        if (GameManager.Instance == null || GameManager.Instance.currentAnomalyID != anomalyID) return;

        if (!isPlayerInZone) return;

        if (!isAnomalyTriggered)
        {
            zoneTimer += Time.deltaTime;
            if (zoneTimer >= 2f)
            {
                isAnomalyTriggered = true;

                // ★ 불화가 우그러들기 시작할 때 효과음 재생!
                if (glitchAudio != null) glitchAudio.Play();
            }
        }
        else
        {
            for (int i = 0; i < paintingRenderers.Length; i++)
            {
                Renderer rend = paintingRenderers[i];
                if (rend == null || rend.material == null) continue;

                rend.material.color = Color.Lerp(rend.material.color, new Color(0.8f, 0f, 0f, 1f), Time.deltaTime * 2f);

                float noiseX = Mathf.Sin(Time.time * distortionSpeed) * distortionIntensity;
                float noiseY = Mathf.Cos(Time.time * distortionSpeed) * distortionIntensity;

                rend.material.mainTextureOffset = originalOffsets[i] + new Vector2(noiseX, noiseY);
                rend.material.mainTextureScale = originalTilings[i] + new Vector2(noiseX * 0.5f, noiseY * 0.5f);
            }
        }
    }

    /// <summary>
    /// 불화의 텍스처와 오디오를 초기 정상 상태로 되돌리는 함수
    /// </summary>
    private void ResetToOriginal()
    {
        isPlayerInZone = false;
        zoneTimer = 0f;
        isAnomalyTriggered = false;

        if (glitchAudio != null) glitchAudio.Stop();

        if (paintingRenderers == null) return;

        for (int i = 0; i < paintingRenderers.Length; i++)
        {
            if (paintingRenderers[i] == null || paintingRenderers[i].material == null) continue;

            // 원래 저장해둔 조절값으로 원상복구
            paintingRenderers[i].material.mainTextureOffset = originalOffsets[i];
            paintingRenderers[i].material.mainTextureScale = originalTilings[i];
            paintingRenderers[i].material.color = originalColors[i];
        }
    }
}