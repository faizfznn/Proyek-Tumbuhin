using UnityEngine;

public class TanahPertanian : MonoBehaviour
{
    // Kita buat status untuk tanah ini
    // Apakah kosong? Apakah ada bibit? Atau sudah siap panen?
    public enum StatusTanah { Kosong, AdaBibit, SiapPanen }

    [Header("Info Tanah")]
    public StatusTanah statusSaatIni = StatusTanah.Kosong;

    // Fungsi ini nanti dipanggil saat pemain klik tanah
    public void Berinteraksi()
    {
        if (statusSaatIni == StatusTanah.Kosong)
        {
            Debug.Log("Tanah ini kosong, ayo tanam sesuatu!");
            // Nanti kita tambahkan logika menanam di sini
        }
        else if (statusSaatIni == StatusTanah.AdaBibit)
        {
            Debug.Log("Tanaman sedang tumbuh...");
        }
        else if (statusSaatIni == StatusTanah.SiapPanen)
        {
            Debug.Log("Hore! Panen berhasil!");
        }
    }
}