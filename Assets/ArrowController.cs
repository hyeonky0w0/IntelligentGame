using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    GameObject player;

    void Start()
    {
        this.player = GameObject.Find("player_0");
    }

    void Update()
    {
        transform.Translate(0, -0.1f, 0);

        if(transform.position.y < -5.0f)
        {
            Destroy(gameObject);
        }

        //충돌 판정
        Vector2 p1 = transform.position; //화살 중심 좌표
        Vector2 p2 = this.player.transform.position; //플레이어 중심 좌표

        Vector2 dir = p1 - p2;
        float d = dir.magnitude; //d = 화살과 플레이어간의 거리
        float r1 = 0.5f;
        float r2 = 1.0f;

        if (d < r1 + r2)
        {
            Destroy(gameObject);
        }

    }
}
