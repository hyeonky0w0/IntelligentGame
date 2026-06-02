using System.Collections;
using UnityEngine;

public class BasketController2 : MonoBehaviour
{
    [Header("=== 체스판 격자 설정 ===")]
    public float cellSize = 12.5f;
    public Transform boardTransform;
    public int boardCols = 8;
    public int boardRows = 8;
    public float basketY = 2.819f;

    [Header("=== 이동 설정 ===")]
    public float moveSpeed = 8f;

    [Header("=== 게임 디렉터 ===")]
    public MiniGame2Director director;

    [Header("=== 시각 연출 ===")]
    public Light collectLight;
    public Color enlightenmentColor = new Color(1f, 0.95f, 0.5f);
    public Color bonraeColor = new Color(0.6f, 0f, 0.8f);

    private Vector3 targetPosition;
    private Camera mainCam;
    private Coroutine lightCoroutine;
    private float boardMinX, boardMaxX, boardMinZ, boardMaxZ;

    void Start()
    {
        mainCam = Camera.main;

        if (director == null)
            director = FindFirstObjectByType<MiniGame2Director>();

        if (collectLight != null)
            collectLight.intensity = 0f;

        CalculateBoardBounds();
        targetPosition = SnapToGrid(new Vector3(transform.position.x, basketY, transform.position.z));
        transform.position = targetPosition;

        Collider myCol = GetComponent<Collider>();

        Rigidbody rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        HandleClick();
        SmoothMove();
    }

    void HandleClick()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) return;
        if (hit.collider.gameObject == gameObject) return;

        Vector3 snapped = SnapToGrid(hit.point);
        if (IsInsideBoard(snapped))
            targetPosition = snapped;
    }

    void SmoothMove()
    {
        transform.position = Vector3.MoveTowards(
            transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enlightenment"))
        {
            director?.GetEnlightenment();
            TriggerLight(enlightenmentColor);
            other.GetComponent<ItemController2>()?.OnCollected();
        }
        else if (other.CompareTag("Bonrae"))
        {
            director?.GetBonrae();
            TriggerLight(bonraeColor);
            other.GetComponent<ItemController2>()?.OnCollected();
        }
    }

    Vector3 SnapToGrid(Vector3 worldPos)
    {
        float originX = boardTransform != null ? boardTransform.position.x : 0f;
        float originZ = boardTransform != null ? boardTransform.position.z : 0f;
        float halfW = (boardCols * cellSize) / 2f;
        float halfH = (boardRows * cellSize) / 2f;

        int col = Mathf.Clamp(Mathf.FloorToInt((worldPos.x - (originX - halfW)) / cellSize), 0, boardCols - 1);
        int row = Mathf.Clamp(Mathf.FloorToInt((worldPos.z - (originZ - halfH)) / cellSize), 0, boardRows - 1);

        return new Vector3(
            (originX - halfW) + (col + 0.5f) * cellSize,
            basketY,
            (originZ - halfH) + (row + 0.5f) * cellSize
        );
    }

    void CalculateBoardBounds()
    {
        float originX = boardTransform != null ? boardTransform.position.x : 0f;
        float originZ = boardTransform != null ? boardTransform.position.z : 0f;
        float halfW = (boardCols * cellSize) / 2f;
        float halfH = (boardRows * cellSize) / 2f;

        boardMinX = originX - halfW;
        boardMaxX = originX + halfW;
        boardMinZ = originZ - halfH;
        boardMaxZ = originZ + halfH;
    }

    bool IsInsideBoard(Vector3 pos) =>
        pos.x >= boardMinX && pos.x <= boardMaxX &&
        pos.z >= boardMinZ && pos.z <= boardMaxZ;

    void TriggerLight(Color color)
    {
        if (collectLight == null) return;
        if (lightCoroutine != null) StopCoroutine(lightCoroutine);
        lightCoroutine = StartCoroutine(FlashLight(color));
    }

    IEnumerator FlashLight(Color color)
    {
        collectLight.color = color;
        collectLight.intensity = 3f;
        yield return new WaitForSeconds(0.05f);
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.35f;
            collectLight.intensity = Mathf.Lerp(3f, 0f, t);
            yield return null;
        }
        collectLight.intensity = 0f;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (boardTransform == null) return;
        float originX = boardTransform.position.x;
        float originZ = boardTransform.position.z;
        float halfW   = (boardCols * cellSize) / 2f;
        float halfH   = (boardRows * cellSize) / 2f;
        float y       = basketY;

        Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
        for (int c = 0; c <= boardCols; c++)
        {
            float x = (originX - halfW) + c * cellSize;
            Gizmos.DrawLine(new Vector3(x, y, originZ - halfH), new Vector3(x, y, originZ + halfH));
        }
        for (int r = 0; r <= boardRows; r++)
        {
            float z = (originZ - halfH) + r * cellSize;
            Gizmos.DrawLine(new Vector3(originX - halfW, y, z), new Vector3(originX + halfW, y, z));
        }

        Gizmos.color = new Color(0f, 1f, 1f, 0.7f);
        Vector3 t2 = Application.isPlaying ? targetPosition : SnapToGrid(transform.position);
        Gizmos.DrawWireCube(t2, new Vector3(cellSize * 0.9f, 0.05f, cellSize * 0.9f));
    }
#endif
}