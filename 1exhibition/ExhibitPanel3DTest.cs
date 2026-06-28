using UnityEngine;

public class ExhibitPanel3DTest : MonoBehaviour
{
    public static ExhibitPanel3DTest Instance { get; private set; }

    [Header("Anomaly ID Config")]
    [Tooltip("GameManager에서 판정할 이 기믹의 고유 ID")]
    public int anomalyID = 4; // 전시 안내판 ID: 4번

    [Header("3D Renderer Reference")]
    public MeshRenderer panelMeshRenderer;

    [Header("Panel Textures (PNG)")]
    public Texture2D normalTexture;
    public Texture2D errorTexture;

    private bool isAnomalyActivatedThisRound = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 💡 오브젝트가 켜질 때 플래그 초기화
    private void OnEnable()
    {
        isAnomalyActivatedThisRound = false;

        // 🔍 콘솔창에 이게 찍히는지 반드시 확인하세요!
        Debug.Log($"<color=#33FF33><b>[디버그 포착] {gameObject.name}의 OnEnable() 정상 가동됨!</b></color>");
    }

    private void OnDisable()
    {
        // 게임을 끌 때는 리셋 패스
        if (GameManager.Instance == null) return;

        // 이번 전시가 안내판 이상현상(4번) 세팅 상황이라면 정상 리셋 거부 (ON 유지)
        if (GameManager.Instance.currentAnomalyID == anomalyID)
        {
            return;
        }

        SetAnomalyState(false);
    }

    private void Update()
    {
        // 이미 이번 라운드에 에러 상태를 켰다면 Update 연산 차단 (계속 ON 유지)
        if (isAnomalyActivatedThisRound) return;

        // 🚨 [핵심 해결벽] 매니저가 ID 배정을 끝마칠 때까지 Update에서 실시간으로 추적 감시합니다.
        if (GameManager.Instance != null && GameManager.Instance.currentAnomalyID == anomalyID)
        {
            Debug.LogWarning($"<color=#FFFF33><b>[디버그 성공] 실시간 추적을 통해 매니저 ID가 {anomalyID}번인 것을 감지했습니다!</b></color>");
            isAnomalyActivatedThisRound = true;
            SetAnomalyState(true); // 에러 텍스처로 강제 고정
        }
    }

    public void SetAnomalyState(bool isAnomalyActive)
    {
        if (panelMeshRenderer == null)
        {
            Debug.LogError("[오류] Cube의 MeshRenderer가 지정되지 않았습니다!");
            return;
        }

        if (isAnomalyActive)
        {
            if (panelMeshRenderer.material.HasProperty("_BaseMap"))
                panelMeshRenderer.material.SetTexture("_BaseMap", errorTexture);
            else
                panelMeshRenderer.material.mainTexture = errorTexture;

            Debug.LogWarning("🚨 [텍스처 변경] '1-4-error' (이상 상태) 적용 완료!");
        }
        else
        {
            if (panelMeshRenderer.material.HasProperty("_BaseMap"))
                panelMeshRenderer.material.SetTexture("_BaseMap", normalTexture);
            else
                panelMeshRenderer.material.mainTexture = normalTexture;

            Debug.Log("🟢 [텍스처 변경] '1-4' (정상 상태) 적용 완료!");
        }
    }

    [ContextMenu("Test: Trigger Anomaly ON")]
    public void TestAnomalyOn() => SetAnomalyState(true);

    [ContextMenu("Test: Trigger Anomaly OFF")]
    public void TestAnomalyOff() => SetAnomalyState(false);
}