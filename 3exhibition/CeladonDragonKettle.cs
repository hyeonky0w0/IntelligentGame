using UnityEngine;

public class CeladonDragonKettle : MonoBehaviour
{
    [Header("Anomaly ID Config")]
    public int anomalyID = 34; // 청자 주전자 ID: 34번

    public GameObject bloodEffect;
    public float disappearDistance = 2.0f;

    private Transform player;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    private void OnEnable()
    {
        // 🚨 [GameManager 체크] 현재 기믹이 선택되었을 때만 액체 이펙트 켜기
        if (GameManager.Instance != null && GameManager.Instance.currentAnomalyID == anomalyID)
        {
            if (bloodEffect != null) bloodEffect.SetActive(true);
            Debug.LogWarning("🚨 [이상현상 발동] 3-4. 청자 용모양 주전자에서 피가 새어 나옵니다.");
        }
        else
        {
            ResetAnomaly();
        }
    }

    private void OnDisable()
    {
        ResetAnomaly();
    }

    private void Update()
    {
        // 실시간 매니저 ID 체크
        if (GameManager.Instance == null || GameManager.Instance.currentAnomalyID != anomalyID) return;

        if (player == null || bloodEffect == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        Vector3 dirToKettle = (transform.position - player.position).normalized;
        float dot = Vector3.Dot(player.forward, dirToKettle);

        bool isLooking = dot > 0.4f;

        if (isLooking)
        {
            if (distance <= disappearDistance)
            {
                if (bloodEffect.activeSelf) bloodEffect.SetActive(false);
            }
            else
            {
                if (!bloodEffect.activeSelf) bloodEffect.SetActive(true);
            }
        }
    }

    public void ResetAnomaly()
    {
        if (bloodEffect != null) bloodEffect.SetActive(false);
    }
}