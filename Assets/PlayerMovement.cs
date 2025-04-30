	using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
      [Header("Movement Settings")]
         public float moveSpeed = 7f;
         public float acceleration = 20f;
         public float drag = 4f;
     
         private Rigidbody rb;
         private Vector2 moveInput;
         private Vector3 moveDirection;
     
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
             // Convert input to world direction
             // Debug.Log("Move input: " + moveInput);
             moveDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
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
         }
}
