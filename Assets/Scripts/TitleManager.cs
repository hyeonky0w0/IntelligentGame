using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [Header("패널")]
    public GameObject gameSelectPanel;

    [Header("케이크 버튼 이미지 교체")]
    public Image cakeImage;       // 케이크 버튼의 Image 컴포넌트
    public Sprite cake1Sprite;    // 기본 이미지 (Cake1)
    public Sprite cake2Sprite;    // 눌렸을 때 이미지 (Cake2)

    private bool _cakeSelected = false;

    void Start()
    {
        if (gameSelectPanel != null)
            gameSelectPanel.SetActive(false);

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.ResetScore();

        // 시작할 때 cake1로 초기화
        if (cakeImage != null && cake1Sprite != null)
            cakeImage.sprite = cake1Sprite;
    }

    public void OnClickStart()
    {
        if (gameSelectPanel != null)
            gameSelectPanel.SetActive(true);
    }


    // 케이크 버튼 OnClick에 연결
    public void OnClickCake()
    {
        // 이미지 Cake2로 교체
        if (cakeImage != null && cake2Sprite != null)
            cakeImage.sprite = cake2Sprite;

        // 잠깐 보여주고 씬 전환 (0.2초 딜레이)
        Invoke(nameof(LoadIntroScene), 0.2f);
    }

    void LoadIntroScene()
    {
        SceneManager.LoadScene("IntroScene");
    }
}