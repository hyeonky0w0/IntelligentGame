using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpatulaController : MonoBehaviour
{
    // ===================================================================
    // Inspector 연결
    // ===================================================================
    [Header("Canvas UI")]
    public RectTransform spatulaRect;
    public Image spatulaImage;

    [Header("케이크 영역")]
    public RectTransform cakeRect;

    [Header("기울기 설정")]
    public float tiltAngle = 15f;  // 기울어지는 각도
    public float tiltDuration = 0.08f; // 기울어지는 시간 (초)
    public float returnDuration = 0.12f; // 돌아오는 시간 (초)

    // ===================================================================
    // 내부 상태
    // ===================================================================
    private float startX;
    private float cakeWidth;
    private float targetX;
    private const float MOVE_SPEED = 3f;

    private Coroutine tiltRoutine = null;

    // ===================================================================
    // Start
    // ===================================================================
    void Start()
    {
        if (cakeRect != null)
        {
            cakeWidth = cakeRect.rect.width;
            startX = cakeRect.position.x - cakeWidth / 2f;
        }
        else
        {
            cakeWidth = Screen.width * 0.6f;
            startX = Screen.width * 0.2f;
        }

        targetX = startX;
        if (spatulaRect != null)
        {
            spatulaRect.position = new Vector3(startX, spatulaRect.position.y, 0f);
            spatulaRect.rotation = Quaternion.identity;
        }
    }

    // ===================================================================
    // Update
    // ===================================================================
    void Update()
    {
        UpdateSection();
        UpdatePosition();
    }

    // ===================================================================
    // 구간 완료에 따라 목표 X 갱신
    // ===================================================================
    void UpdateSection()
    {
        if (Stage5Director.Section1Done && Stage5Director.Section1HitCount >= 2)
            targetX = startX + cakeWidth * (1f / 3f);

        if (Stage5Director.Section2Done && Stage5Director.Section2HitCount >= 2)
            targetX = startX + cakeWidth * (2f / 3f);

        if (Stage5Director.Section3Done && Stage5Director.Section3HitCount >= 2)
            targetX = startX + cakeWidth;
    }

    // ===================================================================
    // 부드러운 X 이동
    // ===================================================================
    void UpdatePosition()
    {
        if (spatulaRect == null) return;

        float currentX = spatulaRect.position.x;
        float newX = Mathf.Lerp(currentX, targetX, Time.deltaTime * MOVE_SPEED);
        spatulaRect.position = new Vector3(newX, spatulaRect.position.y, 0f);
    }

    // ===================================================================
    // 스페이스바 성공 판정 시 Stage5Director에서 호출
    // ===================================================================
    public void OnHit()
    {
        if (tiltRoutine != null) StopCoroutine(tiltRoutine);
        tiltRoutine = StartCoroutine(TiltRoutine());
    }

    // ===================================================================
    // 기울기 코루틴: 기울어졌다가 돌아옴
    // ===================================================================
    IEnumerator TiltRoutine()
    {
        float elapsed = 0f;

        // 기울어지기
        while (elapsed < tiltDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / tiltDuration;
            float angle = Mathf.Lerp(0f, tiltAngle, t);
            spatulaRect.rotation = Quaternion.Euler(0f, 0f, angle);
            yield return null;
        }
        spatulaRect.rotation = Quaternion.Euler(0f, 0f, tiltAngle);

        elapsed = 0f;

        // 돌아오기
        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / returnDuration;
            float angle = Mathf.Lerp(tiltAngle, 0f, t);
            spatulaRect.rotation = Quaternion.Euler(0f, 0f, angle);
            yield return null;
        }
        spatulaRect.rotation = Quaternion.identity;
        tiltRoutine = null;
    }
}