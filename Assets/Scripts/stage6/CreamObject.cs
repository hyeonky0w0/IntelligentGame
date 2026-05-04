using UnityEngine;
using UnityEngine.UI;

public class CreamObject : MonoBehaviour
{
    private float _hitTime;
    private BGMManager _bgm;
    private GuideLight _linkedGuide;
    private bool _placed = false;
    private bool _missed = false;

    public float perfectWindow = 0.3f;
    public float goodWindow = 0.5f;
    public float missWindow = 0.7f;

    private GameObject _creamPrefab;
    private Transform _canvasTransform;
    private Vector2 _guidePosition;

    public void Init(float hitTime, BGMManager bgm, GuideLight guide,
                     Vector2 guidePos, GameObject creamPrefab, Transform canvasTransform)
    {
        _hitTime = hitTime;
        _bgm = bgm;
        _linkedGuide = guide;
        _guidePosition = guidePos;
        _creamPrefab = creamPrefab;
        _canvasTransform = canvasTransform;
    }

    void Update()
    {
        if (_bgm == null || _placed) return;
        float current = _bgm.GetCurrentTime();

        if (!_missed && current > _hitTime + missWindow)
        {
            _missed = true;
            GameDirector.Instance.AddScore(-10, "MISS");
            Destroy(gameObject); // 히트 영역만 제거, 크림 비주얼은 건드리지 않음
        }
    }

    public bool TryPlace(Vector2 clickAnchoredPos)
    {
        if (_placed || _missed) return false;

        float dist = Vector2.Distance(clickAnchoredPos, _guidePosition);
        if (dist > 200f) 
        {
            GameDirector.Instance.AddScore(-10, "MISS");
            return false;
        }

        float current = _bgm.GetCurrentTime();
        float diff = Mathf.Abs(current - _hitTime);

        if (diff <= perfectWindow)
        {
            _placed = true;
            GameDirector.Instance.AddScore(50, "PERFECT!");
            SpawnCreamVisual();
            return true;
        }
        else if (diff <= goodWindow)
        {
            _placed = true;
            GameDirector.Instance.AddScore(30, "GOOD");
            SpawnCreamVisual();
            return true;
        }

        GameDirector.Instance.AddScore(-10, "MISS");
        return false;
    }

    void SpawnCreamVisual()
    {
        if (_linkedGuide != null)
            _linkedGuide.OnCreamPlaced();

        if (_creamPrefab != null && _canvasTransform != null)
        {
            GameObject cream = Instantiate(_creamPrefab, _canvasTransform);
            RectTransform rt = cream.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = _guidePosition;
            }
            // SetSiblingIndex(0) 제거하고 커서 바로 아래에 배치
            GameObject iceBag = GameObject.Find("Cursor_IceBag");
            GameObject strawCur = GameObject.Find("Cursor_Strawberry");

            int cursorIndex = int.MaxValue;
            if (iceBag != null) cursorIndex = Mathf.Min(cursorIndex, iceBag.transform.GetSiblingIndex());
            if (strawCur != null) cursorIndex = Mathf.Min(cursorIndex, strawCur.transform.GetSiblingIndex());

            if (cursorIndex != int.MaxValue)
                cream.transform.SetSiblingIndex(cursorIndex - 1);
        }

        Destroy(gameObject);
    }
}