using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DelilYoneticisi : MonoBehaviour
{
    public static DelilYoneticisi Instance;

    [Header("Delil Ayarları")]
    public int toplamDelilSayisi = 11; // 5 Gerçek + 6 Sahte = 11 Toplam Nesne
    private int bulunanDelilSayisi = 0;

    [Header("UI Elemanları")]
    public TextMeshProUGUI delilSayaciText;
    public GameObject gorevTamamPanel;

    [Header("Delil Bildirim Paneli")]
    public GameObject bildirimPanel;
    public TextMeshProUGUI bildirimBaslik;
    public TextMeshProUGUI bildirimIsim;
    public TextMeshProUGUI bildirimNot;

    [Header("Müdürün Odası Görev Kontrolü")]
    private bool yirtikDefterBulundu = false;
    private bool anonimNotBulundu = false;
    private bool yeniGorevTetiklendi = false;

    string DelilNotunuGetir(string delilAdi)
    {
        switch (delilAdi)
        {
            // --- GERÇEK DELİLLER ---
            case "Kırık Vinç Kancası":
                return "Vincin kancasındaki derin çatlaklar gizlenmeye çalışılmış. Kaza göz göre göre gelmiş.";
            case "Yırtık Bakım Defteri":
                return "Bakım kayıtları sahte. Biri bu defterin sayfalarını bilerek yırtmış.";
            case "Kırık Vinç Teli":
                return "Bu çelik tel yıpranmayla kopmaz. Ağzı spiral taşıyla kesilmiş gibi duruyor.";
            case "Anonim Not":
                return "Murat'ın baretine sıkıştırılan not: 'Konuşursan sonun limanın dibi olur' yazıyor.";
            case "Güvenlik Kamera Kaydı":
                return "Gece 02:00 kayıtları. Müdür Kemal'in elinde bir aletle vinç dairesine girdiğini gösteriyor.";
            case "İlaç":
                return "Murat'ın kullandığı ağır göz ilacı. Gece vardiyasında çalışması yasal olarak imkansızdı.";

            // --- SAHTE DELİLLER ---
            case "Kırık Kahve Kupası":
                return "İçinde sadece kahve tortusu var. Kazayla doğrudan bir bağı yok.";
            case "Paslı Çelik Halat":
                return "Aylardır burada çürümeye bırakılmış eski bir parça, olayla ilgisiz.";
            case "Eski Telsiz Bataryası":
                return "Sıradan bir batarya arızası, Murat'ın düşüş sebebiyle bağı yok.";
            case "Liman Giriş Kartı":
                return "Üç gün önce kaybolmuş sıradan bir kart. Bizi şaşırtmak için bırakılmış olabilir.";
            case "Kirli İş Eldiveni":
                return "Standart motor yağı lekeleri taşıyor, şüpheli bir iz yok.";
            case "Araba Anahtarı":
                return "Sıradan bir binek araç anahtarı, cinayet planıyla alakası yok.";
            default:
                return "Bu delil inceleniyor...";
        }
    }

    void Awake()
    {
        Instance = this;
    }

    public int BulunanDelilSayisiniGetir()
    {
        return bulunanDelilSayisi;
    }

    void Start()
    {
        if (gorevTamamPanel != null) gorevTamamPanel.SetActive(false);
        if (bildirimPanel != null) bildirimPanel.SetActive(false);
        DelilSayaciniGuncelle();
    }

    public void DelilBulundu(string delilAdi)
    {
        if (bildirimBaslik != null) bildirimBaslik.text = "DELİL BULUNDU";
        if (bildirimIsim != null) bildirimIsim.text = delilAdi;
        if (bildirimNot != null) bildirimNot.text = DelilNotunuGetir(delilAdi);

        StopCoroutine("BildirimAnimasyon");
        StartCoroutine(BildirimAnimasyon());

        bulunanDelilSayisi++;
        DelilSayaciniGuncelle();

        if (delilAdi == "Yırtık Bakım Defteri")
        {
            yirtikDefterBulundu = true;
        }
        else if (delilAdi == "Anonim Not")
        {
            anonimNotBulundu = true;
        }

        if (yirtikDefterBulundu && anonimNotBulundu && !yeniGorevTetiklendi)
        {
            yeniGorevTetiklendi = true;
            MudurOdasindakiGoreviTamamla();
        }
    }

    void MudurOdasindakiGoreviTamamla()
    {
        if (GorevYoneticisi.Instance != null)
        {
            GorevYoneticisi.Instance.OdaVePanoTamamla(); 
        }
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
            delilSayaciText.text = bulunanDelilSayisi + "/" + toplamDelilSayisi + " Delil Bulundu";
    }

    public void GorevTamamlandiPaneliAc()
    {
        if (gorevTamamPanel != null) gorevTamamPanel.SetActive(true);
        Invoke("PaneliKapat", 2f);
    }

    void PaneliKapat()
    {
        if (gorevTamamPanel != null) gorevTamamPanel.SetActive(false);
        if (SecimYoneticisi.Instance != null)
            SecimYoneticisi.Instance.SecimEkraniniAc();
    }
}