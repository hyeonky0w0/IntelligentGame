using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class Stage3Director : MonoBehaviour
{
    
    public TMP_Text scoreText;
    public TMP_Text judgementText;

    
    public float perfectRange = 0.15f;
    public float goodRange = 0.30f;

    
    public float promptShowTime = 0.6f;

    
    public float[] noteTimes = { 58.660f, 58.895f, 59.395f, 60.191f, 60.380f, 60.615f, 61.027f };
    public int[] noteDirs = { 3, 3, 3, 3, 3, 3, 3 };

    
    GameObject dough;
    GameObject whipping;
    DoughStageChanger doughChanger;

    
    int spawnIndex = 0;
    int hitIndex = 0;
    List<int> activeQueue = new List<int>();
    Coroutine hideRoutine;

    void Start()
    {
        this.dough = GameObject.Find("dough");
        this.whipping = GameObject.Find("whipping");
        this.doughChanger = GameObject.Find("dough")?.GetComponent<DoughStageChanger>();

        this.spawnIndex = 0;
        this.hitIndex = 0;
        this.activeQueue.Clear();

        UpdateScoreUI();
    }

    void Update()
    {
        PromptCheck();
        MissCheck();
        InputCheck();
    }

    void PromptCheck()
    {
        if (this.spawnIndex >= this.noteTimes.Length) return;

        float bgmTime = BGMManager.Instance.GetCurrentTime();
        float showAt = this.noteTimes[this.spawnIndex] - this.promptShowTime;

        if (bgmTime >= showAt)
        {
            this.activeQueue.Add(this.spawnIndex);
            this.spawnIndex++;
        }
    }

    void MissCheck()
    {
        if (this.hitIndex >= this.noteTimes.Length) return;

        float bgmTime = BGMManager.Instance.GetCurrentTime();
        float hitTime = this.noteTimes[this.hitIndex];

        if (bgmTime > hitTime + this.goodRange)
        {
            if (this.activeQueue.Count > 0)
                this.activeQueue.RemoveAt(0);

            this.hitIndex++;
            ApplyJudgement("MISS");
        }
    }

    void InputCheck()
    {
        bool pressedD = Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame;

        if (!pressedD) return;
        if (this.hitIndex >= this.noteTimes.Length) return;

        float bgmTime = BGMManager.Instance.GetCurrentTime();
        float hitTime = this.noteTimes[this.hitIndex];

        float diff = Mathf.Abs(bgmTime - hitTime);
        if (diff > this.goodRange * 2f) return;

        string judgement = "";
        if (diff <= this.perfectRange) judgement = "PERFECT!";
        else if (diff <= this.goodRange) judgement = "GOOD";
        else judgement = "MISS";

        if (this.activeQueue.Count > 0)
            this.activeQueue.RemoveAt(0);

        this.hitIndex++;

        TriggerWhipping(judgement);
        ApplyJudgement(judgement);
    }

    void TriggerWhipping(string judgement)
    {
        if (this.whipping != null)
            this.whipping.GetComponent<WhippingController>().OnHit(judgement);
    }

    void ApplyJudgement(string judgement)
    {
        int score = 0;
        if (judgement == "PERFECT!") score = 100;
        else if (judgement == "GOOD") score = 50;
        else score = -50;

        ScoreManager.Instance.AddScore(score);
        UpdateScoreUI();
        ShowJudgement(judgement);

        if (judgement == "PERFECT!" || judgement == "GOOD")
        {
            if (this.doughChanger != null)
                this.doughChanger.OnGoodHit();
        }
    }

    void UpdateScoreUI()
    {
        if (this.scoreText != null)
            this.scoreText.text = "SCORE\n" + ScoreManager.Instance.Score.ToString();
    }

    void ShowJudgement(string judgement)
    {
        if (this.judgementText == null) return;

        if (judgement == "PERFECT!") this.judgementText.color = new Color(1f, 0.85f, 0f);
        else if (judgement == "GOOD") this.judgementText.color = new Color(0f, 1f, 0.5f);
        else this.judgementText.color = new Color(1f, 0.3f, 0.3f);

        this.judgementText.text = judgement;

        if (this.hideRoutine != null) StopCoroutine(this.hideRoutine);
        this.hideRoutine = StartCoroutine(HideJudgement());
    }

    IEnumerator HideJudgement()
    {
        yield return new WaitForSeconds(0.6f);
        if (this.judgementText != null)
            this.judgementText.text = "";
    }

}