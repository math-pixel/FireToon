using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private PlayerConfig playerConfig;
    
    [Header("References")]
    [SerializeField] private Gun gun;
    [SerializeField] private GameObject gunMesh;
    [SerializeField] private GameObject centerOfMass;
    [SerializeField] public Animator animator;

    [Header("Animations")]
    [SerializeField] private AnimationClipData idleAnim;
    [SerializeField] private AnimationClipData walkAnim;
    [SerializeField] private AnimationClipData shootAnim;
    [SerializeField] private AnimationClipData dieAnim;
    [SerializeField] private AnimationClipData emoteYesAnim;
    [SerializeField] private AnimationClipData emoteNoAnim;

    [Header("Movement Settings")]
    [SerializeField] private bool useVelocityBasedRotation = true;
    [SerializeField] private float minVelocityForRotation = 0.1f;

    [Header("Events")]
    public UnityEvent OnShoot;
    public UnityEvent OnStartMoving;
    public UnityEvent OnStopMoving;

    // Public properties
    public bool CanMove { get; set; } = true;
    public bool IsFireHeld { get; private set; }
    public bool IsMoving { get; private set; }

    // Private fields
    private Rigidbody rb;
    private Camera mainCamera;
    private SimpleAnimationController animController;
    
    // Input
    private Vector2 moveInput;
    private Vector3 moveDirection;
    private Vector3 lastMoveDirection;
    
    // Fire state
    private bool firePressed;
    private bool fireHeld;
    
    // Animation state
    private bool isWalkingAnimation;
    private bool isFireAnimation;
    
    // Cached values
    private float moveSpeed;
    private float acceleration;
    private float rotationSpeed;
    private float gunBackForce;

    #region Unity Lifecycle

    private void Awake()
    {
        InitializeComponents();
        CacheConfigValues();
    }

    private void Start()
    {
        InitializePhysics();
        ValidateReferences();
    }

    private void Update()
    {
        if (!CanMove) return;

        UpdateMovementDirection();
        UpdateRotation();
        HandleShooting();
        UpdateAnimations();
        ResetFrameState();
    }

    private void FixedUpdate()
    {
        if (!CanMove || playerConfig == null) return;
        
        ApplyMovement();
    }

    #endregion

    #region Initialization

    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        animController = GetComponentInChildren<SimpleAnimationController>();
        mainCamera = Camera.main;
    }

    private void InitializePhysics()
    {
        if (centerOfMass != null)
            rb.centerOfMass = centerOfMass.transform.localPosition;

        if (playerConfig != null)
            rb.linearDamping = playerConfig.drag;
    }

    private void CacheConfigValues()
    {
        if (playerConfig != null)
        {
            moveSpeed = playerConfig.moveSpeed;
            acceleration = playerConfig.acceleration;
            rotationSpeed = playerConfig.rotationSpeedLerp;
            gunBackForce = playerConfig.gunBackForce;
        }
        else
        {
            // Valeurs par défaut si pas de config
            moveSpeed = 5f;
            acceleration = 10f;
            rotationSpeed = 5f;
            gunBackForce = 100f;
        }
    }

    private void ValidateReferences()
    {
        if (mainCamera == null)
        {
            Debug.LogWarning($"[{name}] Main Camera not found. Movement will not work properly.");
        }

        if (playerConfig == null)
        {
            Debug.LogWarning($"[{name}] PlayerConfig is missing. Using default values.");
        }

        if (gun == null)
        {
            Debug.LogWarning($"[{name}] Gun reference is missing. Shooting will not work.");
        }
    }

    #endregion

    #region Movement

    private void UpdateMovementDirection()
    {
        if (mainCamera == null) return;

        Vector3 forward = mainCamera.transform.forward * moveInput.y;
        Vector3 right = mainCamera.transform.right * moveInput.x;
        
        // ✅ CORRECTION : Normaliser correctement pour éviter la vitesse diagonale supérieure
        moveDirection = (forward + right).normalized;
        moveDirection.y = 0f;

        // Mettre à jour la dernière direction de mouvement
        if (moveDirection.magnitude > 0.01f)
        {
            lastMoveDirection = moveDirection;
        }

        // Détecter le changement d'état de mouvement
        bool wasMoving = IsMoving;
        IsMoving = moveDirection.magnitude > 0.01f;

        if (IsMoving && !wasMoving)
            OnStartMoving?.Invoke();
        else if (!IsMoving && wasMoving)
            OnStopMoving?.Invoke();
    }

    private void UpdateRotation()
    {
        if (!useVelocityBasedRotation) return;

        if (rb.linearVelocity.magnitude > minVelocityForRotation)
        {
            Quaternion targetRotation = Quaternion.LookRotation(rb.linearVelocity.normalized);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void ApplyMovement()
    {
        Vector3 targetVelocity = moveDirection * moveSpeed;
        Vector3 velocityChange = targetVelocity - rb.linearVelocity;
        velocityChange.y = 0f;

        rb.AddForce(velocityChange * acceleration, ForceMode.Acceleration);
    }

    #endregion

    #region Shooting

    private void HandleShooting()
    {
        if (firePressed && gun != null)
        {
            gun.Shoot();
            ApplyGunRecoil();
            OnShoot?.Invoke();
            firePressed = false; // Reset immédiatement après le tir
        }
    }

    private void ApplyGunRecoil()
    {
        if (lastMoveDirection.magnitude > 0.01f)
        {
            rb.AddForce(-lastMoveDirection * gunBackForce, ForceMode.Impulse);
        }
    }

    public void ForceFire()
    {
        if (CanMove)
        {
            firePressed = true;
        }
    }

    #endregion

    #region Animations

    private void UpdateAnimations()
    {
        if (animController == null) return;

        // Priorité : Fire > Walk > Idle
        if (isFireAnimation && shootAnim != null)
        {
            animController.Play(shootAnim);
        }
        else if (isWalkingAnimation && walkAnim != null)
        {
            animController.Play(walkAnim);
        }
        else if (idleAnim != null)
        {
            animController.Play(idleAnim);
        }
    }

    private void ResetFrameState()
    {
        isFireAnimation = false;
    }

    #endregion

    #region Input Handlers

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        isWalkingAnimation = moveInput.magnitude > 0.01f;
    }

    public void OnFire(InputValue value)
    {
        if (value.isPressed)
        {
            firePressed = true;
            fireHeld = true;
            IsFireHeld = true;
            isFireAnimation = true;
        }
        else
        {
            fireHeld = false;
            IsFireHeld = false;
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

    #endregion

    #region Public Methods

    public void RemoveGun()
    {
        if (gunMesh != null)
            gunMesh.SetActive(false);
    }

    public void SetAnimator(Animator newAnimator)
    {
        animator = newAnimator;
        animController = newAnimator?.GetComponent<SimpleAnimationController>();
    }

    public void SetCanMove(bool canMove)
    {
        CanMove = canMove;
        if (!canMove)
        {
            // Arrêter le mouvement immédiatement
            rb.linearVelocity = Vector3.zero;
            moveInput = Vector2.zero;
            isWalkingAnimation = false;
        }
    }

    public void SetRotationMode(bool useVelocityRotation)
    {
        useVelocityBasedRotation = useVelocityRotation;
    }

    /// <summary>
    /// Force la rotation du joueur vers une direction spécifique
    /// </summary>
    public void SetRotation(Vector3 direction)
    {
        if (direction.magnitude > 0.01f)
        {
            direction.y = 0f;
            transform.rotation = Quaternion.LookRotation(direction.normalized);
        }
    }

    /// <summary>
    /// Applique une force externe au joueur (ex: explosion, knockback)
    /// </summary>
    public void ApplyExternalForce(Vector3 force, ForceMode forceMode = ForceMode.Impulse)
    {
        rb.AddForce(force, forceMode);
    }

    #endregion

    #region Debug

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            // Afficher la direction de mouvement
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, moveDirection * 2f);
            
            // Afficher la dernière direction de mouvement
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, lastMoveDirection * 1.5f);
            
            // Afficher le centre de masse
            if (centerOfMass != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.TransformPoint(rb.centerOfMass), 0.1f);
            }
        }
    }

    #endregion
}