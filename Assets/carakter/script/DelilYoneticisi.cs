using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DelilYoneticisi : MonoBehaviour
{
    public static DelilYoneticisi Instance;

    [Header("Delil Ayarları")]
    public int toplamDelilSayisi = 5;
    private int bulunanDelilSayisi = 0;

    [Header("UI Elemanları")]
    public TextMeshProUGUI delilSayaciText;
    public GameObject gorevTamamPanel;

    [Header("Delil Bildirim Paneli")]
    public GameObject bildirimPanel;
    public TextMeshProUGUI bildirimBaslik;
    public TextMeshProUGUI bildirimIsim;
    public TextMeshProUGUI bildirimNot;

    // Sabitlenmiş ve eşleşen delil notları
    string DelilNotunuGetir(string delilAdi)
    {
        switch (delilAdi)
        {
            case "Yırtık Bakım Defteri":
                return "Bakım kayıtları sahte. Biri bu defterin sayfalarını bilerek yırtmış.";
            case "Kırık Vinç Teli":
                return "Bu çelik tel yıpranmayla kopmaz. Ağzı spiral taşıyla kesilmiş gibi duruyor.";
            case "Anonim Not":
                return "Murat'ın baretine sıkıştırılan not: 'Konuşursan sonun limanın dibi olur' yazıyor.";
            case "Güvenlik Kamera Kaydı":
                return "Gece 02:00 kayıtları. Müdür Kemal'in elinde bir aletle vinç dairesine girdiğini gösteriyor.";
            case "İlaç Kutusu":
                return "Murat'ın kullandığı ağır göz ilacı. Gece vardiyasında çalışması yasal olarak imkansızdı.";
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

        if (bulunanDelilSayisi >= toplamDelilSayisi)
            Invoke("GorevTamamlandi", 3.5f);
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

    void GorevTamamlandi()
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