using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DelilYoneticisi : MonoBehaviour
{
    public static DelilYoneticisi Instance;

    [Header("Delil Ayarları")]
    public int toplamDelilSayisi = 11; // 5 Gerçek + 6 Sahte = 11 Toplam Nesne[cite: 3]
    private int bulunanDelilSayisi = 0;

    [Header("UI Elemanları")]
    public TextMeshProUGUI delilSayaciText; //[cite: 3]
    public GameObject gorevTamamPanel; //[cite: 3]

    [Header("Delil Bildirim Paneli")]
    public GameObject bildirimPanel; //[cite: 3]
    public TextMeshProUGUI bildirimBaslik; //[cite: 3]
    public TextMeshProUGUI bildirimIsim; //[cite: 3]
    public TextMeshProUGUI bildirimNot; //[cite: 3]

    [Header("Ses Sistemi Entegrasyonu")]
    public AudioSource dedektifSesKaynagi;

    // ElevenLabs ses dosyası isimlerini ve notları eşleştiren fonksiyon[cite: 3]
    public string DelilNotunuGetir(string delilAdi, out string sesDosyaAdi, out bool gercekMi)
    {
        gercekMi = true;
        switch (delilAdi)
        {
            // --- GERÇEK DELİLLER ---
            case "Kırık Vinç Teli":
                sesDosyaAdi = "Dedektif_Delil_Tel";
                return "Liflerde yıpranma yok. Ağzı spiral taşıyla, yani demir testeresiyle milimetrik olarak kesilmiş. Bu bir kaza değil, sabotaj.";
            case "İlaç":
                sesDosyaAdi = "Dedektif_Delil_Ilac";
                return "Kuvvetli bir sakinleştirici. Murat'ın o gece neden refleks kaybı yaşadığını ve vince zorla çıkarıldığını açıklıyor.";
            case "Şantiye Günlüğü":
                sesDosyaAdi = "Dedektif_Delil_Gunluk";
                return "Müdürün 'kazadan haberim yoktu' ifadesini yerle bir eden doğrudan planlama kanıtı.";
            case "USB Bellek":
                sesDosyaAdi = "Dedektif_Delil_USB";
                return "Güvenlik Rıza'nın elektrik kesildi yalanını çürüten orijinal kayıtlar. Murat'ın vince çıkarken arkasından birinin tırmandığını gösteriyor.";
            case "Şantaj Mektubu":
                sesDosyaAdi = "Dedektif_Delil_Mektup";
                return "Müdür Kemal Demir'i bu cinayete azmettiren holding baskısının resmi belgesi.";

            // --- SAHTE DELİLLER ---
            case "Kırık Kahve Kupası":
                sesDosyaAdi = "Dedektif_Sahte_Kupa"; gercekMi = false;
                return "Kupa yere düşüp kırılmış. İçinde sadece kahve kalıntıları var. Kazayla doğrudan bir bağı yok.";
            case "Paslı Çelik Halat":
                sesDosyaAdi = "Dedektif_Sahte_Halat"; gercekMi = false;
                return "Tamamen paslanmış ve çürümüş bir halat. Bu parça aylardır burada paslanmaya bırakılmış.";
            case "Eski Telsiz Bataryası":
                sesDosyaAdi = "Dedektif_Sahte_Batarya"; gercekMi = false;
                return "Aşırı ısınmadan şişmiş bir batarya. Telsiz ağındaki arızayı açıklayabilir ama Murat'ın düşüş sebebiyle bir ilgisi yok.";
            case "Liman Giriş Kartı":
                sesDosyaAdi = "Dedektif_Sahte_Kart"; gercekMi = false;
                return "Başka bir liman işçisine ait kayıp kart. Kaza gecesinden üç gün önce kayıp ilanı verilmiş, sahte bir iz.";
            case "Kirli İş Eldiveni":
                sesDosyaAdi = "Dedektif_Sahte_Eldiven"; gercekMi = false;
                return "Üzerinde standart motor yağı lekeleri olan bir eldiven. Herhangi bir parmak izi ya da boğuşma kanıtı taşımıyor.";
            case "Araba Anahtarı":
                sesDosyaAdi = "Dedektif_Sahte_Anahtar"; gercekMi = false;
                return "Sıradan bir binek araç anahtarı. Şantiyedeki şirket araçlarından birine ait, cinayet planıyla alakası yok.";
            
            default:
                sesDosyaAdi = "";
                return "Bu nesne inceleniyor...";
        }
    }

    void Awake()
    {
        Instance = this; //[cite: 3]
    }

    void Start()
    {
        if (gorevTamamPanel != null) gorevTamamPanel.SetActive(false); //[cite: 3]
        if (bildirimPanel != null) bildirimPanel.SetActive(false); //[cite: 3]
        DelilSayaciniGuncelle(); //[cite: 3]
    }

    public void DelilBulundu(string delilAdi) //[cite: 3]
    {
        string sesIsmi;
        bool gercekMi;
        string aciklama = DelilNotunuGetir(delilAdi, out sesIsmi, out gercekMi); //[cite: 3]

        if (bildirimBaslik != null) bildirimBaslik.text = gercekMi ? "KRİTİK DELİL BULUNDU" : "ŞÜPHELİ NESNE KAYDEDİLDİ"; //[cite: 3]
        if (bildirimIsim != null) bildirimIsim.text = delilAdi; //[cite: 3]
        if (bildirimNot != null) bildirimNot.text = aciklama; //[cite: 3]

        // --- ELEVENLABS SESİNİ OYNATMA MANTIĞI ---
        if (dedektifSesKaynagi != null && !string.IsNullOrEmpty(sesIsmi))
        {
            AudioClip icSes = Resources.Load<AudioClip>("Audio/Sounds/" + sesIsmi);
            if (icSes != null)
            {
                dedektifSesKaynagi.clip = icSes;
                dedektifSesKaynagi.Play();
            }
        }

        StopCoroutine("BildirimAnimasyon"); //[cite: 3]
        StartCoroutine(BildirimAnimasyon()); //[cite: 3]

        bulunanDelilSayisi++; //[cite: 3]
        DelilSayaciniGuncelle(); //[cite: 3]
    }

    IEnumerator BildirimAnimasyon() //[cite: 3]
    {
        if (bildirimPanel == null) yield break; //[cite: 3]

        bildirimPanel.SetActive(true); //[cite: 3]
        RectTransform rect = bildirimPanel.GetComponent<RectTransform>(); //[cite: 3]

        Vector2 baslangic = new Vector2(500f, rect.anchoredPosition.y); //[cite: 3]
        Vector2 hedef = new Vector2(-230f, rect.anchoredPosition.y); //[cite: 3]

        float sure = 0.4f; //[cite: 3]
        float gecenSure = 0f; //[cite: 3]

        while (gecenSure < sure) //[cite: 3]
        {
            gecenSure += Time.deltaTime; //[cite: 3]
            float t = Mathf.SmoothStep(0f, 1f, gecenSure / sure); //[cite: 3]
            rect.anchoredPosition = Vector2.Lerp(baslangic, hedef, t); //[cite: 3]
            yield return null; //[cite: 3]
        }

        yield return new WaitForSeconds(6f); //[cite: 3]

        gecenSure = 0f; //[cite: 3]
        while (gecenSure < sure) //[cite: 3]
        {
            gecenSure += Time.deltaTime; //[cite: 3]
            float t = Mathf.SmoothStep(0f, 1f, gecenSure / sure); //[cite: 3]
            rect.anchoredPosition = Vector2.Lerp(hedef, baslangic, t); //[cite: 3]
            yield return null; //[cite: 3]
        }

        bildirimPanel.SetActive(false); //[cite: 3]
    }

    void DelilSayaciniGuncelle() //[cite: 3]
    {
        if (delilSayaciText != null)
            delilSayaciText.text = bulunanDelilSayisi + "/" + toplamDelilSayisi + " Nesne Klasörde"; //[cite: 3]
    }

    public void GorevTamamlandiPaneliAc() //[cite: 3]
    {
        if (gorevTamamPanel != null) gorevTamamPanel.SetActive(true); //[cite: 3]
        Invoke("PaneliKapat", 2f); //[cite: 3]
    }

    void PaneliKapat() //[cite: 3]
    {
        if (gorevTamamPanel != null) gorevTamamPanel.SetActive(false); //[cite: 3]
        if (SecimYoneticisi.Instance != null)
            SecimYoneticisi.Instance.SecimEkraniniAc(); //[cite: 3]
    }
}