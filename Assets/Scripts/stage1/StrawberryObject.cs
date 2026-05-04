using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StrawberryObject : MonoBehaviour
{
    [Header("Parts")]
    public GameObject shadowObj;
    public GameObject bodyObj;
    public GameObject topObj;

    private float _hitTime;
    private float _lastHitTime;
    private BGMManager _bgmManager;
    private bool _isSliced = false;
    private bool _bodyShown = false;
    private bool _isMissed = false;

    public float hitWindow = 0.5f;
    public float perfectWindow = 0.1f; // Perfect 판정 범위

    public void Init(float hitTime, float lastHitTime, BGMManager bgm)
    {
        _hitTime = hitTime;
        _lastHitTime = lastHitTime;
        _bgmManager = bgm != null ? bgm : FindObjectOfType<BGMManager>();

        if (shadowObj != null) shadowObj.SetActive(true);
        if (bodyObj != null) bodyObj.SetActive(false);
        if (topObj != null) topObj.SetActive(false);
    }

    void Update()
    {
        if (_bgmManager == null || _isSliced) return;

        float current = _bgmManager.GetCurrentTime();

        if (!_bodyShown && current >= _hitTime)
        {
            _bodyShown = true;
            if (bodyObj != null) bodyObj.SetActive(true);
            if (topObj != null) topObj.SetActive(true);
        }

       
        if (!_isMissed && _bodyShown && current > _hitTime + hitWindow)
        {
            _isMissed = true;
            GameDirector.Instance.AddScore(-10, "MISS");
        }

       
        if (current > _lastHitTime + hitWindow + 0.5f)
            Destroy(gameObject);
    }

    public bool TrySlice()
    {
        if (_isSliced) return false;

        if (!_bodyShown)
        {
            GameDirector.Instance.AddScore(-10, "MISS");
            return false;
        }

        if (_isMissed) return false;

        float current = _bgmManager.GetCurrentTime();
        float diff = Mathf.Abs(current - _hitTime);

        if (diff <= perfectWindow)
        {
            _isSliced = true;
            GameDirector.Instance.AddScore(50, "PERFECT!");
            StartCoroutine(SliceAnimation());
            return true;
        }
        else if (diff <= hitWindow)
        {
            _isSliced = true;
            GameDirector.Instance.AddScore(30, "GOOD");
            StartCoroutine(SliceAnimation());
            return true;
        }

        return false;
    }

    IEnumerator SliceAnimation()
    {
        if (topObj != null)
        {
            RectTransform topRt = topObj.GetComponent<RectTransform>();
            float elapsed = 0f;
            float duration = 0.5f;
            Vector2 startPos = topRt.anchoredPosition;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                float x = startPos.x + 40f * t;
                float y = startPos.y + 120f * t - 200f * t * t;
                topRt.anchoredPosition = new Vector2(x, y);
                topRt.rotation = Quaternion.Euler(0, 0, -360f * t);

                yield return null;
            }
        }

        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}