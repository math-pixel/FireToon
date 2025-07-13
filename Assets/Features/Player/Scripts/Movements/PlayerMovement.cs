using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Configuration")]
    public PlayerConfig playerConfig;
    
    [Header("References")]
    public Gun gun;
    public GameObject gunMesh;
    public GameObject centerOfMass;
    public Animator animator;
    
    public bool canMove = true;
    
    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector3 moveDirection;
    private Vector3 lastMoveDirection;
    private bool fire = false;
    private bool walkingAnimation = false;
    private bool fireStateAnimation = false;
    private Camera mainCamera;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        mainCamera = Camera.main;
        canMove = true;
        
        if (centerOfMass != null)
            rb.centerOfMass = centerOfMass.transform.position;
            
        if (playerConfig != null)
            rb.linearDamping = playerConfig.drag;
    }

    private void Update()
    {
        if (canMove)
        {
            Vector3 forward = mainCamera.transform.forward * moveInput.y;
            Vector3 right = mainCamera.transform.right * moveInput.x;
            moveDirection = forward.normalized + right.normalized;
            moveDirection.y = 0f;
        }
        
        if (moveDirection != Vector3.zero)
        {
            lastMoveDirection = moveDirection;
        }
        
        if (rb.linearVelocity.magnitude > 0.1f)
        {
            float rotationSpeed = playerConfig != null ? playerConfig.rotationSpeedLerp : 1f;
            Quaternion targetRotation = Quaternion.LookRotation(rb.linearVelocity.normalized);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        if (fire && canMove)
        {
            gun.Shoot();
            float backForce = playerConfig != null ? playerConfig.gunBackForce : 100f;
            rb.AddForce(-lastMoveDirection * backForce, ForceMode.Impulse);
        }
        
        if (animator != null)
        {
            animator.SetBool("fire", fireStateAnimation);
            animator.SetBool("walk", walkingAnimation);
        }
        
        ResetState();
    }

    private void ResetState()
    {
        fire = false;
        fireStateAnimation = false;
    }

    private void FixedUpdate()
    {
        if (playerConfig == null) return;
        
        Vector3 targetVelocity = moveDirection * playerConfig.moveSpeed;
        Vector3 velocityChange = (targetVelocity - rb.linearVelocity);
        velocityChange.y = 0f;

        rb.AddForce(velocityChange * playerConfig.acceleration, ForceMode.Acceleration);
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        walkingAnimation = moveInput.x != 0 || moveInput.y != 0;
    }

    public void OnFire(InputValue value)
    {
        fire = value.isPressed;
        if (fire)
        {
            fireStateAnimation = true;
        }
    }

    public void OnUpdateSkin(InputValue value)
    {
        if (value.isPressed)
        {
            SkinManager skinManager = GetComponent<SkinManager>();
            skinManager?.NextSkin();
        }
    }

    public void RemoveGun()
    {
        if (gunMesh != null)
            gunMesh.SetActive(false);
    }
    
    public void SetAnimator(Animator newAnimator)
    {
        animator = newAnimator;
    }
}
