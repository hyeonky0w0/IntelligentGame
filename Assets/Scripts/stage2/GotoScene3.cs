using UnityEngine;
using UnityEngine.SceneManagement;

public class GotoScene3 : MonoBehaviour
{
    [Header("Stage 종료 시간 (BGM 기준 초)")]
    public float stage1EndTime = 52.7f; 

    [Header("전환할 씬 이름")]
    public string nextSceneName = "Stage3";

    private bool _transitioned = false;
    private BGMManager _bgm;

    void Start()
    {
        _bgm = BGMManager.Instance;
    }

    void Update()
    {
        if (_transitioned || _bgm == null) return;

        if (_bgm.GetCurrentTime() >= stage1EndTime)
        {
            _transitioned = true;
            SceneManager.LoadScene(nextSceneName);
        }
    }
}