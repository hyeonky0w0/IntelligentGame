using UnityEngine;
using UnityEngine.UI;

public class JumpEgg : MonoBehaviour
{
    [Header("낙하 설정")]
    public float fallSpeed = 300f;
    public float gravity = 800f;
    public float destroyY = -700f;

    [Header("사운드")]
    public AudioClip hitSound;       
    private AudioSource _audioSource;

    [Header("판정 윈도우")]
    public float perfectWindow = 0.12f;
    public float goodWindow = 0.25f;

    private float _hitTime;
    private BGMManager _bgm;
    private RectTransform _rect;
    private Image _image;          

    private bool _isVisible = false;
    private bool _isFalling = false;
    private bool _isJudged = false;
    private float _currentSpeed = 0f;

    void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        _audioSource = GetComponent<AudioSource>();

        SetVisible(false);

        Debug.Log("AudioSource: " + _audioSource);
        Debug.Log("HitSound: " + hitSound);
    }

    public void Init(float hitTime, BGMManager bgm)
    {
        _hitTime = hitTime;
        _bgm = bgm;
    }

    void Update()
    {
        if (_isFalling)
        {
            _currentSpeed += gravity * Time.deltaTime;
            _rect.anchoredPosition += Vector2.down * _currentSpeed * Time.deltaTime;

            if (_rect.anchoredPosition.y < destroyY)
                Destroy(gameObject);
        }
    }

    public bool TryHit()
    {
        if (_isJudged) return false;

        float current = _bgm.GetCurrentTime();
        float diff = Mathf.Abs(current - _hitTime);

        _isJudged = true;

        if (diff <= perfectWindow)
        {
            GameDirector.Instance.AddScore(50, "PERFECT!");
            PlaySound(hitSound);    // Perfect일 때 재생
        }
        else if (diff <= goodWindow)
        {
            GameDirector.Instance.AddScore(30, "GOOD");
            PlaySound(hitSound);    // Good일 때도 같은 사운드
        }
        else
        {
            GameDirector.Instance.AddScore(-10, "MISS");
        }

        SetVisible(true);
        StartFall();
        return true;
    }

    void PlaySound(AudioClip clip)
    {
        Debug.Log($"PlaySound 호출됨 / clip={clip} / audioSource={_audioSource}");
        if (clip == null || _audioSource == null) return;
        _audioSource.PlayOneShot(clip);
    }

    public void ForceMiss()
    {
        if (_isJudged) return;
        _isJudged = true;
        GameDirector.Instance.AddScore(-10, "MISS");
        Destroy(gameObject);
    }

    public float HitTime => _hitTime;
    public bool IsJudged => _isJudged;

    void StartFall()
    {
        _currentSpeed = fallSpeed;
        _isFalling = true;
    }

    void SetVisible(bool visible)
    {
        _isVisible = visible;
        if (_image != null) _image.enabled = visible;
    }

}