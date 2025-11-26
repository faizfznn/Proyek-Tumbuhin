using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Pengaturan Raycast")]
    public float jarakAmbil = 5f;
    public LayerMask layerTanah;

    [Header("UI Indikator")]
    public GameObject panelIndikator;
    public TextMeshProUGUI teksIndikator;

    [Header("Sistem Bibit (Isi 4 Slot)")]
    public CropData[] daftarBibit;
    private int indexBibitTerpilih = 0;

    void Update()
    {
        // --- FITUR GANTI BIBIT (Tombol 1, 2, 3, 4) ---
        PilihBibitInput();

        // --- LOGIKA RAYCAST ---
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        bool melihatTanah = Physics.Raycast(ray, out hit, jarakAmbil, layerTanah);

        // Reset panel jika tidak melihat tanah, lalu return agar tidak lanjut ke bawah
        if (!melihatTanah)
        {
            if (panelIndikator.activeSelf) panelIndikator.SetActive(false);
            return;
        }

        // Cari script tanah
        TanahPertanian tanah = hit.collider.GetComponent<TanahPertanian>();

        if (tanah != null)
        {
            panelIndikator.SetActive(true);

            // Ambil data bibit yang sedang dipilih dari array
            // Pastikan array tidak kosong untuk mencegah error IndexOutRange
            CropData bibitSekarang = null;
            if (daftarBibit.Length > 0)
            {
                bibitSekarang = daftarBibit[indexBibitTerpilih];
            }

            // --- LOGIKA TEXT INDIKATOR & WARNA ---
            if (tanah.statusSaatIni == TanahPertanian.StatusTanah.Kosong)
            {
                string namaBibit = (bibitSekarang != null) ? bibitSekarang.namaTanaman : "Belum Ada Bibit";
                teksIndikator.text = "Tekan [E] untuk Tanam " + namaBibit;
                teksIndikator.color = Color.white;
            }
            else if (tanah.statusSaatIni == TanahPertanian.StatusTanah.Kering)
            {
                teksIndikator.text = "Tekan [F] untuk Menyiram";
                teksIndikator.color = Color.cyan;
            }
            else if (tanah.statusSaatIni == TanahPertanian.StatusTanah.Basah)
            {
                teksIndikator.text = "Sedang Tumbuh... (Tunggu)";
                teksIndikator.color = Color.yellow;
            }
            else if (tanah.statusSaatIni == TanahPertanian.StatusTanah.SiapPanen)
            {
                teksIndikator.text = "Tekan [E] untuk Panen!";
                teksIndikator.color = Color.green;
            }

            // --- LOGIKA INPUT INTERAKSI ---

            // Tombol E (Menanam / Panen)
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (tanah.statusSaatIni == TanahPertanian.StatusTanah.Kosong)
                {
                    if (bibitSekarang != null)
                    {
                        // PERBAIKAN ERROR 2: Menggunakan bibitSekarang, bukan bibitPilihan
                        tanah.Tanam(bibitSekarang);
                    }
                    else
                    {
                        Debug.LogWarning("Daftar Bibit Kosong atau belum dipilih!");
                    }
                }
                else if (tanah.statusSaatIni == TanahPertanian.StatusTanah.SiapPanen)
                {
                    tanah.Panen();
                }
            }

            // Tombol F (Menyiram)
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (tanah.statusSaatIni == TanahPertanian.StatusTanah.Kering)
                {
                    tanah.SiramTanaman();
                }
            }
        }
        else
        {
            // Jika melihat objek layer tanah tapi tidak ada script TanahPertanian
            if (panelIndikator.activeSelf) panelIndikator.SetActive(false);
        }
    }

    void PilihBibitInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && daftarBibit.Length > 0) indexBibitTerpilih = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2) && daftarBibit.Length > 1) indexBibitTerpilih = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3) && daftarBibit.Length > 2) indexBibitTerpilih = 2;
        if (Input.GetKeyDown(KeyCode.Alpha4) && daftarBibit.Length > 3) indexBibitTerpilih = 3;
    }
}