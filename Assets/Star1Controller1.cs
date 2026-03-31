using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star1Controller1 : MonoBehaviour
{
    GameObject player;

    void Start()
    {
        this.player = GameObject.Find("player_0");
    }

    void Update()
    {
        transform.Rotate(0, 1, 0); 

        transform.Translate(0, -0.1f, 0);

        if (transform.position.y < -5.0f)
        {
            Destroy(gameObject);
        }

        if (this.player == null)
        {
            return;
        }

        Vector2 p1 = transform.position; //스타1 중심 좌표
        Vector2 p2 = this.player.transform.position; //플레이어 중심 좌표

        Vector2 dir = p1 - p2;
        float d = dir.magnitude; //d = 화살과 플레이어간의 거리
        float r1 = 0.5f;
        float r2 = 1.0f;

        if (d < r1 + r2)
        {
            GameObject director = GameObject.Find("GameDirector");
            director.GetComponent<GameDirector>().IncreaseHp();

            //충돌했다면 별 소멸
            Destroy(gameObject);
        }

    }
}
