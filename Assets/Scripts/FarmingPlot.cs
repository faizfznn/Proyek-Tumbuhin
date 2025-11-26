using UnityEngine;
using UnityEngine.InputSystem; // Wajib untuk Input System baru

public class FarmingPlot : MonoBehaviour
{
    [Header("Referensi UI")]
    public GameObject plantingPanel; // Referensi ke Panel UI "Tekan F"

    [Header("Pengaturan Tanaman")]
    public GameObject cropPrefab;    // Prefab tanaman yang akan muncul (misal: Corn/Jagung)
    public Transform spawnPoint;     // Titik tepatnya tanaman akan muncul (opsional)

    // Variabel internal
    private bool isPlayerNearby = false;
    private bool hasPlanted = false; // Mengecek apakah tanah ini sudah ditanami

    void Start()
    {
        // Pastikan panel mati saat mulai
        if (plantingPanel != null)
            plantingPanel.SetActive(false);
    }

    void Update()
    {
        // Cek: Pemain dekat? + Belum ditanam? + Tombol F ditekan?
        if (isPlayerNearby && !hasPlanted && Keyboard.current.fKey.wasPressedThisFrame)
        {
            PlantCrop();
        }
    }

    void PlantCrop()
    {
        if (cropPrefab != null)
        {
            // Tentukan posisi spawn (jika spawnPoint kosong, pakai posisi objek ini)
            Vector3 position = (spawnPoint != null) ? spawnPoint.position : transform.position;

            // Munculkan tanaman
            Instantiate(cropPrefab, position, Quaternion.identity);

            hasPlanted = true; // Tandai tanah ini sudah terpakai

            // Sembunyikan panel segera setelah menanam
            if (plantingPanel != null)
                plantingPanel.SetActive(false);

            Debug.Log("Berhasil menanam!");
        }
        else
        {
            Debug.LogError("Crop Prefab belum diisi di Inspector!");
        }
    }

    // --- Logika Trigger (Deteksi Pemain) ---

    private void OnTriggerEnter(Collider other)
    {
        // Pastikan yang masuk adalah Player dan tanah belum ditanami
        if (other.CompareTag("Player") && !hasPlanted)
        {
            isPlayerNearby = true;
            if (plantingPanel != null)
                plantingPanel.SetActive(true); // Munculkan tombol
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (plantingPanel != null)
                plantingPanel.SetActive(false); // Sembunyikan tombol
        }
    }
}