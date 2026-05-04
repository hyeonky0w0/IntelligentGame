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

        if (current > _linkedCreamTime + 0.3f)
        {
            _expired = true;
            Destroy(gameObject);
        }
    }

    public void OnCreamPlaced()
    {
        _expired = true;
        Destroy(gameObject);
    }
}