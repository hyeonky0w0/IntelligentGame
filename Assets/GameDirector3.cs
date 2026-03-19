using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDirector3 : MonoBehaviour{

    GameObject car;
    GameObject flag;
    GameObject distance;

    void Start(){
        this.flag = GameObject.Find("flag_0");
        this.distance = GameObject.Find("Distance3");
    }

    void Update(){
        int count = this.flag.GetComponent<flagController>().rorationCount;
        this.distance.GetComponent<TMPro.TextMeshProUGUI>().text = "깃발 회전 횟수: " + count.ToString() + "회";
        
    }
}
