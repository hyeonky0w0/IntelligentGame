using UnityEngine;
using UnityEngine.SceneManagement;

public class StageProgressUI : MonoBehaviour
{
    private static StageProgressUI instance;
    public static StageProgressUI Instance => instance;

    [Header("Rabbit (Stage1 씬 Inspector에서 직접 연결)")]
    public RectTransform rabbitRect;

    [Header("Rabbit 이동 범위")]
    public float rabbitStartX = -10f;
    public float rabbitEndX = 300f;
    public float rabbitY = 303f;

    [Header("각 스테이지가 끝나는 BGM 시간 (초) - Stage1~6")]
    public float[] stageEndTimes = { 40.716f, 52.7f, 79f, 87f, 95.5f, 129f };

    private static readonly string[] validScenes =
        { "Stage1", "Stage2", "Stage3", "Stage4", "Stage5", "Stage6" };

    private RectTransform progressBarRect;
    private bool isActive = false;
    private int currentStageIndex = -1;
    private float totalAllStagesDuration = 0f;

    void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);
        totalAllStagesDuration = stageEndTimes[stageEndTimes.Length - 1];
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void SetProgressBar(RectTransform barRect)
    {
        progressBarRect = barRect;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        int idx = System.Array.IndexOf(validScenes, scene.name);
        isActive = idx >= 0;

        if (rabbitRect != null)
            rabbitRect.gameObject.SetActive(isActive);

        if (!isActive)
        {
            currentStageIndex = -1;
            progressBarRect = null;
            return;
        }

        currentStageIndex = idx;
        progressBarRect = null;

        if (currentStageIndex == 0)
        {
            if (BGMManager.Instance != null)
                BGMManager.Instance.MarkGameStart();
            if (rabbitRect != null)
                rabbitRect.anchoredPosition = new Vector2(rabbitStartX, rabbitY);
        }

        if (rabbitRect != null)
            rabbitRect.gameObject.SetActive(true);
    }

    void Update()
    {
        if (!isActive) return;
        if (rabbitRect == null) return;
        if (BGMManager.Instance == null) return;
        if (totalAllStagesDuration <= 0f) return;

        float totalElapsed = BGMManager.Instance.GetTotalGameElapsedTime();
        float globalProgress = Mathf.Clamp01(totalElapsed / totalAllStagesDuration);
        float rabbitX = Mathf.Lerp(rabbitStartX, rabbitEndX, globalProgress);
        rabbitRect.anchoredPosition = new Vector2(rabbitX, rabbitY);
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}