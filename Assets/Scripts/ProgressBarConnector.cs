using UnityEngine;

public class ProgressBarConnector : MonoBehaviour
{
    void Start()
    {
        // 자기 자신을 StageProgressUI에 직접 등록
        if (StageProgressUI.Instance != null)
        {
            StageProgressUI.Instance.SetProgressBar(GetComponent<RectTransform>());
            Debug.Log("ProgressBar 연결 완료!");
        }
        else
        {
            Debug.LogError("StageProgressUI.Instance가 없습니다!");
        }
    }
}