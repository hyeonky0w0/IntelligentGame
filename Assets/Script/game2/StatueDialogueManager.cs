using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StatueDialogueManager : MonoBehaviour
{
    [Header("두 반가사유상 오브젝트")]
    public GameObject statue1;
    public GameObject statue2;

    [Header("플레이어")]
    public GameObject player;

    [Header("상호작용 설정")]
    [Tooltip("반가사유상과 이 거리 안에 있어야 대화 시작")]
    public float interactRange = 5f;

    [Header("대화 UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI continueHintText;

    [Header("성공 패널")]
    public GameObject successPanel;
    public TextMeshProUGUI successText;

    [Header("씬 이름")]
    public string miniGameSceneName = "MiniGame2Scene";
    public string resultSceneName = "EndingScene";   // ← 엔딩 씬 이름

    // ────────────────────────────────────────
    [Header("셔터 설정 (성공 시 올라감)")]
    public List<GameObject> shutterDoors = new List<GameObject>();
    public float shutterRiseHeight = 10f;
    public float shutterRiseDuration = 3f;

    [Header("페이드 설정")]
    public Image fadeImage;        // 전체화면 검정 Image (alpha 0으로 시작)
    public float fadeDuration = 1.5f;
    // ────────────────────────────────────────

    [System.Serializable]
    public class DialogueLine
    {
        public string speakerName;
        [TextArea(2, 5)]
        public string line;
    }

    [Header("대화 내용 (인스펙터에서 수정 가능)")]
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();

    private const string KEY_PLAYER_X = "PlayerReturnX";
    private const string KEY_PLAYER_Y = "PlayerReturnY";
    private const string KEY_PLAYER_Z = "PlayerReturnZ";
    private const string KEY_MINIGAME = "MiniGame2Result";

    private int currentLineIndex = 0;
    private bool dialogueActive = false;
    private bool typingDone = false;
    private Coroutine typeCoroutine;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        dialoguePanel.SetActive(false);
        if (continueHintText != null)
            continueHintText.gameObject.SetActive(false);

        // 페이드 이미지 투명 초기화
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }

        CheckStatueCollider(statue1, "Statue1");
        CheckStatueCollider(statue2, "Statue2");

        int result = PlayerPrefs.GetInt(KEY_MINIGAME, -1);
        if (result != -1)
        {
            RestorePlayerPosition();
            PlayerPrefs.DeleteKey(KEY_MINIGAME);
            if (result == 1)
                StartCoroutine(SuccessSequence());
        }

        if (dialogueLines.Count == 0)
        {
            dialogueLines.Add(new DialogueLine { speakerName = "금동 반가사유상 1", line = "…오랜 침묵이 깨지는구나." });
            dialogueLines.Add(new DialogueLine { speakerName = "금동 반가사유상 2", line = "이 밤, 이곳까지 발을 들인 자가 있을 줄이야." });
            dialogueLines.Add(new DialogueLine { speakerName = "금동 반가사유상 1", line = "나가고 싶은가.\n그렇다면… 우리를 통과해야 한다." });
            dialogueLines.Add(new DialogueLine { speakerName = "금동 반가사유상 2", line = "번뇌를 피하고 깨달음을 모아라.\n그러지 못하면, 영원히 이곳에 남게 될 것이다." });
            dialogueLines.Add(new DialogueLine { speakerName = "금동 반가사유상 1", line = "…시작하라." });
        }
    }

    // ── 성공 시퀀스: 셔터 올리기 → 성공 패널 → 페이드 아웃 → ResultScene ──
    IEnumerator SuccessSequence()
    {
        // 1) 셔터 동시에 올리기
        foreach (GameObject door in shutterDoors)
        {
            if (door != null)
            {
                Rigidbody rb = door.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = true;
                StartCoroutine(RiseSingleShutter(door));
            }
        }

        // 2) 성공 패널 표시
        if (successPanel != null)
        {
            yield return new WaitForSeconds(0.5f);
            successPanel.SetActive(true);
            if (successText != null)
                successText.text = "사유의 문이 열렸다.\n나아가라.";
            yield return new WaitForSeconds(3.5f);
            successPanel.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }

        // 3) 페이드 아웃 → ResultScene 전환
        yield return StartCoroutine(FadeAndLoad());
    }

    IEnumerator FadeAndLoad()
    {
        // 페이드 이미지가 없으면 바로 씬 전환
        if (fadeImage == null)
        {
            SceneManager.LoadScene(resultSceneName);
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            Color c = fadeImage.color;
            c.a = Mathf.Clamp01(elapsed / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        SceneManager.LoadScene(resultSceneName);
    }

    IEnumerator RiseSingleShutter(GameObject door)
    {
        Vector3 startPos = door.transform.position;
        Vector3 endPos = startPos + Vector3.up * shutterRiseHeight;
        float elapsed = 0f;

        while (elapsed < shutterRiseDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / shutterRiseDuration);
            door.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
        door.transform.position = endPos;
    }

    // ── 이하 기존 코드 그대로 ──────────────────────────────────────────────

    void Update()
    {
        if (dialogueActive)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!typingDone)
                {
                    if (typeCoroutine != null) StopCoroutine(typeCoroutine);
                    dialogueText.text = dialogueLines[currentLineIndex].line;
                    typingDone = true;
                    ShowContinueHint(true);
                }
                else
                {
                    AdvanceLine();
                }
            }
            return;
        }

        if (!Input.GetMouseButtonDown(0)) return;

        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (!Physics.Raycast(ray, out RaycastHit hit, interactRange)) return;

        Debug.Log($"[Dialogue] 정중앙 Raycast 히트: {hit.collider.gameObject.name}");

        if (IsStatue(hit.collider.gameObject))
        {
            Debug.Log("[Dialogue] ✅ 반가사유상 감지 → 대화 시작");
            StartDialogue();
        }
    }

    bool IsStatue(GameObject obj)
    {
        if (statue1 != null && (obj == statue1 || obj.transform.IsChildOf(statue1.transform))) return true;
        if (statue2 != null && (obj == statue2 || obj.transform.IsChildOf(statue2.transform))) return true;
        return false;
    }

    void CheckStatueCollider(GameObject statue, string label)
    {
        if (statue == null) { Debug.LogError($"[Dialogue] ❌ {label} 슬롯 비어있음"); return; }
        Collider[] cols = statue.GetComponentsInChildren<Collider>();
        if (cols.Length == 0)
            Debug.LogError($"[Dialogue] ❌ {label}({statue.name}) Collider 없음 → Box Collider 추가하세요");
        else
            Debug.Log($"[Dialogue] ✅ {label}({statue.name}) Collider {cols.Length}개 확인");
    }

    void StartDialogue()
    {
        dialogueActive = true;
        currentLineIndex = 0;
        dialoguePanel.SetActive(true);
        ShowContinueHint(false);
        ShowLine(0);
    }

    void ShowLine(int index)
    {
        speakerNameText.text = dialogueLines[index].speakerName;
        dialogueText.text = "";
        typingDone = false;
        ShowContinueHint(false);
        if (typeCoroutine != null) StopCoroutine(typeCoroutine);
        typeCoroutine = StartCoroutine(TypeLine(dialogueLines[index].line));
    }

    IEnumerator TypeLine(string line)
    {
        float speed = currentLineIndex >= 3 ? 0.06f : 0.04f;
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(speed);
        }
        typeCoroutine = null;
        typingDone = true;
        ShowContinueHint(true);
    }

    void AdvanceLine()
    {
        currentLineIndex++;
        if (currentLineIndex < dialogueLines.Count)
            ShowLine(currentLineIndex);
        else
            EndDialogue();
    }

    void ShowContinueHint(bool show)
    {
        if (continueHintText != null)
            continueHintText.gameObject.SetActive(show);
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        dialogueActive = false;
        ShowContinueHint(false);
        SavePlayerPosition();
        StartCoroutine(LoadMiniGameScene());
    }

    void SavePlayerPosition()
    {
        if (player == null) return;
        Vector3 pos = player.transform.position;
        PlayerPrefs.SetFloat(KEY_PLAYER_X, pos.x);
        PlayerPrefs.SetFloat(KEY_PLAYER_Y, pos.y);
        PlayerPrefs.SetFloat(KEY_PLAYER_Z, pos.z);
        PlayerPrefs.Save();
    }

    void RestorePlayerPosition()
    {
        if (player == null || !PlayerPrefs.HasKey(KEY_PLAYER_X)) return;
        player.transform.position = new Vector3(
            PlayerPrefs.GetFloat(KEY_PLAYER_X),
            PlayerPrefs.GetFloat(KEY_PLAYER_Y),
            PlayerPrefs.GetFloat(KEY_PLAYER_Z)
        );
        PlayerPrefs.DeleteKey(KEY_PLAYER_X);
        PlayerPrefs.DeleteKey(KEY_PLAYER_Y);
        PlayerPrefs.DeleteKey(KEY_PLAYER_Z);
    }

    IEnumerator LoadMiniGameScene()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(miniGameSceneName);
    }

    void OnDrawGizmosSelected()
    {
        if (statue1 != null)
        {
            Gizmos.color = new Color(1f, 0.8f, 0.2f, 0.3f);
            Gizmos.DrawWireSphere(statue1.transform.position, interactRange);
        }
        if (statue2 != null)
        {
            Gizmos.color = new Color(1f, 0.8f, 0.2f, 0.3f);
            Gizmos.DrawWireSphere(statue2.transform.position, interactRange);
        }
    }
}