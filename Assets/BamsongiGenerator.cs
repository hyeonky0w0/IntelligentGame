using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BamsongiGenerator : MonoBehaviour
{
    public GameObject bamsongiPrefab;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject bamsongi = Instantiate(bamsongiPrefab) as GameObject;
            bamsongi.GetComponent<BamsongiController>().Shoot(new Vector3(0, 200, 2000));
        }
    }
}
