using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class SecimYoneticisi : MonoBehaviour
{
    public static SecimYoneticisi Instance;

    [Header("Paneller")]
    public GameObject secimPanel;
    public GameObject dogruBitisPanel;
    public GameObject yanlisBitisPanel;

    [Header("Her Panele Özel Metin Kutuları")]
    public TextMeshProUGUI dogruPanelMetni;  
    public TextMeshProUGUI yanlisPanelMetni; 

    [Header("Doğru Panel Butonları")]
    public GameObject dogruAnaMenuButonu;  // 1 dakika sonra çıkacak buton

    [Header("Yanlış Panel Butonları (ANINDA ÇIKACAK)")]
    public GameObject yanlisTekrarButonu;  
    public GameObject yanlisAnaMenuButonu; 

    [Header("Karartma")]
    public Image karartmaEkrani;

    [Header("Kapatılacak Diğer Arayüzler")]
    public GameObject diyalogPanel;
    public GameObject notDefteriPanel;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (secimPanel != null) secimPanel.SetActive(false);
        if (dogruBitisPanel != null) dogruBitisPanel.SetActive(false);
        if (yanlisBitisPanel != null) yanlisBitisPanel.SetActive(false);
        
        // Butonları oyun başında kapatıyoruz
        if (dogruAnaMenuButonu != null) dogruAnaMenuButonu.SetActive(false);
        if (yanlisTekrarButonu != null) yanlisTekrarButonu.SetActive(false);
        if (yanlisAnaMenuButonu != null) yanlisAnaMenuButonu.SetActive(false);
        
        if (karartmaEkrani != null) karartmaEkrani.gameObject.SetActive(false);
    }

    public void SecimEkraniniAc()
    {
        if (secimPanel != null)
        {
            secimPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void SucluSec(int secimIndeksi)
    {
        if (secimPanel != null) secimPanel.SetActive(false);
        if (diyalogPanel != null) diyalogPanel.SetActive(false);
        if (notDefteriPanel != null) notDefteriPanel.SetActive(false);

        StartCoroutine(BitisAkisi(secimIndeksi));
    }

    IEnumerator BitisAkisi(int secimIndeksi)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 1. Karartma Akışı
        if (karartmaEkrani != null)
        {
            karartmaEkrani.gameObject.SetActive(true);
            float sure = 1.0f;
            float gecenSure = 0f;
            Color renk = karartmaEkrani.color;
            while (gecenSure < sure)
            {
                gecenSure += Time.deltaTime;
                renk.a = Mathf.Clamp01(gecenSure / sure);
                karartmaEkrani.color = renk;
                yield return null;
            }
        }

        yield return new WaitForSeconds(0.2f);

        // 2. Doğru Seçim Mantığı (Kemal Demir)
        if (secimIndeksi == 0)
        {
            if (dogruBitisPanel != null)
            {
                dogruBitisPanel.SetActive(true);
                dogruBitisPanel.transform.SetAsLastSibling();
            }
            if (dogruPanelMetni != null)
            {
                dogruPanelMetni.text = "<b><color=#00FF00>■ VAKA ÇÖZÜLDÜ ■</color></b>\n\n" +
                                       "Müdür Kemal Demir'in önüne USB kamera kayıtlarını koydun. Vinç dairesine gizlice " +
                                       "girdiğini gören Kemal daha fazla inkâr edemedi. Murat'ın kazasının bir sabotaj olduğunu ve " +
                                       "ihmalleri gizlemek için yapıldığını itiraf etti. Adalet yerini buldu dedektif!";
            }

            // 1 Dakika bekle ve sadece Ana Menü butonunu aç
            yield return new WaitForSeconds(10f);
            if (dogruAnaMenuButonu != null) dogruAnaMenuButonu.SetActive(true);
        }
        // 3. Yanlış Seçim Mantığı (Deli Rıza veya Kaza)
        else
        {
            if (yanlisBitisPanel != null)
            {
                yanlisBitisPanel.SetActive(true);
                yanlisBitisPanel.transform.SetAsLastSibling();
            }

            // Yanlış paneldeki devasa butonları ANINDA görünür yap
            if (yanlisTekrarButonu != null) yanlisTekrarButonu.SetActive(true);
            if (yanlisAnaMenuButonu != null) yanlisAnaMenuButonu.SetActive(true);

            if (secimIndeksi == 1) // Deli Rıza
            {
                if (yanlisPanelMetni != null)
                {
                    yanlisPanelMetni.text = "<b><color=#FF3B30>■ YANLIŞ TEŞHİS ■</color></b>\n\n" +
                                            "Güvenlik görevlisi Deli Rıza'yı suçladın. Rıza korkudan ve baskıdan her şeyi kabul etse de, " +
                                            "mahkemede avukatlar elindeki delillerin asıl faili işaret ettiğini kanıtladı. " +
                                            "Rıza haksız yere suçlanırken, Müdür Kemal tüm izleri temizleyerek tersaneden kaçtı!";
                }
            }
            else // Kaza Dosyası
            {
                if (yanlisPanelMetni != null)
                {
                    yanlisPanelMetni.text = "<b><color=#A0A0A0>■ DOSYA KAPATILDI ■</color></b>\n\n" +
                                            "Tüm o şüpheli delillere rağmen bunun sadece trajik bir kaza olduğunu rapor ettin. " +
                                            "Müdür Kemal Demir arkasında hiçbir iz bırakmadan şantiyeyi yönetmeye devam ediyor. " +
                                            "Murat'ın davası limanın karanlık sularına gömüldü...";
                }
            }
        }
    }

    public void YenidenBasla()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void AnaMenuyeDon()
    {
        SceneManager.LoadScene(0);
    }
}