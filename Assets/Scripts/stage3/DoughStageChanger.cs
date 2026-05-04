using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoughStageChanger : MonoBehaviour
{
    [Header("반죽 이미지 (dough 오브젝트의 Image)")]
    public Image doughImage;
    public Sprite dough1;   // 기본
    public Sprite dough2;   // 58초 구간 클리어
    public Sprite dough3;   // 65초 구간 클리어
    public Sprite dough4;   // 72초 구간 클리어
    public Sprite dough5;   // 77초 구간 클리어

    [Header("구간 클리어 기준 (good 이상 최소 횟수)")]
    public int clearCount = 2;

    // ── 구간 정의 (시작~끝 BGM 시간) ──
    // 각 구간 안에서 good/perfect 횟수 카운트
    float[][] sections = new float[][]
    {
        new float[] { 58.660f, 61.027f },   // 구간1 → dough2
        new float[] { 65.424f, 67.878f },   // 구간2 → dough3
        new float[] { 72.221f, 73.881f },   // 구간3 → dough4
        new float[] { 77.400f, 78.686f },   // 구간4 → dough5
    };

    // 내부 변수
    int currentSection = 0;   // 현재 체크 중인 구간 인덱스
    int hitCount = 0;   // 현재 구간에서 good 이상 횟수
    bool sectionDone = false;

    void Start()
    {
        this.currentSection = 0;
        this.hitCount = 0;
        this.sectionDone = false;

        // 시작 시 dough1로 초기화
        SetDough(1);
    }

    void Update()
    {
        if (this.currentSection >= this.sections.Length) return;

        float bgmTime = BGMManager.Instance.GetCurrentTime();
        float sectionEnd = this.sections[this.currentSection][1];

        // 구간이 끝났는데 아직 처리 안 됐으면
        if (bgmTime > sectionEnd + 0.5f && !this.sectionDone)
        {
            // 클리어 못해도 다음 구간으로 넘어감
            NextSection();
        }
    }

    // ──────────────────────────────────────────
    // Stage3Director에서 판정 결과 전달받음
    // good 이상일 때만 호출
    // ──────────────────────────────────────────
    public void OnGoodHit()
    {
        if (this.currentSection >= this.sections.Length) return;

        float bgmTime = BGMManager.Instance.GetCurrentTime();
        float sectionStart = this.sections[this.currentSection][0];
        float sectionEnd = this.sections[this.currentSection][1];

        // 현재 구간 안에서 눌렀는지 확인
        if (bgmTime < sectionStart - 0.5f || bgmTime > sectionEnd + 0.5f) return;

        this.hitCount++;

        // clearCount 이상이면 dough 이미지 변경
        if (this.hitCount >= this.clearCount && !this.sectionDone)
        {
            this.sectionDone = true;
            SetDough(this.currentSection + 2); // 구간1→dough2, 구간2→dough3 ...
            NextSection();
        }
    }

    // ──────────────────────────────────────────
    // 다음 구간으로 이동
    // ──────────────────────────────────────────
    void NextSection()
    {
        this.currentSection++;
        this.hitCount = 0;
        this.sectionDone = false;
    }

    // ──────────────────────────────────────────
    // dough 이미지 교체
    // ──────────────────────────────────────────
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