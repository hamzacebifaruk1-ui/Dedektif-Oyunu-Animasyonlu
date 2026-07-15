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
            // Eğer ekrandaki delil sayacı UI nesnesi aktif değilse (konuşmalar bitmediyse) toplamayı engelle
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

        // 2. 3D İnceleme Ekranını Aç (Eğer sistem sahnede varsa)
        if (DelilIncelemeSistemi.Instance != null)
        {
            DelilIncelemeSistemi.Instance.IncelemeyiBaslat(delilPrefab, delilAdi, incelemeBoyutu, incelemeMesafesi);
        }

        // ===================================================
        // 🚨 YENİ: SİSTEMLER ARASI İLETİŞİM KÖPRÜSÜ 🚨
        // ===================================================
        
        // Delil yöneticisine bu delilin bulunduğunu haber ver 
        // (Bu sayede sayaç artar, açıklama yazılır ve dedektif iç sesi oynatılır)
        if (DelilYoneticisi.Instance != null)
        {
            DelilYoneticisi.Instance.DelilBulundu(delilAdi);
        }
        else
        {
            Debug.LogWarning("[UYARI] Sahnede DelilYoneticisi bulunamadığı için delil işlenemedi!");
        }

        // Görev yöneticisine haber ver (Arama aşamalarında hedefleri günceller ve aşama atlatır)
        if (GorevYoneticisi.Instance != null)
        {
            GorevYoneticisi.Instance.DelilToplandi(delilAdi);
        }
        else
        {
            Debug.LogWarning("[UYARI] Sahnede GorevYoneticisi bulunamadığı için görev ilerletilemedi!");
        }
        
        // ===================================================

        // Görsel ve fiziksel olarak sahnedeki nesneyi kapat (İnceleme bitene kadar yok etmiyoruz)
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