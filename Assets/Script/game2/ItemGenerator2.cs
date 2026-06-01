using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator2 : MonoBehaviour
{
    [Header("=== 프리팹 ===")]
    public GameObject enlightenmentPrefab;
    public GameObject bonraePrefab;

    [Header("=== 체스판 기준 생성 ===")]
    [Tooltip("Chess Board Play Surface 연결 (Floor 말고!)")]
    public Transform boardTransform;

    [Tooltip("체스판 한 칸 크기 — BasketController2와 동일하게")]
    public float cellSize = 12.5f;

    [Tooltip("가로 칸 수")]
    public int boardCols = 8;
    [Tooltip("세로 칸 수")]
    public int boardRows = 8;

    [Tooltip("아이템 생성 높이")]
    public float spawnHeight = 20f;

    private float span = 1.2f;
    private float speed = -0.03f;
    private int bonraeRatio = 2;
    private float delta = 0f;

    // 외부에서 생성 on/off 제어
    private bool active = false;

    private List<Vector3> cellCenters = new List<Vector3>();

    // ───────────────────────────────
    // Director에서 호출
    // ───────────────────────────────
    public void SetParameter(float span, float speed, int bonraeRatio)
    {
        this.span = span;
        this.speed = speed;
        this.bonraeRatio = bonraeRatio;
    }

    /// <summary>튜토리얼 끝나면 Director가 호출 → 생성 시작</summary>
    public void StartSpawning()
    {
        active = true;
        delta = 0f;  // 첫 아이템이 즉시 나오지 않도록 리셋
    }

    /// <summary>게임 종료 시 Director가 호출 → 생성 중단</summary>
    public void StopSpawning()
    {
        active = false;
    }

    // ───────────────────────────────
    void Start()
    {
        BuildCellCenters();
        Debug.Log($"[ItemGenerator] 칸 수={cellCenters.Count}, 첫 칸={cellCenters[0]}, 마지막 칸={cellCenters[cellCenters.Count - 1]}");
        // 튜토리얼 중에는 생성 안 함 — StartSpawning() 호출 전까지 대기
    }

    void Update()
    {
        if (!active) return;  // 생성 비활성 상태면 무시

        delta += Time.deltaTime;
        if (delta < span) return;
        delta = 0f;
        SpawnItem();
    }

    void SpawnItem()
    {
        if (cellCenters.Count == 0) return;

        int dice = Random.Range(1, 11);
        GameObject prefab = (dice <= bonraeRatio) ? bonraePrefab : enlightenmentPrefab;
        if (prefab == null) { Debug.LogWarning("[ItemGenerator2] 프리팹 없음"); return; }

        Vector3 cell = cellCenters[Random.Range(0, cellCenters.Count)];
        Vector3 spawnPos = new Vector3(cell.x, spawnHeight, cell.z);

        GameObject item = Instantiate(prefab);
        item.transform.position = spawnPos;

        ItemController2 ctrl = item.GetComponent<ItemController2>();
        if (ctrl != null) ctrl.dropSpeed = speed;
    }

    void BuildCellCenters()
    {
        cellCenters.Clear();

        float cx = boardTransform != null ? boardTransform.position.x : 0f;
        float cz = boardTransform != null ? boardTransform.position.z : 0f;
        float startX = cx - (boardCols * cellSize) / 2f;
        float startZ = cz - (boardRows * cellSize) / 2f;

        for (int col = 0; col < boardCols; col++)
            for (int row = 0; row < boardRows; row++)
            {
                float x = startX + (col + 0.5f) * cellSize;
                float z = startZ + (row + 0.5f) * cellSize;
                cellCenters.Add(new Vector3(x, 0f, z));
            }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (boardTransform == null) return;

        float cx     = boardTransform.position.x;
        float cz     = boardTransform.position.z;
        float startX = cx - (boardCols * cellSize) / 2f;
        float startZ = cz - (boardRows * cellSize) / 2f;

        Gizmos.color = new Color(0f, 1f, 0f, 0.8f);
        for (int col = 0; col < boardCols; col++)
        for (int row = 0; row < boardRows; row++)
        {
            float x = startX + (col + 0.5f) * cellSize;
            float z = startZ + (row + 0.5f) * cellSize;
            Gizmos.DrawSphere(new Vector3(x, spawnHeight, z), cellSize * 0.15f);
        }

        Gizmos.color = new Color(0.3f, 0.6f, 1f, 0.3f);
        for (int col = 0; col < boardCols; col++)
        for (int row = 0; row < boardRows; row++)
        {
            float x = startX + (col + 0.5f) * cellSize;
            float z = startZ + (row + 0.5f) * cellSize;
            Gizmos.DrawLine(new Vector3(x, spawnHeight, z), new Vector3(x, 0f, z));
        }
    }
#endif
}