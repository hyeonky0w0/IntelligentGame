using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage6Spawner : MonoBehaviour
{
    [Header("References")]
    public GameObject guideLightPrefab;
    public GameObject creamPrefab;
    public GameObject strawberryPrefab;
    public Transform canvasTransform;

    [Header("Cursors")]
    public GameObject iceBagCursorObj;
    public GameObject strawberryCursorObj;

    [Header("GuideLight Colors")]
    public Color creamGuideColor = Color.white;
    public Color strawberryGuideColor = Color.red;

    [Header("Positions (6개) - 가이드라이트 위치")]
    public List<Vector2> positions = new List<Vector2>();

    private BGMManager _bgm;

    private readonly float[] _creamGuideTimes = {
        96140f/1000f, 96340f/1000f, 96575f/1000f,
        96997f/1000f, 97201f/1000f, 97428f/1000f
    };
    private readonly float[] _creamHitTimes = {
         99552f/1000f,  99778f/1000f,  99994f/1000f,
        100425f/1000f, 100618f/1000f, 100845f/1000f
    };
    private readonly float[] _strawGuideTimes = {
        102988f/1000f, 103204f/1000f, 103430f/1000f,
        103861f/1000f, 104076f/1000f, 104281f/1000f
    };
    private readonly float[] _strawHitTimes = {
        106414f/1000f, 106618f/1000f, 106844f/1000f,
        107275f/1000f, 107502f/1000f, 107695f/1000f
    };

    private readonly float _cursorSwitchTime = 101500f / 1000f;

    private int _creamGuideIndex = 0;
    private int _creamHitIndex = 0;
    private int _strawGuideIndex = 0;
    private int _strawHitIndex = 0;
    private bool _cursorSwitched = false;

    private List<GuideLight> _creamGuides = new List<GuideLight>();
    private List<GuideLight> _strawGuides = new List<GuideLight>();
    private List<CreamObject> _creamHits = new List<CreamObject>();
    private List<StrawberryHitObject> _strawHits = new List<StrawberryHitObject>();

    void Start()
    {
        _bgm = FindObjectOfType<BGMManager>();
        if (strawberryCursorObj != null) strawberryCursorObj.SetActive(false);
        if (iceBagCursorObj != null) iceBagCursorObj.SetActive(true);
    }

    void Update()
    {
        if (_bgm == null) return;
        float current = _bgm.GetCurrentTime();

        if (!_cursorSwitched && current >= _cursorSwitchTime)
        {
            _cursorSwitched = true;
            if (iceBagCursorObj != null) iceBagCursorObj.SetActive(false);
            if (strawberryCursorObj != null)
            {
                strawberryCursorObj.SetActive(true);
                strawberryCursorObj.transform.SetAsLastSibling();
            }
        }

        while (_creamGuideIndex < _creamGuideTimes.Length &&
               current >= _creamGuideTimes[_creamGuideIndex])
        {
            SpawnGuideLight(_creamGuideTimes[_creamGuideIndex],
                            _creamHitTimes[_creamGuideIndex],
                            _creamGuideIndex, _creamGuides, creamGuideColor);
            _creamGuideIndex++;
        }

        while (_creamHitIndex < _creamHitTimes.Length &&
               current >= _creamHitTimes[_creamHitIndex] - 1.0f)
        {
            SpawnCreamHit(_creamHitIndex);
            _creamHitIndex++;
        }

        while (_strawGuideIndex < _strawGuideTimes.Length &&
               current >= _strawGuideTimes[_strawGuideIndex])
        {
            SpawnGuideLight(_strawGuideTimes[_strawGuideIndex],
                            _strawHitTimes[_strawGuideIndex],
                            _strawGuideIndex, _strawGuides, strawberryGuideColor);
            _strawGuideIndex++;
        }

        while (_strawHitIndex < _strawHitTimes.Length &&
               current >= _strawHitTimes[_strawHitIndex] - 1.0f)
        {
            SpawnStrawberryHit(_strawHitIndex);
            _strawHitIndex++;
        }
    }

    void SpawnGuideLight(float guideTime, float hitTime, int index,
                         List<GuideLight> guideList, Color color)
    {
        Vector2 pos = index < positions.Count ? positions[index] : Vector2.zero;

        GameObject obj = Instantiate(guideLightPrefab, canvasTransform);

        Image img = obj.GetComponent<Image>();
        if (img != null) img.color = color;

        RectTransform rt = obj.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = pos;
        }

        GameObject iceBag = GameObject.Find("Cursor_IceBag");
        GameObject strawCur = GameObject.Find("Cursor_Strawberry");

        int cursorIndex = int.MaxValue;
        if (iceBag != null) cursorIndex = Mathf.Min(cursorIndex, iceBag.transform.GetSiblingIndex());
        if (strawCur != null) cursorIndex = Mathf.Min(cursorIndex, strawCur.transform.GetSiblingIndex());

        if (cursorIndex != int.MaxValue)
            obj.transform.SetSiblingIndex(cursorIndex);
        else
            obj.transform.SetSiblingIndex(canvasTransform.childCount - 1);

        GuideLight guide = obj.GetComponent<GuideLight>();
        if (guide != null)
            guide.Init(guideTime, hitTime, _bgm);

        while (guideList.Count <= index) guideList.Add(null);
        guideList[index] = guide;
    }

    void SpawnCreamHit(int index)
    {
        Vector2 pos = index < positions.Count ? positions[index] : Vector2.zero;

        GameObject obj = new GameObject($"CreamHitArea_{index}");
        obj.transform.SetParent(canvasTransform, false);

        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(250f, 250f);

        Image img = obj.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0);

        CreamObject cream = obj.AddComponent<CreamObject>();
        GuideLight linkedGuide = (index < _creamGuides.Count) ? _creamGuides[index] : null;
        cream.Init(_creamHitTimes[index], _bgm, linkedGuide, pos, creamPrefab, canvasTransform);

        while (_creamHits.Count <= index) _creamHits.Add(null);
        _creamHits[index] = cream;
    }

    void SpawnStrawberryHit(int index)
    {
        Vector2 pos = index < positions.Count ? positions[index] : Vector2.zero;

        GameObject obj = new GameObject($"StrawberryHitArea_{index}");
        obj.transform.SetParent(canvasTransform, false);

        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(150f, 150f);

        Image img = obj.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0);

        StrawberryHitObject straw = obj.AddComponent<StrawberryHitObject>();
        GuideLight linkedGuide = (index < _strawGuides.Count) ? _strawGuides[index] : null;
        straw.Init(_strawHitTimes[index], _bgm, linkedGuide, pos,
                   strawberryPrefab, canvasTransform, 125f);

        while (_strawHits.Count <= index) _strawHits.Add(null);
        _strawHits[index] = straw;
    }
}