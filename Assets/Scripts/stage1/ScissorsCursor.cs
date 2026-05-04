using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class ScissorsCursor : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite scissorsIdle;
    public Sprite scissorsCut;

    [Header("Settings")]
    public float cutImageDuration = 0.1f;
    public AudioClip cutSound;

    private Image _image;
    private AudioSource _audioSource;
    private float _cutTimer = 0f;
    private bool _isCutting = false;
    private RectTransform _rectTransform;
    private Canvas _canvas;

    void Start()
    {
        _image = GetComponent<Image>();
        _audioSource = GetComponent<AudioSource>();
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();

        Cursor.visible = false;
    }

    void Update()
    {
        // Input System 방식으로 마우스 위치
        Vector2 mousePos = Mouse.current.position.ReadValue();

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.GetComponent<RectTransform>(),
            mousePos,
            _canvas.worldCamera,
            out localPoint
        );
        _rectTransform.anchoredPosition = localPoint;

        // Input System 방식으로 클릭 감지
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            _image.sprite = scissorsCut;
            _isCutting = true;
            _cutTimer = 0f;

            if (_audioSource != null && cutSound != null)
                _audioSource.PlayOneShot(cutSound);

            TrySliceStrawberry();
        }

        if (_isCutting)
        {
            _cutTimer += Time.deltaTime;
            if (_cutTimer >= cutImageDuration)
            {
                _image.sprite = scissorsIdle;
                _isCutting = false;
            }
        }
    }

    void TrySliceStrawberry()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = mousePos;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            // 본인(가위) 제외
            if (result.gameObject == gameObject) continue;

            StrawberryObject strawberry = result.gameObject.GetComponent<StrawberryObject>();
            if (strawberry == null)
                strawberry = result.gameObject.GetComponentInParent<StrawberryObject>();

            if (strawberry != null)
            {
                strawberry.TrySlice();
                break;
            }
        }
    }
}