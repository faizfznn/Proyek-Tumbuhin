using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Pengaturan Raycast")]
    public float jarakAmbil = 5f;
    public LayerMask layerTanah;

    // [Header("UI Indikator Lama")]  <-- Hapus atau Comment UI lama Anda
    // public GameObject panelIndikator;
    // public TextMeshProUGUI teksIndikator;

    [Header("UI Sistem Baru")]
    public HotbarUI hotbarUI; // Masukkan script HotbarUI di sini

    [Header("Sistem Bibit")]
    public CropData[] daftarBibit;
    private int indexBibitTerpilih = 0;

    [Header("Animasi & Gerak")]
    public Animator playerAnimator;
    public float delayAnimasi = 0.5f;
    private bool sedangBerinteraksi = false;
    public MonoBehaviour scriptGerakKarakter;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // --- [BARU] Kirim data gambar ke Hotbar saat mulai ---
        if (hotbarUI != null)
        {
            hotbarUI.InisialisasiSlot(daftarBibit);
            hotbarUI.UpdateSeleksiSlot(indexBibitTerpilih);
        }
    }

    void Update()
    {
        if (sedangBerinteraksi) return;

        // --- 1. INPUT GANTI BIBIT ---
        int indexLama = indexBibitTerpilih;
        PilihBibitInput();

        // Jika index berubah, update UI Slot
        if (indexLama != indexBibitTerpilih && hotbarUI != null)
        {
            hotbarUI.UpdateSeleksiSlot(indexBibitTerpilih);
        }

        // --- 2. DETEKSI TANAH ---
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        bool melihatTanah = Physics.Raycast(ray, out hit, jarakAmbil, layerTanah);

        // Variabel Status untuk UI
        bool bisaTanam = false;
        bool bisaPanen = false;
        bool bisaSiram = false;
        TanahPertanian tanahTarget = null;

        if (melihatTanah)
        {
            tanahTarget = hit.collider.GetComponent<TanahPertanian>();

            if (tanahTarget != null)
            {
                // Cek Status Tanah untuk menentukan tombol mana yang aktif
                if (tanahTarget.statusSaatIni == TanahPertanian.StatusTanah.Kosong)
                {
                    bisaTanam = true;
                }
                else if (tanahTarget.statusSaatIni == TanahPertanian.StatusTanah.SiapPanen)
                {
                    bisaPanen = true;
                }

                if (tanahTarget.statusSaatIni == TanahPertanian.StatusTanah.Kering)
                {
                    bisaSiram = true;
                }
            }
        }

        // --- 3. KIRIM STATUS KE HOTBAR UI ---
        if (hotbarUI != null)
        {
            hotbarUI.UpdateVisualAksi(bisaTanam, bisaPanen, bisaSiram);
        }

        // --- 4. EKSEKUSI INPUT ---
        if (tanahTarget != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (bisaTanam && daftarBibit.Length > 0)
                    StartCoroutine(ProsesInteraksi(tanahTarget, "Tekan E Untuk Tanam"));

                else if (bisaPanen)
                    StartCoroutine(ProsesInteraksi(tanahTarget, "Tekan E Untuk Panen"));
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                if (bisaSiram)
                    StartCoroutine(ProsesInteraksi(tanahTarget, "Tekan F Untuk Siram"));
            }
        }
    }

    // ... (Fungsi Coroutine ProsesInteraksi TETAP SAMA, jangan dihapus) ...
    // ... (Copy Paste fungsi IEnumerator ProsesInteraksi dari kode sebelumnya ke sini) ...

    IEnumerator ProsesInteraksi(TanahPertanian tanah, string aksi)
    {
        sedangBerinteraksi = true;

        if (scriptGerakKarakter != null) scriptGerakKarakter.enabled = false;
        if (rb != null) rb.linearVelocity = Vector3.zero;

        if (playerAnimator != null) playerAnimator.SetTrigger("Interact");

        yield return new WaitForSeconds(delayAnimasi);

        if (aksi == "Tekan E Untuk Tanam") tanah.Tanam(daftarBibit[indexBibitTerpilih]);
        else if (aksi == "Tekan E Untuk Panen") tanah.Panen();
        else if (aksi == "Tekan F Untuk Siram") tanah.SiramTanaman();

        yield return new WaitForSeconds(0.5f);

        if (scriptGerakKarakter != null) scriptGerakKarakter.enabled = true;
        sedangBerinteraksi = false;
    }

    void PilihBibitInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && daftarBibit.Length > 0) indexBibitTerpilih = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2) && daftarBibit.Length > 1) indexBibitTerpilih = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3) && daftarBibit.Length > 2) indexBibitTerpilih = 2;
        if (Input.GetKeyDown(KeyCode.Alpha4) && daftarBibit.Length > 3) indexBibitTerpilih = 3;
    }
}