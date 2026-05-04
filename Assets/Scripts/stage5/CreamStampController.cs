using UnityEngine;

public class CreamStampController : MonoBehaviour
{
    // ===================================================================
    // Inspector 연결
    // ===================================================================
    [Header("크림 오브젝트 (구간별 1개씩)")]
    public GameObject cream1Object;  // Section1 완료 시 표시 (cakeCream01)
    public GameObject cream2Object;  // Section2 완료 시 표시 (cakeCream02)
    public GameObject cream3Object;  // Section3 완료 시 표시 (cakeCream03)

    // ===================================================================
    // Start - 전부 비활성화
    // ===================================================================
    void Start()
    {
        if (cream1Object != null) cream1Object.SetActive(false);
        if (cream2Object != null) cream2Object.SetActive(false);
        if (cream3Object != null) cream3Object.SetActive(false);
    }

    // ===================================================================
    // Update - 구간 완료 + 성공 횟수 충족 시 크림 표시
    // ===================================================================
    void Update()
    {
        // Section1 완료 & 2번 이상 성공 → cream1 표시
        if (Stage5Director.Section1Done && Stage5Director.Section1HitCount >= 2)
            if (cream1Object != null) cream1Object.SetActive(true);

        // Section2 완료 & 2번 이상 성공 → cream2 표시
        if (Stage5Director.Section2Done && Stage5Director.Section2HitCount >= 2)
            if (cream2Object != null) cream2Object.SetActive(true);

        // Section3 완료 & 2번 이상 성공 → cream3 표시
        if (Stage5Director.Section3Done && Stage5Director.Section3HitCount >= 2)
            if (cream3Object != null) cream3Object.SetActive(true);
    }
}