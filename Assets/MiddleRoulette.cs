using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiddleRoulette : MonoBehaviour
{
    float rotSpeed = 0; //회전 속도 -> 클릭을 했을 때 시작하도록 클릭하기전에는 rotSepeed 변수 값을 0으로 설정

    void Start()
    {
    }

    void Update()
    {
        //클릭하면 회전 속도를 설정한다.
        if (Input.GetMouseButtonDown(0))
        {
            this.rotSpeed = 20.0f;
        }

        // 회전 속도만큼 룰렛을 회전시킨다
        transform.Rotate(0, 0, this.rotSpeed);

        // 우클릭을 누르면 회전을 1씩 감속
        if (Input.GetMouseButtonDown(1))
        {   
            this.rotSpeed *= 0.96f;
        }
    }
}
