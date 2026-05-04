using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class IceBagCursor : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite iceBag1;
    public Sprite iceBag2;

    [Header("Settings")]
    public float clickImageDuration = 0.15f;
    public AudioClip placeSound;

    private Image _image;
    private AudioSource _audioSource;
    private RectTransform _rectTransform;
    private Canvas _canvas;
    private Coroutine _resetRoutine;

    void Start()
    {
        _image = GetComponent<Image>();
        _audioSource = GetComponent<AudioSource>();
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();

        Cursor.visible = false;
        _image.sprite = iceBag1;

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
            _image.sprite = iceBag2;
            if (_resetRoutine != null) StopCoroutine(_resetRoutine);
            _resetRoutine = StartCoroutine(ResetSprite());

            if (_audioSource != null && placeSound != null)
                _audioSource.PlayOneShot(placeSound);

            TryCream(localPoint);
        }
    }

    IEnumerator ResetSprite()
    {
        yield return new WaitForSeconds(clickImageDuration);
        _image.sprite = iceBag1;
    }

    void TryCream(Vector2 clickAnchoredPos)
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = mousePos;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject == gameObject) continue;

            CreamObject cream = result.gameObject.GetComponent<CreamObject>();
            if (cream == null)
                cream = result.gameObject.GetComponentInParent<CreamObject>();

            if (cream != null)
            {
                cream.TryPlace(clickAnchoredPos);
                break;
            }
        }
    }
}