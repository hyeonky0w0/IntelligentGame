using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MiniGame2Director : MonoBehaviour
{
    [Header("=== UI 참조 ===")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI enlightenmentText;
    public TextMeshProUGUI resultTitleText;
    public TextMeshProUGUI resultDescText;
    public GameObject resultPanel;
    public Button retryButton;
    public Image screenFlash;
    public Image darknessOverlay;
    public Slider enlightenmentGauge;

    [Header("=== 튜토리얼 패널 ===")]
    public GameObject tutorialPanel;
    [Tooltip("튜토리얼 표시 시간 (초)")]
    public float tutorialDuration = 4f;

    [Header("=== 게임 설정 ===")]
    public int clearScore = 500;
    public string mainSceneName = "MainScene";

    [Header("=== 연출 ===")]
    public AudioSource bgmSource;
    public AudioClip bgmClip;
    public AudioClip enlightenmentSE;
    public AudioClip bonraeSE;
    public AudioClip clearSE;
    public AudioClip failSE;

    private float time = 30.0f;
    private int enlightenmentPoint = 0;
    private bool gameOver = false;
    private bool gameStarted = false;
    private ItemGenerator2 generator;
    private Coroutine flashCoroutine;

    // ───────────────────────────────
    public void GetEnlightenment()
    {
        if (gameOver || !gameStarted) return;
        enlightenmentPoint += 50;
        UpdateGauge();
        if (enlightenmentSE != null)
            AudioSource.PlayClipAtPoint(enlightenmentSE, Camera.main.transform.position);
    }

    public void GetBonrae()
    {
        if (gameOver || !gameStarted) return;
        enlightenmentPoint = Mathf.Max(0, enlightenmentPoint / 2);
        UpdateGauge();
        if (bonraeSE != null)
            AudioSource.PlayClipAtPoint(bonraeSE, Camera.main.transform.position);
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashRed());
    }

    // ───────────────────────────────
    void Start()
    {
        generator = FindFirstObjectByType<ItemGenerator2>();

        // 마우스 커서 표시
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // UI 초기화
        resultPanel.SetActive(false);
        if (retryButton != null)
        {
            retryButton.gameObject.SetActive(false);
            retryButton.onClick.AddListener(OnRetryClicked);
        }
        if (screenFlash != null)
        {
            screenFlash.color = new Color(1, 0, 0, 0);
            screenFlash.raycastTarget = false;
        }
        if (darknessOverlay != null)
        {
            darknessOverlay.color = new Color(0, 0, 0, 0);
            darknessOverlay.raycastTarget = false;
        }
        if (enlightenmentGauge != null)
        {
            enlightenmentGauge.minValue = 0;
            enlightenmentGauge.maxValue = clearScore;
            enlightenmentGauge.value = 0;
        }

        // 튜토리얼 → 일정 시간 후 자동 시작
        float duration = PlayerPrefs.GetFloat("TutorialDuration", tutorialDuration);
        PlayerPrefs.DeleteKey("TutorialDuration");
        StartCoroutine(TutorialThenStart(duration));
    }

    // ───────────────────────────────
    // 튜토리얼 표시 → 시간 지나면 자동으로 게임 시작
    // ───────────────────────────────
    IEnumerator TutorialThenStart(float duration)
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
            yield return new WaitForSecondsRealtime(duration); // timeScale 영향 안 받음
            tutorialPanel.SetActive(false);
        }

        StartGame();
    }

    void StartGame()
    {
        gameStarted = true;

        // 튜토리얼 끝났으니 아이템 생성 시작
        if (generator != null) generator.StartSpawning();

        if (bgmSource != null && bgmClip != null)
        {
            bgmSource.clip = bgmClip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    // ───────────────────────────────
    void Update()
    {
        if (gameOver || !gameStarted) return;

        time -= Time.deltaTime;
        UpdateTimerUI();
        UpdateDarknessOverlay();

        if (time <= 0)
        {
            time = 0;
            if (generator != null) generator.StopSpawning();
            EndGame();
        }
        else if (time < 5f) generator.SetParameter(0.35f, -0.07f, 7);
        else if (time < 10f) generator.SetParameter(0.5f, -0.06f, 5);
        else if (time < 20f) generator.SetParameter(0.75f, -0.045f, 4);
        else generator.SetParameter(1.2f, -0.03f, 2);
    }

    // ───────────────────────────────
    void UpdateTimerUI()
    {
        if (timerText == null) return;
        timerText.color = time <= 10f ? Color.red : Color.white;
        timerText.text = Mathf.CeilToInt(time).ToString();
    }

    void UpdateGauge()
    {
        if (enlightenmentText != null)
            enlightenmentText.text = $"깨달음  {enlightenmentPoint} / {clearScore}";
        if (enlightenmentGauge != null)
            enlightenmentGauge.value = enlightenmentPoint;
    }

    void UpdateDarknessOverlay()
    {
        if (darknessOverlay == null) return;
        float alpha = Mathf.Lerp(0f, 0.4f, 1f - (time / 30f));
        darknessOverlay.color = new Color(0, 0, 0, alpha);
    }

    void EndGame()
    {
        gameOver = true;
        bool success = enlightenmentPoint >= clearScore;

        if (screenFlash != null) screenFlash.raycastTarget = false;
        if (darknessOverlay != null) darknessOverlay.raycastTarget = false;
        if (bgmSource != null) StartCoroutine(FadeOutBGM());

        resultPanel.SetActive(true);

        if (success)
        {
            resultTitleText.text = "수행 완료";
            resultTitleText.color = new Color(1f, 0.9f, 0.4f);
            resultDescText.text =
                $"깨달음  {enlightenmentPoint}  /  {clearScore}\n\n" +
                "그대의 마음이 맑아졌도다.\n사유의 문이 열리리니, 나아가라.";
            if (retryButton != null) retryButton.gameObject.SetActive(false);
            if (clearSE != null)
                AudioSource.PlayClipAtPoint(clearSE, Camera.main.transform.position);
            PlayerPrefs.SetInt("MiniGame2Result", 1);
            StartCoroutine(ReturnToMainScene());
        }
        else
        {
            resultTitleText.text = "번뇌에 잠기다";
            resultTitleText.color = new Color(0.8f, 0.1f, 0.1f);
            resultDescText.text =
                $"깨달음  {enlightenmentPoint}  /  {clearScore}\n\n" +
                "마음을 비우지 못하였구나.\n다시 수행하라.";
            if (retryButton != null) retryButton.gameObject.SetActive(true);
            if (failSE != null)
                AudioSource.PlayClipAtPoint(failSE, Camera.main.transform.position);
            PlayerPrefs.SetInt("MiniGame2Result", 0);
        }
    }

    void OnRetryClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator ReturnToMainScene()
    {
        yield return new WaitForSeconds(4.0f);
        PlayerPrefs.SetInt("FromMiniGame", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene(mainSceneName);
    }

    IEnumerator FlashRed()
    {
        if (screenFlash == null) yield break;
        screenFlash.color = new Color(1, 0, 0, 0.45f);
        yield return new WaitForSeconds(0.08f);
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.4f;
            screenFlash.color = new Color(1, 0, 0, Mathf.Lerp(0.45f, 0f, t));
            yield return null;
        }
        screenFlash.color = new Color(1, 0, 0, 0);
    }

    IEnumerator FadeOutBGM()
    {
        float startVol = bgmSource.volume;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 1.5f;
            bgmSource.volume = Mathf.Lerp(startVol, 0f, t);
            yield return null;
        }
        bgmSource.Stop();
    }
}