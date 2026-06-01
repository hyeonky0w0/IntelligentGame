using UnityEngine;

public class CameraWallClip : MonoBehaviour
{
    [Header("플레이어 눈 위치 기준점")]
    public Transform playerBody;

    [Header("기본 카메라 로컬 위치")]
    public Vector3 defaultLocalPos = Vector3.zero;

    [Header("벽 감지 레이어")]
    public LayerMask wallLayer;

    Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (playerBody == null) return;

        Vector3 origin = playerBody.position + Vector3.up * 1.6f;
        Vector3 dir = transform.position - origin;
        float dist = dir.magnitude;

        if (dist < 0.01f) return;

        if (Physics.SphereCast(origin, 0.2f, dir.normalized,
                               out RaycastHit hit, dist, wallLayer))
        {
            // 벽에 닿으면 카메라를 벽 바로 앞으로 당김
            float safeDist = Mathf.Max(hit.distance - 0.15f, 0.05f);
            transform.position = origin + dir.normalized * safeDist;
        }
        else
        {
            // 정상 위치 복원
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                defaultLocalPos,
                Time.deltaTime * 15f
            );
        }
    }
}