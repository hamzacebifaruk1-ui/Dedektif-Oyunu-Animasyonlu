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

    // Her delilin adına göre not döndür
    string DelilNotunuGetir(string delilAdi)
    {
        switch (delilAdi)
        {
            case "Yırtık Bakım Defteri":
                return "Bakım kayıtları sahte. Biri bu defterde oynama yapmış.";
            case "Kırık Vinç Teli":
                return "Bu tel yıpranmadan kopmaz. Bilerek mi kesildi?";
            case "Anonim Not":
                return "Bu notu kim yazdı? Murat uyarılmış ama gitmemiş.";
            case "Güvenlik Kamera Kaydı":
                return "Gece 2'de makine odasında kim var?";
   case "İlaç Kutusu":
    return "Murat'ın ilacı konteyner içinde. Görmesi bozuktu, gece çalışması yasaktı.";
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
        gorevTamamPanel.SetActive(false);
        bildirimPanel.SetActive(false);
        DelilSayaciniGuncelle();
    }

    public void DelilBulundu(string delilAdi)
    {
        bildirimBaslik.text = "DELİL BULUNDU";
        bildirimIsim.text = delilAdi;
        bildirimNot.text = DelilNotunuGetir(delilAdi);

        StopCoroutine("BildirimAnimasyon");
        StartCoroutine(BildirimAnimasyon());

        bulunanDelilSayisi++;
        DelilSayaciniGuncelle();

        if (bulunanDelilSayisi >= toplamDelilSayisi)
            Invoke("GorevTamamlandi", 3.5f);
    }

    IEnumerator BildirimAnimasyon()
    {
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
        delilSayaciText.text = bulunanDelilSayisi + "/" + toplamDelilSayisi + " Delil Bulundu";
    }

    void GorevTamamlandi()
    {
        gorevTamamPanel.SetActive(true);
        Invoke("PaneliKapat", 2f);
    }

    void PaneliKapat()
    {
        gorevTamamPanel.SetActive(false);
        SecimYoneticisi.Instance.SecimEkraniniAc();
    }
}