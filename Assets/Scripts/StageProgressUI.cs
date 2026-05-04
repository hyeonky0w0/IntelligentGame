using UnityEngine;
using UnityEngine.SceneManagement;

public class StageProgressUI : MonoBehaviour
{
    private static StageProgressUI instance;
    public static StageProgressUI Instance => instance;

    [Header("Progress Bar")]
    public RectTransform progressBarRect;
    public RectTransform rabbitRect;

    [Header("Settings")]
    public float barWidth = 458f;
    public float rabbitWidth = 100f;
    public float stageStartTime = 0f;
    public float stageEndTime = 0f;
    public float rabbitStartX = -300f;
    public float rabbitEndX = 107f;

    private static readonly string[] validScenes =
        { "Stage1", "Stage2", "Stage3", "Stage4", "Stage5", "Stage6" };

    private bool isActive = false;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isActive = System.Array.IndexOf(validScenes, scene.name) >= 0;
        if (!isActive) return;

        // ProgressBar만 씬마다 찾기
        GameObject barObj = GameObject.Find("ProgressBar");
        if (barObj != null)
            progressBarRect = barObj.GetComponent<RectTransform>();
        else
            Debug.LogWarning("[StageProgressUI] ProgressBar 못 찾음!");

        // rabbitRect는 Inspector에서 연결된 채로 유지 (건드리지 않음)
        Debug.Log($"[StageProgressUI] 씬: {scene.name} | bar: {progressBarRect} | rabbit: {rabbitRect}");
    }

    void Update()
    {
        if (!isActive) return;
        if (BGMManager.Instance == null) return;
        if (progressBarRect == null || rabbitRect == null) return;

        float currentTime = BGMManager.Instance.GetCurrentTime();
        float totalTime = stageEndTime > 0f ? stageEndTime : BGMManager.Instance.GetBGMLength();
        float stageDuration = totalTime - stageStartTime;

        if (stageDuration <= 0f) return;

        float progress = Mathf.Clamp01((currentTime - stageStartTime) / stageDuration);
        UpdateProgressBar(progress);
    }

    void UpdateProgressBar(float progress)
    {
        float rabbitX = Mathf.Lerp(rabbitStartX, rabbitEndX, progress);
        rabbitRect.anchoredPosition = new Vector2(rabbitX, rabbitRect.anchoredPosition.y);
    }
}