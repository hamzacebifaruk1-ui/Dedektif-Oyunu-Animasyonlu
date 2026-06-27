using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class SinematikGiris : MonoBehaviour
{
    [Header("Görseller")]
    public Sprite[] gorseller; // Müfettiş Notu: Inspector'dan en az 6 adet fotoğraf atamalısın (0, 1, 2, 3, 4, 5)

    [Header("UI")]
    public Image arkaPlanGorsel;
    public Image karartmaEkrani;
    public TextMeshProUGUI altyaziText;
    public TextMeshProUGUI devamText;

    [Header("Müzik")]
    public AudioClip muzikDosyasi;
    private AudioSource muzikSource;

    [Header("Seslendirme")]
    public AudioClip[] seslendirmeler;
    private AudioSource seslendirmeSource;

    string[] altyazilar = new string[]
    {
        "14 Kasım. Gece 02:30.",
        "Karadeniz Limanı — 3 Numaralı Yükleme İskelesi.",
        "Vinç operatörü Murat Çelik, 18 metre yüksekten düşerek can verdi.",
        "Polis raporu: Ekipman arızası ve işçi ihmali dedi. Dosya kapatıldı.",
        "Ama bu sabah dedektiflik ofisinin telefonu acı acı çaldı.",
        "Murat'ın eşi ağlayarak konuşuyordu:",
        "'Kocam tehdit ediliyordu dedektif. Onu o vince zorla çıkardılar, lütfen yardım edin!'",
        "Sen bu şehrin en karanlık gizemlerini çözen dedektifsin.",
        "Andım olsun ki, bu gece şantiyede gerçeği bulmak için tek bir şansın var."
    };

    // TAMİR EDİLDİ: Altyazı dizisi 9 elemanlı olduğu için bu dizi de tam 9 elemana eşitlendi!
  // GÜNCELLENDİ: Tam 9 elemanlı, son iki satırda da son görseli (5) gösterecek şekilde ayarlandı
    int[] gorselIndeksleri = new int[]
    {
        0, 0,  // 14 Kasım ve Karadeniz Limanı
        1, 1,  // Murat Çelik ve Polis raporu
        2,     // Telefon çalma
        3,     // Eşinin konuşması
        4,     // "Sen bu şehrin en karanlık gizemlerini çözen dedektifsin."
        5, 5   // "Andım olsun ki..." ve bitişte son görsel (5)
    };
    private bool basladi = false;
    private bool gecisYapiliyor = false;

    void Awake()
    {
        AudioSource[] kaynaklar = GetComponents<AudioSource>();
        if (kaynaklar.Length >= 2)
        {
            muzikSource = kaynaklar[0];
            seslendirmeSource = kaynaklar[1];
        }
        else
        {
            muzikSource = gameObject.AddComponent<AudioSource>();
            seslendirmeSource = gameObject.AddComponent<AudioSource>();
        }

        muzikSource.loop = true;
        muzikSource.playOnAwake = false;
        muzikSource.volume = 0.4f;
        seslendirmeSource.loop = false;
        seslendirmeSource.playOnAwake = false;
        seslendirmeSource.volume = 1f;

        if (muzikDosyasi != null)
            muzikSource.clip = muzikDosyasi;
    }

    void Start()
    {
        if (devamText != null) devamText.gameObject.SetActive(false);
        if (altyaziText != null)
        {
            altyaziText.text = "";
            altyaziText.alpha = 0f;
        }
        karartmaEkrani.color = new Color(0, 0, 0, 1);
        arkaPlanGorsel.color = new Color(1, 1, 1, 0);
        StartCoroutine(SinematikBaslat());
        Invoke("BaslamaIzniVer", 2f);
    }

    void BaslamaIzniVer() { basladi = true; }

    void Update()
    {
        if (!basladi || gecisYapiliyor) return;

        Keyboard klavye = Keyboard.current;
        if (klavye == null) return;

        if (klavye.spaceKey.wasPressedThisFrame || klavye.enterKey.wasPressedThisFrame)
        {
            gecisYapiliyor = true;
            StopAllCoroutines();
            if (seslendirmeSource != null) seslendirmeSource.Stop();
            StartCoroutine(OyunaGec());
        }
    }

    IEnumerator SinematikBaslat()
    {
        if (muzikSource != null) muzikSource.Play();

        yield return StartCoroutine(FadeYap(1f, 0f, 1.5f));

        int mevcutGorselIndex = -1;

        for (int i = 0; i < altyazilar.Length; i++)
        {
            int yeniGorselIndex = gorselIndeksleri[i];
            if (yeniGorselIndex != mevcutGorselIndex && gorseller != null && yeniGorselIndex < gorseller.Length)
            {
                mevcutGorselIndex = yeniGorselIndex;
                yield return StartCoroutine(GorselGecis(gorseller[yeniGorselIndex]));
            }

            if (seslendirmeSource != null && seslendirmeler != null && i < seslendirmeler.Length && seslendirmeler[i] != null)
            {
                seslendirmeSource.clip = seslendirmeler[i];
                seslendirmeSource.Play();
            }

            yield return StartCoroutine(AltyaziYaz(altyazilar[i]));

            if (seslendirmeSource != null && seslendirmeSource.isPlaying)
                yield return new WaitWhile(() => seslendirmeSource.isPlaying);
            else
                yield return new WaitForSeconds(2.5f);

            yield return StartCoroutine(AltyaziSil(0.5f));
            yield return new WaitForSeconds(0.3f);
        }

        if (devamText != null)
        {
            devamText.gameObject.SetActive(true);
            devamText.text = "[ SPACE — Limana Giriş Yap ]";
        }
    }

    IEnumerator GorselGecis(Sprite yeniGorsel)
    {
        if (arkaPlanGorsel == null) yield break;

        float gecenSure = 0f;
        Color renk = arkaPlanGorsel.color;

        while (gecenSure < 0.4f)
        {
            gecenSure += Time.deltaTime;
            renk.a = Mathf.Lerp(renk.a, 0f, gecenSure / 0.4f);
            arkaPlanGorsel.color = renk;
            yield return null;
        }

        arkaPlanGorsel.sprite = yeniGorsel;

        gecenSure = 0f;
        while (gecenSure < 0.8f)
        {
            gecenSure += Time.deltaTime;
            renk.a = Mathf.Lerp(0f, 1f, gecenSure / 0.8f);
            arkaPlanGorsel.color = renk;
            yield return null;
        }
    }

    IEnumerator AltyaziYaz(string metin)
    {
        if (altyaziText == null) yield break;
        altyaziText.text = "";
        altyaziText.alpha = 1f;
        foreach (char harf in metin)
        {
            altyaziText.text += harf;
            yield return new WaitForSeconds(0.04f);
        }
    }

    IEnumerator AltyaziSil(float sure)
    {
        if (altyaziText == null) yield break;
        float gecenSure = 0f;
        while (gecenSure < sure)
        {
            gecenSure += Time.deltaTime;
            altyaziText.alpha = Mathf.Lerp(1f, 0f, gecenSure / sure);
            yield return null;
        }
        altyaziText.text = "";
        altyaziText.alpha = 1f;
    }

    IEnumerator FadeYap(float baslangic, float bitis, float sure)
    {
        if (karartmaEkrani == null) yield break;
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

    IEnumerator OyunaGec()
    {
        yield return StartCoroutine(FadeYap(0f, 1f, 1.2f));
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}