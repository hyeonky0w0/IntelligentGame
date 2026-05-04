// NoteSpawner4.cs
using UnityEngine;

public class NoteSpawner4 : MonoBehaviour
{
    public GameObject buttonNotePrefab;
    public RectTransform spawnPoint;
    public RectTransform hitZoneRect;
    public Transform canvasTransform;
    public float noteSpeed = 300f;

    private float[] _beatTimes = new float[]
    {
        85.549f,
        // 추가 타이밍 여기에
    };

    private int _nextIndex = 0;

    void Update()
    {
        if (_nextIndex >= _beatTimes.Length) return;

        float currentTime = BGMManager.Instance.GetCurrentTime();
        float travelTime = TravelTime();
        float spawnAt = _beatTimes[_nextIndex] - travelTime;

        if (currentTime >= spawnAt)
        {
            Spawn();
            _nextIndex++;
        }
    }

    float TravelTime()
    {
        // HitZone 중심까지의 거리
        float dist = hitZoneRect.anchoredPosition.x - spawnPoint.anchoredPosition.x;
        return dist / noteSpeed;
    }

    void Spawn()
    {
        GameObject note = Instantiate(buttonNotePrefab, canvasTransform);
        RectTransform rt = note.GetComponent<RectTransform>();
        rt.anchoredPosition = spawnPoint.anchoredPosition;
    }
}