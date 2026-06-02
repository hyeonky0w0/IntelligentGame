using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [Header("다시 도전하기 버튼")]
    public Button retryButton;

    void Start()
    {
        if (retryButton == null)
            retryButton = FindObjectOfType<Button>();

        if (retryButton != null)
            retryButton.onClick.AddListener(OnRetryClicked);
        else
            Debug.LogWarning("[GameOverUI] Retry Button을 찾을 수 없습니다. Inspector에서 연결해주세요.");
    }

    public void OnRetryClicked()
    {
        if (LifeManager.Instance != null)
            LifeManager.Instance.ResetLives();

        SceneManager.LoadScene(SceneNames.MiniGame);
    }
}