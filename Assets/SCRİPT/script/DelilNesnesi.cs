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
        // "3 Şüpheliyle konuşuldu, görev tetiklendi!" anında çalışan sistem diyalog yöneticisindedir.
        if (DiyalogYoneticisi.Instance != null && DiyalogYoneticisi.Instance.delilYazisiText != null)
        {
            // Eğer senin ekrandaki "Toplanan Delil: 0 / 5" yazısını tutan sayaç UI nesnesi 
            // hiyerarşide AKTİF DEĞİLSE (yani konuşmalar bitip görev tetiklenmediyse) KESİNLİKLE FALSE DÖN!
            if (!DiyalogYoneticisi.Instance.delilYazisiText.gameObject.activeInHierarchy)
            {
                return false; // Konuşmalar bitene kadar yazı çıkmaz, nesne toplanmaz!
            }
        }
        else
        {
            // Eğer sahnede diyalog yöneticisi henüz uyanmadıysa koruma amaçlı engelle
            return false;
        }

        // --- Konuşmalar bittiyse (Yazı aktif olduysa) artık tüm delillere doğrudan izin ver ---
        return !toplandiMi;
    }

    public void Topla(bool karakterEgildiMi)
    {
        if (!ToplanabilirMi()) return;
        if (delilKonumu == DelilKonumu.Yerde && !karakterEgildiMi) return; 

        toplandiMi = true;

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

        if (DelilIncelemeSistemi.Instance != null)
        {
            DelilIncelemeSistemi.Instance.IncelemeyiBaslat(delilPrefab, delilAdi, incelemeBoyutu, incelemeMesafesi);
        }

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