using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class SinematikGiris : MonoBehaviour
{
    [Header("Görseller")]
    public Sprite[] gorseller;

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
        "Vinç operatörü Murat Çelik, 18 metre yüksekten düştü.",
        "Polis raporu: Ekipman arızası dedi. Dava kapatıldı.",
        "Ama bu sabah telefon çaldı.",
        "Murat'ın eşi ağlayarak konuştu:",
        "'Kocam tehdit almıştı. Birisi onu öldürdü.'",
        "Sen bu şehrin en iyi dedektifisin.",
        "Ve bu gece gerçeği bulmak için tek şansın var."
    };

    // KRİTİK GÜNCELLEME BURASI: 6. Görsel (Index 5) çıkarıldı, Son iki konuşma 7. Görsele (Index 6) bağlandı!
    int[] gorselIndeksleri = new int[]
    {
        0, 0,  // 1. ve 2. Yazı -> Görsel 1 (Liman girişi)
        1, 1,  // 3. ve 4. Yazı -> Görsel 2 (Kaza yeri)
        2,     // 5. Yazı -> Görsel 3 (Telefon)
        3,     // 6. Yazı -> Görsel 4 (Ağlayan kadın)
        4,     // 7. Yazı -> Görsel 5 (Tehdit mektubu)
        6, 6   // 8. ve 9. Yazı -> Görsel 7 (Dedektif limana bakıyor)
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
        devamText.gameObject.SetActive(false);
        altyaziText.text = "";
        altyaziText.alpha = 0f;
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
            if (yeniGorselIndex != mevcutGorselIndex)
            {
                mevcutGorselIndex = yeniGorselIndex;
                yield return StartCoroutine(GorselGecis(gorseller[yeniGorselIndex]));
            }

            if (seslendirmeSource != null && seslendirmeler != null && i < seslendirmeler.Length)
            {
                seslendirmeSource.clip = seslendirmeler[i];
                seslendirmeSource.Play();
            }

            yield return StartCoroutine(AltyaziYaz(altyazilar[i]));

            if (seslendirmeSource != null && seslendirmeSource.isPlaying)
                yield return new WaitWhile(() => seslendirmeSource.isPlaying);
            else
                yield return new WaitForSeconds(2f);

            yield return StartCoroutine(AltyaziSil(0.5f));
            yield return new WaitForSeconds(0.3f);
        }

        devamText.gameObject.SetActive(true);
        devamText.text = "[ SPACE — Devam Et ]";
    }

    IEnumerator GorselGecis(Sprite yeniGorsel)
    {
        float gecenSure = 0f;
        Color renk = arkaPlanGorsel.color;

        while (gecenSure < 0.6f)
        {
            gecenSure += Time.deltaTime;
            renk.a = Mathf.Lerp(renk.a, 0f, gecenSure / 0.6f);
            arkaPlanGorsel.color = renk;
            yield return null;
        }

        arkaPlanGorsel.sprite = yeniGorsel;

        gecenSure = 0f;
        while (gecenSure < 1f)
        {
            gecenSure += Time.deltaTime;
            renk.a = Mathf.Lerp(0f, 1f, gecenSure / 1f);
            arkaPlanGorsel.color = renk;
            yield return null;
        }
    }

    IEnumerator AltyaziYaz(string metin)
    {
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
        yield return StartCoroutine(FadeYap(0f, 1f, 1.5f));
        SceneManager.LoadScene(3); // Limana kusursuz geçiş!
    }
}