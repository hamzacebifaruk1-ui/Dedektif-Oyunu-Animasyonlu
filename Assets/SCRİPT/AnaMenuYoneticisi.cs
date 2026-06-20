using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class AnaMenuYoneticisi : MonoBehaviour
{
    [Header("UI Elemanları")]
    public Image karartmaEkrani;

    [Header("Ses Ayarları")]
    public AudioSource menuMusic; // Inspector'dan Audio Source'u buraya bağla
    public float sesGecisSuresi = 1.0f;

    void Start()
    {
        // Fareyi serbest bırak ve görünür yap
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Sahne açılırken karartmayı yavaşça kaldır
        if (karartmaEkrani != null)
        {
            karartmaEkrani.gameObject.SetActive(true);
            StartCoroutine(FadeYap(1f, 0f, 1.5f));
        }

        // Müzik varsa ve çalmıyorsa başlat
        if (menuMusic != null && !menuMusic.isPlaying)
        {
            menuMusic.Play();
        }
    }

    // --- BUTON METODLARI ---

    public void OyunuBaslat()
    {
        // Build Settings Index 2: GirisSahnesi (Senin sıralamana göre)
        StartCoroutine(SahneGecAsync(2)); 
    }

    public void AyarlarGit()
    {
        // Build Settings Index 4: AyarlarSahnesi
        StartCoroutine(SahneGecAsync(4)); 
    }

    public void MenuyeDon()
    {
        // Build Settings Index 1: AnaMenu
        StartCoroutine(SahneGecAsync(1));
    }

    public void OyundanCik()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    // --- GEÇİŞ SİSTEMİ (Müzik Fade Destekli) ---

    IEnumerator SahneGecAsync(int sahneIndex)
    {
        // 1. Ekran kararırken müziği de yavaşça kıs
        if (menuMusic != null)
        {
            StartCoroutine(MuzikKismayaBasla(sesGecisSuresi));
        }

        yield return StartCoroutine(FadeYap(0f, 1f, 1f));
        
        // 2. Arka planda sahneyi yükle
        AsyncOperation operasyon = SceneManager.LoadSceneAsync(sahneIndex);
        
        while (!operasyon.isDone)
        {
            yield return null;
        }
    }

    // Müziği yavaşça kısmak için yardımcı Coroutine
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