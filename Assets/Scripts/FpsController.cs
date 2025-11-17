using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [Header("Player Settings")]
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f; // <-- Ini akan dikirim ke Animator saat lari
    public float jumpPower = 12f;
    public float gravity = 30f;

    [Header("Look Settings")]
    public float lookSpeed = 0.2f;
    public float lookXLimit = 45f;

    [Header("Animation Settings")]
    public Animator anim; 

    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;
    public bool canMove = true;

    CharacterController characterController;
    private PlayerControls playerControls;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool jumpPressed;
    private bool isRunning;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerControls = new PlayerControls();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (anim == null)
        {
            anim = GetComponentInChildren<Animator>();
        }
    }

    private void OnEnable()
    {
        playerControls.Player.Enable();
    }

    private void OnDisable()
    {
        playerControls.Player.Disable();
    }

    void Update()
    {
        // 1. BACA INPUT
        moveInput = playerControls.Player.Move.ReadValue<Vector2>();
        lookInput = playerControls.Player.Look.ReadValue<Vector2>();
        jumpPressed = playerControls.Player.Jump.WasPressedThisFrame();
        isRunning = playerControls.Player.Run.IsPressed();

        #region Handles Movement
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Logika kecepatan: Jika lari pakai runSpeed, jika tidak pakai walkSpeed
        float currentSpeedValue = canMove ? (isRunning ? runSpeed : walkSpeed) : 0;
        
        float curSpeedX = currentSpeedValue * moveInput.y;
        float curSpeedY = currentSpeedValue * moveInput.x;
        float movementDirectionY = moveDirection.y;
        
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        #endregion

        #region Handles Jumping
        if (jumpPressed && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
            if (anim != null) anim.SetTrigger("Jump");
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        #endregion

        #region Handles Rotation
        if (canMove)
        {
            rotationX += -lookInput.y * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, lookInput.x * lookSpeed, 0);
        }
        #endregion

        #region Apply Movement
        characterController.Move(moveDirection * Time.deltaTime);
        #endregion

        // =================================================================
        // LOGIKA ANIMASI "MoveSpeed" DENGAN THRESHOLD
        // =================================================================
        if (anim != null)
        {
            // 1. Tetap kirim parameter pendukung (opsional, biar aman)
            anim.SetFloat("MotionSpeed", 1f);
            anim.SetBool("Grounded", characterController.isGrounded);
            anim.SetBool("FreeFall", !characterController.isGrounded);

            // 2. Kirim nilai "MoveSpeed"
            // Logic: 
            // - Jika Diam -> kirim 0
            // - Jika Jalan -> kirim 6 (nilai walkSpeed)
            // - Jika Lari -> kirim 12 (nilai runSpeed)
            
            float animSpeed = 0f;
            
            if (moveInput.magnitude > 0.1f) // Jika ada tombol ditekan
            {
                if (isRunning) 
                {
                    animSpeed = runSpeed; // Kirim angka 12
                }
                else 
                {
                    animSpeed = walkSpeed; // Kirim angka 6
                }
            }

            // Kirim ke Animator dengan nama "MoveSpeed" sesuai gambar Anda
            // Menggunakan 0.1f sebagai 'dampTime' agar perubahan angkanya halus (tidak kaget)
            anim.SetFloat("MoveSpeed", animSpeed, 0.1f, Time.deltaTime);
        }
    }
}