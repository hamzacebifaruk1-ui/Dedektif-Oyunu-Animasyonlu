using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class AnaMenuYoneticisi : MonoBehaviour
{
    [Header("UI Elemanları")]
    public Image karartmaEkrani;

    [Header("Ses Ayarları")]
    public AudioSource menuMusic; // Inspector'dan arka plan müzik AudioSource'unu bağla
    public float sesGecisSuresi = 1.0f;

    void Start()
    {
        // Fare imlecini serbest bırak ve görünür kıl
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Sahne açılırken karartmayı yavaşça kaldır
        if (karartmaEkrani != null)
        {
            karartmaEkrani.gameObject.SetActive(true);
            StartCoroutine(FadeYap(1f, 0f, 1.5f));
        }

        // Müzik varsa ve çalmıycorsa başlat
        if (menuMusic != null && !menuMusic.isPlaying)
        {
            menuMusic.Play();
        }
    }

    public void OyunuBaslat()
    {
        // Giriş Sinematiği Sahnesine yumuşak geçiş yap (Build Settings index: 1 veya sonraki sahne)
        StartCoroutine(SahneGecAsync(SceneManager.GetActiveScene().buildIndex + 1)); 
    }

    public void AyarlarGit()
    {
        // Ayarlar Sahnesine git (Build Settings index: 4 veya kendi belirlediğin)
        StartCoroutine(SahneGecAsync(4)); 
    }

    public void MenuyeDon()
    {
        // Ana Menü Sahnesine dön (Build Settings index: 0)
        StartCoroutine(SahneGecAsync(0));
    }

    public void OyundanCik()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    IEnumerator SahneGecAsync(int sahneIndex)
    {
        // 1. Ekran kararırken müziği de yavaşça kıs
        if (menuMusic != null)
        {
            StartCoroutine(MuzikKismayaBasla(sesGecisSuresi));
        }

        yield return StartCoroutine(FadeYap(0f, 1f, 1.2f));
        
        // 2. Sahneyi yükle
        SceneManager.LoadScene(sahneIndex);
    }

    IEnumerator MuzikKismayaBasla(float sure)
    {
        float baslangicSes = menuMusic.volume;
        float gecenSure = 0;

        while (gecenSure < sure)
        {
            gecenSure += Time.deltaTime;
            menuMusic.volume = Mathf.Lerp(baslangicSes, 0, gecenSure / sure);
            yield return null;
        }
        menuMusic.volume = 0;
    }

    IEnumerator FadeYap(float baslangic, float bitis, float sure)
    {
        if (karartmaEkrani == null) yield break;

        karartmaEkrani.gameObject.SetActive(true);
        float gecenSure = 0f;
        Color renk = karartmaEkrani.color;

        while (gecenSure < sure)
        {
            gecenSure += Time.deltaTime;
            renk.a = Mathf.Lerp(baslangic, bitis, gecenSure / sure);
            karartmaEkrani.color = renk;
            yield return null;
        }

        renk.a = bitis;
        karartmaEkrani.color = renk;

        if (bitis <= 0.1f)
        {
            karartmaEkrani.gameObject.SetActive(false);
        }
    }
}