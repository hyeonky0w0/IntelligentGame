using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private AudioClip titleBGM;  
    [SerializeField] private float bgmVolume = 1f;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = titleBGM;
        audioSource.loop = true;
        audioSource.volume = bgmVolume;
        audioSource.playOnAwake = false;
        audioSource.Play();

        startButton.onClick.AddListener(() =>
        {
            audioSource.Stop(); 
            SceneManager.LoadScene("MainScene");
        });
    }

    void OnDestroy()
    {
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
    }
}