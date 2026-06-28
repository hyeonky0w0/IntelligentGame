using UnityEngine;
public class IntroPanel : MonoBehaviour
{
    [SerializeField] private GameObject introPanel;
    private MouseLook mouseLook;

    void Start()
    {
        mouseLook = FindObjectOfType<MouseLook>();

        if (PlayerPrefs.GetInt("FromMiniGame", 0) == 1)
        {
            PlayerPrefs.DeleteKey("FromMiniGame");
            introPanel.SetActive(false);
            Time.timeScale = 1f;
            if (mouseLook != null) mouseLook.LockCursor();
            return;
        }

        if (mouseLook != null)
            mouseLook.FreeCursor();
        introPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    void Update()
    {
        if (introPanel.activeSelf && Input.GetMouseButtonDown(0))
        {
            ClosePanel();
        }
    }

    void ClosePanel()
    {
        PlayerPrefs.SetInt("HasSeenIntro", 1);
        PlayerPrefs.Save();
        introPanel.SetActive(false);
        Time.timeScale = 1f;
        if (mouseLook != null)
            mouseLook.LockCursor();
    }
}