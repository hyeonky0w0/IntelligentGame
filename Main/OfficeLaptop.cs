using UnityEngine;
using UnityEngine.SceneManagement; // 🚨 씬 전환을 위해 라이브러리를 추가했습니다!

public class OfficeLaptop : MonoBehaviour
{
    [Header("안내선 연출")]
    public float interactDistance = 3f;
    private Transform playerTransform;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    /// <summary>
    /// 플레이어가 노트북 앞에서 상호작용 버튼을 누르면 작동하는 상호작용 함수
    /// </summary>
    public void Interact()
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        if (distance > interactDistance)
        {
            Debug.Log("노트북이 너무 멩니다.");
            return;
        }

        if (GameManager.Instance != null)
        {
            // 🚨 3, 6, 9회 완료 및 중복 상호작용 방어 가드
            if (GameManager.Instance.totalVisitCount > 0 &&
                GameManager.Instance.totalVisitCount % 3 == 0 &&
                !GameManager.Instance.isOfficeLaptopInteracted)
            {
                // ====================================================================
                // 🚨 [9회차 마감: 다이렉트 EndingScene 이동 프로토콜]
                // ====================================================================
                if (GameManager.Instance.totalVisitCount == 9)
                {
                    Debug.LogWarning("<color=gold><b>[최종 순찰 마감 완수]</b></color> 9회 순찰 완료 포착! 즉시 EndingScene으로 화면을 전환합니다.");

                    // 💡 마우스 커서가 갇혀있다면 엔딩 UI 조작을 위해 풀어줍니다.
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;

                    // 🎯 [핵심] 여기서 씬 매니저를 통해 다이렉트로 엔딩 씬을 불러옵니다!
                    SceneManager.LoadScene("EndingScene");
                }
                // ====================================================================
                // 🚨 [1, 2회차 마감: 일반 라운드 리셋 프로토콜]
                // ====================================================================
                else
                {
                    Debug.Log("<color=green>노트북 보안 시스템 승인 완료. 다음 순찰 회차 문들이 리셋되었습니다.</color>");
                    GameManager.Instance.InteractWithOfficeLaptop();
                }
            }
            else
            {
                Debug.Log("지금은 노트북을 조작할 필요가 없습니다. (순찰 코스를 먼저 완료하십시오.)");
            }
        }
    }
}