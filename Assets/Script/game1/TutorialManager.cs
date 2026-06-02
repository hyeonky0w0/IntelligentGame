using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [Header("튜토리얼 패널")]
    public GameObject tutorialPanel;

    [Header("표시 시간 (초) — JijangStatue에서 설정한 값이 자동 적용")]
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

        yield return new WaitForSeconds(duration);

        tutorialPanel.SetActive(false);
        OrbSpawner spawner = FindObjectOfType<OrbSpawner>(true); 
        if (spawner != null) spawner.gameObject.SetActive(true);
    }
}