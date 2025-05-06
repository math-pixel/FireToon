using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 7f;
    public float acceleration = 20f;
    public float drag = 4f;
    public float rotationSpeedLerp = 1f;
    
    [Header("Shoot Settings")]
    public Gun gun;
    
    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector3 moveDirection;
    private bool fire = false;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Optional: avoid tipping over
    }

    private void Start()
    {
        rb.drag = drag;
    }

    private void Update()
    {
        // Mouvement
        Vector3 forward = Camera.main.transform.forward * moveInput.y;
        Vector3 right = Camera.main.transform.right * moveInput.x;
        moveDirection = forward.normalized + right.normalized;
        moveDirection.y = 0f;
        
        // Rotation
        if (rb.velocity.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(rb.velocity.normalized);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeedLerp);
        }

        if (fire)
        {
            gun.Shoot();
        }
        
        resetState();
    }

    private void resetState()
    {
        fire = false;
    }

    private void FixedUpdate()
    {
        // Apply force based on input
        Vector3 targetVelocity = moveDirection * moveSpeed;
        Vector3 velocityChange = (targetVelocity - rb.velocity);
        velocityChange.y = 0f; // Don't affect vertical movement

        rb.AddForce(velocityChange * acceleration, ForceMode.Acceleration);
    }

    // Input System callback
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        // Debug.Log("Move input: " + moveInput);
    }

    public void OnFire(InputValue value)
    {
        Debug.Log(value);
        fire = value.isPressed;
    }
}