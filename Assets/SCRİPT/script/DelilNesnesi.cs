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
        // İsmi tamamen küçük harfe çevirip arama yapıyoruz (Karakter hatasını önler)
        string aramaAdi = delilAdi.ToLower();

        if (aramaAdi.Contains("sahte")) return !toplandiMi;
        if (GorevYoneticisi.Instance == null) return !toplandiMi;

        if (aramaAdi.Contains("ilac") || aramaAdi.Contains("ilaç"))
        {
            if (!GorevYoneticisi.Instance.kemalleKonusuldu) return false;
        }
        if (aramaAdi.Contains("defter") || aramaAdi.Contains("not"))
        {
            if (!GorevYoneticisi.Instance.ahmetleKonusuldu) return false;
        }
        if (aramaAdi.Contains("tel"))
        {
            if (!GorevYoneticisi.Instance.kemalPanikledi) return false;
        }
        if (aramaAdi.Contains("kamera") || aramaAdi.Contains("kaydı") || aramaAdi.Contains("usb"))
        {
            if (!GorevYoneticisi.Instance.rizaItirafEtti) return false;
        }

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

        if (DelilYoneticisi.Instance != null)
            DelilYoneticisi.Instance.DelilBulundu(delilAdi);

        if (NotDefteriYoneticisi.Instance != null)
            NotDefteriYoneticisi.Instance.DelilEkle(delilAdi);

        // Görev kontrol isimlerini de güvenli hale getirdik
        string aramaAdi = delilAdi.ToLower();
        if (GorevYoneticisi.Instance != null)
        {
            if (aramaAdi.Contains("ilac") || aramaAdi.Contains("ilaç"))
                GorevYoneticisi.Instance.IlacKutusuBulundu();
            else if (aramaAdi.Contains("tel"))
            {
                GorevYoneticisi.Instance.kirikTelAlindi = true;
                GorevYoneticisi.Instance.TeknikDelillerTamamla();
            }
            else if (aramaAdi.Contains("kamera") || aramaAdi.Contains("kaydı") || aramaAdi.Contains("usb"))
                GorevYoneticisi.Instance.KameraKaydiBulunduTamamla();
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