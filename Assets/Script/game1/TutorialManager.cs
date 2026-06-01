using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [Header("튜토리얼 패널")]
    public GameObject tutorialPanel;

    [Header("표시 시간 (초) — JijangStatue에서 설정한 값이 자동 적용됨")]
    public float defaultDuration = 3f;

    void Start()
    {
        if (tutorialPanel == null) return;

        float duration = PlayerPrefs.GetFloat("TutorialDuration", defaultDuration);
        PlayerPrefs.DeleteKey("TutorialDuration");

        StartCoroutine(ShowTutorial(duration));
    }

    IEnumerator ShowTutorial(float duration)
    {
        tutorialPanel.SetActive(true);

        // OrbSpawner는 이미 비활성 상태이므로 그냥 대기
        yield return new WaitForSeconds(duration);

        tutorialPanel.SetActive(false);

        // 튜토리얼 끝난 후 OrbSpawner 활성화 → OnEnable() + WaveLoop() 시작
        OrbSpawner spawner = FindObjectOfType<OrbSpawner>(true); // 비활성 오브젝트도 탐색
        if (spawner != null) spawner.gameObject.SetActive(true);
    }
}