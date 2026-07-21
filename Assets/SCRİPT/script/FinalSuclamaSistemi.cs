using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class FinalSuclamaSistemi : MonoBehaviour
{
    public static FinalSuclamaSistemi Instance;

    [Header("Ses Efektleri")]
    public AudioClip rizaAsagilamaSesi;
    public AudioClip ahmetAsagilamaSesi;
    public AudioClip kelKabulSesi;

    [Header("UI Panelleri")]
    public GameObject tebriklerPaneli;
    public GameObject tekrarDenePaneli;

    [Header("Butonlar")]
    public Button anaMenuButonu;
    public Button tekrarDeneButonu;

    private AudioSource audioSource;
    private bool suclamaYapildiMi = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        if (tebriklerPaneli != null) tebriklerPaneli.SetActive(false);
        if (tekrarDenePaneli != null) tekrarDenePaneli.SetActive(false);

        if (anaMenuButonu != null)
            anaMenuButonu.onClick.AddListener(AnaMenuyeDon);

        if (tekrarDeneButonu != null)
            tekrarDeneButonu.onClick.AddListener(OyunuYenidenBaslat);
    }

    // NPC'lere tıklayınca bu metod çağrılır (NPC Adı string olarak gönderilir)
    public void SuclamaYap(string npcAdi)
    {
        if (suclamaYapildiMi) return;

        // Sadece FinalSuclama aşamasındaysak çalışır
        if (GorevYoneticisi.Instance != null && GorevYoneticisi.Instance.mevcutAsama != GorevYoneticisi.GorevAsamasi.FinalSuclama)
            return;

        suclamaYapildiMi = true;

        if (npcAdi == "Kel")
        {
            StartCoroutine(DogruSucluAkisi());
        }
        else if (npcAdi == "Riza")
        {
            StartCoroutine(YanlisSucluAkisi(rizaAsagilamaSesi));
        }
        else if (npcAdi == "Ahmet")
        {
            StartCoroutine(YanlisSucluAkisi(ahmetAsagilamaSesi));
        }
    }

    // 🟢 GERÇEK SUÇLU (KEL) SEÇİLDİĞİNDE
    private IEnumerator DogruSucluAkisi()
    {
        if (kelKabulSesi != null)
        {
            audioSource.PlayOneShot(kelKabulSesi);
            yield return new WaitForSeconds(kelKabulSesi.length);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (tebriklerPaneli != null) tebriklerPaneli.SetActive(true);
    }

    // 🔴 YANLIŞ SUÇLU (RIZA VEYA AHMET) SEÇİLDİĞİNDE
    private IEnumerator YanlisSucluAkisi(AudioClip asagilamaSesi)
    {
        if (asagilamaSesi != null)
        {
            audioSource.PlayOneShot(asagilamaSesi);
            yield return new WaitForSeconds(asagilamaSesi.length);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (tekrarDenePaneli != null) tekrarDenePaneli.SetActive(true);
    }

    public void AnaMenuyeDon()
    {
        // Ana Menü sahnenizin adı "AnaMenu" ise buraya yazın
        SceneManager.LoadScene("AnaMenu"); 
    }

    public void OyunuYenidenBaslat()
    {
        // Mevcut sahneyi baştan yükler
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}