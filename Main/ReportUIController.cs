using UnityEngine;

public class ReportUIController : MonoBehaviour
{
    [Header("일지 UI 판넬")]
    [Tooltip("화면에 크게 뜨는 일지 UI 판넬 오브젝트를 넣어주세요.")]
    public GameObject reportUIPanel;

    private void Start()
    {
        // 🚨 [핵심 해결 포인트]
        // 인스펙터 창에서 일지 UI를 켜두었더라도, 게임이 시작되는 순간 자동으로 숨깁니다.
        if (reportUIPanel != null)
        {
            reportUIPanel.SetActive(false);
            Debug.Log("<color=#00FF00><b>[일지 시스템]</b></color> 시작 시 일지 UI 판넬을 성공적으로 숨겼습니다.");
        }
        else
        {
            Debug.LogError("<color=red><b>[일지 시스템 오류]</b></color> Report UIPanel이 인스펙터 창에 연결되지 않았습니다!");
        }
    }

    /// <summary>
    /// 출구 문 앞의 일지를 클릭(상호작용)했을 때 호출할 함수
    /// </summary>
    public void OpenReportUI()
    {
        if (reportUIPanel != null)
        {
            reportUIPanel.SetActive(true);

            // 💡 마우스 커서가 화면 중앙에 가려져 있다면 커서를 보이게 해줍니다.
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // UI에서 [ O (이상 있음) ] 버튼을 눌렀을 때
    public void Click_Anomaly_YES()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SubmitReport(true);
        }
        CloseUI();
    }

    // UI에서 [ X (이상 없음) ] 버튼을 눌렀을 때
    public void Click_Anomaly_NO()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SubmitReport(false);
        }
        CloseUI();
    }

    private void CloseUI()
    {
        if (reportUIPanel != null)
        {
            reportUIPanel.SetActive(false);
        }

        // 💡 일지를 닫았으므로 마우스 커서를 다시 게임 화면 속에 가둡니다.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}