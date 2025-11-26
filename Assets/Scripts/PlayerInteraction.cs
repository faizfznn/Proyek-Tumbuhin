using UnityEngine;
using TMPro;
using System.Collections; // Wajib untuk Coroutine

public class PlayerInteraction : MonoBehaviour
{
    [Header("Pengaturan Raycast")]
    public float jarakAmbil = 5f;
    public LayerMask layerTanah;

    [Header("UI Indikator")]
    public GameObject panelIndikator;
    public TextMeshProUGUI teksIndikator;

    [Header("Sistem Bibit")]
    public CropData[] daftarBibit;
    private int indexBibitTerpilih = 0;

    [Header("Animasi")]
    public Animator playerAnimator; // Masukkan Animator karakter di sini
    public float delayAnimasi = 0.5f; // Waktu tunggu sampai tangan menyentuh tanah
    private bool sedangBerinteraksi = false; // Mencegah spam tombol

    void Update()
    {
        // Jangan proses input jika sedang animasi
        if (sedangBerinteraksi) return;

        PilihBibitInput();

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        bool melihatTanah = Physics.Raycast(ray, out hit, jarakAmbil, layerTanah);

        if (!melihatTanah)
        {
            if (panelIndikator.activeSelf) panelIndikator.SetActive(false);
            return;
        }

        TanahPertanian tanah = hit.collider.GetComponent<TanahPertanian>();

        if (tanah != null)
        {
            panelIndikator.SetActive(true);
            UpdateTeksIndikator(tanah); // Fungsi helper biar rapi (lihat bawah)

            // --- INPUT INTERAKSI ---
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (tanah.statusSaatIni == TanahPertanian.StatusTanah.Kosong)
                {
                    if (daftarBibit.Length > 0)
                    {
                        // Mulai urutan animasi menanam
                        StartCoroutine(ProsesInteraksi(tanah, "Tanam"));
                    }
                }
                else if (tanah.statusSaatIni == TanahPertanian.StatusTanah.SiapPanen)
                {
                    // Mulai urutan animasi panen
                    StartCoroutine(ProsesInteraksi(tanah, "Panen"));
                }
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                if (tanah.statusSaatIni == TanahPertanian.StatusTanah.Kering)
                {
                    // Mulai urutan animasi menyiram
                    StartCoroutine(ProsesInteraksi(tanah, "Siram"));
                }
            }
        }
        else
        {
            if (panelIndikator.activeSelf) panelIndikator.SetActive(false);
        }
    }

    // --- COROUTINE UNTUK ANIMASI ---
    IEnumerator ProsesInteraksi(TanahPertanian tanah, string aksi)
    {
        sedangBerinteraksi = true; // Kunci input

        // 1. Putar Animasi
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Interact");
        }

        // 2. Tunggu sampai karakter membungkuk (misal 0.5 detik)
        yield return new WaitForSeconds(delayAnimasi);

        // 3. Eksekusi Logika Game (Munculkan tanaman/air)
        if (aksi == "Tanam")
        {
            tanah.Tanam(daftarBibit[indexBibitTerpilih]);
        }
        else if (aksi == "Panen")
        {
            tanah.Panen();
        }
        else if (aksi == "Siram")
        {
            tanah.SiramTanaman();
        }

        // 4. Tunggu sisa animasi selesai (opsional, biar tidak langsung lari)
        yield return new WaitForSeconds(0.2f);

        sedangBerinteraksi = false; // Buka kunci input
    }

    // Helper untuk merapikan teks
    void UpdateTeksIndikator(TanahPertanian tanah)
    {
        CropData bibitSekarang = (daftarBibit.Length > 0) ? daftarBibit[indexBibitTerpilih] : null;

        if (tanah.statusSaatIni == TanahPertanian.StatusTanah.Kosong)
        {
            string nama = (bibitSekarang != null) ? bibitSekarang.namaTanaman : "...";
            teksIndikator.text = "Tekan [E] untuk Tanam " + nama;
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
    }

    void PilihBibitInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && daftarBibit.Length > 0) indexBibitTerpilih = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2) && daftarBibit.Length > 1) indexBibitTerpilih = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3) && daftarBibit.Length > 2) indexBibitTerpilih = 2;
        if (Input.GetKeyDown(KeyCode.Alpha4) && daftarBibit.Length > 3) indexBibitTerpilih = 3;
    }
}