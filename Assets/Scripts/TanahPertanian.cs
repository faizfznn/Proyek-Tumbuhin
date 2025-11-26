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

    // Variabel Internal
    private GameObject modelTanamanSaatIni;
    private CropData dataTanaman;
    private int faseSekarang = 0;
    private float timerPertumbuhan = 0f;

    void Awake()
    {
        // --- AUTO DETECT RENDERER (Supaya tidak perlu drag-drop manual) ---
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

    public void Tanam(CropData bibitBaru)
    {
        if (statusSaatIni == StatusTanah.Kosong)
        {
            dataTanaman = bibitBaru;
            faseSekarang = 0;
            timerPertumbuhan = 0f;

            // Set status awal: Kering (Perlu disiram pertama kali)
            statusSaatIni = StatusTanah.Kering;
            UbahWarnaTanah(warnaKering);

            UpdateTampilanVisual();
        }
    }

    public void SiramTanaman()
    {
        if (statusSaatIni == StatusTanah.Kering)
        {
            statusSaatIni = StatusTanah.Basah;
            UbahWarnaTanah(warnaBasah);
        }
    }

    void TumbuhSatuLevel()
    {
        faseSekarang++;
        timerPertumbuhan = 0f;
        UpdateTampilanVisual();

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

            // --- [BAGIAN BARU] JALANKAN ANIMASI POP-UP ---
            StartCoroutine(AnimasiPopUp(modelTanamanSaatIni.transform));
        }
    }

    public void Panen()
    {
        if (statusSaatIni == StatusTanah.SiapPanen)
        {
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

    // --- [FITUR BARU] FUNGSI ANIMASI POP-UP ---
    IEnumerator AnimasiPopUp(Transform target)
    {
        float durasi = 0.4f; // Kecepatan animasi (makin kecil makin cepat)
        float timer = 0f;

        Vector3 skalaAwal = Vector3.zero;      // Mulai dari 0
        Vector3 skalaAkhir = Vector3.one;      // Ke ukuran asli (1)

        target.localScale = skalaAwal; // Set awal jadi 0

        while (timer < durasi)
        {
            timer += Time.deltaTime;
            float progress = timer / durasi;

            // --- RUMUS MATEMATIKA "ELASTIC OUT" ---
            // Ini bikin efek membal sedikit melebihi ukuran asli lalu kembali normal
            float curve = Mathf.Sin(progress * Mathf.PI * (0.2f + 2.5f * progress * progress * progress)) * Mathf.Pow(1f - progress, 2.2f) + progress;

            // Terapkan skala
            target.localScale = Vector3.LerpUnclamped(skalaAwal, skalaAkhir, curve);

            yield return null; // Tunggu frame berikutnya
        }

        target.localScale = skalaAkhir; // Pastikan ukuran final pas 1
    }
}