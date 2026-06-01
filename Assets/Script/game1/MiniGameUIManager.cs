using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiniGameUIManager : MonoBehaviour
{
    public static MiniGameUIManager Instance { get; private set; }

    [Header("하트 UI — Heart1, Heart2, Heart3 Image 순서대로")]
    public Image[] heartImages;

    [Tooltip("하트 스프라이트 (없으면 Color로 대체됨)")]
    public Sprite heartFull;
    public Sprite heartEmpty;

    [Tooltip("스프라이트 없을 때 켜진 하트 색 (기본 빨강)")]
    public Color heartOnColor = new Color(1f, 0.2f, 0.2f, 1f);

    [Tooltip("스프라이트 없을 때 꺼진 하트 색 (기본 어두운 회색)")]
    public Color heartOffColor = new Color(0.3f, 0.3f, 0.3f, 1f);

    [Header("웨이브 텍스트")]
    public TextMeshProUGUI waveText;

    [Header("격파 카운트")]
    public TextMeshProUGUI deflectText;
    int _deflectCount = 0;

    [Header("결과 패널")]
    public GameObject resultPanel;
    public TextMeshProUGUI resultLabel;
    public TextMeshProUGUI resultSubLabel;
    public Button retryButton;

    [Header("결과 문구")]
    public string successMain = "시련을 견뎌냈다";
    public string successSub = "석상이 길을 열어준다…";
    public string failMain = "심등(心燈)이 꺼졌다";
    public string failSub = "경비원은 어둠에 삼켜졌다.";

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        resultPanel?.SetActive(false);
        _deflectCount = 0;
        if (deflectText != null) deflectText.text = "격파: 0";

        // LifeManager보다 늦게 초기화될 수 있으니 Start에서도 갱신
    }

    void Start()
    {
        int lives = LifeManager.Instance != null ? LifeManager.Instance.CurrentLives : 3;
        UpdateHearts(lives);

        // 다시하기 버튼 초기화
        if (retryButton != null)
        {
            retryButton.gameObject.SetActive(false);
            retryButton.onClick.AddListener(OnRetryClicked);
        }
    }

    void OnRetryClicked()
    {
        LifeManager.Instance?.ResetLives();
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }


    // ── 하트 ──────────────────────────────────
    public void UpdateHearts(int remaining)
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] == null) continue;

            bool alive = i < remaining;

            if (heartFull != null && heartEmpty != null)
            {
                // 스프라이트가 연결된 경우
                heartImages[i].sprite = alive ? heartFull : heartEmpty;
                heartImages[i].color = Color.white;
            }
            else
            {
                // 스프라이트 없을 때: Color로 대체
                heartImages[i].color = alive ? heartOnColor : heartOffColor;
            }
        }
    }

    // ── 웨이브 ────────────────────────────────
    public void UpdateWave(int current, int total)
    {
        if (waveText != null)
            waveText.text = $"웨이브 {current} / {total}";
    }

    // ── 격파 카운트 ───────────────────────────
    public void AddDeflectCount()
    {
        _deflectCount++;
        if (deflectText != null)
            deflectText.text = $"격파: {_deflectCount}";
    }

    // ── 결과 패널 ─────────────────────────────
    public void ShowResult(bool success)
    {
        resultPanel?.SetActive(true);
        if (resultLabel != null)
            resultLabel.text = success ? successMain : failMain;
        if (resultSubLabel != null)
            resultSubLabel.text = success ? successSub : failSub;
        if (retryButton != null)
            retryButton.gameObject.SetActive(!success);
    }
}