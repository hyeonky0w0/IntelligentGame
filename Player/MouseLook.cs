using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("플레이어 몸체")]
    public Transform playerBody;

    [Header("감도 (1~3 권장)")]
    public float sensitivity = 1.8f;

    [Header("스무딩 — 클수록 묵직함 (0.05~0.15)")]
    public float smoothTime = 0.08f;

    [Header("상호작용 설정")]
    [Tooltip("일지나 노트북을 조준 클릭할 수 있는 최대 거리")]
    public float interactDistance = 3.5f;

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
        // Escape 키로 커서 임시 해제 통제
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_cursorFree) LockCursor();
            else FreeCursor();
        }

        // UI가 켜져서 커서가 풀린 상태라면 시선 회전 및 클릭 연산을 중단합니다.
        if (_cursorFree) return;

        // 🚨 [상호작용 클릭 감지]
        if (Input.GetMouseButtonDown(0))
        {
            HandleInteraction();
        }

        // 시선 회전 로직 (기존 코드 유지)
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

    /// <summary>
    /// 화면 중앙의 오브젝트를 레이저로 감지해서 상호작용하는 통합 시스템
    /// </summary>
    private void HandleInteraction()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Scene 뷰에서 조준선 시각화 (디버깅용)
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.red, 0.5f);

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            // 🔍 1. 부딪힌 오브젝트가 경비 일지인지 확인
            ReportUIController reportUI = hit.collider.GetComponent<ReportUIController>();
            if (reportUI != null)
            {
                Debug.LogWarning($"<color=#33FF33><b>[조준 성공]</b></color> 화면 정중앙에서 일지 <b>{hit.collider.name}</b> 클릭 포착!");
                reportUI.OpenReportUI();
                FreeCursor();
                return; // 상호작용을 처리했으므로 리턴
            }

            // 🚨 2. [노트북 미연동 버그 해결벽] 부딪힌 오브젝트가 경비실 노트북인지 확인
            OfficeLaptop laptop = hit.collider.GetComponent<OfficeLaptop>();
            if (laptop != null)
            {
                Debug.LogWarning($"<color=#FFFF33><b>[조준 성공]</b></color> 화면 정중앙에서 경비실 노트북 <b>{hit.collider.name}</b> 클릭 포착!");

                // 노트북 내부의 Interact 함수를 직접 기동시킵니다.
                laptop.Interact();
                return;
            }

            // 🔍 3. 만약 컴포넌트가 부모나 자식에 찢어져 있는 경우를 대비한 가드라인 예외 수색
            OfficeLaptop laptopInParent = hit.collider.GetComponentInParent<OfficeLaptop>();
            if (laptopInParent != null)
            {
                Debug.LogWarning($"<color=#FFFF33><b>[조준 성공]</b></color> 상위 오브젝트에서 노트북 컴포넌트 포착! <b>{laptopInParent.name}</b> 기동.");
                laptopInParent.Interact();
                return;
            }
        }
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