using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour
{
    private static BGMManager instance;
    private AudioSource audioSource;

    private float stageStartTime = 0f;
    private float bgmStartOffset = 0f;
    private float gameStartOffset = 0f;

    [Header("BGM 클립")]
    public AudioClip titleBGM;
    public AudioClip gameBGM;

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

    public void MarkStageStart()
    {
        stageStartTime = audioSource.time;
    }

    public float GetStageElapsedTime()
    {
        return audioSource.time - stageStartTime;
    }

    public void MarkGameStart()
    {
        gameStartOffset = audioSource.time;
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