using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoughStageChanger : MonoBehaviour
{
    [Header("반죽 이미지 (dough 오브젝트의 Image)")]
    public Image doughImage;
    public Sprite dough1;
    public Sprite dough2;
    public Sprite dough3;
    public Sprite dough4;
    public Sprite dough5;

    [Header("구간 클리어 기준 (good 이상 최소 횟수)")]
    public int clearCount = 2;

    float[][] sections = new float[][]
    {
        new float[] { 58.660f, 61.027f },
        new float[] { 65.424f, 67.878f },
        new float[] { 72.221f, 73.881f },
        new float[] { 77.400f, 78.686f },
    };

    int currentSection = 0;
    int hitCount = 0;
    bool sectionDone = false;

    void Start()
    {
        this.currentSection = 0;
        this.hitCount = 0;
        this.sectionDone = false;

        SetDough(1);
    }

    void Update()
    {
        if (this.currentSection >= this.sections.Length) return;

        float bgmTime = BGMManager.Instance.GetCurrentTime();
        float sectionEnd = this.sections[this.currentSection][1];

        if (bgmTime > sectionEnd + 0.5f && !this.sectionDone)
        {
            NextSection();
        }
    }

    public void OnGoodHit()
    {
        if (this.currentSection >= this.sections.Length) return;

        float bgmTime = BGMManager.Instance.GetCurrentTime();
        float sectionStart = this.sections[this.currentSection][0];
        float sectionEnd = this.sections[this.currentSection][1];

        if (bgmTime < sectionStart - 0.5f || bgmTime > sectionEnd + 0.5f) return;

        this.hitCount++;

        if (this.hitCount >= this.clearCount && !this.sectionDone)
        {
            this.sectionDone = true;
            SetDough(this.currentSection + 2);
            NextSection();
        }
    }

    void NextSection()
    {
        this.currentSection++;
        this.hitCount = 0;
        this.sectionDone = false;
    }

    void SetDough(int stage)
    {
        if (this.doughImage == null) return;

        Sprite target = null;
        if (stage == 1) target = this.dough1;
        else if (stage == 2) target = this.dough2;
        else if (stage == 3) target = this.dough3;
        else if (stage == 4) target = this.dough4;
        else if (stage == 5) target = this.dough5;

        if (target != null)
            this.doughImage.sprite = target;
    }
}