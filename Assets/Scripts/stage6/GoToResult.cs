using UnityEngine;

// Stage6 씬의 빈 오브젝트에 부착
public class GotoResult : MonoBehaviour
{
    void Start()
    {
        // BGMManager에게 BGM이 끝나면 Result 씬으로 전환하도록 지시
        if (BGMManager.Instance != null)
        {
            BGMManager.Instance.EnableResultTransitionOnEnd();
        }
    }
}