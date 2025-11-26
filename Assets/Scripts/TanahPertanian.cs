using UnityEngine;
using System.Collections; // Wajib ada untuk fitur Timer (Coroutine)

public class TanahPertanian : MonoBehaviour
{
    public enum StatusTanah { Kosong, AdaBibit, SiapPanen }

    [Header("Info Status")]
    public StatusTanah statusSaatIni = StatusTanah.Kosong;

    [Header("Pengaturan")]
    public Transform spawnPoint;

    private GameObject modelTanamanSaatIni;
    private CropData dataTanaman;
    private int faseSekarang = 0;

    // Fungsi Menanam
    public void Tanam(CropData bibitBaru)
    {
        if (statusSaatIni == StatusTanah.Kosong)
        {
            dataTanaman = bibitBaru;
            statusSaatIni = StatusTanah.AdaBibit;
            faseSekarang = 0;

            UpdateTampilanVisual();

            // MULAI TIMER TUMBUH OTOMATIS DISINI!
            StartCoroutine(ProsesTumbuhOtomatis());

            Debug.Log("Menanam " + bibitBaru.namaTanaman + ". Tunggu " + bibitBaru.waktuTumbuhPerFase + " detik...");
        }
    }

    // --- FITUR BARU: Timer Otomatis ---
    IEnumerator ProsesTumbuhOtomatis()
    {
        // Selama tanaman belum siap panen...
        while (statusSaatIni == StatusTanah.AdaBibit && faseSekarang < dataTanaman.modelFase.Length - 1)
        {
            // 1. Tunggu selama X detik (sesuai data di CropData)
            yield return new WaitForSeconds(dataTanaman.waktuTumbuhPerFase);

            // 2. Setelah menunggu, cek apakah tanaman masih ada (belum mati/dihapus)
            if (statusSaatIni == StatusTanah.AdaBibit)
            {
                TumbuhSatuLevel();
            }
        }
    }

    void TumbuhSatuLevel()
    {
        faseSekarang++;
        UpdateTampilanVisual();

        // Cek apakah sudah tahap terakhir?
        if (faseSekarang >= dataTanaman.modelFase.Length - 1)
        {
            statusSaatIni = StatusTanah.SiapPanen;
            Debug.Log("Tanaman Siap Panen!");
        }
    }

    // Fungsi debug manual (tetap disimpan kalau mau cheat tumbuh instan)
    public void TumbuhManual()
    {
        if (statusSaatIni == StatusTanah.AdaBibit && faseSekarang < dataTanaman.modelFase.Length - 1)
        {
            TumbuhSatuLevel();
        }
    }

    void UpdateTampilanVisual()
    {
        if (modelTanamanSaatIni != null) Destroy(modelTanamanSaatIni);

        // Pastikan index tidak error
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
            Debug.Log("Panen Berhasil!");
            Destroy(modelTanamanSaatIni);
            statusSaatIni = StatusTanah.Kosong;
            dataTanaman = null;
            // Stop semua timer yang berjalan di tanah ini supaya aman
            StopAllCoroutines();
        }
    }
}