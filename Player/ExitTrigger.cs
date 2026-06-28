using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


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

    Vector3 _prevPos;
    bool _playerInside = false;
    Transform _playerTr;

    void Awake()
    {
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

        Vector3 moveDir = (_playerTr.position - _prevPos);

        if (moveDir.sqrMagnitude > 0.0001f)
        {
            moveDir.Normalize();
            float dot = Vector3.Dot(moveDir, exitDirection.normalized);

            if (dot >= directionThreshold)
            {
                if (PlayerPrefs.GetInt("MiniGame2Result", 0) == 1)
                {
                    _triggered = true;
                    StartCoroutine(FadeAndEnd());
                }
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
        if (fadeCanvas != null)
            fadeCanvas.gameObject.SetActive(true);

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
        PlayerPrefs.DeleteKey("MiniGame2Result");
        SceneManager.LoadScene(endingSceneName);
    }

    void OnDrawGizmosSelected()
    {
        if (!showGizmo) return;
        Gizmos.color = Color.cyan;
        Vector3 origin = transform.position;
        Gizmos.DrawLine(origin, origin + exitDirection.normalized * 2f);
        Gizmos.DrawWireSphere(origin + exitDirection.normalized * 2f, 0.15f);


        Gizmos.color = new Color(0f, 1f, 1f, 0.2f);
        Collider col = GetComponent<Collider>();
        if (col != null)
            Gizmos.DrawCube(col.bounds.center, col.bounds.size);
    }
}