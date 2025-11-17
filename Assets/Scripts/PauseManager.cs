using UnityEngine;
using UnityEngine.SceneManagement; // Diperlukan untuk memuat scene

/// <summary>
/// Mengelola status Pause dan Resume game.
/// Skrip ini harus ditempatkan pada sebuah GameObject di scene 'game' (misal: GameManager).
/// </summary>
public class PauseManager : MonoBehaviour
{
    // Referensi ke panel UI yang akan ditampilkan/disembunyikan.
    // Hubungkan (drag) 'Panel Pause Menu' Anda ke slot ini di Inspector.
    public GameObject panelPauseMenu;

    // Variabel statis untuk melacak status game, bisa diakses dari skrip lain jika perlu.
    public static bool isGamePaused = false;

    void Start()
    {
        // Pastikan pada awal scene 'game',
        // panel pause disembunyikan dan game berjalan normal.
        panelPauseMenu.SetActive(false);
        Time.timeScale = 1f; // 1f = kecepatan normal
        isGamePaused = false;
    }

    void Update()
    {
        // Mendeteksi 'Keybind' tombol Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Memanggil fungsi TogglePause
            TogglePause();
        }
    }

    /// <summary>
    /// Fungsi ini akan membolak-balik status pause (pause jika sedang play, play jika sedang pause).
    /// Fungsi ini akan kita gunakan untuk 'Pause Button' dan 'Back Button'.
    /// </summary>
    public void TogglePause()
    {
        if (isGamePaused)
        {
            // Jika sedang pause, panggil ResumeGame()
            ResumeGame();
        }
        else
        {
            // Jika sedang play, panggil PauseGame()
            PauseGame();
        }
    }

    /// <summary>
    /// Menghentikan game dan menampilkan menu pause.
    /// </summary>
    void PauseGame()
    {
        panelPauseMenu.SetActive(true);
        Time.timeScale = 0f; // Menghentikan semua pergerakan dan fisika
        isGamePaused = true;

        // Opsional: Tampilkan kursor mouse agar bisa klik tombol
        // Cursor.lockState = CursorLockMode.None;
        // Cursor.visible = true;
    }

    /// <summary>
    /// Melanjutkan game dan menyembunyikan menu pause.
    /// </summary>
    void ResumeGame()
    {
        panelPauseMenu.SetActive(false);
        Time.timeScale = 1f; // Mengembalikan kecepatan game ke normal
        isGamePaused = false;

        // Opsional: Sembunyikan kembali kursor mouse
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    /// <summary>
    /// Fungsi untuk 'Quit Button'.
    /// Akan kembali ke scene 'main menu'.
    /// </summary>
    public void QuitToMainMenu()
    {
        // PENTING: Selalu set Time.timeScale kembali ke 1 sebelum pindah scene!
        // Jika tidak, scene 'main menu' mungkin akan ikut ter-pause.
        Time.timeScale = 1f;
        isGamePaused = false;

        // Pastikan nama scene "main menu" sudah benar (case sensitive)
        SceneManager.LoadScene("main menu");
    }
}