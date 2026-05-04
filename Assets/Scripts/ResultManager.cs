using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ResultManager : MonoBehaviour
{
    [Header("Result 판정 기준")]
    public int goodThreshold = 2000;    // 2000점 이상 → GOOD
    public int perfectThreshold = 4000; // 4000점 이상 → PERFECT

    [Header("Result 패널 오브젝트 (Inspector에서 연결)")]
    public GameObject perfectPanel;  // PERFECT Result UI
    public GameObject goodPanel;     // GOOD Result UI
    public GameObject missPanel;     // MISS Result UI

    [Header("Result BGM")]
    public AudioClip perfectBGM;     // PERFECT 전용 BGM
    public AudioClip normalBGM;      // GOOD / MISS 공통 BGM

    [Header("UI - 최종 점수 표시")]
    public TMP_Text perfectScoreText;
    public TMP_Text goodScoreText;
    public TMP_Text missScoreText;

    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        int score = ScoreManager.Instance.Score;

        // 패널 전부 비활성화 후 해당 패널만 활성화
        if (perfectPanel) perfectPanel.SetActive(false);
        if (goodPanel) goodPanel.SetActive(false);
        if (missPanel) missPanel.SetActive(false);

        if (score >= perfectThreshold)
        {
            ShowResult(perfectPanel, perfectScoreText, score);
            PlayBGM(perfectBGM);
        }
        else if (score >= goodThreshold)
        {
            ShowResult(goodPanel, goodScoreText, score);
            PlayBGM(normalBGM);
        }
        else
        {
            ShowResult(missPanel, missScoreText, score);
            PlayBGM(normalBGM);
        }
    }

    void ShowResult(GameObject panel, TMP_Text scoreText, int score)
    {
        if (panel != null)
            panel.SetActive(true);

        if (scoreText != null)
            scoreText.text = "SCORE\n" + score.ToString();
    }

    void PlayBGM(AudioClip clip)
    {
        if (_audioSource == null || clip == null) return;
        _audioSource.clip = clip;
        _audioSource.loop = false;
        _audioSource.Play();
    }

    // 버튼에 연결 - 타이틀로 돌아가기
    public void GoToTitle()
    {
        ScoreManager.Instance.ResetScore();
        SceneManager.LoadScene("Title"); // 타이틀 씬 이름에 맞게 수정
    }

    // 버튼에 연결 - 다시 하기
    public void Retry()
    {
        if (BGMManager.Instance != null)
            BGMManager.Instance.RestartBGM();

        ScoreManager.Instance.ResetScore();
        SceneManager.LoadScene("IntroScene"); 
    }
}