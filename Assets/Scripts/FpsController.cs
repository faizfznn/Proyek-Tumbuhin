using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;

    // --- NILAI DISESUAIKAN DENGAN KODE LAMA ---
    public float jumpPower = 12f; // <-- Diubah dari 4f menjadi 7f
    public float gravity = 30f; // <-- Diubah dari 30f menjadi 10f
    // ----------------------------------------

    // --- PENTING: Nilai ini HARUS tetap kecil ---
    public float lookSpeed = 0.2f;
    // JANGAN UBAH MENJADI 2f. Penjelasan di bawah.
    // ----------------------------------------

    public float lookXLimit = 45f;


    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    public bool canMove = true;


    CharacterController characterController;

    // --- Variabel untuk Input System ---
    private PlayerControls playerControls;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool jumpPressed;
    private bool isRunning;
    // ------------------------------------------

    void Awake()
    {
        characterController = GetComponent<CharacterController>();

        // --- Inisialisasi Input System ---
        playerControls = new PlayerControls();
        // ------------------------------------------

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // --- Tambahkan OnEnable dan OnDisable ---
    private void OnEnable()
    {
        // "Player" adalah nama Action Map yang kita buat
        playerControls.Player.Enable();
    }

    private void OnDisable()
    {
        playerControls.Player.Disable();
    }
    // ------------------------------------------

    void Update()
    {
        // --- Baca input dari PlayerControls ---
        moveInput = playerControls.Player.Move.ReadValue<Vector2>();
        lookInput = playerControls.Player.Look.ReadValue<Vector2>();
        jumpPressed = playerControls.Player.Jump.WasPressedThisFrame();
        isRunning = playerControls.Player.Run.IsPressed();
        // ------------------------------------------


        #region Handles Movment
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Gunakan input yang sudah dibaca
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * moveInput.y : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * moveInput.x : 0;

        // Simpan y-velocity dari frame sebelumnya
        float movementDirectionY = moveDirection.y;

        // Hitung gerakan X dan Z
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        #endregion

        #region Handles Jumping
        // Ganti Input.GetButton("Jump") dengan 'jumpPressed'
        if (jumpPressed && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            // Kembalikan y-velocity dari frame sebelumnya
            moveDirection.y = movementDirectionY;
        }

        // Terapkan gravitasi jika tidak di darat
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        #endregion

        #region Handles Rotation
        if (canMove)
        {
            // Ganti Input.GetAxis("Mouse Y") dan "Mouse X"
            rotationX += -lookInput.y * lookSpeed; // Mouse Y
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, lookInput.x * lookSpeed, 0); // Mouse X
        }
        #endregion

        #region Apply Movement
        // --- PERBAIKAN: Pindahkan Move() ke akhir Update ---
        // Menerapkan semua gerakan (horizontal, lompat, gravitasi) ke controller
        characterController.Move(moveDirection * Time.deltaTime);
        #endregion
    }
}