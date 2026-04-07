using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rigid2D;
    float jumpForce = 600.0f;
    float walkForce = 30.0f;
    float maxWalkSpeed = 2.0f; //최대속도
    public Sprite[] walkSprites;
    public Sprite jumpSprite;
    float time = 0;
    int idx = 0;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        Application.targetFrameRate = 60;
        this.rigid2D = GetComponent<Rigidbody2D>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {   
        //점프
        if (Input.GetMouseButtonDown(0))
        {
            this.rigid2D.AddForce(transform.up * this.jumpForce);
        }
        
        //오른쪽
        if(this.rigid2D.linearVelocityX < this.maxWalkSpeed)
        {
            this.rigid2D.AddForce(transform.right * walkForce);
        }

        //애니메이션
        if (this.rigid2D.linearVelocityY != 0)
        {
            this.spriteRenderer.sprite = this.jumpSprite;
        }
        else
        {
            this.time += Time.deltaTime;
            if (this.time > 0.1f)   //0.1초마다 실행
            {
                this.time = 0;      //0.1초 되면, 시간 리셋
                this.spriteRenderer.sprite = this.walkSprites[this.idx]; //sprite 설정
                this.idx = 1 - this.idx; //현재 idx가 0이면 1, 현재 idx가 1이면 0으로 변경됨 
            }
        }

    }
}
