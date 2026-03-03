using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Speed Settings")]
    public float minSpeed = 5f;
    public float maxSpeed = 30f;
    public float acceleration = 15f;
    private float currentForwardSpeed;

    [Header("Lateral Movement")]
    public float sideSpeed = 15f;
    public float roadWidth = 4.5f;

    [Header("Mobile Drag Settings")]
    public bool enableMobileControls = true;
    public float dragSensitivity = 3.0f; // steering responsiveness

    [Header("Game State")]
    public int score = 0;
    public GameManager gameManager;

    private Rigidbody rb;
    private bool isGameStopped = false;

    // Drag state
    private int activeFingerId = -1;
    private Vector2 touchStartPos;
    private float mobileHorizontal = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Time.timeScale = 1f;
        currentForwardSpeed = minSpeed;
    }

    void Update()
    {
        if (isGameStopped) return;

        float speedInput = 0f;

        // Desktop speed control
        if (Input.GetKey(KeyCode.W)) speedInput += 1f;
        if (Input.GetKey(KeyCode.S)) speedInput -= 1f;

        // Mobile drag steering
        if (enableMobileControls && Input.touchCount > 0)
        {
            HandleTouch();
        }

        if (Mathf.Abs(speedInput) > 0.01f)
        {
            currentForwardSpeed += speedInput * acceleration * Time.deltaTime;
        }

        currentForwardSpeed = Mathf.Clamp(currentForwardSpeed, minSpeed, maxSpeed);
    }

    void FixedUpdate()
    {
        if (isGameStopped) return;

        float horizontalInput = Input.GetAxis("Horizontal");

        if (enableMobileControls)
        {
            horizontalInput = mobileHorizontal;
        }

        Vector3 v = rb.linearVelocity;
        v.z = currentForwardSpeed;
        v.x = horizontalInput * sideSpeed;
        rb.linearVelocity = v;

        Vector3 p = rb.position;
        p.x = Mathf.Clamp(p.x, -roadWidth, roadWidth);
        rb.position = p;
    }

    private void HandleTouch()
    {
        Touch t = Input.GetTouch(0);

        if (t.phase == TouchPhase.Began)
        {
            activeFingerId = t.fingerId;
            touchStartPos = t.position;
        }
        else if (t.phase == TouchPhase.Moved && t.fingerId == activeFingerId)
        {
            float deltaX = t.position.x - touchStartPos.x;

            // Normalize to screen width
            float normalized = (deltaX / Screen.width) * dragSensitivity * 5f;
            mobileHorizontal = Mathf.Clamp(normalized, -1f, 1f);
        }
        else if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
        {
            mobileHorizontal = 0f;
            activeFingerId = -1;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacles"))
        {
            StopMovement();
            gameManager?.EndGame();
        }
    }

    private void StopMovement()
    {
        isGameStopped = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}