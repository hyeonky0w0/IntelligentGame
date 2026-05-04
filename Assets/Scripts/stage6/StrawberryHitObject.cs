using UnityEngine;
using UnityEngine.UI;

public class StrawberryHitObject : MonoBehaviour
{
    private float _hitTime;
    private BGMManager _bgm;
    private GuideLight _linkedGuide;
    private bool _placed = false;
    private bool _missed = false;

    public float perfectWindow = 0.3f;
    public float goodWindow = 0.5f;
    public float missWindow = 0.7f;

    private GameObject _strawberryPrefab;
    private Transform _canvasTransform;
    private Vector2 _guidePosition;
    private float _placedSize = 125f;

    public void Init(float hitTime, BGMManager bgm, GuideLight guide,
                     Vector2 guidePos, GameObject strawberryPrefab,
                     Transform canvasTransform, float placedSize = 155f)
    {
        _hitTime = hitTime;
        _bgm = bgm;
        _linkedGuide = guide;
        _guidePosition = guidePos;
        _strawberryPrefab = strawberryPrefab;
        _canvasTransform = canvasTransform;
        _placedSize = placedSize;
    }

    void Update()
    {
        if (_bgm == null || _placed) return;
        float current = _bgm.GetCurrentTime();

        if (!_missed && current > _hitTime + missWindow)
        {
            _missed = true;
            GameDirector.Instance.AddScore(-10, "MISS");
            Destroy(gameObject);
        }
    }

    public bool TryPlace(Vector2 clickAnchoredPos)
    {
        if (_placed || _missed) return false;

        float dist = Vector2.Distance(clickAnchoredPos, _guidePosition);
        float current = _bgm.GetCurrentTime();
        float diff = Mathf.Abs(current - _hitTime);

        if (dist > 200f)
        {
            GameDirector.Instance.AddScore(-10, "MISS");
            return false;
        }

        if (diff <= perfectWindow)
        {
            _placed = true;
            GameDirector.Instance.AddScore(100, "PERFECT!");
            SpawnStrawberryVisual();
            return true;
        }
        else if (diff <= goodWindow)
        {
            _placed = true;
            GameDirector.Instance.AddScore(50, "GOOD");
            SpawnStrawberryVisual();
            return true;
        }

        GameDirector.Instance.AddScore(-10, "MISS");
        return false;
    }

    void SpawnStrawberryVisual()
    {
        if (_linkedGuide != null)
            _linkedGuide.OnCreamPlaced();

        if (_strawberryPrefab == null) return;

        GameObject straw = Instantiate(_strawberryPrefab, _canvasTransform);
        RectTransform rt = straw.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = _guidePosition;
            rt.sizeDelta = new Vector2(_placedSize, _placedSize);
        }

        GameObject iceBag = GameObject.Find("Cursor_IceBag");
        GameObject strawCur = GameObject.Find("Cursor_Strawberry");

        int cursorIndex = int.MaxValue;
        if (iceBag != null) cursorIndex = Mathf.Min(cursorIndex, iceBag.transform.GetSiblingIndex());
        if (strawCur != null) cursorIndex = Mathf.Min(cursorIndex, strawCur.transform.GetSiblingIndex());

        if (cursorIndex != int.MaxValue)
            straw.transform.SetSiblingIndex(cursorIndex - 1);

        Destroy(gameObject);
    }
}