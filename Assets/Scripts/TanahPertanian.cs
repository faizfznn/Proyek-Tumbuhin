using UnityEngine;

public class TanahPertanian : MonoBehaviour
{
    // Kita pecah status 'AdaBibit' menjadi 'Kering' dan 'Basah'
    public enum StatusTanah { Kosong, Kering, Basah, SiapPanen }

    [Header("Info Status")]
    public StatusTanah statusSaatIni = StatusTanah.Kosong;

    [Header("Visual Tanah")]
    public MeshRenderer rendererTanah; // Drag MeshRenderer tanah ke sini di Inspector
    public Color warnaKering = Color.white; // Warna tanah biasa
    public Color warnaBasah = new Color(0.4f, 0.2f, 0f); // Warna coklat tua (disiram)

    [Header("Pengaturan")]
    public Transform spawnPoint;

    // Variabel Internal
    private GameObject modelTanamanSaatIni;
    private CropData dataTanaman;
    private int faseSekarang = 0;
    private float timerPertumbuhan = 0f; // Timer manual

    void Awake() // Gunakan Awake agar dijalankan paling awal
    {
        // --- PERBAIKAN TOTAL: PAKSA CARI KOMPONEN SENDIRI ---
        // Kita HAPUS pengecekan "if (rendererTanah == null)"
        // Kita paksa script mengambil MeshRenderer milik objek ini, 
        // tidak peduli apa yang tertulis di Inspector.

        rendererTanah = GetComponent<MeshRenderer>();

        // Jika tidak ketemu di objek utama, cari di anak objek (child)
        if (rendererTanah == null)
        {
            rendererTanah = GetComponentInChildren<MeshRenderer>();
        }

        // Validasi terakhir
        if (rendererTanah == null)
        {
            Debug.LogError("ERROR FATAL: Tidak ada MeshRenderer di " + gameObject.name);
        }
        else
        {
            // PENTING: Akses .material untuk membuat instance unik
            // Ini mencegah satu lahan berubah warna ikut mengubah lahan lain (Material Leaking)
            Material unikMaterial = rendererTanah.material;
        }
    }

    void Start()
    {
        // Set warna awal di sini
        UbahWarnaTanah(warnaKering);
    }

    void Update()
    {
        // LOGIKA UTAMA: Hanya tumbuh jika statusnya BASAH
        if (statusSaatIni == StatusTanah.Basah)
        {
            timerPertumbuhan += Time.deltaTime;

            // Cek apakah waktu fase ini sudah selesai?
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

            // Saat pertama ditanam, tanah dalam kondisi KERING (Butuh air pertama)
            statusSaatIni = StatusTanah.Kering;
            UbahWarnaTanah(warnaKering);

            UpdateTampilanVisual();
            Debug.Log("Menanam " + bibitBaru.namaTanaman + ". Tanah Kering, perlu disiram!");
        }
    }

    // Fungsi Baru: Menyiram Tanaman
    public void SiramTanaman()
    {
        if (statusSaatIni == StatusTanah.Kering)
        {
            statusSaatIni = StatusTanah.Basah;
            UbahWarnaTanah(warnaBasah); // Ubah jadi gelap
            Debug.Log("Tanaman disiram! Mulai tumbuh...");
        }
    }

    void TumbuhSatuLevel()
    {
        faseSekarang++;
        timerPertumbuhan = 0f; // Reset timer untuk fase berikutnya
        UpdateTampilanVisual();

        // Cek apakah sudah tahap terakhir (Siap Panen)?
        if (faseSekarang >= dataTanaman.modelFase.Length - 1)
        {
            statusSaatIni = StatusTanah.SiapPanen;
            UbahWarnaTanah(warnaKering); // Kembalikan warna normal saat panen
            Debug.Log("Tanaman Siap Panen!");
        }
        else
        {
            // Jika belum panen, tanah KEMBALI KERING (Pemain harus siram lagi)
            statusSaatIni = StatusTanah.Kering;
            UbahWarnaTanah(warnaKering);
            Debug.Log("Tanaman tumbuh besar. Tanah kembali kering!");
        }
    }

    void UpdateTampilanVisual()
    {
        if (modelTanamanSaatIni != null) Destroy(modelTanamanSaatIni);

        if (faseSekarang < dataTanaman.modelFase.Length)
        {
            GameObject modelBaru = dataTanaman.modelFase[faseSekarang];
            modelTanamanSaatIni = Instantiate(modelBaru, spawnPoint.position, Quaternion.identity);
            modelTanamanSaatIni.transform.parent = this.transform;
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
            Debug.Log("Panen Berhasil!");
        }
    }

    // Helper untuk ubah warna material tanah
    void UbahWarnaTanah(Color warnaTarget)
    {
        if (rendererTanah != null)
        {
            rendererTanah.material.color = warnaTarget;
        }
    }
}