using UnityEngine;

public class CreamStampController : MonoBehaviour
{
    [Header("크림 오브젝트 (구간별 1개씩)")]
    public GameObject cream1Object;
    public GameObject cream2Object;
    public GameObject cream3Object;

    void Start()
    {
        if (cream1Object != null) cream1Object.SetActive(false);
        if (cream2Object != null) cream2Object.SetActive(false);
        if (cream3Object != null) cream3Object.SetActive(false);
    }

    void Update()
    {
        if (Stage5Director.Section1Done && Stage5Director.Section1HitCount >= 2)
            if (cream1Object != null) cream1Object.SetActive(true);

        if (Stage5Director.Section2Done && Stage5Director.Section2HitCount >= 2)
            if (cream2Object != null) cream2Object.SetActive(true);

        if (Stage5Director.Section3Done && Stage5Director.Section3HitCount >= 2)
            if (cream3Object != null) cream3Object.SetActive(true);
    }
}