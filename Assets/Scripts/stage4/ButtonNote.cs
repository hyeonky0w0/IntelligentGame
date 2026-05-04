using UnityEngine;

public class ButtonNote : MonoBehaviour
{
    public float speed = 300f;
    [HideInInspector] public bool judged = false;

    private RectTransform _rect;

    void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        _rect.anchoredPosition += Vector2.right * speed * Time.deltaTime;

        if (_rect.anchoredPosition.x > 652.4f && !judged)
            HitZoneChecker.Instance?.RegisterMiss(this);
    }

    public void DestroyNote()
    {
        judged = true;
        Destroy(gameObject);
    }
}