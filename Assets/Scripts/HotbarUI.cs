using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HotbarUI : MonoBehaviour
{
    [Header("Slot Bibit (Drag object Slot_1 dst kesini)")]
    public Image[] slotBackgrounds; // Background kotak (yang akan berubah warna saat dipilih)
    public Image[] slotIcons;       // Tempat gambar jagung/wortel muncul

    [Header("Indikator Tombol Aksi")]
    public Image imgTombolE;       // Gambar tombol E
    public TextMeshProUGUI textE;  // Teks keterangan E (Tanam/Panen)
    public Image imgTombolF;       // Gambar tombol F
    public TextMeshProUGUI textF;  // Teks keterangan F (Siram)

    [Header("Setting Warna")]
    public Color warnaSlotNormal = new Color(0.5f, 0.5f, 0.5f, 0.5f); // Abu transparan
    public Color warnaSlotAktif = Color.white;                        // Putih terang
    public Color warnaTombolAktif = Color.white;
    public Color warnaTombolMati = new Color(1f, 1f, 1f, 0.1f);       // Pudar

    // --- 1. Setup Awal: Mengisi Gambar Ikon ---
    public void InisialisasiSlot(CropData[] daftarBibit)
    {
        for (int i = 0; i < slotIcons.Length; i++)
        {
            if (i < daftarBibit.Length) // Jika ada datanya
            {
                slotIcons[i].sprite = daftarBibit[i].iconTanaman;
                slotIcons[i].enabled = true;
            }
            else
            {
                slotIcons[i].enabled = false; // Matikan jika slot kosong
            }
        }
    }

    // --- 2. Update Visual: Highlight Slot Terpilih ---
    public void UpdateSeleksiSlot(int index)
    {
        for (int i = 0; i < slotBackgrounds.Length; i++)
        {
            if (i == index)
                slotBackgrounds[i].color = warnaSlotAktif;
            else
                slotBackgrounds[i].color = warnaSlotNormal;
        }
    }

    // --- 3. Update Visual: Tombol E dan F ---
    public void UpdateVisualAksi(bool bisaTanam, bool bisaPanen, bool bisaSiram)
    {
        // LOGIKA VISUAL TOMBOL E
        if (bisaTanam)
        {
            imgTombolE.color = warnaTombolAktif;
            textE.text = "Tanam";
        }
        else if (bisaPanen)
        {
            imgTombolE.color = Color.green; // Hijau saat panen
            textE.text = "Panen!";
        }
        else
        {
            imgTombolE.color = warnaTombolMati; // Pudar jika tidak bisa apa-apa
            textE.text = "";
        }

        // LOGIKA VISUAL TOMBOL F
        if (bisaSiram)
        {
            imgTombolF.color = Color.cyan; // Biru air
            textF.text = "Siram";
        }
        else
        {
            imgTombolF.color = warnaTombolMati;
            textF.text = "";
        }
    }
}