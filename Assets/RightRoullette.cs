using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightRoullette : MonoBehaviour
{
    float rotSpeed = 0; //회전 속도 -> 클릭을 했을 때 시작하도록 클릭하기전에는 rotSepeed 변수 값을 0으로 설정

    void Start()
    {
    }

    void Update()
    {

        //클릭하면 회전 속도를 설정한다.
        if (Input.GetMouseButton(0))
        {
            this.rotSpeed = 1.0f;
        }
        else 
        {
            this.rotSpeed *= 0.96f;
        }

        transform.Rotate(0, 0, rotSpeed);
    }
}
