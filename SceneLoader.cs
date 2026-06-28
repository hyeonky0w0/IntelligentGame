using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void GoToMainScene()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        PlayerPrefs.DeleteKey("HasSeenIntro");
        PlayerPrefs.Save();

        SceneManager.LoadScene("MainScene");
    }
}