using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Pengaturan Raycast")]
    public float jarakAmbil = 5f; // Jarak sudah disesuaikan
    public LayerMask layerTanah;

    [Header("UI Indikator")]
    public GameObject panelIndikator;
    public TextMeshProUGUI teksIndikator;

    [Header("Bibit")]
    public CropData bibitPilihan;

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        bool melihatTanah = Physics.Raycast(ray, out hit, jarakAmbil, layerTanah);

        if (melihatTanah)
        {
            TanahPertanian tanah = hit.collider.GetComponent<TanahPertanian>();

            if (tanah != null)
            {
                panelIndikator.SetActive(true);

                // --- LOGIKA TEXT & WARNA ---
                if (tanah.statusSaatIni == TanahPertanian.StatusTanah.Kosong)
                {
                    teksIndikator.text = "Tekan [E] untuk Menanam";
                    teksIndikator.color = Color.white;
                }
                else if (tanah.statusSaatIni == TanahPertanian.StatusTanah.SiapPanen)
                {
                    teksIndikator.text = "Tekan [E] untuk Panen!";
                    teksIndikator.color = Color.green; // Kasih warna hijau biar menarik
                }
                else // Sedang Tumbuh (AdaBibit)
                {
                    teksIndikator.text = "Sedang Tumbuh... (Tunggu)";
                    teksIndikator.color = Color.yellow;
                }

                // --- LOGIKA INPUT (YANG DIPERBAIKI) ---
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (tanah.statusSaatIni == TanahPertanian.StatusTanah.Kosong)
                    {
                        tanah.Tanam(bibitPilihan);
                    }
                    else if (tanah.statusSaatIni == TanahPertanian.StatusTanah.SiapPanen)
                    {
                        tanah.Panen();
                    }

                    // PERUBAHAN DISINI:
                    // Kita HAPUS bagian "else { tanah.TumbuhManual() }"
                    // Jadi kalau sedang tumbuh, tekan E tidak akan terjadi apa-apa.
                }
            }
        }
        else
        {
            if (panelIndikator.activeSelf) panelIndikator.SetActive(false);
        }
    }
}