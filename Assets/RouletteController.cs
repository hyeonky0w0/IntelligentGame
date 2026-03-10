using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouletteController : MonoBehaviour
{
    float rotSpeed = 0; //회전 속도 -> 클릭을 했을 때 시작하도록 클릭하기전에는 rotSepeed 변수 값을 0으로 설정

    void Start()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    void Update()
    {   
        //클릭하면 회전 속도를 설정한다.
        if (Input.GetMouseButtonDown(0)){
            this.rotSpeed = 10.0f;
        }

        // 회전 속도만큼 룰렛을 회전시킨다
        transform.Rotate(0, 0, this.rotSpeed);

        //룰렛을 감속시킨다
        this.rotSpeed *= 0.96f;


        float currentAngle = transform.eulerAngles.z; //Euler은 각도 0~360도를 반환함

        if ( 30.0f <= currentAngle && currentAngle < 90.0f && this.rotSpeed < 0.01f ) {
            Debug.Log("운수 대통");
        }
        else if(90.0f <= currentAngle && currentAngle <150.0f && this.rotSpeed < 0.01f){
            Debug.Log("운수 매우 나쁨");
        }
        else if(150.0f <= currentAngle && currentAngle < 210.0f && this.rotSpeed < 0.01f){
            Debug.Log("운수 보통");
        }
        else if (210.0f <= currentAngle && currentAngle < 270.0f && this.rotSpeed < 0.01f)
        {
            Debug.Log("운수 조심");
        }
        else if (270.0f <= currentAngle && currentAngle < 330.0f && this.rotSpeed < 0.01f)
        {
            Debug.Log("운수 좋음");
        }
        else if (330.0f <= currentAngle && currentAngle < 390.0f && this.rotSpeed < 0.01f)
        {
            Debug.Log("운수 나쁨");
        }
    }
}
