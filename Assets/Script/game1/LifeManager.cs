using UnityEngine;
using UnityEngine.SceneManagement;

public class LifeManager : MonoBehaviour
{
    public static LifeManager Instance { get; private set; }

    [Header("목숨 설정")]
    public int maxLives = 3;

    int _currentLives;
    public int CurrentLives => _currentLives;

    void Awake()
    {
        Instance = this;
        _currentLives = maxLives;
    }

    public static LifeManager GetOrCreate()
    {
        if (Instance != null) return Instance;
        return new GameObject("LifeManager").AddComponent<LifeManager>();
    }

    public void LoseLife()
    {
        _currentLives--;
        Debug.Log($"[LifeManager] 남은 목숨: {_currentLives}");
        MiniGameUIManager.Instance?.UpdateHearts(_currentLives);

        if (_currentLives <= 0)
        {
            _currentLives = 0;
            MiniGameUIManager.Instance?.ShowResult(false);
            FindObjectOfType<OrbSpawner>()?.StopGame();
        }
    }



    public void TriggerSuccess()
    {
        PlayerPrefs.SetInt("MinigameSuccess", 1);
        PlayerPrefs.SetInt("FromMiniGame", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneNames.BuddhistHall);
    }

    public void ResetLives()
    {
        _currentLives = maxLives;
    }
}