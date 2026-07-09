using System.Collections.Generic;
using UnityEngine;

public class HaritaRotaCizici : MonoBehaviour
{
    [Header("Zorunlu Referanslar")]
    public Transform baslangicNoktasi; // Oyuncu
    public Transform aktifHedef;      // Görev Hedefi
    public UIRotaCizgisi cizgiCizici;
    public RectTransform haritaPanelRect;

    [Header("Kalibrasyon (Minimap Ayarları)")]
    public Transform kalibrasyonA_Dunya;
    public Vector2 kalibrasyonA_Piksel;
    public Transform kalibrasyonB_Dunya;
    public Vector2 kalibrasyonB_Piksel;
    public Vector2 gorselCozunurluk = new Vector2(1024, 1024);

    [Header("Davranış Modu")]
    public bool oyuncuMerkezliMod = true;

    [Header("Performans & Hassasiyet")]
    public float yenidenHesaplamaAraligi = 0.05f;
    [Tooltip("Hedefe bu kadar metre kalala çizgi tamamen yok olur.")]
    public float hedefeUlasmaMesafesi = 2.5f;

    private float sonHesaplamaZamani;

    void Update()
    {
        if (aktifHedef == null || baslangicNoktasi == null || cizgiCizici == null) return;
        if (Time.time - sonHesaplamaZamani < yenidenHesaplamaAraligi) return;
        sonHesaplamaZamani = Time.time;

        Vector3 p1 = baslangicNoktasi.position;
        Vector3 p2 = aktifHedef.position;

        // --- HEDEFE ULAŞINCA SİLME MANTIĞI ---
        // Oyuncu ile hedef arasındaki gerçek mesafeyi ölçüyoruz
        float hedefMesafe = Vector3.Distance(p1, p2);
        if (hedefMesafe <= hedefeUlasmaMesafesi)
        {
            cizgiCizici.RotayiTemizle(); // Çizgiyi tamamen yok et
            return; // Kodun geri kalanını çalıştırma
        }
        // -------------------------------------

        // Oyuncu ile hedef arasında tam 90 derecelik temiz bir ara nokta hesaplıyoruz
        float xFark = Mathf.Abs(p2.x - p1.x);
        float zFark = Mathf.Abs(p2.z - p1.z);

        Vector3 araNokta = (xFark > zFark) 
            ? new Vector3(p2.x, p1.y, p1.z) 
            : new Vector3(p1.x, p1.y, p2.z);

        // Dünya üzerindeki saf 3 noktayı listeye ekle
        List<Vector3> dunyaYolu = new List<Vector3> { p1, araNokta, p2 };

        List<Vector2> haritaNoktalari = new List<Vector2>();
        Vector2 oyuncuOfset = oyuncuMerkezliMod
            ? DunyayiPikseleCevir(p1)
            : Vector2.zero;

        foreach (Vector3 nokta in dunyaYolu)
        {
            Vector2 pikselKonum = DunyayiPikseleCevir(nokta);
            haritaNoktalari.Add(pikselKonum - oyuncuOfset);
        }

        cizgiCizici.RotayiGuncelle(haritaNoktalari);
    }

    Vector2 DunyayiPikseleCevir(Vector3 dunyaPozisyonu)
    {
        Vector3 a = kalibrasyonA_Dunya.position;
        Vector3 b = kalibrasyonB_Dunya.position;

        float oranX = Mathf.InverseLerp(a.z, b.z, dunyaPozisyonu.z);
        float oranY = Mathf.InverseLerp(a.x, b.x, dunyaPozisyonu.x);

        float pikselX = Mathf.Lerp(kalibrasyonA_Piksel.x, kalibrasyonB_Piksel.x, oranX);
        float pikselY = gorselCozunurluk.y - Mathf.Lerp(kalibrasyonA_Piksel.y, kalibrasyonB_Piksel.y, oranY);

        float olcekX = haritaPanelRect.rect.width / gorselCozunurluk.x;
        float olcekY = haritaPanelRect.rect.height / gorselCozunurluk.y;

        float haritaX = (pikselX - gorselCozunurluk.x / 2f) * olcekX;
        float haritaY = (pikselY - gorselCozunurluk.y / 2f) * olcekY;

        return new Vector2(haritaX, haritaY);
    }

    public void HedefiDegistir(Transform yeniHedef)
    {
        aktifHedef = yeniHedef;
    }
}