using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackCatController : MonoBehaviour
{
    public GameObject player;
    void Start()
    {
        this.player = GameObject.Find("player_0");
    }

    void Update()
    {
        // 1. 플레이어가 이미 파괴되었다면(null이라면) 아래 코드를 실행하지 않고 돌아감
        if (this.player == null)
        {
            return;
        }
        //충돌 판정
        Vector2 p1 = transform.position; //검은 고양이 중심 좌표
        Vector2 p2 = this.player.transform.position; //플레이어 중심 좌표

        Vector2 dir = p1 - p2;
        float d = dir.magnitude; //d = 검은 고양이와 플레이어간의 거리
        float r1 = 1.0f;
        float r2 = 1.0f;

        if (d < r1 + r2)
        {
            Destroy(this.player);
        }

    }
}
