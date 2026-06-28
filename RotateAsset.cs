using UnityEngine;

public class RotateAsset : MonoBehaviour
{
    [Tooltip("회전 속도 조절")]
    public float rotationSpeed = 30f;

    void Update()
    {
        // 월드 기준이 아니라 오브젝트 자체의 정직한 Z축(오브젝트가 바라보는 정면 축)을 기준으로 회전
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime, Space.Self);

        // 만약 위 코드로도 이상하게 돈다면 (에셋 자체의 피벗이 꺾여있는 경우), 
        // 아래 주석(//)을 지우고 위 코드를 지워서 이 방식을 써보세요.
        // transform.localEulerAngles += new Vector3(0, 0, rotationSpeed * Time.deltaTime);
    }
}