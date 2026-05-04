using UnityEngine;

public class Stage5Director : MonoBehaviour
{
    [Header("사운드")]
    public AudioClip hitSound;
    private AudioSource _audioSource;

    public static Stage5Director Instance;

    public static readonly long[] SECTION1_TIMES = { 92730, 92935, 93140, 93350 };
    public static readonly long[] SECTION2_TIMES = { 93582, 93790, 93997, 94207 };
    public static readonly long[] SECTION3_TIMES = { 94433, 94870, 95285 };

    public static long PLAY_START = 92730;
    public static long END_TIME = 95999;

    private const int JUDGEMENT_OFFSET_MS = 0;
    private const int SUCCESS_WINDOW_MS = 250;
    private const int GOOD_WINDOW_MS = 220;

    public static long CurrentMusicTimeMs = 0;

    public static int Section1HitCount = 0;
    public static int Section2HitCount = 0;
    public static int Section3HitCount = 0;

    public static bool Section1Done = false;
    public static bool Section2Done = false;
    public static bool Section3Done = false;

    private int sec1Index = 0;
    private int sec2Index = 0;
    private int sec3Index = 0;

    private SpatulaController spatulaController;

    void Awake()
    {
        Instance = this;
        Section1HitCount = 0;
        Section2HitCount = 0;
        Section3HitCount = 0;
        Section1Done = false;
        Section2Done = false;
        Section3Done = false;
        sec1Index = 0;
        sec2Index = 0;
        sec3Index = 0;
        _audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        spatulaController = FindObjectOfType<SpatulaController>();
    }

    void Update()
    {
        CurrentMusicTimeMs = (long)(BGMManager.Instance.GetCurrentTime() * 1000);

        if (!Section1Done && CurrentMusicTimeMs > SECTION1_TIMES[SECTION1_TIMES.Length - 1] + SUCCESS_WINDOW_MS)
            Section1Done = true;

        if (!Section2Done && CurrentMusicTimeMs > SECTION2_TIMES[SECTION2_TIMES.Length - 1] + SUCCESS_WINDOW_MS)
            Section2Done = true;

        if (!Section3Done && CurrentMusicTimeMs > SECTION3_TIMES[SECTION3_TIMES.Length - 1] + SUCCESS_WINDOW_MS)
            Section3Done = true;
    }

    public void OnSpacePressed()
    {
        long now = CurrentMusicTimeMs;

        bool inSec1 = now >= SECTION1_TIMES[0] - SUCCESS_WINDOW_MS
                   && now <= SECTION1_TIMES[SECTION1_TIMES.Length - 1] + SUCCESS_WINDOW_MS
                   && !Section1Done;

        bool inSec2 = now >= SECTION2_TIMES[0] - SUCCESS_WINDOW_MS
                   && now <= SECTION2_TIMES[SECTION2_TIMES.Length - 1] + SUCCESS_WINDOW_MS
                   && !Section2Done;

        bool inSec3 = now >= SECTION3_TIMES[0] - SUCCESS_WINDOW_MS
                   && now <= SECTION3_TIMES[SECTION3_TIMES.Length - 1] + SUCCESS_WINDOW_MS
                   && !Section3Done;

        if (inSec1) ProcessSection(SECTION1_TIMES, ref sec1Index, ref Section1HitCount, "Section1");
        else if (inSec2) ProcessSection(SECTION2_TIMES, ref sec2Index, ref Section2HitCount, "Section2");
        else if (inSec3) ProcessSection(SECTION3_TIMES, ref sec3Index, ref Section3HitCount, "Section3");
    }

    void ProcessSection(long[] times, ref int index, ref int hitCount, string sectionName)
    {
        if (index >= times.Length) return;

        long adjusted = CurrentMusicTimeMs + JUDGEMENT_OFFSET_MS;

        while (index < times.Length && adjusted - times[index] > SUCCESS_WINDOW_MS)
            index++;

        if (index >= times.Length) return;

        long target = times[index];
        long diff = System.Math.Abs(adjusted - target);

        if (target - adjusted > SUCCESS_WINDOW_MS) return;
        if (diff > SUCCESS_WINDOW_MS) return;

        string judgement;
        int scoreAmount;

        if (diff <= 80) { judgement = "PERFECT!"; scoreAmount = 100; }
        else if (diff <= GOOD_WINDOW_MS) { judgement = "GOOD"; scoreAmount = 50; }
        else { judgement = "MISS"; scoreAmount = -10; }

        if (judgement != "MISS") hitCount++;
        index++;

        GameDirector.Instance.AddScore(scoreAmount, judgement);

        if (judgement != "MISS" && _audioSource != null && hitSound != null)
            _audioSource.PlayOneShot(hitSound);

        if (judgement != "MISS" && spatulaController != null)
            spatulaController.OnHit();
    }
}