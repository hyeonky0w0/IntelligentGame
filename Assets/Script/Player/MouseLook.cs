using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("플레이어 몸체")]
    public Transform playerBody;

    [Header("감도 (1~3 권장)")]
    public float sensitivity = 1.8f;

    [Header("스무딩 — 클수록 묵직함 (0.05~0.15)")]
    public float smoothTime = 0.08f;

    float xRotation = 0f;
    float yRotation = 0f;
    Vector2 currentDelta;
    Vector2 smoothVelocity;

    // 대화 중 마우스 잠금 해제 여부
    bool _cursorFree = false;

    void Start()
    {
        currentDelta = Vector2.zero;
        smoothVelocity = Vector2.zero;
        xRotation = 0f;
        yRotation = playerBody.eulerAngles.y;

        // FreeCursor() 제거 — IntroPanel이 직접 호출하므로 여기선 필요 없음
        // 기본은 잠금 상태로 시작, IntroPanel이 열리면 FreeCursor 호출됨
        LockCursor();
    }
    void Update()
    {
        // ── 커서 잠금 토글 (ESC 키) ──
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_cursorFree) LockCursor();
            else FreeCursor();
        }

        // 커서가 풀려있으면 카메라 회전 안 함
        if (_cursorFree) return;

        Vector2 targetDelta = new Vector2(
            Input.GetAxis("Mouse X"),
            Input.GetAxis("Mouse Y")
        ) * sensitivity;

        currentDelta = Vector2.SmoothDamp(
            currentDelta, targetDelta,
            ref smoothVelocity, smoothTime
        );

        xRotation -= currentDelta.y;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        yRotation += currentDelta.x;

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.localRotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    /// <summary>마우스 화면 중앙 고정 + 숨김 (게임 중)</summary>
    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _cursorFree = false;
    }

    /// <summary>마우스 잠금 해제 + 표시 (대화/UI)</summary>
    public void FreeCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _cursorFree = true;
    }
}