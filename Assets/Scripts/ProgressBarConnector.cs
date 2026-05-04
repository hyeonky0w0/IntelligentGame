using UnityEngine;

public class ProgressBarConnector : MonoBehaviour
{
    void Start()
    {
        if (StageProgressUI.Instance != null)
        {
            StageProgressUI.Instance.SetProgressBar(GetComponent<RectTransform>());
        }
    }
}