using UnityEngine;

public class Star2Controller : MonoBehaviour
{
    bool isFalling = true; //현재 내려가는 중인지 확인
    void Start()
    {
        
    }

    void Update()
    {

        if (isFalling)
        {
            transform.Translate(0, -0.1f, 0);

            if (transform.position.y <= -5.0f)
            {
                isFalling = false;
            }
        }
        else
        {
            transform.Translate(0, 0.1f, 0);
            if (transform.position.y >= 3.8f)
            {
                isFalling = true;
            }
        }
    }
}
