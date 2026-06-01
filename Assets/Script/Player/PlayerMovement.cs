using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 5f;
    public float gravity = -9.81f;

    [Header("경사로 설정")]
    public float slopeRayLength = 2f;

    [Header("걷기 소리 설정")]
    public AudioSource footstepAudio;   // AudioSource 컴포넌트 연결
    public AudioClip footstepClip;      // 3초짜리 클립 하나 연결

    Vector3 velocity;

    void Start()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();

        if (footstepAudio != null && footstepClip != null)
        {
            footstepAudio.clip = footstepClip;
            footstepAudio.loop = true;
            footstepAudio.playOnAwake = false;
        }
    }

    void Update()
    {
        if (!controller.enabled) return;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;

        bool isMoving = move.magnitude > 0.1f && controller.isGrounded;

        if (IsOnSlope() && move.magnitude > 0)
        {
            Vector3 slopeMove = GetSlopeMoveDirection(move);
            slopeMove += Vector3.up * 0.1f;
            controller.Move(slopeMove * speed * Time.deltaTime);
            velocity.y = -2f;
        }
        else
        {
            controller.Move(move * speed * Time.deltaTime);
            if (controller.isGrounded && velocity.y < 0)
                velocity.y = -2f;
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }

        // 걷기 소리: 움직일 때만 재생, 멈추면 즉시 정지
        if (footstepAudio != null)
        {
            if (isMoving && !footstepAudio.isPlaying)
                footstepAudio.Play();
            else if (!isMoving && footstepAudio.isPlaying)
                footstepAudio.Stop();
        }
    }

    bool IsOnSlope()
    {
        if (!controller.isGrounded) return false;
        if (Physics.Raycast(transform.position, Vector3.down,
                            out RaycastHit hit, slopeRayLength))
        {
            return hit.normal != Vector3.up;
        }
        return false;
    }

    Vector3 GetSlopeMoveDirection(Vector3 move)
    {
        if (Physics.Raycast(transform.position, Vector3.down,
                            out RaycastHit hit, slopeRayLength))
        {
            return Vector3.ProjectOnPlane(move, hit.normal).normalized;
        }
        return move;
    }
}