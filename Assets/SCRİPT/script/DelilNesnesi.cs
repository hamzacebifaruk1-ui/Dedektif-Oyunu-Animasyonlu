using UnityEngine;
using System.Collections;

public class DelilNesnesi : MonoBehaviour
{
    public enum DelilKonumu { Yerde, Masada }

    [Header("Bu Delilin Adı (Sözlüktekiyle Aynı Olmalı)")]
    public string delilAdi = "Delil";

    [Header("ElevenLabs Ses Dosyası")]
    public AudioClip toplamaSesi;

    [Header("Akıllı Delil Ayarları")]
    public DelilKonumu delilKonumu;

    [Header("3D Döndürme Ayarı")]
    public GameObject delilPrefab; 

    [Header("İNCELEME AYARLARI")]
    public float incelemeBoyutu = 1.0f;
    public float incelemeMesafesi = 1.5f;

    private bool toplandiMi = false;

    public bool ToplanabilirMi()
    {
        if (toplandiMi) return false;

        // >>> %100 GARANTİLİ KONTROL KATMANI <<<
        if (DiyalogYoneticisi.Instance != null && DiyalogYoneticisi.Instance.delilYazisiText != null)
        {
            if (!DiyalogYoneticisi.Instance.delilYazisiText.gameObject.activeInHierarchy)
            {
                return false; 
            }
        }
        else
        {
            return false;
        }

        return !toplandiMi;
    }

    public void Topla(bool karakterEgildiMi)
    {
        if (!ToplanabilirMi()) return;
        if (delilKonumu == DelilKonumu.Yerde && !karakterEgildiMi) return; 

        toplandiMi = true;

        // 1. Fiziksel toplama ses efektini çal
        if (toplamaSesi != null)
        {
            GameObject sesObjesi = new GameObject("GeciciSesOynatici");
            AudioSource asSource = sesObjesi.AddComponent<AudioSource>();
            asSource.clip = toplamaSesi;
            asSource.spatialBlend = 0f; 
            asSource.volume = 1f;       
            asSource.Play();
            Destroy(sesObjesi, toplamaSesi.length);
        }

        // 2. 3D İnceleme Ekranını Aç
        if (DelilIncelemeSistemi.Instance != null)
        {
            DelilIncelemeSistemi.Instance.IncelemeyiBaslat(delilPrefab, delilAdi, incelemeBoyutu, incelemeMesafesi);
        }

        // ===================================================
        // 🚨 SİSTEMLER ARASI İLETİŞİM KÖPRÜSÜ 🚨
        // ===================================================
        
        // A) NOT DEFTERİNE EKLE (Artık doğrudan deftere ekleniyor!)
        if (NotDefteriYoneticisi.Instance != null)
        {
            NotDefteriYoneticisi.Instance.DelilEkle(delilAdi);
        }
        else
        {
            Debug.LogWarning("[UYARI] Sahnede NotDefteriYoneticisi bulunamadı!");
        }

        // B) Delil yöneticisine haber ver
        if (DelilYoneticisi.Instance != null)
        {
            DelilYoneticisi.Instance.DelilBulundu(delilAdi);
        }

        // C) Görev yöneticisine haber ver
        if (GorevYoneticisi.Instance != null)
        {
            GorevYoneticisi.Instance.DelilToplandi(delilAdi);
        }
        
        // ===================================================

        // Görsel ve fiziksel olarak sahnedeki nesneyi kapat
        if (GetComponent<MeshRenderer>() != null) GetComponent<MeshRenderer>().enabled = false;
        foreach (var childRenderer in GetComponentsInChildren<Renderer>()) childRenderer.enabled = false;
        if (GetComponent<Collider>() != null) GetComponent<Collider>().enabled = false;

        StartCoroutine(IncelemeBitinceYokEt());
    }

    private IEnumerator IncelemeBitinceYokEt()
    {
        while (DelilIncelemeSistemi.Instance != null && DelilIncelemeSistemi.Instance.incelemePaneli.activeSelf)
        {
            yield return null;
        }
        gameObject.SetActive(false);
    }
}