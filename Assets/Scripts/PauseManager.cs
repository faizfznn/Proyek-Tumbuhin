using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Referensi UI")]
    public GameObject pausePanel;

    [Header("Referensi Pemain")]
    public FPSController fpsController;

    // --- BARIS BARU 1 ---
    [Header("Referensi Audio")]
    public AudioSource gameMusicSource; // Slot untuk AudioSource di scene 'game'

    // Variabel internal
    private PlayerControls playerControls;
    private InputAction pauseAction;
    private bool isPaused = false;

    void Awake()
    {
        if (fpsController == null)
        {
            Debug.LogError("Referensi FPSController belum di-set di PauseManager!");
            return;
        }

        // --- BLOK BARU (Otomatisasi) ---
        // Coba ambil AudioSource secara otomatis jika belum di-set
        if (gameMusicSource == null)
        {
            gameMusicSource = GetComponent<AudioSource>();
        }
        if (gameMusicSource == null)
        {
            Debug.LogWarning("Referensi AudioSource (Game Music Source) belum di-set di PauseManager.");
        }
        // --- END BLOK BARU ---

        playerControls = new PlayerControls();
        pauseAction = playerControls.Player.Pause;

        if (pauseAction == null)
        {
            Debug.LogError("Action 'Pause' tidak ditemukan!");
        }
    }

    void Start()
    {
        ResumeGame();
    }

    void OnEnable()
    {
        if (pauseAction != null)
        {
            playerControls.Player.Enable();
            pauseAction.performed += OnPausePerformed;
        }
    }

    void OnDisable()
    {
        if (pauseAction != null)
        {
            pauseAction.performed -= OnPausePerformed;
            playerControls.Player.Disable();
        }
    }

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        fpsController.enabled = false;

        // --- BARIS BARU 2 ---
        // Jeda musiknya
        if (gameMusicSource != null)
        {
            gameMusicSource.Pause();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        fpsController.enabled = true;

        // --- BARIS BARU 3 ---
        // Lanjutkan lagi musiknya
        if (gameMusicSource != null && Time.timeScale > 0) // Pastikan game benar-benar resume
        {
            gameMusicSource.UnPause();
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // --- Fungsi untuk Tombol (Button) ---

    public void OnResumeButtonPressed()
    {
        ResumeGame();
    }

    public void OnQuitToMenuPressed()
    {
        Time.timeScale = 1f;
        // Kita tidak perlu mematikan musik di sini.
        // Saat scene "main menu" dimuat, AudioSource ini akan hancur
        // dan AudioSource di "main menu" akan otomatis 'Play On Awake'.
        SceneManager.LoadScene("main menu"); // (Ganti ini jika menggunakan Scene Transition)
    }
}