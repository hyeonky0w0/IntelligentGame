using UnityEngine;
using UnityEngine.UI;

public class StrawberryHitObject : MonoBehaviour
{
    private float _hitTime;
    private BGMManager _bgm;
    private GuideLight _linkedGuide;
    private bool _placed = false;
    private bool _missed = false;

    public float perfectWindow = 0.3f;   // ±300ms
    public float goodWindow = 0.5f;   // ±500ms
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

        Debug.Log($"[StrawberryHit] Init 완료 - hitTime: {hitTime:F3}s / guidePos: {guidePos}");
    }

    void Update()
    {
        if (_bgm == null || _placed) return;
        float current = _bgm.GetCurrentTime();

        if (!_missed && current > _hitTime + missWindow)
        {
            _missed = true;
            Debug.Log($"[StrawberryHit] MISS (시간 초과) - hitTime: {_hitTime:F3} / current: {current:F3}");
            GameDirector.Instance.AddScore(-10, "MISS");
            Destroy(gameObject);
        }
    }

    public bool TryPlace(Vector2 clickAnchoredPos)
    {
        if (_placed || _missed)
        {
            Debug.Log($"[StrawberryHit] TryPlace 무시 - placed:{_placed} missed:{_missed}");
            return false;
        }

        float dist = Vector2.Distance(clickAnchoredPos, _guidePosition);
        float current = _bgm.GetCurrentTime();
        float diff = Mathf.Abs(current - _hitTime);

        Debug.Log($"[StrawberryHit] 클릭 감지 - dist:{dist:F1}px / diff:{diff * 1000:F0}ms / hitTime:{_hitTime:F3} / current:{current:F3}");

        if (dist > 200f)
        {
            Debug.Log($"[StrawberryHit] 위치 실패 - dist:{dist:F1} > 120px");
            GameDirector.Instance.AddScore(-10, "MISS");
            return false;
        }

        if (diff <= perfectWindow)
        {
            Debug.Log($"[StrawberryHit] PERFECT! dist:{dist:F1}px / diff:{diff * 1000:F0}ms");
            _placed = true;
            GameDirector.Instance.AddScore(50, "PERFECT!");
            SpawnStrawberryVisual();
            return true;
        }
        else if (diff <= goodWindow)
        {
            Debug.Log($"[StrawberryHit] GOOD dist:{dist:F1}px / diff:{diff * 1000:F0}ms");
            _placed = true;
            GameDirector.Instance.AddScore(30, "GOOD");
            SpawnStrawberryVisual();
            return true;
        }

        Debug.Log($"[StrawberryHit] 타이밍 실패 - diff:{diff * 1000:F0}ms > {goodWindow * 1000:F0}ms");
        GameDirector.Instance.AddScore(-10, "MISS");
        return false;
    }

    void SpawnStrawberryVisual()
    {
        if (_linkedGuide != null)
            _linkedGuide.OnCreamPlaced();

        if (_strawberryPrefab == null)
        {
            Debug.LogError("[StrawberryHit] strawberryPrefab이 null!");
            return;
        }

        GameObject straw = Instantiate(_strawberryPrefab, _canvasTransform);
        RectTransform rt = straw.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = _guidePosition;
            rt.sizeDelta = new Vector2(_placedSize, _placedSize);
        }

        // 커서 바로 아래에 배치
        GameObject iceBag = GameObject.Find("Cursor_IceBag");
        GameObject strawCur = GameObject.Find("Cursor_Strawberry");

        int cursorIndex = int.MaxValue;
        if (iceBag != null) cursorIndex = Mathf.Min(cursorIndex, iceBag.transform.GetSiblingIndex());
        if (strawCur != null) cursorIndex = Mathf.Min(cursorIndex, strawCur.transform.GetSiblingIndex());

        if (cursorIndex != int.MaxValue)
            straw.transform.SetSiblingIndex(cursorIndex - 1);

        Debug.Log($"[StrawberryHit] 딸기 배치 성공! pos:{_guidePosition} siblingIndex:{straw.transform.GetSiblingIndex()}");
        Destroy(gameObject);
    }
}