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

    bool _cursorFree = false;

    void Start()
    {
        currentDelta = Vector2.zero;
        smoothVelocity = Vector2.zero;
        xRotation = 0f;
        yRotation = playerBody.eulerAngles.y;

        LockCursor();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_cursorFree) LockCursor();
            else FreeCursor();
        }

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
    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _cursorFree = false;
    }

    public void FreeCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _cursorFree = true;
    }
}