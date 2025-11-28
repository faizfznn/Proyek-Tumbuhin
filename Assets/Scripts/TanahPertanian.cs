using UnityEngine;
using System.Collections; // Wajib untuk fitur animasi Coroutine

public class TanahPertanian : MonoBehaviour
{
    // Status tanah
    public enum StatusTanah { Kosong, Kering, Basah, SiapPanen }

    [Header("Info Status")]
    public StatusTanah statusSaatIni = StatusTanah.Kosong;

    [Header("Visual Tanah")]
    // [HideInInspector] // Aktifkan jika ingin menyembunyikan dari Inspector
    public MeshRenderer rendererTanah;
    public Color warnaKering = Color.white;
    public Color warnaBasah = new Color(0.4f, 0.2f, 0f);

    [Header("Pengaturan")]
    public Transform spawnPoint;

    [Header("Efek Visual (Partikel)")]
    // --- Slot untuk Prefab Partikel ---
    public GameObject efekDebuPrefab; // Masukkan Particle System Debu di sini
    public GameObject efekAirPrefab;  // Masukkan Particle System Air di sini

    // Variabel Internal
    private GameObject modelTanamanSaatIni;
    private CropData dataTanaman;
    private int faseSekarang = 0;
    private float timerPertumbuhan = 0f;

    void Awake()
    {
        // --- AUTO DETECT RENDERER ---
        rendererTanah = GetComponent<MeshRenderer>();
        if (rendererTanah == null) rendererTanah = GetComponentInChildren<MeshRenderer>();

        // Buat instance material agar warna unik per petak
        if (rendererTanah != null) { Material unik = rendererTanah.material; }
    }

    void Start()
    {
        UbahWarnaTanah(warnaKering);
    }

    void Update()
    {
        // Logika Timer: Hanya jalan jika tanah BASAH dan belum panen
        if (statusSaatIni == StatusTanah.Basah)
        {
            timerPertumbuhan += Time.deltaTime;

            if (timerPertumbuhan >= dataTanaman.waktuTumbuhPerFase)
            {
                TumbuhSatuLevel();
            }
        }
    }

    // --- FUNGSI UTAMA PARTIKEL (DIPERTAHANKAN) ---
    void MunculkanPartikel(GameObject prefabPartikel)
    {
        if (prefabPartikel != null)
        {
            // 1. Munculkan partikel
            GameObject partikelBaru = Instantiate(prefabPartikel, spawnPoint.position, Quaternion.identity);

            // 2. [PENTING] Hancurkan objek partikel setelah 2 detik
            // Ini mencegah partikel menumpuk di Hierarchy dan memastikan air berhenti total
            Destroy(partikelBaru, 2f);
        }
    }

    public void Tanam(CropData bibitBaru)
    {
        if (statusSaatIni == StatusTanah.Kosong)
        {
            dataTanaman = bibitBaru;
            faseSekarang = 0;
            timerPertumbuhan = 0f;

            // Set status awal: Kering
            statusSaatIni = StatusTanah.Kering;
            UbahWarnaTanah(warnaKering);

            // --- Munculkan Efek Debu saat menanam ---
            MunculkanPartikel(efekDebuPrefab);

            UpdateTampilanVisual();
        }
    }

    public void SiramTanaman()
    {
        if (statusSaatIni == StatusTanah.Kering)
        {
            statusSaatIni = StatusTanah.Basah;
            UbahWarnaTanah(warnaBasah);

            // --- Munculkan Efek Air saat menyiram ---
            MunculkanPartikel(efekAirPrefab);
        }
    }

    void TumbuhSatuLevel()
    {
        faseSekarang++;
        timerPertumbuhan = 0f;
        UpdateTampilanVisual();

        // --- Munculkan Efek Debu kecil saat tumbuh (Opsional) ---
        MunculkanPartikel(efekDebuPrefab);

        if (faseSekarang >= dataTanaman.modelFase.Length - 1)
        {
            statusSaatIni = StatusTanah.SiapPanen;
            UbahWarnaTanah(warnaKering); // Balik kering saat siap panen
        }
        else
        {
            statusSaatIni = StatusTanah.Kering; // Balik kering, minta disiram lagi
            UbahWarnaTanah(warnaKering);
        }
    }

    void UpdateTampilanVisual()
    {
        // Hapus model lama
        if (modelTanamanSaatIni != null) Destroy(modelTanamanSaatIni);

        // Munculkan model baru sesuai fase
        if (faseSekarang < dataTanaman.modelFase.Length)
        {
            GameObject modelBaru = dataTanaman.modelFase[faseSekarang];
            modelTanamanSaatIni = Instantiate(modelBaru, spawnPoint.position, Quaternion.identity);
            modelTanamanSaatIni.transform.parent = this.transform;

            // JALANKAN ANIMASI POP-UP
            StartCoroutine(AnimasiPopUp(modelTanamanSaatIni.transform));
        }
    }

    public void Panen()
    {
        if (statusSaatIni == StatusTanah.SiapPanen)
        {
            // --- Munculkan Efek Debu/Confetti saat panen ---
            MunculkanPartikel(efekDebuPrefab);

            Destroy(modelTanamanSaatIni);
            statusSaatIni = StatusTanah.Kosong;
            dataTanaman = null;
            faseSekarang = 0;
            timerPertumbuhan = 0;
        }
    }

    void UbahWarnaTanah(Color warnaTarget)
    {
        if (rendererTanah != null) rendererTanah.material.color = warnaTarget;
    }

    // FUNGSI ANIMASI POP-UP
    IEnumerator AnimasiPopUp(Transform target)
    {
        float durasi = 0.4f;
        float timer = 0f;

        Vector3 skalaAwal = Vector3.zero;
        Vector3 skalaAkhir = Vector3.one;

        target.localScale = skalaAwal;

        while (timer < durasi)
        {
            timer += Time.deltaTime;
            float progress = timer / durasi;

            // Rumus Elastic Out
            float curve = Mathf.Sin(progress * Mathf.PI * (0.2f + 2.5f * progress * progress * progress)) * Mathf.Pow(1f - progress, 2.2f) + progress;

            target.localScale = Vector3.LerpUnclamped(skalaAwal, skalaAkhir, curve);

            yield return null;
        }

        target.localScale = skalaAkhir;
    }
}