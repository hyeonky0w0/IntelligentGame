using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 입구/출구가 같은 위치일 때, 플레이어 이동 방향으로 입구/출구를 구분합니다.
///
/// [세팅 방법]
/// 1. 문밖 Collider 오브젝트에 이 스크립트 부착
/// 2. Collider → Is Trigger 체크
/// 3. Inspector에서 아래 항목 연결
///    - Exit Direction : 이 오브젝트의 "밖을 향하는 방향" (예: 문이 남쪽이면 Vector3(0,0,-1))
///    - Fade Canvas    : 페이드용 Canvas (전체화면 검정 Image 포함)
///    - Fade Image     : Canvas 안의 Image (Color alpha 0으로 시작)
///    - Ending Scene Name : 로드할 씬 이름
/// </summary>
public class ExitTrigger : MonoBehaviour
{
    [Header("방향 설정")]
    [Tooltip("'밖'을 향하는 방향 (World Space). Scene뷰에서 Gizmo 확인 가능.")]
    public Vector3 exitDirection = Vector3.forward;

    [Tooltip("이 값보다 Dot 결과가 커야 '나가는 방향'으로 판정 (0 = 수직, 1 = 완전히 같은 방향)")]
    [Range(0f, 1f)]
    public float directionThreshold = 0.3f;

    [Header("페이드 설정")]
    public Canvas fadeCanvas;
    public Image fadeImage;
    public float fadeDuration = 1.5f;

    [Header("씬 설정")]
    public string endingSceneName = "EndingScene";

    [Header("디버그")]
    public bool showGizmo = true;

    bool _triggered = false;

    // 플레이어 이전 프레임 위치 (이동 방향 계산용)
    Vector3 _prevPos;
    bool _playerInside = false;
    Transform _playerTr;

    void Awake()
    {
        // 페이드 이미지 투명하게 초기화
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }

        if (fadeCanvas != null)
            fadeCanvas.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerTr = other.transform;
        _prevPos = _playerTr.position;
        _playerInside = true;
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (_triggered) return;
        if (_playerTr == null) return;

        // 이동 방향 계산
        Vector3 moveDir = (_playerTr.position - _prevPos);

        if (moveDir.sqrMagnitude > 0.0001f) // 움직일 때만 판정
        {
            moveDir.Normalize();
            float dot = Vector3.Dot(moveDir, exitDirection.normalized);

            if (dot >= directionThreshold)
            {
                // minigame2 클리어 확인
                if (PlayerPrefs.GetInt("Minigame2Success", 0) == 1)
                {
                    _triggered = true;
                    StartCoroutine(FadeAndEnd());
                }
                // 클리어 안 됐으면 그냥 통과 (셔터가 막고 있으니 실제론 못 나감)
            }
        }

        _prevPos = _playerTr.position;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInside = false;
        _playerTr = null;
    }

    IEnumerator FadeAndEnd()
    {
        // 페이드 캔버스 활성화
        if (fadeCanvas != null)
            fadeCanvas.gameObject.SetActive(true);

        // 페이드 인 (검정으로)
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeDuration);
            if (fadeImage != null)
            {
                Color c = fadeImage.color;
                c.a = alpha;
                fadeImage.color = c;
            }
            yield return null;
        }

        // 씬 전환
        PlayerPrefs.DeleteKey("Minigame2Success"); // 플래그 정리
        SceneManager.LoadScene(endingSceneName);
    }

    // Scene 뷰에서 exitDirection 시각화
    void OnDrawGizmosSelected()
    {
        if (!showGizmo) return;
        Gizmos.color = Color.cyan;
        Vector3 origin = transform.position;
        Gizmos.DrawLine(origin, origin + exitDirection.normalized * 2f);
        Gizmos.DrawWireSphere(origin + exitDirection.normalized * 2f, 0.15f);

        // 트리거 박스 강조
        Gizmos.color = new Color(0f, 1f, 1f, 0.2f);
        Collider col = GetComponent<Collider>();
        if (col != null)
            Gizmos.DrawCube(col.bounds.center, col.bounds.size);
    }
}