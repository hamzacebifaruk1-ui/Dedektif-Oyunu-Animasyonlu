using UnityEngine;
using UnityEngine.InputSystem; // New Input System için
using System.Collections.Generic;

public class DelilPanosuSistemi : MonoBehaviour
{
    [Header("UI Panelleri")]
    public GameObject panoPanel; // Açıp kapatacağımız ana UI paneli
    public Transform delilKonteyner; // Grid Layout Group olan obje

    [Header("Prefablar")]
    public GameObject delilKartiPrefab; // Oluşturacağımız delil kartı şablonu

    // ⚡ Boşluk hatası "panoAcikMi" olarak birleştirilerek düzeltildi!
    private bool panoAcikMi = false; 

    void Start()
    {
        if (panoPanel != null)
            panoPanel.SetActive(false); // Oyun başlarken pano kapalı olsun
    }

    void Update()
    {
        // "I" tuşuna basıldığında panoyu aç/kapat (New Input System uyumlu)
        if (Keyboard.current != null && Keyboard.current.iKey.wasPressedThisFrame)
        {
            if (GorevYoneticisi.Instance != null && GorevYoneticisi.Instance.mevcutAsama >= GorevYoneticisi.GorevAsamasi.DelilTasnifPanosu)
            {
                PanoDurumunuDegistir();
            }
            else
            {
                Debug.Log("[PANO] Henüz delil tasnif aşamasına gelmediniz!");
            }
        }
    }

    public void PanoDurumunuDegistir()
    {
        // ⚡ Değişken isimleri birleştirildi
        panoAcikMi = !panoAcikMi;
        panoPanel.SetActive(panoAcikMi);

        if (panoAcikMi)
        {
            // Fare imlecini serbest bırak ki panoda tıklama yapabilesin
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            PanoyuDoldur();
        }
        else
        {
            // Panoyu kapatınca fareyi tekrar oyuna kilitle
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // ENVANTERDEKİ DELİLLERİ EKRANA BASAN FONKSİYON
    private void PanoyuDoldur()
    {
        // Önce içeride eski kartlar kalmışsa temizleyelim (üst üste binmesin diye)
        foreach (Transform child in delilKonteyner)
        {
            Destroy(child.gameObject);
        }

        if (GorevYoneticisi.Instance == null)
        {
            Debug.LogError("[PANO] GorevYoneticisi bulunamadı!");
            return;
        }

        // Görev yöneticisindeki benzersiz delil listesini alıyoruz
        HashSet<string> toplananlar = GorevYoneticisi.Instance.GetToplananDeliller();

        if (toplananlar == null) return;

        foreach (string delil in toplananlar)
        {
            // Şablondan yeni kartı üretiyoruz
            GameObject yeniKart = Instantiate(delilKartiPrefab, delilKonteyner);
            
            // Kartın üzerindeki DelilKartiUI scriptine ulaşıyoruz
            DelilKartiUI kartBileseni = yeniKart.GetComponent<DelilKartiUI>();
            
            if (kartBileseni != null)
            {
                // Karta "Adın şu, git resmini bul ve butonlarını hazırla" diyoruz
                kartBileseni.KartKurulumu(delil);
            }
        }
    }
}