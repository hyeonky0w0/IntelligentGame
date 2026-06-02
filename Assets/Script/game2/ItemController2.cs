using System.Collections;
using UnityEngine;

public class ItemController2 : MonoBehaviour
{
    [Header("=== 낙하 ===")]
    public float dropSpeed = -0.03f;

    [Header("=== 연출: 흔들림 ===")]
    [Tooltip("번뇌 아이템에만 ON — 좌우로 흔들리며 낙하")]
    public bool wobble = false;
    public float wobbleFrequency = 2.5f;
    public float wobbleAmplitude = 0.5f;

    [Header("=== 연출: 발광 (Light) ===")]
    public Light itemLight;         

    private float startX;
    private float timeAlive = 0f;
    private bool isDestroying = false;

    void Start()
    {
        startX = transform.position.x;
        if (itemLight != null)
            StartCoroutine(PulsateLight());
    }

    void Update()
    {
        if (isDestroying) return;

        timeAlive += Time.deltaTime;

        transform.Translate(0, dropSpeed, 0);

        if (wobble)
        {
            float offsetX = Mathf.Sin(timeAlive * wobbleFrequency) * wobbleAmplitude;
            Vector3 pos = transform.position;
            pos.x = startX + offsetX;
            transform.position = pos;
        }

        if (transform.position.y < -1.5f)
        {
            DestroyItem();
        }
    }

    public void OnCollected()
    {
        DestroyItem();
    }

    void DestroyItem()
    {
        if (isDestroying) return;
        isDestroying = true;
        Destroy(gameObject);
    }

    IEnumerator PulsateLight()
    {
        float baseIntensity = itemLight.intensity;
        while (true)
        {
            float pulse = wobble
                ? Mathf.Abs(Mathf.Sin(Time.time * 6f + Random.Range(-0.5f, 0.5f))) 
                : Mathf.Abs(Mathf.Sin(Time.time * 2f));                            
            itemLight.intensity = baseIntensity * (0.6f + pulse * 0.4f);
            yield return null;
        }
    }
}
