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
    public bool canMove = true;
    
    [Header("Shoot Settings")]
    public Gun gun;
    public GameObject gunMesh;
    public float gunBackForce = 100f;
    
    // Rigidbody
    [Header("Rigidbody Settings")]
    public GameObject centerOfMass;
    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector3 moveDirection;
    private Vector3 lastMoveDirection;
    private bool fire = false;
    
    // ANIMATION
    [Header("Animation Settings")]
    public Animator animator;
    private bool walkingAnimation = false;
    private bool fireStateAnimation = false;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Optional: avoid tipping over
        
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        canMove = true;
        rb.centerOfMass = centerOfMass.transform.position;
        rb.drag = drag;
    }

    private void Update()
    {
        // Mouvement
        if (canMove)
        {
            Vector3 forward = Camera.main.transform.forward * moveInput.y;
            Vector3 right = Camera.main.transform.right * moveInput.x;
            moveDirection = forward.normalized + right.normalized;
            moveDirection.y = 0f;
        }
        
        // save last direction for gun back force
        if (moveDirection != Vector3.zero)
        {
            lastMoveDirection = moveDirection;
        }
        
        // Rotation
        if (rb.velocity.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(rb.velocity.normalized);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeedLerp);
        }

        if (fire && canMove)
        {
            gun.Shoot();
            rb.AddForce(-lastMoveDirection * gunBackForce, ForceMode.Impulse);
        }
        
        animator.SetBool("fire", fireStateAnimation);
        animator.SetBool("walk", walkingAnimation);
        resetState();
    }

    private void resetState()
    {
        fire = false;
        fireStateAnimation = false;
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
        if (moveInput.x != 0 || moveInput.y != 0)
        {
            walkingAnimation = true;
        }
        else
        {
            walkingAnimation = false;
        }
        
        // Debug.Log("Move input: " + moveInput);
    }

    public void OnFire(InputValue value)
    {
        fire = value.isPressed;
        Debug.Log("fire : " + fire);
        if (fire)
        {
            fireStateAnimation = true;
        }
        
    }

    public void OnUpdateSkin(InputValue value)
    {
        Debug.Log("Update Skin : " + value.isPressed);
        gameObject.GetComponent<SkinManager>().nextSkin();
    }

    public void removeGun()
    {
        gunMesh.SetActive(false);
    }
}