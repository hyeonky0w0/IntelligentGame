//// TimerController.cs
//using UnityEngine;
//using TMPro;
//using UnityEngine.SceneManagement;

//public class TimerController : MonoBehaviour
//{
//    private static TimerController instance;

//    [Header("Timer Settings")]
//    public TMP_Text timerText;

//    float elapsedTime = 0f;
//    bool timerRunning = true;

//    void Awake()
//    {
//        if (instance != null)
//        {
//            Destroy(gameObject);
//            return;
//        }
//        instance = this;
//        DontDestroyOnLoad(gameObject);
//        SceneManager.sceneLoaded += OnSceneLoaded;
//    }

//    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
//    {
//        // 새 씬의 TimerText 오브젝트 자동 연결
//        GameObject timerObj = GameObject.Find("TimerText");
//        if (timerObj != null)
//        {
//            timerText = timerObj.GetComponent<TMP_Text>();
//        }
//        else
//        {
//            timerText = null; // 없으면 비워서 null 체크로 안전하게 처리
//        }
//    }

//    void Update()
//    {
//        if (!timerRunning) return;

//        elapsedTime += Time.deltaTime;
//        UpdateTimerUI();
//    }

//    void UpdateTimerUI()
//    {
//        if (timerText == null) return;

//        int minutes = (int)(elapsedTime / 60);
//        int seconds = (int)(elapsedTime % 60);
//        int milliseconds = (int)((elapsedTime * 100) % 100);
//        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
//    }

//    public void StopTimer() => timerRunning = false;
//    public void StartTimer() => timerRunning = true;
//    public float GetElapsedTime() => elapsedTime;

//    public static TimerController Instance => instance;
//}