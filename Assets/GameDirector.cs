using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour{

    GameObject car;
    GameObject flag;
    GameObject distance;

    void Start(){
        this.car = GameObject.Find("car_0");
        this.flag = GameObject.Find("flag_0");
        this.distance = GameObject.Find("Distance");
    }

    void Update()    {
        float length = this.flag.transform.position.x - this.car.transform.position.x;
        this.distance.GetComponent<TMPro.TextMeshProUGUI>().text = "목표 지점까지: " + length.ToString("F2") + "m";
        
    }
}
