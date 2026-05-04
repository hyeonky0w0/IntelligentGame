using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private static ScoreManager instance;
    private int _score = 0;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static ScoreManager Instance => instance;

    public int Score => _score;

    public void AddScore(int amount)
    {
        _score += amount;
        if (_score < 0) _score = 0;
    }

    public void ResetScore() => _score = 0;
}