using UnityEngine;
using UnityEngine.SceneManagement;

public class GotoStage5 : MonoBehaviour
{
    [Header("Stage 종료 시간 (BGM 기준 초)")]
    public float stage1EndTime = 87f; 

    [Header("전환할 씬 이름")]
    public string nextSceneName = "Stage5";

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