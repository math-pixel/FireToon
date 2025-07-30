using UnityEngine;

[RequireComponent(typeof(Player), typeof(Rigidbody))]
public class Movement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float drag = 1f;
    
    private Player player;
    private Rigidbody rb;
    private Camera mainCamera;
    private Vector3 moveDirection;
    
    public bool IsMoving => moveDirection.magnitude > 0.01f;
    public Vector3 MoveDirection => moveDirection;
    
    public void Init()
    {
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.linearDamping = drag;
        
        mainCamera = Camera.main;
    }
    
    void Update()
    {
        if (player && player.IsDead) return;
        
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        if (mainCamera != null)
        {
            Vector3 forward = mainCamera.transform.forward * vertical;
            Vector3 right = mainCamera.transform.right * horizontal;
            moveDirection = (forward + right).normalized;
            moveDirection.y = 0f;
        }
        else
        {
            moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
        }
        
        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, 
                Quaternion.LookRotation(moveDirection), rotationSpeed * Time.deltaTime);
        }
    }
    
    void FixedUpdate()
    {
        if (player && player.IsDead) return;
        
        Vector3 targetVelocity = moveDirection * moveSpeed;
        Vector3 velocityChange = targetVelocity - rb.linearVelocity;
        velocityChange.y = 0f;
        
        rb.AddForce(velocityChange * acceleration, ForceMode.Acceleration);
    }
    
    public void AddImpulse(Vector3 force)
    {
        rb.AddForce(force, ForceMode.Impulse);
    }
}