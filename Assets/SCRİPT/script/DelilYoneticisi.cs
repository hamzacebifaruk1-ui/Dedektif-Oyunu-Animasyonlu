using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DelilYoneticisi : MonoBehaviour
{
    public static DelilYoneticisi Instance;

    // --- YENİ SEÇİM SİSTEMİ ENTEGRASYONU ---
    public enum OyunRotasi { Secilmedi, SirketYolsuzlugu, KisiselHusumet }
    
    [Header("Aktif Hikaye Rotası")]
    public OyunRotasi aktifRota = OyunRotasi.Secilmedi;

    [Header("Delil Ayarları")]
    public int toplamDelilSayisi = 8; // Sahnedeki toplam 8 delil
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

    void Awake()
    {
        Instance = this; 
    }

    void Start()
    {
        if (gorevTamamPanel != null) gorevTamamPanel.SetActive(false); 
        if (bildirimPanel != null) bildirimPanel.SetActive(false); 
        DelilSayaciniGuncelle(); 
    }

    // Ahmet'le konuşulduğunda seçilen rotayı buraya kaydetmek için kullanılacak
    public void RotaBelirle(OyunRotasi yeniRota)
    {
        aktifRota = yeniRota;
        Debug.Log("Sistem: Hikaye Rotası Kilitlendi -> " + aktifRota);
    }

    public string DelilNotunuGetir(string delilAdi, out string sesDosyaAdi, out bool gercekMi)
    {
        gercekMi = false; // Varsayılan olarak sahte kabul edilsin

        switch (delilAdi)
        {
            // ==========================================
            // 1) USB BELLEĞİ (Her iki rotada da GERÇEK)
            // ==========================================
            case "USB Bellek":
                sesDosyaAdi = "Dedektif_Delil_USB";
                gercekMi = true;
                return "Güvenlik Rıza'nın elektrik kesildi yalanını çürüten orijinal kayıtlar. Murat'ın vince çıkarken arkasından birinin tırmandığını gösteriyor.";

            // ==========================================
            // 2) ŞİRKET EVRAKLARI (Yolsuzlukta GERÇEK, Husumette SAHTE)
            // ==========================================
            case "Şirket Evrakları":
                sesDosyaAdi = "Dedektif_Evrak_Bulundu_IcSes";
                gercekMi = (aktifRota == OyunRotasi.SirketYolsuzlugu);
                return "Bu evraklar... Şantiyeden çalınan paraları ve sahte faturaları gösteriyor. Murat bunu fark etmiş ve müdürü tehdit etmiş olmalı!";

            // ==========================================
            // 3) YIRTIDIK KADIN FOTOĞRAFI (Husumette GERÇEK, Yolsuzlukta SAHTE)
            // ==========================================
            case "Yırtık Kadın Fotoğrafı":
                sesDosyaAdi = "Dedektif_Delil_Mektup";
                gercekMi = (aktifRota == OyunRotasi.KisiselHusumet);
                return "Murat'ın müdürün kızıyla gizli ilişkisini gösterir. Müdürün ailesini ve namusunu korumak için cinayeti işlediğinin kanıtıdır.";

            // ==========================================
            // 4) KIRIK VİNÇ TELİ (Yolsuzlukta GERÇEK, Husumette SAHTE)
            // ==========================================
            case "Kırık Vinç Teli":
                sesDosyaAdi = "Dedektif_Delil_Tel";
                gercekMi = (aktifRota == OyunRotasi.SirketYolsuzlugu);
                return "Liflerde yıpranma yok. Ağzı spiral taşıyla, yani demir testeresiyle milimetrik olarak kesilmiş. Bu bir kaza değil, sabotaj.";

            // ==========================================
            // 5) SPİRAL TAŞLAMA MAKİNESİ (Yolsuzlukta GERÇEK, Husumette SAHTE)
            // ==========================================
            case "Spiral Taşlama Makinesi":
                sesDosyaAdi = "Dedektif_Sahte_Batarya"; 
                gercekMi = (aktifRota == OyunRotasi.SirketYolsuzlugu);
                return "Korkuluk demirlerinin vidalarını ve kaynaklarını aşındırmak için kullanılan atölyedeki cihaz.";

            // ==========================================
            // 6) ÇAMURLU LASTİK İZİ (Husumette GERÇEK, Yolsuzlukta SAHTE)
            // ==========================================
            case "Çamurlu Lastik İzi":
                sesDosyaAdi = "Dedektif_Sahte_Anahtar"; 
                gercekMi = (aktifRota == OyunRotasi.KisiselHusumet);
                return "Olay gecesi şantiyeye giren yabancı bir lüks araca aittir. Kemal Müdür'ün özel aracıyla birebir eşleşir.";

            // ==========================================
            // 7) KİRLENMİŞ BARET (Her iki rotada da SAHTE)
            // ==========================================
            case "Kirlenmiş Baret":
                sesDosyaAdi = "Dedektif_Sahte_Eldiven"; 
                gercekMi = false;
                return "Olay yerinin uzağında bulunan çamurlu baret. Dedektifi oyalayıp vakit kaybettirmek için bilerek oraya atılmıştır.";

            // ==========================================
            // 8) BOŞ İLAÇ ŞİŞESİ (Her iki rotada da SAHTE)
            // ==========================================
            case "Boş İlaç Şişesi":
                sesDosyaAdi = "Dedektif_Delil_Ilac"; 
                gercekMi = false;
                return "Revirden çalınmış boş sakinleştirici şişesi. Murat'ın intihar ettiği izlenimini uyandırmaya çalışan sahte bir kanıttır.";

            default:
                sesDosyaAdi = "";
                return "Bu nesne inceleniyor...";
        }
    }

    public void DelilBulundu(string delilAdi) 
    {
        bulunanDelilSayisi++; 
        DelilSayaciniGuncelle(); 

        string sesIsmi;
        bool gercekMi;
        string aciklama = DelilNotunuGetir(delilAdi, out sesIsmi, out gercekMi); 

        if (bildirimBaslik != null) bildirimBaslik.text = gercekMi ? "KRİTİK DELİL BULUNDU" : "ŞÜPHELİ NESNE KAYDEDİLDİ"; 
        if (bildirimIsim != null) bildirimIsim.text = delilAdi; 
        if (bildirimNot != null) bildirimNot.text = aciklama; 

        if (dedektifSesKaynagi != null && !string.IsNullOrEmpty(sesIsmi))
        {
            AudioClip icSes = Resources.Load<AudioClip>("Audio/Sounds/" + sesIsmi);
            if (icSes != null)
            {
                dedektifSesKaynagi.clip = icSes;
                dedektifSesKaynagi.Play();
            }
        }

        StopCoroutine("BildirimAnimasyon"); 
        StartCoroutine(BildirimAnimasyon()); 
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

    void DelilSayaciniGuncelle() 
    {
        if (delilSayaciText != null)
            delilSayaciText.text = bulunanDelilSayisi + "/" + toplamDelilSayisi + " Nesne Klasörde"; 

        if (DiyalogYoneticisi.Instance != null && DiyalogYoneticisi.Instance.delilYazisiText != null)
        {
            DiyalogYoneticisi.Instance.delilYazisiText.text = "Toplanan Delil: " + bulunanDelilSayisi + " / 8";
        }
    }

    public void GorevTamamlandiPaneliAc() 
    {
        // GorevYoneticisi.cs ile uyumluluk için içi boş bırakılmıştır.
    }
}