using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDirector2 : MonoBehaviour{

    GameObject car;
    GameObject flag;
    GameObject distance;

    void Start(){
        this.car = GameObject.Find("Green_car_0");
        this.flag = GameObject.Find("Green_flag_0");
        this.distance = GameObject.Find("Distance2");
    }

    void Update()    {
        float length = this.car.transform.position.y - this.flag.transform.position.y;
        this.distance.GetComponent<TMPro.TextMeshProUGUI>().text = "목표 지점까지: " + length.ToString("F2") + "m";
        
    }
}
