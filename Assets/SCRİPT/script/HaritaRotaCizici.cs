using System.Collections.Generic;
using UnityEngine;

public class HaritaRotaCizici : MonoBehaviour
{
    public static HaritaRotaCizici Instance;

    [Header("Zorunlu Referanslar")]
    public Transform baslangicNoktasi; // Oyuncu[cite: 5]
    public Transform aktifHedef;      // Görev Hedefi[cite: 5]
    public UIRotaCizgisi cizgiCizici;
    public RectTransform haritaPanelRect; //[cite: 5]

    [Header("Kalibrasyon (Minimap Ayarları)")] //[cite: 5]
    public Transform kalibrasyonA_Dunya; //[cite: 5]
    public Vector2 kalibrasyonA_Piksel; //[cite: 5]
    public Transform kalibrasyonB_Dunya; //[cite: 5]
    public Vector2 kalibrasyonB_Piksel; //[cite: 5]
    public Vector2 gorselCozunurluk = new Vector2(1024, 1024); //[cite: 5]

    [Header("Davranış Modu")] //[cite: 5]
    public bool oyuncuMerkezliMod = true; //[cite: 5]

    [Header("Performans & Hassasiyet")] //[cite: 5]
    public float yenidenHesaplamaAraligi = 0.05f; //[cite: 5]
    [Tooltip("Hedefe bu kadar metre kalala çizgi tamamen yok olur.")]
    public float hedefeUlasmaMesafesi = 2.5f; //[cite: 5]

    private float sonHesaplamaZamani;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Update()
    {
        if (aktifHedef == null || baslangicNoktasi == null || cizgiCizici == null) 
        {
            if(cizgiCizici != null) cizgiCizici.RotayiTemizle();
            return;
        }
        
        if (Time.time - sonHesaplamaZamani < yenidenHesaplamaAraligi) return; //[cite: 5]
        sonHesaplamaZamani = Time.time; //[cite: 5]

        Vector3 p1 = baslangicNoktasi.position; //[cite: 5]
        Vector3 p2 = aktifHedef.position; //[cite: 5]

        float hedefMesafe = Vector3.Distance(p1, p2); //[cite: 5]
        if (hedefMesafe <= hedefeUlasmaMesafesi) //[cite: 5]
        {
            cizgiCizici.RotayiTemizle(); //[cite: 5]
            return; //[cite: 5]
        }

        float xFark = Mathf.Abs(p2.x - p1.x); //[cite: 5]
        float zFark = Mathf.Abs(p2.z - p1.z); //[cite: 5]

        Vector3 araNokta = (xFark > zFark) //[cite: 5]
            ? new Vector3(p2.x, p1.y, p1.z) //[cite: 5]
            : new Vector3(p1.x, p1.y, p2.z); //[cite: 5]

        List<Vector3> dunyaYolu = new List<Vector3> { p1, araNokta, p2 }; //[cite: 5]
        List<Vector2> haritaNoktalari = new List<Vector2>();
        Vector2 oyuncuOfset = oyuncuMerkezliMod ? DunyayiPikseleCevir(p1) : Vector2.zero; //[cite: 5]

        foreach (Vector3 nokta in dunyaYolu)
        {
            Vector2 pikselKonum = DunyayiPikseleCevir(nokta); //[cite: 5]
            haritaNoktalari.Add(pikselKonum - oyuncuOfset); //[cite: 5]
        }

        cizgiCizici.RotayiGuncelle(haritaNoktalari); //[cite: 5]
    }

    Vector2 DunyayiPikseleCevir(Vector3 dunyaPozisyonu) //[cite: 5]
    {
        Vector3 a = kalibrasyonA_Dunya.position; //[cite: 5]
        Vector3 b = kalibrasyonB_Dunya.position; //[cite: 5]

        float oranX = Mathf.InverseLerp(a.z, b.z, dunyaPozisyonu.z); //[cite: 5]
        float oranY = Mathf.InverseLerp(a.x, b.x, dunyaPozisyonu.x); //[cite: 5]

        float pikselX = Mathf.Lerp(kalibrasyonA_Piksel.x, kalibrasyonB_Piksel.x, oranX); //[cite: 5]
        float pikselY = gorselCozunurluk.y - Mathf.Lerp(kalibrasyonA_Piksel.y, kalibrasyonB_Piksel.y, oranY); //[cite: 5]

        float olcekX = haritaPanelRect.rect.width / gorselCozunurluk.x; //[cite: 5]
        float olcekY = haritaPanelRect.rect.height / gorselCozunurluk.y; //[cite: 5]

        float haritaX = (pikselX - gorselCozunurluk.x / 2f) * olcekX; //[cite: 5]
        float haritaY = (pikselY - gorselCozunurluk.y / 2f) * olcekY; //[cite: 5]

        return new Vector2(haritaX, haritaY); //[cite: 5]
    }
public void HedefiDegistir(Transform yeniHedef)
    {
        aktifHedef = yeniHedef;
        
        // Eğer yeni hedef null geldiyse, Update'i beklemeden çizgiyi hemen temizle
        if (aktifHedef == null && cizgiCizici != null)
        {
            cizgiCizici.RotayiTemizle();
        }
    }
    }
    
    
