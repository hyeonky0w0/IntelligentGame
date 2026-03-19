using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenCarController : MonoBehaviour
{
    float speed = 0; //자동차 스피드 초기화
    Vector2 startPos; //자동차 초기 위치

    void Start()
    {
        
    }

    void Update() {
        if (Input.GetMouseButtonDown(1)) { //마우스를 클릭하면
            this.startPos = Input.mousePosition;
        }
        else if(Input.GetMouseButtonUp(1)){
            Vector2 endPos = Input.mousePosition;
            float swipeLength = (endPos.y - this.startPos.y);

            this.speed = swipeLength / 500.0f;
        }

        transform.Translate(this.speed, 0, 0); //이동 
        this.speed *= 0.98f; //감속
        
    }
}
