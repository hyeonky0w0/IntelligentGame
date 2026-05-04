// GuideLight.cs
using UnityEngine;

public class GuideLight : MonoBehaviour
{
    private float _spawnTime;
    private float _linkedCreamTime;
    private BGMManager _bgm;
    private bool _expired = false;

    public void Init(float spawnTime, float linkedCreamTime, BGMManager bgm)
    {
        _spawnTime = spawnTime;
        _linkedCreamTime = linkedCreamTime;
        _bgm = bgm;
    }

    void Update()
    {
        if (_bgm == null || _expired) return;
        float current = _bgm.GetCurrentTime();

        // 크림 판정 시간 + 여유 지나면 제거
        if (current > _linkedCreamTime + 0.3f)
        {
            _expired = true;
            Destroy(gameObject);
        }
    }

    // 크림이 성공적으로 놓이면 외부에서 호출
    public void OnCreamPlaced()
    {
        _expired = true;
        Destroy(gameObject);
    }
}