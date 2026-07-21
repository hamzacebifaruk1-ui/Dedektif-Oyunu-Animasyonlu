using UnityEngine;
using System.Collections.Generic;

public class AhmetSecimKontrol : MonoBehaviour
{
    // Oyun rotalarını tutan ana yapımız
    public enum OyunRotasi { Belirlenmedi, SirketYolsuzluk, KisiselHusumet }
    public static OyunRotasi aktifRota = OyunRotasi.Belirlenmedi;

    [Header("Yüzleşme İtiraf Diyalogları")]
    public List<DiyalogSatiri> yolsuzlukItirafi = new List<DiyalogSatiri>();
    public List<DiyalogSatiri> husumetItirafi = new List<DiyalogSatiri>();

    private void Start()
    {
        // 🟢 SEÇENEK 1: Üstüne Git / Baskı Yap (Atölyeyi İpucu Verir)
        // Ses Dosyası Klasörü: Resources/Dialogs/Ahmet_Yuzlesme_Baski.mp3
        yolsuzlukItirafi.Clear();
        yolsuzlukItirafi.Add(new DiyalogSatiri { 
            konusmaciAdi = "İşçi Ahmet", 
            textIcerik = "Vallahi ben yapmadım amirim! O gece atölyeden spiral taşlama sesleri geliyordu, korkup kulübeye saklandım. Atölyeye git bak, ne arıyorsan orada!", 
            elevenLabsSesDosyaAdi = "Ahmet_Yuzlesme_Baski" 
        });

        // 🔴 SEÇENEK 2: Sakin Ol / Güven Ver (Vinç Telini İpucu Verir)
        // Ses Dosyası Klasörü: Resources/Dialogs/Ahmet_Yuzlesme_Sakin.mp3
        husumetItirafi.Clear();
        husumetItirafi.Add(new DiyalogSatiri { 
            konusmaciAdi = "İşçi Ahmet", 
            textIcerik = "Murat iyi çocuktu abi... Olaydan birkaç saat önce vinç dairesinin orada birilerinin gizlice bir şeyler kestiğini duydum. Atölyedeki takımlara ve vinç tellerine bakarsanız ne demek istediğimi anlarsınız...", 
            elevenLabsSesDosyaAdi = "Ahmet_Yuzlesme_Sakin" 
        });
    }

    // 🟢 SEÇENEK 1 BUTONU (Sert / Baskı Yap)
    public void SirketYolsuzluguSecenegi()
    {
        Debug.Log("[SEÇİM] Ahmet'e baskı yapıldı, atölye ipucu alındı.");
        aktifRota = OyunRotasi.SirketYolsuzluk;

        KapatVeDiyalogBaslat(yolsuzlukItirafi);
    }

    // 🔴 SEÇENEK 2 BUTONU (Sakin / Güven Ver)
    public void KisiselHusumetSecenegi()
    {
        Debug.Log("[SEÇİM] Ahmet'e güven verildi, vinç teli ipucu alındı.");
        aktifRota = OyunRotasi.KisiselHusumet;

        KapatVeDiyalogBaslat(husumetItirafi);
    }

    private void KapatVeDiyalogBaslat(List<DiyalogSatiri> secilenDiyalog)
    {
        gameObject.SetActive(false); // Seçim panelini kapat

        // Fareyi oyuna tekrar kilitler
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Ahmet'in yeni dolaylı ses kaydı çalışır
        if (DiyalogYoneticisi.Instance != null)
        {
            DiyalogYoneticisi.Instance.DiyalogBaslat(secilenDiyalog, "İşçi Ahmet");
        }

        // Görevi "Kalan Delilleri Topla" aşamasına aktarır
        if (GorevYoneticisi.Instance != null)
        {
            GorevYoneticisi.Instance.AsamaAtla(GorevYoneticisi.GorevAsamasi.KalanDelilleriTopla);
        }
    }
}