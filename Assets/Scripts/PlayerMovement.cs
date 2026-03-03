using UnityEngine;
using UnityEngine.InputSystem;

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
    public float dragSensitivity = 3.0f; // steering responsiveness (tune)

    [Header("Game State")]
    public int score = 0;
    public GameManager gameManager;

    private Rigidbody rb;
    private bool isGameStopped = false;

    // Drag state
    private bool dragging = false;
    private Vector2 dragStartPos; // screen-space
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

        // Keyboard speed control (Editor / Desktop)
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) speedInput += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) speedInput -= 1f;
        }

        // Touch / Mouse drag handling updates mobileHorizontal
        if (enableMobileControls)
            HandlePointerDrag();

        // Apply speed input
        if (Mathf.Abs(speedInput) > 0.01f)
        {
            currentForwardSpeed += speedInput * acceleration * Time.deltaTime;
        }

        currentForwardSpeed = Mathf.Clamp(currentForwardSpeed, minSpeed, maxSpeed);
    }

    void FixedUpdate()
    {
        if (isGameStopped) return;

        float horizontalInput = 0f;

        // Keyboard horizontal (desktop)
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) horizontalInput = -1f;
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) horizontalInput = 1f;
        }

        // Touch/mouse drag overrides keyboard when active
        if (enableMobileControls)
            horizontalInput = mobileHorizontal;

        Vector3 v = rb.linearVelocity;
        v.z = currentForwardSpeed;
        v.x = horizontalInput * sideSpeed;
        rb.linearVelocity = v;

        Vector3 p = rb.position;
        p.x = Mathf.Clamp(p.x, -roadWidth, roadWidth);
        rb.position = p;
    }

    private void HandlePointerDrag()
    {
        // Mouse (Editor) - support left button drag
        if (Mouse.current != null && Mouse.current.leftButton != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                dragging = true;
                dragStartPos = Mouse.current.position.ReadValue();
            }
            else if (Mouse.current.leftButton.isPressed && dragging)
            {
                Vector2 pos = Mouse.current.position.ReadValue();
                Vector2 delta = pos - dragStartPos;
                float normalized = (delta.x / Screen.width) * dragSensitivity * 3f;
                mobileHorizontal = Mathf.Clamp(normalized, -1f, 1f);
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                dragging = false;
                mobileHorizontal = 0f;
            }
        }

        // Touchscreen (Mobile)
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch != null)
        {
            var touch = Touchscreen.current.primaryTouch;
            if (touch.press.wasPressedThisFrame)
            {
                dragging = true;
                dragStartPos = touch.position.ReadValue();
            }
            else if (touch.press.isPressed && dragging)
            {
                Vector2 pos = touch.position.ReadValue();
                Vector2 delta = pos - dragStartPos;
                float normalized = (delta.x / Screen.width) * dragSensitivity * 3f;
                mobileHorizontal = Mathf.Clamp(normalized, -1f, 1f);
            }
            else if (touch.press.wasReleasedThisFrame)
            {
                dragging = false;
                mobileHorizontal = 0f;
            }
        }

        // Accelerometer fallback (optional) - uncomment if you want tilt steering
        // if (!dragging && enableMobileControls)
        // {
        //     mobileHorizontal = Mathf.Clamp(Input.acceleration.x * 2f, -1f, 1f);
        // }
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