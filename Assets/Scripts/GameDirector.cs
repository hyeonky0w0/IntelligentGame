using System.Collections;
using UnityEngine;
using TMPro;

public class GameDirector : MonoBehaviour
{
    public static GameDirector Instance;

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text judgementText;

    private Coroutine _hideRoutine;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateScoreUI();
    }

    public void AddScore(int amount, string judgement)
    {
        ScoreManager.Instance.AddScore(amount);
        UpdateScoreUI();
        ShowJudgement(judgement);
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = ScoreManager.Instance.Score.ToString();
    }

    void ShowJudgement(string judgement)
    {
        if (judgementText == null) return;

        switch (judgement)
        {
            case "PERFECT!":
                judgementText.color = new Color(1f, 0.85f, 0f);
                break;
            case "GOOD":
                judgementText.color = new Color(0f, 1f, 0.5f);
                break;
            case "MISS":
                judgementText.color = new Color(1f, 0.3f, 0.3f);
                break;
        }

        judgementText.text = judgement;
        if (_hideRoutine != null) StopCoroutine(_hideRoutine);
        _hideRoutine = StartCoroutine(HideJudgement());
    }

    IEnumerator HideJudgement()
    {
        yield return new WaitForSeconds(0.6f);
        if (judgementText != null)
            judgementText.text = "";
    }
}