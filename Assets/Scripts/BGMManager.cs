using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour
{
    private static BGMManager instance;
    private AudioSource audioSource;

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
                break;

            case "IntroScene":
                // 타이틀 BGM 멈추고 게임 BGM 시작
                PlayBGM(gameBGM, loop: true);
                break;

            case "Result":
                // ResultManager가 자체 BGM 처리하므로 여기선 정지만
                StopBGM();
                break;

                // Stage1~6은 IntroScene에서 시작한 gameBGM 계속 유지
                // → 아무것도 안 하면 그냥 이어서 재생됨
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

    public float GetCurrentTime() => audioSource.time;
    public bool IsPlaying() => audioSource.isPlaying;
    public static BGMManager Instance => instance;
}