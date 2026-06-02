using System.Collections;
using UnityEngine;

public class BuddhistHallReturn : MonoBehaviour
{
    [Header("셔터 오브젝트")]
    public GameObject door;
    public float shutterRiseHeight = 3f;
    public float shutterRiseDuration = 1.5f;

    [Header("플레이어 위치 복원")]
    public Transform playerTransform;

    [Header("석상 반응")]
    public ParticleSystem statueGlow;
    public AudioClip doorOpenSFX;
    public AudioClip statueSuccessSFX;

    [Header("성공 안내 UI")]
    public GameObject successBanner;
    public float bannerDuration = 3f;

    void Awake()
    {
        if (successBanner != null) successBanner.SetActive(false);
    }

    void Start()
    {
        if (successBanner != null) successBanner.SetActive(false);

        if (playerTransform != null && PlayerPrefs.HasKey("PlayerX"))
        {
            float x = PlayerPrefs.GetFloat("PlayerX");
            float y = PlayerPrefs.GetFloat("PlayerY");
            float z = PlayerPrefs.GetFloat("PlayerZ");
            CharacterController cc = playerTransform.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            playerTransform.position = new Vector3(x, y, z);
            if (cc != null) cc.enabled = true;
            PlayerPrefs.DeleteKey("PlayerX");
            PlayerPrefs.DeleteKey("PlayerY");
            PlayerPrefs.DeleteKey("PlayerZ");
        }

        int success = PlayerPrefs.GetInt("MinigameSuccess", 0);
        PlayerPrefs.DeleteKey("MinigameSuccess");
        PlayerPrefs.Save();

        if (success == 1)
            StartCoroutine(SuccessSequence());
    }

    IEnumerator SuccessSequence()
    {
        if (statueGlow != null) statueGlow.Play();
        if (statueSuccessSFX != null)
            AudioSource.PlayClipAtPoint(statueSuccessSFX, transform.position);

        yield return new WaitForSeconds(0.8f);

        if (doorOpenSFX != null)
            AudioSource.PlayClipAtPoint(doorOpenSFX,
                door != null ? door.transform.position : transform.position);

        if (door != null)
            yield return StartCoroutine(RiseShutter());

        if (successBanner != null)
        {
            successBanner.SetActive(true);
            yield return new WaitForSeconds(bannerDuration);
            successBanner.SetActive(false);
        }
    }

    IEnumerator RiseShutter()
    {
        Rigidbody rb = door.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        Vector3 startPos = door.transform.position;
        Vector3 endPos = startPos + Vector3.up * shutterRiseHeight;
        float elapsed = 0f;

        while (elapsed < shutterRiseDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / shutterRiseDuration);
            door.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
        door.transform.position = endPos;
    }
}