using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SecimYoneticisi : MonoBehaviour
{
    public static SecimYoneticisi Instance;

    [Header("Paneller")]
    public GameObject secimPaneli;
    public GameObject dogruBitisPaneli;
    public GameObject yanlisBitisPaneli;

    [Header("Bitiş Metni")]
    public TextMeshProUGUI bitisMetni;

    [Header("Karartma")]
    public Image karartmaEkrani;

    string dogruMetin = "DOĞRU!\n\nKemal Demir gözaltına alındı!\n\nSahte bakım kayıtları ve güvenlik kamera görüntüleri mahkemede aleyhine kullanıldı.\n\nMurat'ın ailesi davayı kazandı.";
    string yanlisMetin1 = "YANLIŞ!\n\nAhmet Yılmaz tutuklandı...\n\nAncak deliller yetersiz bulundu.\n\nGerçek fail kaçtı. Dava kapandı.";
    string yanlisMetin2 = "YANLIŞ!\n\nDosya kapatıldı.\n\nAma sen hâlâ emin değilsin.\n\nBir yerde bir şeyleri kaçırdın...";

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        secimPaneli.SetActive(false);
        dogruBitisPaneli.SetActive(false);
        yanlisBitisPaneli.SetActive(false);
        karartmaEkrani.color = new Color(0, 0, 0, 0);
    }

    public void SecimEkraniniAc()
    {
        secimPaneli.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

  public void SucluSec(int secim)
{
    secimPaneli.SetActive(false);

    if (secim == 0)
    {
        bitisMetni.text = dogruMetin;
        dogruBitisPaneli.SetActive(true);
    }
    else if (secim == 1)
    {
        bitisMetni.text = yanlisMetin1;
        yanlisBitisPaneli.SetActive(true);
        // Yanlış paneldeki text'i de doldur
        yanlisBitisPaneli.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = yanlisMetin1;
    }
    else
    {
        bitisMetni.text = yanlisMetin2;
        yanlisBitisPaneli.SetActive(true);
        yanlisBitisPaneli.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = yanlisMetin2;
    }

    StartCoroutine(OyunuBitir());
}
   IEnumerator OyunuBitir()
{
    yield return new WaitForSeconds(5f);
    yield return StartCoroutine(FadeYap(0f, 1f, 2f));

    // Panelleri kapat
    dogruBitisPaneli.SetActive(false);
    yanlisBitisPaneli.SetActive(false);

    // Fareyi kilitle, oyuna devam et
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
}

    IEnumerator FadeYap(float baslangic, float bitis, float sure)
    {
        float gecenSure = 0f;
        Color renk = karartmaEkrani.color;
        while (gecenSure < sure)
        {
            gecenSure += Time.deltaTime;
            renk.a = Mathf.Lerp(baslangic, bitis, gecenSure / sure);
            karartmaEkrani.color = renk;
            yield return null;
        }
    }
}
