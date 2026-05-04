using UnityEngine;

public class GotoResult : MonoBehaviour
{
    void Start()
    {
        if (BGMManager.Instance != null)
        {
            BGMManager.Instance.EnableResultTransitionOnEnd();
        }
    }
}