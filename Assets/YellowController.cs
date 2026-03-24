using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class YellowController : MonoBehaviour
{

    GameObject player;

    void Start()
    {
        this.player = GameObject.Find("player_0");
    }

    void Update()
    {
        if (this.player == null)
        {
            return;
        }

        //위쪽 화살표가 눌렸을 때
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.Translate(-3, 0, 0); 
        }

        //아래쪽 화살표가 눌렸을 때
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            transform.Translate(3, 0, 0);
        }

        //충돌 판정
        Vector2 p1 = transform.position; //노란 고양이 중심 좌표
        Vector2 p2 = this.player.transform.position; //플레이어 중심 좌표

        Vector2 dir = p1 - p2;
        float d = dir.magnitude; //d = 노란 고양이와 플레이어간의 거리
        float r1 = 1.0f;
        float r2 = 1.0f;

        if (d < r1 + r2)
        {
            transform.Translate(-3.0f, 0, 0, Space.World);
            this.player.transform.Translate(3.0f, 0, 0, Space.World);
        }


    }
}
