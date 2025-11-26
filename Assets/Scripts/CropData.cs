using UnityEngine;

// Baris ini membuat menu baru di Unity saat Klik Kanan
[CreateAssetMenu(fileName = "DataTanamanBaru", menuName = "Farming/Crop Data")]
public class CropData : ScriptableObject
{
    [Header("Info Tanaman")]
    public string namaTanaman; // Misal: "Wortel"

    [Header("Visual Pertumbuhan")]
    // Kita simpan model 3D untuk Fase 1, Fase 2, dan Siap Panen di sini
    public GameObject[] modelFase;

    [Header("Pengaturan")]
    public float waktuTumbuhPerFase = 5f; // Berapa detik untuk tumbuh ke fase berikutnya
}