// StrawberryCursor.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class StrawberryCursor : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite strawberrySprite;

    [Header("Size Settings")]
    public float normalSize = 115f;
    public float placedSize = 125f;

    [Header("Settings")]
    public AudioClip placeSound;

    private Image _image;
    private AudioSource _audioSource;
    private RectTransform _rectTransform;
    private Canvas _canvas;

    void Start()
    {
        _image = GetComponent<Image>();
        _audioSource = GetComponent<AudioSource>();
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();

        Cursor.visible = false;
        _image.sprite = strawberrySprite;
        _rectTransform.sizeDelta = new Vector2(normalSize, normalSize);

        transform.SetAsLastSibling();
    }

    void Update()
    {
        transform.SetAsLastSibling();

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.GetComponent<RectTransform>(),
            mousePos,
            _canvas.worldCamera,
            out localPoint
        );
        _rectTransform.anchoredPosition = localPoint;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (_audioSource != null && placeSound != null)
                _audioSource.PlayOneShot(placeSound);

            TryStrawberry(localPoint);
        }
    }

    void TryStrawberry(Vector2 clickAnchoredPos)
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = mousePos;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject == gameObject) continue;

            StrawberryHitObject straw = result.gameObject.GetComponent<StrawberryHitObject>();
            if (straw == null)
                straw = result.gameObject.GetComponentInParent<StrawberryHitObject>();

            if (straw != null)
            {
                straw.TryPlace(clickAnchoredPos);
                break;
            }
        }
    }
}