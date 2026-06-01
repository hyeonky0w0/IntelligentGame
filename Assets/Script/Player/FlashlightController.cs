using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    [Header("손전등 설정")]
    public Light spotlight;          // 자식의 Light 컴포넌트 연결
    public AudioSource clickSound;   // 켜기/끄기 소리
    public Animator handAnimator;    // 손 애니메이터

    [Header("화면 위치 (카메라 기준 로컬)")]
    public Vector3 restPosition = new Vector3(0.25f, -0.25f, 0.4f);
    public Vector3 walkOffset = new Vector3(0.01f, -0.01f, 0f);
    public float swaySpeed = 8f;

    bool _on = true;
    bool _walking = false;
    float _swayTime = 0f;
    Vector3 _basePos;

    void Start()
    {
        _basePos = restPosition;
        transform.localPosition = _basePos;
    }

    void Update()
    {
        HandleToggle();
        HandleSway();
        HandleAnimation();
    }

    // F키로 손전등 ON/OFF
    void HandleToggle()
    {
        if (!Input.GetKeyDown(KeyCode.F)) return;

        _on = !_on;
        if (spotlight) spotlight.enabled = _on;
        if (clickSound) clickSound.Play();
        if (handAnimator) handAnimator.SetTrigger("on-off");
    }

    // 걸을 때 손 흔들림 (bob)
    void HandleSway()
    {
        _walking = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)
                || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);

        if (_walking)
        {
            _swayTime += Time.deltaTime * swaySpeed;
            float bobX = Mathf.Sin(_swayTime) * walkOffset.x;
            float bobY = Mathf.Sin(_swayTime * 2f) * walkOffset.y; // Y는 2배 빠르게
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                _basePos + new Vector3(bobX, bobY, 0f),
                Time.deltaTime * 10f
            );
        }
        else
        {
            _swayTime = 0f;
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                _basePos,
                Time.deltaTime * 10f
            );
        }
    }

    void HandleAnimation()
    {
        if (handAnimator) handAnimator.SetBool("walking", _walking);
    }
}