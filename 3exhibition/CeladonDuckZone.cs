using UnityEngine;
using System.Collections;

public class CeladonDuckZone : MonoBehaviour
{
    [Header("Anomaly ID Config")]
    public int anomalyID = 33;

    [Header("오리 유물 설정")]
    public Transform duckTransform;

    public float pushForce = 5f;
    [Header("공포 연출 설정")]
    public float shakeIntensity = 0.2f;

    [Header("오디오 설정")]
    public AudioSource audioSource;
    public AudioClip duckSound;

    private bool isInside = false;
    private Vector3 initialCamLocalPos;
    private Quaternion initialCamLocalRot;
    private Coroutine duckRoutine; // 코루틴 제어를 위해 변수 선언

    private void Awake()
    {
        if (Camera.main != null)
        {
            initialCamLocalPos = Camera.main.transform.localPosition;
            initialCamLocalRot = Camera.main.transform.localRotation;
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // 오디오 소스 기본 세팅 안전장치
        if (audioSource != null && duckSound != null)
        {
            audioSource.clip = duckSound;
            audioSource.loop = false; // 깩깩 소리를 직접 제어하므로 루프는 끕니다.
        }
    }

    private void OnEnable()
    {
        ResetAnomaly();
    }

    private void OnDisable()
    {
        ResetAnomaly();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!enabled) return;
        if (!other.CompareTag("Player")) return;
        if (GameManager.Instance == null || GameManager.Instance.currentAnomalyID != anomalyID) return;

        if (!isInside)
        {
            isInside = true;
            duckRoutine = StartCoroutine(DuckZoneRoutine(other.gameObject));
        }
    }

    // 🚪 플레이어가 구역을 지나쳐서 나갔을 때 발동
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log("플레이어가 오리 구역을 벗어남 - 연출 및 사운드 종료");
        ResetAnomaly();
    }

    private IEnumerator DuckZoneRoutine(GameObject player)
    {
        yield return new WaitForSeconds(0.2f);
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        Camera mainCam = Camera.main;
        if (mainCam == null) yield break;

        Vector3 camOrgLocalPos = initialCamLocalPos;

        // 1. 밀치기 및 셰이크 연출 (3번 반복)
        for (int i = 0; i < 3; i++)
        {
            if (GameManager.Instance == null || GameManager.Instance.currentAnomalyID != anomalyID) yield break;

            Debug.Log("3-3. 청자 오리 구역: '깩, 깩' 밀치기");

            // 🔊 사운드 재생 (이전 재생 소리가 있다면 중지하고 처음부터 재생)
            if (audioSource != null && duckSound != null)
            {
                audioSource.Stop();
                audioSource.Play();
            }

            Vector3 pushDir = Quaternion.Euler(0, Random.Range(0, 360), 0) * Vector3.forward;
            if (playerRb != null) playerRb.AddForce(pushDir * pushForce, ForceMode.Impulse);

            float shakeTime = 0.2f;
            while (shakeTime > 0)
            {
                mainCam.transform.localPosition = camOrgLocalPos + (Random.insideUnitSphere * shakeIntensity);
                shakeTime -= Time.deltaTime;
                yield return null;
            }
            mainCam.transform.localPosition = camOrgLocalPos;
            yield return new WaitForSeconds(0.4f);
        }

        Debug.LogError("🚨 [클라이맥스] 오리 유물 방향을 바라보며 전도!");

        // 2. 넘어질 때 회전값 계산
        Quaternion originalCamRot = mainCam.transform.localRotation;
        Quaternion fallRot;

        if (duckTransform != null)
        {
            Vector3 dirToDuck = (duckTransform.position - mainCam.transform.position).normalized;
            Quaternion lookAtDuckRot = Quaternion.LookRotation(dirToDuck);
            fallRot = Quaternion.Inverse(player.transform.rotation) * lookAtDuckRot * Quaternion.Euler(45f, -20f, 65f);
        }
        else
        {
            Vector3 currentEuler = mainCam.transform.localEulerAngles;
            fallRot = Quaternion.Euler(45f, currentEuler.y - 20f, 65f);
        }

        Vector3 fallPos = camOrgLocalPos + new Vector3(0, -0.8f, 0);

        // 3. 넘어지는 애니메이션 (Lerp)
        float elapsed = 0f;
        float fallSpeed = 0.15f;
        while (elapsed < fallSpeed)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fallSpeed;

            mainCam.transform.localRotation = Quaternion.Lerp(originalCamRot, fallRot, t);
            mainCam.transform.localPosition = Vector3.Lerp(camOrgLocalPos, fallPos, t);
            yield return null;
        }

        // 바닥 부딪힘 충격 셰이크
        float impactTime = 0.3f;
        while (impactTime > 0)
        {
            mainCam.transform.localPosition = fallPos + (Random.insideUnitSphere * (shakeIntensity * 1.5f));
            impactTime -= Time.deltaTime;
            yield return null;
        }
        mainCam.transform.localPosition = fallPos;

        yield return new WaitForSeconds(2.0f);

        // 4. 다시 일어설 때 원래 카메라 세팅으로 원상복구
        elapsed = 0f;
        float riseSpeed = 0.6f;
        while (elapsed < riseSpeed)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / riseSpeed;

            mainCam.transform.localRotation = Quaternion.Lerp(fallRot, initialCamLocalRot, t);
            mainCam.transform.localPosition = Vector3.Lerp(fallPos, initialCamLocalPos, t);
            yield return null;
        }

        mainCam.transform.localRotation = initialCamLocalRot;
        mainCam.transform.localPosition = initialCamLocalPos;

        Debug.Log("플레이어 간신히 일어남");
        isInside = false;
    }

    public void ResetAnomaly()
    {
        // 작동 중인 코루틴 중단
        if (duckRoutine != null)
        {
            StopCoroutine(duckRoutine);
            duckRoutine = null;
        }

        isInside = false;

        // 🔇 사운드 즉시 종료
        if (audioSource != null)
        {
            audioSource.Stop();
        }

        // 카메라 원상복구
        if (Camera.main != null)
        {
            Camera.main.transform.localPosition = initialCamLocalPos != Vector3.zero ? initialCamLocalPos : Camera.main.transform.localPosition;
            Camera.main.transform.localRotation = initialCamLocalRot != Quaternion.identity ? initialCamLocalRot : Quaternion.identity;
        }
    }
}