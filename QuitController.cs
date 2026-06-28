using UnityEngine;

public class QuitController : MonoBehaviour
{
    // 버튼에 연결할 함수입니다. 꼭 public으로 적어주셔야 해요!
    public void GameQuit()
    {
#if UNITY_EDITOR
        // 유니티 에디터에서 실행 중일 때는 플레이 모드를 종료
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 실제 빌드된 게임(PC, 모바일 등)일 때는 앱을 완전히 종료
        Application.Quit();
#endif
    }
}