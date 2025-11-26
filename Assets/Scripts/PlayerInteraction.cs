using UnityEngine;
using TMPro;
using System.Collections;

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

    [Header("Animasi & Gerak")]
    public Animator playerAnimator;
    public float delayAnimasi = 0.5f;
    private bool sedangBerinteraksi = false;

    // --- [BARU] Variabel untuk Script Gerak ---
    // Masukkan script yang buat karakter jalan di sini (misal: SimpleSampleCharacterControl)
    public MonoBehaviour scriptGerakKarakter;

    // --- [BARU] Variabel Rigidbody (Otomatis diambil) ---
    private Rigidbody rb;

    void Start()
    {
        // Cari rigidbody di object ini
        rb = GetComponent<Rigidbody>();
    }

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
            UpdateTeksIndikator(tanah);

            // --- INPUT INTERAKSI ---
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (tanah.statusSaatIni == TanahPertanian.StatusTanah.Kosong)
                {
                    if (daftarBibit.Length > 0)
                    {
                        StartCoroutine(ProsesInteraksi(tanah, "Tanam"));
                    }
                }
                else if (tanah.statusSaatIni == TanahPertanian.StatusTanah.SiapPanen)
                {
                    StartCoroutine(ProsesInteraksi(tanah, "Panen"));
                }
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                if (tanah.statusSaatIni == TanahPertanian.StatusTanah.Kering)
                {
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
        sedangBerinteraksi = true; // Kunci input interaksi

        // --- [BARU] Matikan Pergerakan Karakter ---
        if (scriptGerakKarakter != null)
        {
            scriptGerakKarakter.enabled = false; // Matikan script jalan
        }

        // --- [BARU] Hentikan Sisa Kecepatan Fisika (Biar gak meluncur) ---
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // 1. Putar Animasi
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Interact");
        }

        // 2. Tunggu sampai karakter membungkuk
        yield return new WaitForSeconds(delayAnimasi);

        // 3. Eksekusi Logika Game 
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

        // 4. Tunggu sisa animasi selesai (opsional)
        yield return new WaitForSeconds(0.5f); // Tambah sedikit waktu biar animasi berdiri selesai

        // --- [BARU] Nyalakan Kembali Pergerakan ---
        if (scriptGerakKarakter != null)
        {
            scriptGerakKarakter.enabled = true;
        }

        sedangBerinteraksi = false; // Buka kunci input
    }

    void UpdateTeksIndikator(TanahPertanian tanah)
    {
        // (Isi fungsi ini tetap sama seperti kode Anda sebelumnya)
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
        // (Isi fungsi ini tetap sama)
        if (Input.GetKeyDown(KeyCode.Alpha1) && daftarBibit.Length > 0) indexBibitTerpilih = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2) && daftarBibit.Length > 1) indexBibitTerpilih = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3) && daftarBibit.Length > 2) indexBibitTerpilih = 2;
        if (Input.GetKeyDown(KeyCode.Alpha4) && daftarBibit.Length > 3) indexBibitTerpilih = 3;
    }
}