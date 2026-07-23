using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DelilYoneticisi : MonoBehaviour
{
    public static DelilYoneticisi Instance;

    public enum OyunRotasi { Secilmedi, SirketYolsuzlugu, KisiselHusumet }
    
    [Header("Aktif Hikaye Rotası")]
    public OyunRotasi aktifRota = OyunRotasi.Secilmedi;

    [Header("Delil Ayarları")]
    public int toplamDelilSayisi = 9; // Toplam delil sayısı 9 olarak sabitlendi
    private int bulunanDelilSayisi = 0;

    [Header("UI Elemanları")]
    public TextMeshProUGUI delilSayaciText; 
    public GameObject gorevTamamPanel; 

    [Header("Delil Bildirim Paneli")]
    public GameObject bildirimPanel; 
    public TextMeshProUGUI bildirimBaslik; 
    public TextMeshProUGUI bildirimIsim; 
    public TextMeshProUGUI bildirimNot; 

    [Header("Ses Sistemi Entegrasyonu")]
    public AudioSource dedektifSesKaynagi;

    private Coroutine bildirimCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ReferanslariBul();
        if (gorevTamamPanel != null) gorevTamamPanel.SetActive(false); 
        if (bildirimPanel != null) bildirimPanel.SetActive(false); 
        DelilSayaciniGuncelle(); 
    }

    public void ReferanslariBul()
    {
        if (delilSayaciText == null)
        {
            GameObject txtObj = GameObject.Find("DelilYazisi");
            if (txtObj != null) delilSayaciText = txtObj.GetComponent<TextMeshProUGUI>();
        }

        if (bildirimPanel == null) bildirimPanel = GameObject.Find("BildirimPaneli");

        if (dedektifSesKaynagi == null)
        {
            dedektifSesKaynagi = GetComponent<AudioSource>();
            if (dedektifSesKaynagi == null)
            {
                dedektifSesKaynagi = gameObject.AddComponent<AudioSource>();
            }
        }
    }

    public void RotaBelirle(OyunRotasi yeniRota)
    {
        aktifRota = yeniRota;
        Debug.Log("Sistem: Hikaye Rotası Kilitlendi -> " + aktifRota);
    }

    public string DelilNotunuGetir(string delilAdi, out string sesDosyaAdi, out bool gercekMi)
    {
        gercekMi = false; 

        switch (delilAdi)
        {
            case "USB Bellek":
                sesDosyaAdi = "Dedektif_Delil_USB";
                gercekMi = true;
                return "Güvenlik Rıza'nın elektrik kesildi yalanını çürüten orijinal kayıtlar. Murat'ın vince çıkarken arkasından birinin tırmandığını gösteriyor.";

            case "Şirket Evrakları":
                sesDosyaAdi = "Dedektif_Evrak_Bulundu_IcSes";
                gercekMi = (aktifRota == OyunRotasi.SirketYolsuzlugu);
                return "Bu evraklar... Şantiyeden çalınan paraları ve sahte faturaları gösteriyor. Murat bunu fark etmiş ve müdürü tehdit etmiş olmalı!";

            case "Yırtık Kadın Fotoğrafı":
                sesDosyaAdi = "Dedektif_Delil_Mektup";
                gercekMi = (aktifRota == OyunRotasi.KisiselHusumet);
                return "Murat'ın müdürün kızıyla gizli ilişkisini gösterir. Müdürün ailesini ve namusunu korumak için cinayeti işlediğinin kanıtıdır.";

            case "Kırık Vinç Teli":
                sesDosyaAdi = "Dedektif_Delil_Tel";
                gercekMi = (aktifRota == OyunRotasi.SirketYolsuzlugu);
                return "Liflerde yıpranma yok. Ağzı spiral taşıyla milimetrik olarak kesilmiş. Bu bir kaza değil, sabotaj.";

            case "Spiral Taşlama Makinesi":
                sesDosyaAdi = "Dedektif_Sahte_Batarya"; 
                gercekMi = (aktifRota == OyunRotasi.SirketYolsuzlugu);
                return "Korkuluk demirlerinin vidalarını ve kaynaklarını aşındırmak için kullanılan atölyedeki cihaz.";

            case "Çamurlu Lastik İzi":
                sesDosyaAdi = "Dedektif_Sahte_Anahtar"; 
                gercekMi = (aktifRota == OyunRotasi.KisiselHusumet);
                return "Olay gecesi şantiyeye giren yabancı bir lüks araca aittir. Kemal Müdür'ün özel aracıyla birebir eşleşir.";

            case "Kirlenmiş Baret":
                sesDosyaAdi = "Dedektif_Sahte_Eldiven"; 
                gercekMi = false;
                return "Olay yerinin uzağında bulunan çamurlu baret. Dedektifi oyalayıp vakit kaybettirmek için bilerek oraya atılmıştır.";

            case "Boş İlaç Şişesi":
                sesDosyaAdi = "Dedektif_Delil_Ilac"; 
                gercekMi = false;
                return "Revirden çalınmış boş sakinleştirici şişesi. Murat'ın intihar ettiği izlenimini uyandırmaya çalışan sahte bir kanıttır.";

            case "Murat'ın Gizli Mektubu":
                sesDosyaAdi = "Dedektif_Delil_Mektup";
                gercekMi = true;
                return "Murat'ın tehdit edildiğini ve başına bir şey gelirse delilleri sakladığını anlatan ıslak imzalı mektubu.";

            case "Zimmet Kayıt Belgesi":
                sesDosyaAdi = "Dedektif_Evrak_Bulundu_IcSes";
                gercekMi = (aktifRota == OyunRotasi.SirketYolsuzlugu);
                return "Şantiyedeki eksik malzemelerin ve zimmete geçirilen paraların resmi dokümanı.";

            default:
                sesDosyaAdi = "";
                return "Bu nesne inceleniyor...";
        }
    }

    public void DelilBulundu(string delilAdi) 
    {
        bulunanDelilSayisi++; 
        DelilSayaciniGuncelle(); 

        if (GorevYoneticisi.Instance != null)
        {
            GorevYoneticisi.Instance.DelilToplandi(delilAdi);
        }

        string sesIsmi;
        bool gercekMi;
        string aciklama = DelilNotunuGetir(delilAdi, out sesIsmi, out gercekMi); 

        if (bildirimBaslik != null) bildirimBaslik.text = gercekMi ? "KRİTİK DELİL BULUNDU" : "ŞÜPHELİ NESNE KAYDEDİLDİ"; 
        if (bildirimIsim != null) bildirimIsim.text = delilAdi; 
        if (bildirimNot != null) bildirimNot.text = aciklama; 

        if (dedektifSesKaynagi != null && !string.IsNullOrEmpty(sesIsmi))
        {
            try
            {
                AudioClip icSes = Resources.Load<AudioClip>("Audio/Sounds/" + sesIsmi);
                if (icSes != null)
                {
                    dedektifSesKaynagi.clip = icSes;
                    dedektifSesKaynagi.Play();
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[DELİL SİSTEMİ] Ses yükleme uyarısı: {ex.Message}");
            }
        }

        if (bildirimCoroutine != null) StopCoroutine(bildirimCoroutine);
        bildirimCoroutine = StartCoroutine(BildirimAnimasyon()); 
    }

    IEnumerator BildirimAnimasyon() 
    {
        if (bildirimPanel == null) yield break; 

        bildirimPanel.SetActive(true); 
        RectTransform rect = bildirimPanel.GetComponent<RectTransform>(); 

        Vector2 baslangic = new Vector2(500f, rect.anchoredPosition.y); 
        Vector2 hedef = new Vector2(-230f, rect.anchoredPosition.y); 

        float sure = 0.4f; 
        float gecenSure = 0f; 

        while (gecenSure < sure) 
        {
            gecenSure += Time.deltaTime; 
            float t = Mathf.SmoothStep(0f, 1f, gecenSure / sure); 
            rect.anchoredPosition = Vector2.Lerp(baslangic, hedef, t); 
            yield return null; 
        }

        yield return new WaitForSeconds(6f); 

        gecenSure = 0f; 
        while (gecenSure < sure) 
        {
            gecenSure += Time.deltaTime; 
            float t = Mathf.SmoothStep(0f, 1f, gecenSure / sure); 
            rect.anchoredPosition = Vector2.Lerp(hedef, baslangic, t); 
            yield return null; 
        }

        bildirimPanel.SetActive(false); 
    }

    public void DelilSayaciniGuncelle() 
    {
        if (delilSayaciText == null) ReferanslariBul();

        // 1. Ana HUD Delil Sayacı
        if (delilSayaciText != null)
        {
            delilSayaciText.gameObject.SetActive(true); 
            delilSayaciText.text = bulunanDelilSayisi + " / " + toplamDelilSayisi + " Nesne Klasörde"; 
        }

        // 2. Diyalog Panosundaki Delil Sayacı
        if (DiyalogYoneticisi.Instance != null && DiyalogYoneticisi.Instance.delilYazisiText != null)
        {
            DiyalogYoneticisi.Instance.delilYazisiText.gameObject.SetActive(true);
            DiyalogYoneticisi.Instance.delilYazisiText.text = "Toplanan Delil: " + bulunanDelilSayisi + " / " + toplamDelilSayisi;
        }
    }

    // --- "Tekrar Dene" Butonunda Delil Sayacını Tamamen Sıfırlamak İçin Eklendi ---
    public void DelilSayaciniSifirla()
    {
        if (bildirimCoroutine != null) StopCoroutine(bildirimCoroutine);
        if (bildirimPanel != null) bildirimPanel.SetActive(false);

        bulunanDelilSayisi = 0;
        aktifRota = OyunRotasi.Secilmedi;
        DelilSayaciniGuncelle();
    }

    public void GorevTamamlandiPaneliAc() 
    {
        // GorevYoneticisi.cs ile uyumluluk için korundu.
    }
}