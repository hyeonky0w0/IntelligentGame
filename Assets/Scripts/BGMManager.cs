using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour
{
    private static BGMManager instance;
    private AudioSource audioSource;

    private float stageStartTime = 0f;
    private float bgmStartOffset = 0f; // IntroScene에서 gameBGM 시작 시점|
    private float gameStartOffset = 0f;

    [Header("BGM 클립")]
    public AudioClip titleBGM;    // 타이틀 + 패널 배경음악
    public AudioClip gameBGM;     // 기존 게임 BGM (Stage용)

    private bool _resultTransitionEnabled = false;
    private bool _transitioned = false;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _resultTransitionEnabled = false;
        _transitioned = false;
        switch (scene.name)
        {
            case "Title":
                PlayBGM(titleBGM, loop: true);
                bgmStartOffset = 0f;
                break;
            case "IntroScene":
                PlayBGM(gameBGM, loop: true);
                bgmStartOffset = audioSource.time;
                break;
            case "Result":
                StopBGM();
                break;
        }
    }


    void Update()
    {
        if (_resultTransitionEnabled && !_transitioned && audioSource != null)
        {
            if (!audioSource.isPlaying)
            {
                _transitioned = true;
                SceneManager.LoadScene("Result");
            }
        }
    }

    void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (clip == null || audioSource == null) return;

        // 이미 같은 클립 재생 중이면 중복 재시작 방지
        if (audioSource.clip == clip && audioSource.isPlaying) return;

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.Play();
    }

    public void EnableResultTransitionOnEnd()
    {
        _resultTransitionEnabled = true;
        _transitioned = false;
        audioSource.loop = false;
    }

    public void RestartBGM()
    {
        _resultTransitionEnabled = false;
        _transitioned = false;
        audioSource.Stop();
        audioSource.time = 0f;
        audioSource.Play();
    }

    public void StopBGM()
    {
        if (audioSource != null)
            audioSource.Stop();
    }

    public float GetBGMLength()
    {
        if (audioSource != null && audioSource.clip != null)
            return audioSource.clip.length;
        return 0f;
    }

    // Stage 진입 시 호출 - StageProgressUI가 호출함
    public void MarkStageStart()
    {
        stageStartTime = audioSource.time;
    }

    // Stage 기준 경과 시간 반환
    public float GetStageElapsedTime()
    {
        return audioSource.time - stageStartTime;
    }
    public void MarkGameStart()
    {
        gameStartOffset = audioSource.time;
        Debug.Log($"GameStart offset: {gameStartOffset}");
    }

    public float GetTotalGameElapsedTime()
    {
        if (audioSource == null) return 0f;
        return audioSource.time - gameStartOffset;
    }

    public float GetCurrentTime() => audioSource.time;
    public bool IsPlaying() => audioSource.isPlaying;
    public static BGMManager Instance => instance;
}