using UnityEngine;
using UnityEngine.InputSystem;

public class Stage5InputController : MonoBehaviour
{
    private Stage5Director director;

    void Start()
    {
        director = Stage5Director.Instance;
    }

    void Update()
    {
        long now = Stage5Director.CurrentMusicTimeMs;
        if (now < Stage5Director.PLAY_START || now > Stage5Director.END_TIME) return;

        // 스페이스바 입력 감지
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            director.OnSpacePressed();
        }
    }
}