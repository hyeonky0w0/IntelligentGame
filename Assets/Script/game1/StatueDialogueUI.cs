using UnityEngine;
using TMPro;

public class StatueDialogueUI : MonoBehaviour
{
    [Header("UI 요소")]
    public TextMeshProUGUI nameLabel;
    public TextMeshProUGUI lineText;
    public TextMeshProUGUI continueHint;

    [Header("화자 이름")]
    public string speakerName = "석조 지장보살";

    GameObject _panel;

    void Awake()
    {
        _panel = gameObject;
        _panel.SetActive(false);
    }

    public void Show()
    {
        _panel.SetActive(true);
        if (nameLabel != null) nameLabel.text = speakerName;
        if (lineText != null) lineText.text = "";
        if (continueHint != null) continueHint.gameObject.SetActive(false);
    }

    public void Hide()
    {
        _panel.SetActive(false);
    }

    public void SetText(string text)
    {
        if (lineText != null) lineText.text = text;
    }

    public void AppendChar(char c)
    {
        if (lineText != null) lineText.text += c;
    }

    public void ShowContinueHint(bool show)
    {
        if (continueHint != null)
            continueHint.gameObject.SetActive(show);
    }
}