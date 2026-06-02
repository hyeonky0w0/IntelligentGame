using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JijangStatue : MonoBehaviour
{
    [Header("상호작용 설정")]
    public float interactRange = 5f;
    public Transform playerTransform;

    [Header("대화 내용")]
    [TextArea(2, 4)]
    public List<string> dialogueLines = new List<string>
    {
        "……경비원이여.",
        "이 어둠 속에서 나를 찾아왔구나.",
        "정덕 십년, 나는 이 자리를 지켜왔다.",
        "네가 이 관을 지나려거든…",
        "먼저 업화의 시련을 견뎌내어라.",
        "세 번의 심등이 꺼지면, 너는 이 자리에 남는다."
    };

    [Header("연출")]
    public float typewriterSpeed = 0.05f;
    public AudioClip statueVoice;
    public ParticleSystem glowEffect;
    public float sceneLoadDelay = 1.2f;

    [Header("미니게임 설명 패널 (MiniGameScene에 있음)")]
    public float tutorialDuration = 3f;

    [Header("UI 참조")]
    public StatueDialogueUI dialogueUI;

    bool _dialogueActive = false;
    bool _alreadyTriggered = false;
    int _lineIndex = 0;
    Coroutine _typeCoroutine;

    void Start()
    {
        if (tutorialDuration > 0)
            PlayerPrefs.DeleteKey("TutorialDuration");
    }

    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        if (_dialogueActive)
        {
            AdvanceDialogue();
            return;
        }

        TryInteract();
    }

    void TryInteract()
    {
        if (_alreadyTriggered) return;

        if (Camera.main == null)
        {
            return;
        }

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (!Physics.Raycast(ray, out RaycastHit hit, 100f)) return;
        if (hit.transform != this.transform && !hit.transform.IsChildOf(this.transform)) return;

        if (playerTransform != null)
        {
            float dist = Vector3.Distance(playerTransform.position, transform.position);
            if (dist > interactRange)
            {
                return;
            }
        }

        StartDialogue();
    }

    void StartDialogue()
    {
        _alreadyTriggered = true;
        _dialogueActive = true;
        _lineIndex = 0;

        MouseLook mouseLook = Camera.main?.GetComponent<MouseLook>();
        if (mouseLook != null) mouseLook.FreeCursor();

        if (glowEffect != null) glowEffect.Play();
        if (statueVoice != null) AudioSource.PlayClipAtPoint(statueVoice, transform.position);
        if (dialogueUI != null) dialogueUI.Show();

        ShowCurrentLine();
    }

    void ShowCurrentLine()
    {
        if (_typeCoroutine != null) StopCoroutine(_typeCoroutine);
        _typeCoroutine = StartCoroutine(TypeLine(dialogueLines[_lineIndex]));
    }

    IEnumerator TypeLine(string line)
    {
        if (dialogueUI != null)
        {
            dialogueUI.SetText("");
            dialogueUI.ShowContinueHint(false);
        }
        foreach (char c in line)
        {
            if (dialogueUI != null) dialogueUI.AppendChar(c);
            yield return new WaitForSeconds(typewriterSpeed);
        }
        if (dialogueUI != null) dialogueUI.ShowContinueHint(true);
    }

    void AdvanceDialogue()
    {
        if (_typeCoroutine != null)
        {
            StopCoroutine(_typeCoroutine);
            _typeCoroutine = null;
            if (dialogueUI != null)
            {
                dialogueUI.SetText(dialogueLines[_lineIndex]);
                dialogueUI.ShowContinueHint(true);
            }
            return;
        }

        if (dialogueUI != null) dialogueUI.ShowContinueHint(false);
        _lineIndex++;

        if (_lineIndex < dialogueLines.Count)
            ShowCurrentLine();
        else
            EndDialogue();
    }

    void EndDialogue()
    {
        _dialogueActive = false;
        if (dialogueUI != null) dialogueUI.Hide();
        if (glowEffect != null) glowEffect.Stop();
        StartCoroutine(TutorialThenLoad());
    }

    IEnumerator TutorialThenLoad()
    {
        yield return new WaitForSeconds(sceneLoadDelay);

        if (playerTransform != null)
        {
            PlayerPrefs.SetFloat("PlayerX", playerTransform.position.x);
            PlayerPrefs.SetFloat("PlayerY", playerTransform.position.y);
            PlayerPrefs.SetFloat("PlayerZ", playerTransform.position.z);
        }

        PlayerPrefs.SetFloat("TutorialDuration", tutorialDuration);
        PlayerPrefs.Save();

        SceneManager.LoadScene(SceneNames.MiniGame);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.8f, 0.5f, 1f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}