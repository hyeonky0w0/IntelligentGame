using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DistanceDirector : MonoBehaviour
{
    GameObject player;
    GameObject yellowCat;
    GameObject distance;

    void Start()
    {
        this.player = GameObject.Find("player_0");
        this.yellowCat = GameObject.Find("cat_0");
        this.distance = GameObject.Find("Distance");
    }

    void Update()
    {
        float length = this.player.transform.position.x - this.yellowCat.transform.position.x;
        this.distance.GetComponent<TMPro.TextMeshProUGUI>().text = "Yellow cat과의 거리는 " + length.ToString("F2") + "m";
        
    }
}
