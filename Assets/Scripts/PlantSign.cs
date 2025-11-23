using UnityEngine;
using UnityEngine.InputSystem; // Wajib untuk Input System baru

public class PlantSign : MonoBehaviour
{
    [Header("Referensi UI")]
    public GameObject interactionPanel; // Panel bertuliskan "Press E"
    public GameObject tipsPanel;        // Panel besar berisi Tips & Trick

    [Header("Pengaturan")]
    public string playerTag = "Player"; // Pastikan objek player memiliki tag ini

    // Variabel internal
    private bool isPlayerNearby = false;
    private bool isTipsOpen = false;

    void Start()
    {
        // Pastikan UI tertutup saat game mulai
        if (interactionPanel != null) interactionPanel.SetActive(false);
        if (tipsPanel != null) tipsPanel.SetActive(false);
    }

    void Update()
    {
        // Cek apakah pemain ada di dekat sign DAN menekan tombol E
        if (isPlayerNearby && Keyboard.current.eKey.wasPressedThisFrame)
        {
            ToggleTips();
        }
    }

    void ToggleTips()
    {
        isTipsOpen = !isTipsOpen; // Balik status (buka/tutup)

        if (isTipsOpen)
        {
            // Jika tips dibuka:
            tipsPanel.SetActive(true);
            interactionPanel.SetActive(false); // Sembunyikan prompt "Press E" agar bersih

            // Opsional: Munculkan cursor mouse jika ingin membaca dengan tenang
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Opsional: Pause game (Time.timeScale = 0) jika diinginkan
        }
        else
        {
            // Jika tips ditutup:
            tipsPanel.SetActive(false);
            interactionPanel.SetActive(true); // Munculkan lagi prompt "Press E"

            // Kembalikan cursor mouse
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // --- Logika Trigger (Deteksi Pemain) ---

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerNearby = true;
            // Tampilkan prompt "Press E" hanya jika tips sedang tidak terbuka
            if (!isTipsOpen)
            {
                interactionPanel.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerNearby = false;
            isTipsOpen = false; // Reset status tips

            // Sembunyikan semua panel saat pemain menjauh
            interactionPanel.SetActive(false);
            tipsPanel.SetActive(false);

            // Pastikan cursor terkunci kembali jika pemain lari menjauh saat panel terbuka
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}