using UnityEngine;

public class ObjectOscillator : MonoBehaviour
{
    public Vector3 moveDirection = Vector3.up;
    public float speed = 2.0f;
    public float amplitude = 2.0f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {

        float offset = Mathf.Sin(Time.time * speed) * amplitude;
        transform.position = startPosition + (moveDirection.normalized * offset);
    }
}