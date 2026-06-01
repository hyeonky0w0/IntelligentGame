using UnityEngine;

public class SuccessBannerUI : MonoBehaviour
{
    void Awake()
    {
        // 자기 자신을 시작하자마자 숨김
        gameObject.SetActive(false);
    }
}
