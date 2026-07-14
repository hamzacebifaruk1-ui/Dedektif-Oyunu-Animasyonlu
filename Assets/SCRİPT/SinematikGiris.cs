using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class SinematikGiris : MonoBehaviour
{
    [Header("Görseller")]
    public Sprite[] gorseller; // Inspector'dan en az 6 adet fotoğraf atamalısın (0, 1, 2, 3, 4, 5)[cite: 65]

    [Header("UI Elemanları")]
    public Image arkaPlanGorsel;
    public Image karartmaEkrani;
    public TextMeshProUGUI altyaziText;
    public TextMeshProUGUI devamText;

    [Header("Müzik")]
    public AudioClip muzikDosyasi;
    private AudioSource muzikSource;

    [Header("Seslendirmeler")]
    public AudioClip[] seslendirmeler; // İsteğe bağlı tek tek seslendirme dosyaları[cite: 65]
    private AudioSource seslendirmeSource;

    private string[] altyazilar = new string[]
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

    private int[] gorselIndeksleri = new int[]
    {
        0, 0,  // 14 Kasım ve Karadeniz Limanı[cite: 65]
        1, 1,  // Murat Çelik ve Polis raporu[cite: 65]
        2,     // Telefon çalma[cite: 65]
        3,     // Eşinin konuşması[cite: 65]
        4,     // "Sen bu şehrin en karanlık gizemlerini..."[cite: 65]
        5, 5   // "Andım olsun ki..." ve son görsel[cite: 65]
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
        muzikSource.volume = 0.25f;
        
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
        Invoke("BaslamaIzniVer", 1.5f);
    }

    void BaslamaIzniVer() { basladi = true; }

    void Update()
    {
        if (!basladi || gecisYapiliyor) return;

        Keyboard klavye = Keyboard.current;
        if (klavye == null) return;

        // Herhangi bir anda SPACE veya ENTER ile sinematiği geçip doğrudan oyuna aktarabiliriz
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

        // EĞER Inspector'da özel ses dosyaları tanımlanmadıysa, Resources klasöründen ana giriş sesini çek
        if ((seslendirmeler == null || seslendirmeler.Length == 0) && seslendirmeSource != null)
        {
            AudioClip girisSesi = Resources.Load<AudioClip>("Audio/Dialogs/Dedektif_Giris_IcSes");
            if (girisSesi != null)
            {
                seslendirmeSource.clip = girisSesi;
                seslendirmeSource.Play();
            }
        }

        int mevcutGorselIndex = -1;

        for (int i = 0; i < altyazilar.Length; i++)
        {
            int yeniGorselIndex = gorselIndeksleri[i];
            if (yeniGorselIndex != mevcutGorselIndex && gorseller != null && yeniGorselIndex < gorseller.Length)
            {
                mevcutGorselIndex = yeniGorselIndex;
                yield return StartCoroutine(GorselGecis(gorseller[yeniGorselIndex]));
            }

            // Tek tek seslendirme atandıysa onları oynat
            if (seslendirmeler != null && i < seslendirmeler.Length && seslendirmeler[i] != null)
            {
                seslendirmeSource.clip = seslendirmeler[i];
                seslendirmeSource.Play();
            }

            yield return StartCoroutine(AltyaziYaz(altyazilar[i]));

            // Ses çalıyorsa bitmesini bekle, yoksa varsayılan olarak 3.5 saniye bekle
            if (seslendirmeSource != null && seslendirmeSource.isPlaying && (seslendirmeler != null && seslendirmeler.Length > 0))
                yield return new WaitWhile(() => seslendirmeSource.isPlaying);
            else
                yield return new WaitForSeconds(3.5f);

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
            yield return new WaitForSeconds(0.035f);
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