using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovingPlatform : MonoBehaviour
{
    [Header("Move Settings")]
    public float amplitude = 5f;  
    public float speed = 1f;      
    public Vector3 moveAxis = Vector3.right; 

    private Rigidbody rb;
    private Vector3 startPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;  
        startPos = rb.position;
        moveAxis = moveAxis.normalized;
    }

    private void FixedUpdate()
    {
        float offset = Mathf.Sin(Time.time * speed) * amplitude;
        Vector3 target = startPos + moveAxis * offset;
        
        rb.MovePosition(target);
    }
}