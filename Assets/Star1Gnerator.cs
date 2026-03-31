using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star1Gnerator : MonoBehaviour
{
    public GameObject star1Prefab;
    float span = 4.0f;
    float delta = 0;

    void Start()
    {
        
    }

    void Update()
    {
        this.delta += Time.deltaTime;
        if(this.delta > this.span)
        {
            this.delta = 0;
            GameObject go = Instantiate(star1Prefab) as GameObject;
            int px = Random.Range(-6, 7);
            go.transform.position = new Vector3(px, 7, 0);
        }
    }
}
