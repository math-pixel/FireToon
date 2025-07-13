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

    [Header("Animations")]
    public AnimationClipData idleAnim;
    public AnimationClipData walkAnim;
    public AnimationClipData shootAnim;
    public AnimationClipData dieAnim;
    public AnimationClipData emoteYesAnim;
    public AnimationClipData emoteNoAnim;

    public bool canMove = true;

    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector3 moveDirection;
    private Vector3 lastMoveDirection;
    
    // ⭐ CORRECTION : Séparer les états de tir
    private bool firePressed = false; // Pour le tir unique
    private bool fireHeld = false;    // Pour savoir si le bouton est maintenu
    private bool walkingAnimation = false;
    private bool fireStateAnimation = false;
    
    private Camera mainCamera;
    private SimpleAnimationController animController;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        animController = GetComponentInChildren<SimpleAnimationController>();
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

        // ⭐ CORRECTION : Tirer seulement quand firePressed est true (une seule fois)
        if (firePressed && canMove)
        {
            gun.Shoot();
            float backForce = playerConfig != null ? playerConfig.gunBackForce : 100f;
            rb.AddForce(-lastMoveDirection * backForce, ForceMode.Impulse);
            firePressed = false; // ⭐ Reset immédiatement après le tir
        }

        HandleAnimations();
        ResetState();
    }

    private void HandleAnimations()
    {
        if (animController == null) return;

        // Priorité : Fire > Walk > Idle
        if (fireStateAnimation && shootAnim != null)
        {
            animController.Play(shootAnim);
        }
        else if (walkingAnimation && walkAnim != null)
        {
            animController.Play(walkAnim);
        }
        else if (idleAnim != null)
        {
            animController.Play(idleAnim);
        }
    }

    private void ResetState()
    {
        // ⭐ CORRECTION : Ne pas reset firePressed ici !
        // firePressed est reset dans Update après le tir
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
        // ⭐ CORRECTION : Gérer press et release séparément
        if (value.isPressed)
        {
            // Bouton vient d'être pressé
            firePressed = true;
            fireHeld = true;
            fireStateAnimation = true;
            Debug.Log("Fire button pressed!"); // Debug
        }
        else
        {
            // Bouton vient d'être relâché
            fireHeld = false;
            Debug.Log("Fire button released!"); // Debug
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
        animController = newAnimator != null ? newAnimator.GetComponent<SimpleAnimationController>() : null;
    }

    public bool IsFireHeld()
    {
        return fireHeld;
    }

    public bool IsFirePressed()
    {
        return firePressed;
    }

    public void ForceFire()
    {
        if (canMove)
        {
            firePressed = true;
        }
    }
}
