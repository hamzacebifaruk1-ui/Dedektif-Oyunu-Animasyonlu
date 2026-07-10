using System.Collections.Generic;
using UnityEngine;

public class NpcDiyalog : MonoBehaviour
{
    public string npcAdi; // "Güvenlik Rıza", "Liman Müdürü Kemal", "İşçi Ahmet"
    public float etkilesimMesafesi = 3f;
    private Transform oyuncuTransform;

    [Header("Diyalog Aşamaları")]
    public List<DiyalogSatiri> ilkYalanlarDiyalogu = new List<DiyalogSatiri>();
    public List<DiyalogSatiri> yuzlesmeSuclamaDiyalogu = new List<DiyalogSatiri>();

    private GameObject etkilesimUI;
    private bool yaziGosteriliyorMu = false;

    void Start()
    {
        hareket oyuncuScript = FindFirstObjectByType<hareket>();
        if (oyuncuScript != null) oyuncuTransform = oyuncuScript.transform;
        
        // Sahnede gizlediğimiz "EtkilesimYazisi" isimli UI elemanını otomatik buluyoruz
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            Transform textTransform = canvas.transform.Find("EtkilesimYazisi");
            if (textTransform != null) etkilesimUI = textTransform.gameObject;
        }

        DiyaloglariDoldur();
    }

    void Update()
    {
        if (oyuncuTransform == null) return;

        float mesafe = Vector3.Distance(transform.position, oyuncuTransform.position);
        
        // Yeni Giriş Sistemi için klavye kontrolü
        var klavye = UnityEngine.InputSystem.Keyboard.current;
        bool eBasildi = klavye != null && klavye.eKey.wasPressedThisFrame;

        // OYUNCU NPC'YE YAKINSA YAZIYI AÇ
        if (mesafe <= etkilesimMesafesi)
        {
            // Eğer diyalog paneli şu an açık DEĞİLSE yazıyı göster
            if (etkilesimUI != null && !etkilesimUI.activeSelf && !DiyalogYoneticisi.Instance.diyalogPaneli.activeSelf)
            {
                etkilesimUI.SetActive(true);
                yaziGosteriliyorMu = true;
            }

            // E tuşuna basılırsa konuşmayı başlat ve ipucu yazısını gizle
            if (eBasildi && !DiyalogYoneticisi.Instance.diyalogPaneli.activeSelf)
            {
                if (etkilesimUI != null) etkilesimUI.SetActive(false);

                bool gercekDelillerToplandi = false; 

                if (gercekDelillerToplandi)
                {
                    DiyalogYoneticisi.Instance.DiyalogBaslat(yuzlesmeSuclamaDiyalogu, npcAdi);
                }
                else
                {
                    DiyalogYoneticisi.Instance.DiyalogBaslat(ilkYalanlarDiyalogu, npcAdi);
                }
            }
        }
        else
        {
            // OYUNCU UZAKLAŞTIYSA VE YAZIYI BU NPC AÇTIYSA KAPAT
            if (yaziGosteriliyorMu && etkilesimUI != null)
            {
                etkilesimUI.SetActive(false);
                yaziGosteriliyorMu = false;
            }
        }
    }

    private void DiyaloglariDoldur()
    {
        if (npcAdi == "Güvenlik Rıza")
        {
            ilkYalanlarDiyalogu.Add(new DiyalogSatiri { konusmaciAdi = "Dedektif", textIcerik = "Rıza, kaza gecesi nöbet kulübesinde sorumlu personel sendin...", elevenLabsSesDosyaAdi = "Dedektif_Soru_Riza1" });
            ilkYalanlarDiyalogu.Add(new DiyalogSatiri { konusmaciAdi = "Güvenlik Rıza", textIcerik = "Amirim valla billa, ekmeğimin üzerine yemin ederim ki...", elevenLabsSesDosyaAdi = "Riza_Yalan_Trafo" });
            ilkYalanlarDiyalogu.Add(new DiyalogSatiri { konusmaciAdi = "Dedektif İç Ses", textIcerik = "Elektrik arızası ve trafo patlaması mı? Şantiye dijital loglarında...", elevenLabsSesDosyaAdi = "Dedektif_IcSes_Riza_Suphe" });

            yuzlesmeSuclamaDiyalogu.Add(new DiyalogSatiri { konusmaciAdi = "Dedektif", textIcerik = "Elimde ne var bak bakalım Rıza! Orijinal USB bellek! Elektrik gitmemiş...", elevenLabsSesDosyaAdi = "Dedektif_Yuzlesme_Riza" });
            yuzlesmeSuclamaDiyalogu.Add(new DiyalogSatiri { konusmaciAdi = "Güvenlik Rıza", textIcerik = "Amirim... Ne olur merhamet edin! Müdür Kemal Bey kaza akşamı yanıma geldi...", elevenLabsSesDosyaAdi = "Riza_Suclama_Kemal" });
            yuzlesmeSuclamaDiyalogu.Add(new DiyalogSatiri { konusmaciAdi = "Dedektif İç Ses", textIcerik = "Rıza paçasını kurtarmak için suçu direkt Müdür Kemal'in üzerine yıkıyor...", elevenLabsSesDosyaAdi = "Dedektif_IcSes_Riza_Analiz" });
        }
        else if (npcAdi == "Liman Müdürü Kemal")
        {
            ilkYalanlarDiyalogu.Add(new DiyalogSatiri { konusmaciAdi = "Dedektif", textIcerik = "Kemal Bey, vincin çelik halatlarını bizzat inceledim. Sabotaj var...", elevenLabsSesDosyaAdi = "Dedektif_Soru_Kemal1" });
            ilkYalanlarDiyalogu.Add(new DiyalogSatiri { konusmaciAdi = "Liman Müdürü Kemal", textIcerik = "Ne sabotesi, ne cinayeti dedektif bey? Milyarlık liman projesi burası...", elevenLabsSesDosyaAdi = "Kemal_Yalan_Sirket" });
            ilkYalanlarDiyalogu.Add(new DiyalogSatiri { konusmaciAdi = "Dedektif İç Ses", textIcerik = "Bir insan hayatının yitip gitmesinden ziyade holdingin hisselerini düşünüyor...", elevenLabsSesDosyaAdi = "Dedektif_IcSes_Kemal_Suphe" });

            yuzlesmeSuclamaDiyalogu.Add(new DiyalogSatiri { konusmaciAdi = "Dedektif", textIcerik = "Holdingin gücünün arkasına saklanmayı bırak! Şantaj mektubu elimde...", elevenLabsSesDosyaAdi = "Dedektif_Yuzlesme_Kemal" });
            yuzlesmeSuclamaDiyalogu.Add(new DiyalogSatiri { konusmaciAdi = "Liman Müdürü Kemal", textIcerik = "Tamam, evet! Malzemeden kıstım ama katil değilim! İşçi Ahmet'e bakın!", elevenLabsSesDosyaAdi = "Kemal_Suclama_Ahmet" });
            yuzlesmeSuclamaDiyalogu.Add(new DiyalogSatiri { konusmaciAdi = "Dedektif İç Ses", textIcerik = "Kemal mali yolsuzluklarını kabul etti ama cinayeti Ahmet'in üzerine atıyor...", elevenLabsSesDosyaAdi = "Dedektif_IcSes_Kemal_Analiz" });
        }
        else if (npcAdi == "İşçi Ahmet")
        {
            ilkYalanlarDiyalogu.Add(new DiyalogSatiri { konusmaciAdi = "Dedektif", textIcerik = "Ahmet, Murat vince tırmanmadan hemen önce aranızda ne yaşandı?", elevenLabsSesDosyaAdi = "Dedektif_Soru_Ahmet1" });
            ilkYalanlarDiyalogu.Add(new DiyalogSatiri { konusmaciAdi = "İşçi Ahmet", textIcerik = "Ben... Ben hiçbir şey görmedim amirim. Beni bu işlere bulaştırmayın...", elevenLabsSesDosyaAdi = "Ahmet_Korku_Beyan" });
            ilkYalanlarDiyalogu.Add(new DiyalogSatiri { konusmaciAdi = "Dedektif İç Ses", textIcerik = "Ses telleri tir tir titriyor. Tehdit edildiği veya korktuğu çok açık...", elevenLabsSesDosyaAdi = "Dedektif_IcSes_Ahmet_Suphe" });

            yuzlesmeSuclamaDiyalogu.Add(new DiyalogSatiri { konusmaciAdi = "Dedektif", textIcerik = "Murat'ın dolabında bulduğum bu ağır sakinleştirici ilaç şişesini incelettim...", elevenLabsSesDosyaAdi = "Dedektif_Yuzlesme_Ahmet" });
            yuzlesmeSuclamaDiyalogu.Add(new DiyalogSatiri { konusmaciAdi = "İşçi Ahmet", textIcerik = "Amirim yapmayın, o ilacı Murat'ın kupasına Güvenlik Rıza koydu!", elevenLabsSesDosyaAdi = "Ahmet_Suclama_Riza" });
            yuzlesmeSuclamaDiyalogu.Add(new DiyalogSatiri { konusmaciAdi = "Dedektif İç Ses", textIcerik = "İnanılmaz... Kusursuz bir yalan çemberi ve döngü tamamlandı! Herkes bir sonrakini suçluyor...", elevenLabsSesDosyaAdi = "Dedektif_IcSes_Ahmet_Analiz" });
        }
    }
}