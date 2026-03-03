using UnityEngine;

public class CubeCollectible : MonoBehaviour
{
    public float rotateSpeed = 90f;
    public int value = 1;
    public string playerTag = "Player";

    void Update()
    {
        // rotate cube
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }

}
