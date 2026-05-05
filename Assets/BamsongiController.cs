using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BamsongiController : MonoBehaviour
{

    public void Shoot(Vector3 dir)
    {
        GetComponent<Rigidbody>().AddForce(dir);
    }

    void Start()
    {
        //Shoot(new Vector3(0, 200, 2000));
    }

    void OnCollisionEnter(Collision other)
    {
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<ParticleSystem>().Play();
    }

    void Update()
    {

    }
}
