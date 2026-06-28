using UnityEngine;

public class Anomaly_QueenPillow : MonoBehaviour
{
    [Header("Anomaly ID Config")]
    public int anomalyID = 5;

    [Header("Target Settings")]
    [Tooltip("자식으로 들어가 있는 실제 귀신(ghost) 오브젝트를 여기에 넣어주세요")]
    public GameObject ghostVisualObject;
    public Transform playerTransform;

    [Header("Disappear Settings")]
    public float disappearDistance = 2.0f;

    private bool isSpawned = false;
    private bool isLookedAt = false;

    private void Start()
    {
        // 처음에는 자식 귀신 비주얼을 확실히 꺼둡니다.
        if (ghostVisualObject != null) ghostVisualObject.SetActive(false);
    }

    private void Update()
    {
        // 1. 매니저가 주사위 5번을 굴렸는지 항상 깨어서 감시합니다.
        if (GameManager.Instance == null || GameManager.Instance.currentAnomalyID != anomalyID) return;

        // 2. 5번이 당첨되었는데 아직 귀신이 안 켜졌다면 여기서 켜줍니다!
        if (!isSpawned)
        {
            isSpawned = true;
            if (ghostVisualObject != null)
            {
                ghostVisualObject.SetActive(true);
                Debug.Log($"<color=#00FFFF><b>[기믹 발동]</b></color> 매니저가 5번을 선택하여 구석의 귀신 비주얼이 활성화되었습니다!");

                
            }
        }

        // 3. 이미 소멸했거나 플레이어가 없으면 이후 연산 스킵
        if (isLookedAt || playerTransform == null || ghostVisualObject == null) return;

        // 플레이어와 귀신의 평면(X, Z) 거리 계산
        Vector3 playerPosPlane = new Vector3(playerTransform.position.x, 0, playerTransform.position.z);
        Vector3 ghostPosPlane = new Vector3(ghostVisualObject.transform.position.x, 0, ghostVisualObject.transform.position.z);

        float currentDistance = Vector3.Distance(playerPosPlane, ghostPosPlane);
        Debug.DrawLine(playerTransform.position, ghostVisualObject.transform.position, Color.red);

        // 🎯 거리 기준 소멸 체크
        if (currentDistance <= disappearDistance)
        {
            isLookedAt = true;
            ghostVisualObject.SetActive(false); // 귀신 모델링만 쏙 끕니다.
            Debug.Log($"<color=pink><b>[귀신 자체 소멸]</b></color> 플레이어가 {currentDistance:F2}m 까지 접근하여 귀신이 사라집니다.");
        }
    }

    // 라운드가 바뀌거나 리셋될 때 매니저가 리셋 로직을 돌린다면 상태를 초기화해 줍니다.
    private void OnDisable()
    {
        isSpawned = false;
        isLookedAt = false;
        if (ghostVisualObject != null) ghostVisualObject.SetActive(false);
    }
}