using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HitZoneChecker : MonoBehaviour
{
    public static HitZoneChecker Instance;

    public RectTransform hitZoneRect;

    [Header("판정 범위 (픽셀)")]
    public float perfectRange = 30f;
    public float goodRange = 80f;

    [Header("히트존 브래킷 이미지 강조")]
    public Image bracketImage;
    public Color normalColor = new Color(1f, 1f, 1f, 0.5f);
    public Color activeColor = new Color(1f, 0.85f, 0f, 1f);

    [Header("오븐 이미지 전환")]
    public Image ovenImage;
    public Sprite ovenSprite1;
    public Sprite ovenSprite2;

    [Header("효과음")]
    public AudioClip hitSound;
    public AudioClip missSound;

    private AudioSource _audio;
    private ButtonNote _noteInZone = null;

    void Awake()
    {
        Instance = this;
        _audio = GetComponent<AudioSource>();
        if (bracketImage != null)
            bracketImage.color = normalColor;
        if (hitZoneRect == null)
            hitZoneRect = GetComponent<RectTransform>();
    }

    void Update()
    {
        CheckNoteInZone();

        if (Keyboard.current.enterKey.wasPressedThisFrame)
            Judge();
    }

    void CheckNoteInZone()
    {
        ButtonNote[] notes = FindObjectsByType<ButtonNote>(FindObjectsSortMode.None);
        ButtonNote closest = null;
        float closestDist = float.MaxValue;

        float hitZoneHalf = 110.889f;
        float buttonHalf = 63.363f;

        foreach (ButtonNote note in notes)
        {
            if (note.judged) continue;

            RectTransform noteRect = note.GetComponent<RectTransform>();
            float dist = Mathf.Abs(
                noteRect.anchoredPosition.x - hitZoneRect.anchoredPosition.x
            );

            if (dist < hitZoneHalf + buttonHalf)
            {
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = note;
                }
            }
        }

        if (closest != null && _noteInZone == null)
        {
            _noteInZone = closest;
            if (bracketImage != null)
                bracketImage.color = activeColor;
        }
        else if (closest == null && _noteInZone != null)
        {
            if (!_noteInZone.judged)
                RegisterMiss(_noteInZone);
            _noteInZone = null;
            if (bracketImage != null)
                bracketImage.color = normalColor;
        }
    }

    void Judge()
    {
        if (_noteInZone == null || _noteInZone.judged) return;

        RectTransform noteRect = _noteInZone.GetComponent<RectTransform>();
        float dist = Mathf.Abs(
            noteRect.anchoredPosition.x - hitZoneRect.anchoredPosition.x
        );

        if (dist <= perfectRange)
        {
            GameDirector.Instance.AddScore(100, "PERFECT!");
            PlaySound(hitSound);
            SetOven2();
        }
        else if (dist <= goodRange)
        {
            GameDirector.Instance.AddScore(50, "GOOD");
            PlaySound(hitSound);
            SetOven2();
        }
        else
        {
            RegisterMiss(_noteInZone);
            return;
        }

        _noteInZone.DestroyNote();
        _noteInZone = null;
        if (bracketImage != null)
            bracketImage.color = normalColor;
    }

    void SetOven2()
    {
        if (ovenImage == null || ovenSprite2 == null) return;
        ovenImage.sprite = ovenSprite2;
    }

    public void RegisterMiss(ButtonNote note)
    {
        if (note.judged) return;
        note.judged = true;
        GameDirector.Instance?.AddScore(-10, "MISS");
        PlaySound(missSound);
        Destroy(note.gameObject);
        if (_noteInZone == note)
        {
            _noteInZone = null;
            if (bracketImage != null)
                bracketImage.color = normalColor;
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null) _audio.PlayOneShot(clip);
    }
}