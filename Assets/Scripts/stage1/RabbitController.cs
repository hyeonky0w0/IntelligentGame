using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class RabbitController : MonoBehaviour
{
    [Header("Rabbit Sprites")]
    public Sprite idleSprite;
    public Sprite watchSprite;
    public Sprite[] runInSprites;
    public Sprite[] runOutSprites;

    [Header("Size Settings")]
    public Vector2 normalSize = new Vector2(483f, 740f);
    public Vector2 idleSize = new Vector2(483f, 740f);
    public Vector2 watchSize = new Vector2(483f, 740f);

    [Header("Movement Settings")]
    public float moveSpeed = 1000f;      // 이동 속도 증가
    public float startX = 600f;
    public float centerX = 0f;
    public float idleTime = 1f;         // 서있는 시간 줄임
    public float watchTime = 0.1f;        // 시계보는 시간 줄임

    [Header("Speech Bubble")]
    public GameObject speechBubble;
    public TMP_Text dialogueText;
    public AudioClip talkSound;

    [Header("Dialogue")]
    [TextArea] public string dialogue1;
    [TextArea] public string dialogue2;
    public float typingSpeed = 0.01f;   // 타이핑 속도
    public float dialogueWait = 0.2f;   // 대사 사이 대기 시간

    Image rabbitImage;
    AudioSource audioSource;
    RectTransform rectTransform;
    float time = 0;
    int idx = 0;

    void Start()
    {
        rabbitImage = GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();
        rectTransform = GetComponent<RectTransform>();

        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = new Vector2(startX, rectTransform.anchoredPosition.y);

        if (speechBubble != null)
            speechBubble.SetActive(false);

        StartCoroutine(RabbitSequence());
    }

    void SetSize(Vector2 size)
    {
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = size;
    }

    // 태그 안 깨지는 타이핑 효과
    IEnumerator TypeText(string text)
    {
        dialogueText.text = text;
        dialogueText.maxVisibleCharacters = 0;

        // TMP가 텍스트 파싱할 시간 한 프레임 대기
        yield return null;

        int totalChars = dialogueText.textInfo.characterCount;

        for (int i = 0; i <= totalChars; i++)
        {
            dialogueText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    IEnumerator PlayDialogues()
    {
        string[] dialogues = { dialogue1, dialogue2 };

        foreach (string line in dialogues)
        {
            if (string.IsNullOrEmpty(line)) continue;

            yield return StartCoroutine(TypeText(line));
            yield return new WaitForSeconds(dialogueWait);
        }
    }

    IEnumerator RabbitSequence()
    {
        float exitX = startX;

        yield return StartCoroutine(RunTo(centerX, runInSprites));

        rabbitImage.sprite = idleSprite;
        SetSize(idleSize);

        if (speechBubble != null)
            speechBubble.SetActive(true);

        if (audioSource != null && talkSound != null)
            audioSource.PlayOneShot(talkSound);

        yield return StartCoroutine(PlayDialogues());

        rabbitImage.sprite = watchSprite;
        SetSize(watchSize);
        yield return new WaitForSeconds(watchTime);

        if (speechBubble != null)
            speechBubble.SetActive(false);

        yield return StartCoroutine(RunTo(exitX, runOutSprites));
        
        SceneManager.LoadScene("Stage1");
    }

    IEnumerator RunTo(float targetX, Sprite[] sprites)
    {
        float direction = targetX > rectTransform.anchoredPosition.x ? 1f : -1f;

        while (true)
        {
            float newX = rectTransform.anchoredPosition.x + direction * moveSpeed * Time.deltaTime;

            bool arrived = direction > 0 ? newX >= targetX : newX <= targetX;
            if (arrived)
            {
                rectTransform.anchoredPosition = new Vector2(targetX, rectTransform.anchoredPosition.y);
                break;
            }

            rectTransform.anchoredPosition = new Vector2(newX, rectTransform.anchoredPosition.y);

            time += Time.deltaTime;
            if (time > 0.1f)
            {
                time = 0;
                rabbitImage.sprite = sprites[idx];
                SetSize(normalSize);
                idx = (idx + 1) % sprites.Length;
            }

            yield return null;
        }

        idx = 0;
        time = 0;
    }
}