using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// GameOverScene의 빈 GameObject에 부착하세요.
/// 
/// [세팅 방법]
/// 1. GameOverScene Hierarchy에 빈 GameObject 생성 → GameOverUI.cs 부착
/// 2. Inspector에서 Retry Button에 "다시 도전하기" 버튼 드래그 연결
///    (또는 씬에 버튼이 하나뿐이면 자동으로 찾아줌)
/// 3. 버튼 OnClick()에 이 컴포넌트의 OnRetryClicked() 연결해도 됨
/// </summary>
public class GameOverUI : MonoBehaviour
{
    [Header("다시 도전하기 버튼")]
    public Button retryButton;

    void Start()
    {
        // Inspector에서 연결 안 했을 경우 씬에서 자동 탐색
        if (retryButton == null)
            retryButton = FindObjectOfType<Button>();

        if (retryButton != null)
            retryButton.onClick.AddListener(OnRetryClicked);
        else
            Debug.LogWarning("[GameOverUI] Retry Button을 찾을 수 없습니다. Inspector에서 연결해주세요.");
    }

    /// <summary>
    /// 다시 도전하기 버튼 클릭 시 호출.
    /// LifeManager 목숨 초기화 후 MiniGameScene으로 이동.
    /// </summary>
    public void OnRetryClicked()
    {
        // LifeManager가 DontDestroyOnLoad로 살아있으면 목숨 초기화
        if (LifeManager.Instance != null)
            LifeManager.Instance.ResetLives();

        SceneManager.LoadScene(SceneNames.MiniGame);
    }
}