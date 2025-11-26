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

        // Cari script tanah (di objek itu sendiri atau bapaknya)
        TanahPertanian tanah = null;
        if (melihatTanah)
        {
            tanah = hit.collider.GetComponent<TanahPertanian>();
            if (tanah == null) tanah = hit.collider.GetComponentInParent<TanahPertanian>();
        }

        if (tanah != null)
        {
            panelIndikator.SetActive(true);

            // Ambil data bibit yang sedang dipegang sekarang
            CropData bibitSekarang = daftarBibit[indexBibitTerpilih];

            // --- LOGIKA TEXT & WARNA ---
            if (tanah.statusSaatIni == TanahPertanian.StatusTanah.Kosong)
            {
                teksIndikator.text = "Tekan [E] untuk Tanam " + bibitSekarang.namaTanaman;
                teksIndikator.color = Color.white;
            }
            else if (tanah.statusSaatIni == TanahPertanian.StatusTanah.SiapPanen)
            {
                teksIndikator.text = "Tekan [E] untuk Panen!";
                teksIndikator.color = Color.green;
            }
            else // Sedang Tumbuh
            {
                teksIndikator.text = "Sedang Tumbuh... (Tunggu)";
                teksIndikator.color = Color.yellow;
            }

            // --- INPUT INTERAKSI ---
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (tanah.statusSaatIni == TanahPertanian.StatusTanah.Kosong)
                {
                    tanah.Tanam(bibitSekarang); // Tanam sesuai pilihan!
                }
                else if (tanah.statusSaatIni == TanahPertanian.StatusTanah.SiapPanen)
                {
                    tanah.Panen();
                }
            }
        }
        else
        {
            if (panelIndikator.activeSelf) panelIndikator.SetActive(false);
        }
    }

    void PilihBibitInput()
    {
        // Tombol 1 -> Sunflower (Element 0)
        if (Input.GetKeyDown(KeyCode.Alpha1) && daftarBibit.Length > 0)
        {
            indexBibitTerpilih = 0;
        }
        // Tombol 2 -> Wortel (Element 1)
        if (Input.GetKeyDown(KeyCode.Alpha2) && daftarBibit.Length > 1)
        {
            indexBibitTerpilih = 1;
        }
        // Tombol 3 -> Jagung (Element 2)
        if (Input.GetKeyDown(KeyCode.Alpha3) && daftarBibit.Length > 2)
        {
            indexBibitTerpilih = 2;
        }
        // Tombol 4 -> Brokoli (Element 3)
        if (Input.GetKeyDown(KeyCode.Alpha4) && daftarBibit.Length > 3)
        {
            indexBibitTerpilih = 3;
        }
    }
}