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
        // 🟢 Şirket Yolsuzluğu İtirafı
        yolsuzlukItirafi.Clear();
        yolsuzlukItirafi.Add(new DiyalogSatiri { 
            konusmaciAdi = "İşçi Ahmet", 
            textIcerik = "Amirim... Evet, Kemal Müdür Murat'ı susturmak için teknik ekibe sabotaj emri verdi. Murat her şeyi holdinge bildirecekti!", 
            elevenLabsSesDosyaAdi = "Ahmet_Itiraf_Yolsuzluk" 
        });

        // 🔴 Kişisel Husumet İtirafı
        husumetItirafi.Clear();
        husumetItirafi.Add(new DiyalogSatiri { 
            konusmaciAdi = "İşçi Ahmet", 
            textIcerik = "Şaşırdım amirim... O fotoğraf Murat'ın gizli ilişkisine ait. Müdürün ailesiyle tehdit ediliyordu, bu iş tamamen kişisel bir intikam!", 
            elevenLabsSesDosyaAdi = "Ahmet_Itiraf_Husumet" 
        });
    }

    // 🟢 SEÇENEK 1: Şirket Yolsuzluğu Butonu (Üstteki Yeşil Buton)
    public void SirketYolsuzluguSecenegi()
    {
        Debug.Log("[SEÇİM] Şirket Yolsuzluğu rotası kilitlendi!");
        aktifRota = OyunRotasi.SirketYolsuzluk;

        KapatVeDiyalogBaslat(yolsuzlukItirafi);
    }

    // 🔴 SEÇENEK 2: Kişisel Husumet Butonu (Alttaki Kırmızı Buton)
    public void KisiselHusumetSecenegi()
    {
        Debug.Log("[SEÇİM] Kişisel Husumet rotası kilitlendi!");
        aktifRota = OyunRotasi.KisiselHusumet;

        KapatVeDiyalogBaslat(husumetItirafi);
    }

    private void KapatVeDiyalogBaslat(List<DiyalogSatiri> secilenDiyalog)
    {
        gameObject.SetActive(false); // Seçim panelini kapat

        // Fareyi tekrar gizle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Ahmet'in seçilen rotaya göre itiraf etmesini sağla
        if (DiyalogYoneticisi.Instance != null)
        {
            DiyalogYoneticisi.Instance.DiyalogBaslat(secilenDiyalog, "İşçi Ahmet");
        }

        // Görev aşamasını "Kalan Delilleri Topla" olarak güncelle
        if (GorevYoneticisi.Instance != null)
        {
            GorevYoneticisi.Instance.AsamaAtla(GorevYoneticisi.GorevAsamasi.KalanDelilleriTopla);
        }
    }
}