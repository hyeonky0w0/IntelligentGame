using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;          // UI Image 제어용
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("순찰 및 흐름 상태")]
    public int currentRound = 1;     // 총 3회 순찰 (1~3)
    public int currentFloor = 1;     // 현재 순찰 중인 층 (1~3층)
    public int totalVisitCount = 0;  // 총 전시관 방문 횟수 (최대 9회)

    [Header("게임 규칙 설정")]
    public int maxStrikes = 3;
    private int currentStrikes = 0;

    [Header("정신력 UI 설정 (3칸 개별 제어)")]
    [Tooltip("Hierarchy 창의 Sanity_1, 2, 3 이미지 3개를 차례대로 넣어주세요 (원소 3개)")]
    public List<Image> sanityImages = new List<Image>();
    [Tooltip("정신력이 채워져 있을 때의 이미지 (UI-1)")]
    public Sprite fullSanitySprite;
    [Tooltip("정신력이 깎여서 꺼졌을 때의 이미지 (UI-2)")]
    public Sprite damagedSanitySprite;

    [Header("이상현상 관리")]
    [Range(0f, 1f)] public float anomalyChance = 1f; // 이상현상 발생 빈도 확률
    public int currentAnomalyID = -1;                 // 현재 활성화된 이상현상 ID (-1은 정상 상태)
    public bool isKillerEvent = false;                // 현재 킬러 이벤트 진행 여부

    [Header("이상현상 오브젝트 리스트 (인스펙터 할당)")]
    public List<GameObject> floor1Anomalies;
    public List<GameObject> floor2Anomalies;
    public List<GameObject> floor3Anomalies;

    [Header("물리적 차단 및 복귀 시스템")]
    public List<GameObject> entranceDoors;
    public List<GameObject> exitDoors;

    [Tooltip("순찰 완료 후 '경비실로 복귀하십시오'를 띄울 UI 오브젝트 (ReturnPanel)")]
    public GameObject returnToOfficeUI;

    [Tooltip("노트북 상호작용 후 암전 연출을 위해 화면을 덮을 캔버스 내부의 Black 이미지 오브젝트")]
    public GameObject blackoutOverlayUI;

    [HideInInspector] public bool isOfficeLaptopInteracted = false;
    [HideInInspector] public bool isSystemResetting = false;
    private bool isReportSubmitting = false; // 일지 중복 연타 클릭 버그 차단용 플래그

    private GameObject activeAnomalyObject = null;
    private Coroutine uiTimerCoroutine = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (returnToOfficeUI != null) returnToOfficeUI.SetActive(false);
        if (blackoutOverlayUI != null) blackoutOverlayUI.SetActive(false);

        // 최초 구동 시 정신력 UI 3칸 모두 UI-1(풀피) 상태로 갱신
        UpdateSanityUI();

        // 최초 구동 시 문 상태만 원본 정렬
        InitDoorsForNewRound();

        // 초기 층 상태 설정
        currentFloor = 1;
        totalVisitCount = 0;

        Debug.Log("<color=#FFFF33><b>[시스템 구동]</b></color> 박물관 경비 시스템이 정상 시작되었습니다. 1층 진입을 대기합니다.");
    }

    public void UpdateCurrentFloor(int actualFloor)
    {
        if (isSystemResetting) return;

        if ((totalVisitCount == 3 || totalVisitCount == 6 || totalVisitCount == 9) && !isOfficeLaptopInteracted)
        {
            Debug.LogWarning("<color=red><b>[진입 불가]</b></color> 순찰 회차가 마감되었습니다. 경비실 노트북 상호작용이 선행되어야 합니다.");
            return;
        }

        if (currentFloor == actualFloor && totalVisitCount > 0) return;

        currentFloor = actualFloor;
        Debug.LogWarning($"<color=#00FFFF><b>[층 전환 감지]</b></color> 🏃‍♂️ 플레이어가 <b>{currentFloor}층 전시실</b>에 실제로 진입했습니다!");

        LockEntranceDoor(currentFloor, true);
        LockExitDoor(currentFloor, true);

        StartNextExhibition();
    }

    public void StartNextExhibition()
    {
        if (totalVisitCount >= 9)
        {
            GameClear();
            return;
        }

        totalVisitCount++;
        isKillerEvent = false;
        currentAnomalyID = -1;

        if (activeAnomalyObject != null)
        {
            GameObject targetToDisable = activeAnomalyObject;
            activeAnomalyObject = null;
            targetToDisable.SetActive(false);
        }

        float diceRoll = Random.value;
        Debug.Log($"<color=#33FF33><b>[순찰 정보]</b></color> 현재 상태: <b>{currentRound}회차 순찰 중 [{currentFloor}층]</b> (총 방문: {totalVisitCount}/9)");

        if (diceRoll <= anomalyChance)
        {
            DetermineAndSpawnAnomaly();
        }
        else
        {
            Debug.Log($"<color=#00FFFF><b>[전시관 상태]</b></color> 🟢 <b>{currentFloor}층 정상 상태</b>로 확정되었습니다. (출구 잠금 유지)");
            LockExitDoor(currentFloor, true);
        }
    }

    private void DetermineAndSpawnAnomaly()
    {
        List<GameObject> currentFloorList = GetCurrentFloorList();
        if (currentFloorList == null || currentFloorList.Count == 0) return;

        int randomIndex = Random.Range(0, currentFloorList.Count);
        activeAnomalyObject = currentFloorList[randomIndex];

        currentAnomalyID = AssignDebugID(activeAnomalyObject, randomIndex);

        
        //activeAnomalyObject = currentFloorList.Find(obj => obj != null && obj.GetComponent<KillerAI>() != null);

        activeAnomalyObject.SetActive(true);
        if (activeAnomalyObject.GetComponent<KillerAI>() != null)
        {
            isKillerEvent = true;
            LockExitDoor(2, false);
            Debug.LogWarning($"<color=#FF3333><b>[강제 소환]</b></color> 💀 2층 귀형문 킬러 확정 출현!");
            activeAnomalyObject.GetComponent<KillerAI>().StartChase();
        }
        else
        {
            Debug.LogWarning($"<color=#FF9933><b>[이상현상 기믹 발동]</b></color> 🚨 <b>{currentFloor}층 이상현상 당첨!</b> ([{activeAnomalyObject.name}])");
        }
    }

    public void SubmitReport(bool playerAnswer)
    {
        if (isReportSubmitting) return;
        StartCoroutine(SubmitReportRoutine(playerAnswer));
    }

    private IEnumerator SubmitReportRoutine(bool playerAnswer)
    {
        isReportSubmitting = true;

        if (isKillerEvent)
        {
            Debug.LogWarning("<color=#FF3333><b>[특수 채점 스킵]</b></color> 💀 킬러 탈출 대성공! 무조건 <b>'정답'</b> 처리됩니다.");
            LockExitDoor(currentFloor, false);
            Debug.Log($"<color=#00FF00><b>[경비 일지 정답]</b></color> 🎯 킬러 탈출 성공!");
        }
        else
        {
            Debug.Log($"<color=#FF33FF><b>[경비 일지 제출]</b></color> 플레이어 선택: <b>{(playerAnswer ? "예" : "아니오")}</b>");
            LockExitDoor(currentFloor, false);

            bool isAnomalyPresent = (currentAnomalyID != -1);

            if ((isAnomalyPresent && !playerAnswer) || (!isAnomalyPresent && playerAnswer))
            {
                currentStrikes++;
                Debug.LogError($"<color=red><b>[경비 일지 오답!]</b></color> ❌ 실패! 정신력 감소. 누적: {currentStrikes} / {maxStrikes}");

                // 🔥 오답 발생 시 실시간으로 3칸 UI 상태 업데이트
                UpdateSanityUI();
            }
            else
            {
                Debug.Log($"<color=#00FF00><b>[경비 일지 정답]</b></color> 🎯 올바른 분석입니다.");
            }

            if (currentStrikes >= maxStrikes)
            {
                GameOver("정신력 고갈 및 경고 누적 초과");
                isReportSubmitting = false;
                yield break;
            }
        }

        isKillerEvent = false;
        PrepareNextFloorState();

        yield return new WaitForSeconds(0.2f);
        isReportSubmitting = false;
    }

    private void PrepareNextFloorState()
    {
        LockEntranceDoor(currentFloor, true);

        if (totalVisitCount == 3 || totalVisitCount == 6 || totalVisitCount == 9)
        {
            isOfficeLaptopInteracted = false;

            if (uiTimerCoroutine != null) StopCoroutine(uiTimerCoroutine);
            uiTimerCoroutine = StartCoroutine(ShowReturnPanelTimer(3.5f));
        }
    }

    private IEnumerator ShowReturnPanelTimer(float delayTime)
    {
        if (returnToOfficeUI != null)
        {
            var textComp = returnToOfficeUI.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (textComp != null)
            {
                int completedRound = totalVisitCount / 3;
                textComp.text = $"{completedRound}회차 순찰 완료. 경비실로 복귀하십시오.";
            }

            returnToOfficeUI.SetActive(true);
            yield return new WaitForSeconds(delayTime);
            returnToOfficeUI.SetActive(false);
        }
    }

    public void InteractWithOfficeLaptop()
    {
        if (totalVisitCount % 3 != 0 || isSystemResetting) return;

        if (totalVisitCount == 9)
        {
            StartCoroutine(LaptopEndingSequence());
        }
        else
        {
            StartCoroutine(LaptopBlackoutSequence());
        }
    }

    private IEnumerator LaptopEndingSequence()
    {
        isSystemResetting = true;
        isOfficeLaptopInteracted = true;

        if (blackoutOverlayUI != null) blackoutOverlayUI.SetActive(true);
        yield return new WaitForSeconds(1.5f);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("EndingScene");
    }

    private IEnumerator LaptopBlackoutSequence()
    {
        isSystemResetting = true;
        isOfficeLaptopInteracted = true;

        if (blackoutOverlayUI != null) blackoutOverlayUI.SetActive(true);
        yield return new WaitForSeconds(1.0f);

        currentRound++;

        if (activeAnomalyObject != null)
        {
            GameObject targetToDisable = activeAnomalyObject;
            activeAnomalyObject = null;
            targetToDisable.SetActive(false);
        }
        currentAnomalyID = -1;
        isKillerEvent = false;

        InitDoorsForNewRound();
        currentFloor = 0;

        yield return new WaitForSeconds(0.5f);

        if (returnToOfficeUI != null)
        {
            var textComp = returnToOfficeUI.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (textComp != null)
            {
                textComp.text = $"보안 승인 완료.\n<color=#33FF33>[{currentRound}회차 박물관 야간 순찰]</color>을 시작하십시오.";
            }
            returnToOfficeUI.SetActive(true);
        }

        yield return new WaitForSeconds(0.5f);

        if (blackoutOverlayUI != null) blackoutOverlayUI.SetActive(false);
        isSystemResetting = false;

        yield return new WaitForSeconds(3.5f);
        if (returnToOfficeUI != null) returnToOfficeUI.SetActive(false);
    }

    private void LockEntranceDoor(int floor, bool isLocked)
    {
        int index = floor - 1;
        if (index >= 0 && index < entranceDoors.Count && entranceDoors[index] != null)
        {
            entranceDoors[index].SetActive(isLocked);
        }
    }

    private void LockExitDoor(int floor, bool isLocked)
    {
        int index = floor - 1;
        if (index >= 0 && index < exitDoors.Count && exitDoors[index] != null)
        {
            exitDoors[index].SetActive(isLocked);
        }
    }

    private void InitDoorsForNewRound()
    {
        for (int i = 0; i < entranceDoors.Count; i++)
        {
            if (entranceDoors[i] != null) entranceDoors[i].SetActive(false);
        }
        for (int i = 0; i < exitDoors.Count; i++)
        {
            if (exitDoors[i] != null) exitDoors[i].SetActive(true);
        }
    }

    private List<GameObject> GetCurrentFloorList()
    {
        if (currentFloor == 1) return floor1Anomalies;
        if (currentFloor == 2) return floor2Anomalies;
        if (currentFloor == 3) return floor3Anomalies;
        return null;
    }

    private int AssignDebugID(GameObject obj, int index)
    {
        if (obj.GetComponent<Anomaly_BuddhaScare>() != null) return obj.GetComponent<Anomaly_BuddhaScare>().anomalyID;
        if (obj.GetComponent<HorseRiderClayTest>() != null) return obj.GetComponent<HorseRiderClayTest>().anomalyID;
        if (obj.GetComponent<ExhibitPanel3DTest>() != null) return obj.GetComponent<ExhibitPanel3DTest>().anomalyID;
        if (obj.GetComponent<Anomaly_JewelryAudio>() != null) return obj.GetComponent<Anomaly_JewelryAudio>().anomalyID;
        if (obj.GetComponent<Anomaly_QueenPillow>() != null) return obj.GetComponent<Anomaly_QueenPillow>().anomalyID;
        if (obj.GetComponent<Anomaly_BuddhistPaintingGroup>() != null) return obj.GetComponent<Anomaly_BuddhistPaintingGroup>().anomalyID;
        if (obj.GetComponent<Anomaly_ArahatGroup>() != null) return obj.GetComponent<Anomaly_ArahatGroup>().anomalyID;
        if (obj.GetComponent<WhitePorcelainBlackout>() != null) return obj.GetComponent<WhitePorcelainBlackout>().anomalyID;
        if (obj.GetComponent<CeladonDollMovement>() != null) return obj.GetComponent<CeladonDollMovement>().anomalyID;
        if (obj.GetComponent<CeladonDuckZone>() != null) return obj.GetComponent<CeladonDuckZone>().anomalyID;
        if (obj.GetComponent<CeladonDragonKettle>() != null) return obj.GetComponent<CeladonDragonKettle>().anomalyID;
        return -1;
    }

    /// <summary>
    /// ★ 3개 연속 배치된 하트(정신력) UI 스프라이트를 currentStrikes 값에 맞게 개별 업데이트합니다.
    /// </summary>
    private void UpdateSanityUI()
    {
        // 인스펙터에 할당이 안 되었거나 리스트가 비어있으면 안전하게 리턴
        if (sanityImages == null || sanityImages.Count == 0) return;

        // 리스트 내부를 돌며 인덱스가 현재 데미지(currentStrikes)보다 낮으면 남은 목숨, 높거나 같으면 꺼진 목숨 처리
        for (int i = 0; i < sanityImages.Count; i++)
        {
            if (sanityImages[i] == null) continue;

            // 오른쪽 끝(역순)부터 차례대로 꺼지게 하고 싶다면:
            // 예: 총 3칸일 때 strikes가 1이면 index 2번이 꺼짐
            if (i >= (sanityImages.Count - currentStrikes))
            {
                if (damagedSanitySprite != null)
                {
                    sanityImages[i].sprite = damagedSanitySprite; // UI-2 (off)
                }
            }
            else
            {
                if (fullSanitySprite != null)
                {
                    sanityImages[i].sprite = fullSanitySprite;    // UI-1 (on)
                }
            }
        }
    }

    public void GameOver(string reason)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("GameOverScene");
    }

    public void GameClear()
    {
        Debug.LogWarning("<color=gold><b>🏆🏆🏆 야간 순찰 임무 완수! GAME CLEAR! 🏆🏆🏆</b></color>");
    }
}