using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; 
using System.Collections.Generic;

public class GorevYoneticisi : MonoBehaviour
{
    public static GorevYoneticisi Instance;

    public enum GorevAsamasi
    {
        GirisSinematigi,       
        RizaSorgu,             
        KemalSorgu,            
        AhmetSorgu,            
        RizaOfisArama,         
        KemalOfisArama,        
        AhmetYuzlesmeSecim,    
        KalanDelilleriTopla,   
        DelilTasnifPanosu,     
        FinalSuclama           
    }

    [Header("Mevcut Oyun Aşaması")]
    public GorevAsamasi mevcutAsama = GorevAsamasi.GirisSinematigi; 

    [Header("UI Elemanları")]
    public TextMeshProUGUI gorevText; 
    public TextMeshProUGUI ipucuText; 
    public GameObject tebriklerPanel; 
    public GameObject kaybettinPanel; 

    [Header("Ses Entegrasyonu")]
    public AudioSource icSesKaynagi; 

    // Delil Sayaçları (Toplam: 2 + 3 + 4 = 9 Delil)
    private int toplananIlkAramaDelilleri = 0; 
    private int toplananIkinciAramaDelilleri = 0; 
    private int toplananSonAramaDelilleri = 0; 

    // Çift Tetiklenmeyi Önleyen Liste
    private HashSet<string> toplananDelillerSeti = new HashSet<string>();

    // İpucu Zamanlayıcısı için Coroutine Referansı
    private Coroutine ipucuTimerCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this; 
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        ReferanslariBul();
        if (mevcutAsama == GorevAsamasi.GirisSinematigi)
        {
            StartCoroutine(GirisSinematiginiOynat()); 
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ReferanslariBul();
        
        if (kaybettinPanel != null) kaybettinPanel.SetActive(false);
        if (tebriklerPanel != null) tebriklerPanel.SetActive(false);

        GoreviGuncelle();
        SyncDiyalogYazisi();
    }

    public void ReferanslariBul()
    {
        if (gorevText == null)
        {
            GameObject gorevObj = GameObject.Find("GorevYazisi");
            if (gorevObj != null) gorevText = gorevObj.GetComponent<TextMeshProUGUI>();
        }

        if (ipucuText == null)
        {
            GameObject ipucuObj = GameObject.Find("IpucuYazisi");
            if (ipucuObj != null) ipucuText = ipucuObj.GetComponent<TextMeshProUGUI>();
        }

        if (tebriklerPanel == null) tebriklerPanel = GameObject.Find("TebriklerPaneli");
        if (kaybettinPanel == null) kaybettinPanel = GameObject.Find("KaybettinPaneli");

        if (tebriklerPanel != null) tebriklerPanel.SetActive(false); 
        if (kaybettinPanel != null) kaybettinPanel.SetActive(false); 

        if (icSesKaynagi == null)
        {
            icSesKaynagi = GetComponent<AudioSource>();
            if (icSesKaynagi == null)
            {
                icSesKaynagi = gameObject.AddComponent<AudioSource>();
            }
        }

        GoreviGuncelle();
        SyncDiyalogYazisi();
    }

    private System.Collections.IEnumerator GirisSinematiginiOynat()
    {
        AudioClip girisSes = Resources.Load<AudioClip>("Audio/Sounds/Dedektif_Giris_IcSes"); 
        if (icSesKaynagi != null && girisSes != null) 
        {
            icSesKaynagi.clip = girisSes; 
            icSesKaynagi.Play(); 
            
            float elapsed = 0f;
            float duration = girisSes.length + 1f;

            while (elapsed < duration)
            {
                if (Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed && Keyboard.current.enterKey.wasPressedThisFrame)
                {
                    icSesKaynagi.Stop();
                    Debug.Log("[HIZLI GEÇ] Giriş sinematiği oyuncu tarafından geçildi.");
                    break;
                }
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(1f); 
        }

        AsamaAtla(GorevAsamasi.RizaSorgu);
    }

    public void AsamaAtla(GorevAsamasi yeniAsama)
    {
        mevcutAsama = yeniAsama; 
        Debug.Log($"[GÖREV SİSTEMİ] Yeni Aşamaya Geçildi: {yeniAsama}");
        GoreviGuncelle(); 
        SyncDiyalogYazisi(); 
    }

    public void GoreviGuncelle()
    {
        if (gorevText == null) ReferanslariBul();
        if (gorevText == null) return; 

        string guncelMetin = ""; 
        string guncelIpucu = ""; 

        switch (mevcutAsama) 
        {
            case GorevAsamasi.GirisSinematigi:
                guncelMetin = "<color=blue>Giriş: Olay mahalline varılıyor...</color>"; 
                guncelIpucu = "Düşünce: Murat'ın düştüğü yerde bir şeyler dönüyor. Kanıtları bulmalıyım."; 
                break;

            case GorevAsamasi.RizaSorgu:
               guncelMetin = "<color=white>GÖREV: Güvenlik Rıza ile konuş ve ilk ifadesini al (<color=#8B4513>Harita</color> kullanabilirsin (M tuşu ile aç)).</color>";
                guncelIpucu = "İpucu: Haritadan güvenlik kulübesinin yerini bularak oraya yönel."; 
                break;

            case GorevAsamasi.KemalSorgu:
                guncelMetin = "<color=white>GÖREV: Liman Müdürü Kemal ile konuş.</color>"; 
                guncelIpucu = "İpucu: Ofis binasının girişinde seni bekleyen müdürün yanına git."; 
                break;

            case GorevAsamasi.AhmetSorgu:
                guncelMetin = "<color=white>GÖREV: İşçi Ahmet ile görüş.</color>"; 
                guncelIpucu = "İpucu: Şantiye sahasında varil ateşinin başında ısınan işçiyi bul."; 
                break;

            case GorevAsamasi.RizaOfisArama:
                guncelMetin = $"<color=yellow>GÖREV: Güvenlik kulübesini ve çevresini araştır ({toplananIlkAramaDelilleri}/2).</color>"; 
                guncelIpucu = "Düşünce: Rıza elektrik kesildi dedi ama yalan söylüyor gibi. Kamera kaydını ve lastik izlerini ara."; 
                break;

            case GorevAsamasi.KemalOfisArama:
                guncelMetin = $"<color=orange>GÖREV: Kemal Müdürün odasını gizlice araştır ({toplananIkinciAramaDelilleri}/3).</color>"; 
                guncelIpucu = "Düşünce: Kasadaki evrakları, masadaki yırtık fotoğrafı ve dolaptaki ilaç şişesini bul."; 
                break;

            case GorevAsamasi.AhmetYuzlesmeSecim:
                guncelMetin = "<color=red>GÖREV: Ahmet ile yüzleş ve ona soru sor.</color>"; 
                guncelIpucu = "İpucu: Ahmet'in yanına git ve 'E' tuşuna basarak kader seçimini yap."; 
                break;

            case GorevAsamasi.KalanDelilleriTopla:
                guncelMetin = $"<color=purple>GÖREV: Şantiyedeki gizli ipucunu ve kalan delilleri topla ({toplananSonAramaDelilleri}/4).</color>"; 
                // ✨ SOKAK LAMBALARI VE DÜŞÜNCE BİLGİSİ EKLENDİ
                guncelIpucu = "Düşünce: Sokak lambalarına dikkat et; ışıkları yanıp sönerek kalan gizli delillere giden yolu gösteriyor(vinç!)."; 
                break;

            case GorevAsamasi.DelilTasnifPanosu:
                guncelMetin = "<color=lightblue>GÖREV: Toplanan kanıtları panoda analiz et (TAB menüsünden yardım alabirsin).</color>"; 
                guncelIpucu = "İpucu: Envanter Panosunu aç ve delilleri [GERÇEK] - [SAHTE] olarak etiketle."; 
                break;

            case GorevAsamasi.FinalSuclama:
                guncelMetin = "<color=red><b>[FİNAL] GÖREV: Gerçek suçluyu bularak onu suçla ve tutukla!</b></color>"; 
                guncelIpucu = "İpucu: Suçlunun yanına giderek etkileşime gir. Yanlış kişiyi suçlarsan her şey biter."; 
                break;

        }

        // 1. Ana Görev Metni Anında Güncellenir
        gorevText.gameObject.SetActive(true);
        gorevText.text = guncelMetin; 

        // 2. İpucu Metni 1 Dakika (60 sn) Sonra Çıkacak Şekilde Zamanlayıcı Başlatılır
        if (ipucuTimerCoroutine != null) StopCoroutine(ipucuTimerCoroutine);
        ipucuTimerCoroutine = StartCoroutine(IpucuGecikmeliGoster(guncelIpucu));
    }

    // ✨ İPUCUNU 1 DAKİKA (60 SANİYE) SONRA GÖSTEREN METOD
    private System.Collections.IEnumerator IpucuGecikmeliGoster(string ipucuMetni)
    {
        if (ipucuText != null)
        {
            ipucuText.text = ""; 
            ipucuText.gameObject.SetActive(false); // Yeni göreve geçildiğinde ipucu gizlenir
        }

        yield return new WaitForSeconds(60f); // 60 saniye bekle (1 dakika)

        if (ipucuText != null)
        {
            ipucuText.text = ipucuMetni;
            ipucuText.gameObject.SetActive(true); // 1 dakika dolunca ekrana gelir
        }
    }

    public void NPCIleKonusmaBitti(string npcAdi)
    {
        if (string.IsNullOrEmpty(npcAdi)) return;

        string nameClean = npcAdi.Trim().ToLower();

        bool isRiza = nameClean.Contains("riza") || nameClean.Contains("rıza") || nameClean.Contains("güvenlik");
        if (mevcutAsama == GorevAsamasi.RizaSorgu && isRiza)
        {
            AsamaAtla(GorevAsamasi.KemalSorgu);
            return;
        }

        bool isKemal = nameClean.Contains("kemal") || nameClean.Contains("müdür") || nameClean.Contains("mudur");
        if (mevcutAsama == GorevAsamasi.KemalSorgu && isKemal)
        {
            AsamaAtla(GorevAsamasi.AhmetSorgu);
            return;
        }

        bool isAhmet = nameClean.Contains("ahmet") || nameClean.Contains("işçi") || nameClean.Contains("isci");
        if (mevcutAsama == GorevAsamasi.AhmetSorgu && isAhmet)
        {
            AsamaAtla(GorevAsamasi.RizaOfisArama);
            return;
        }
    }

    public void DelilToplandi(string delilAdi)
    {
        ReferanslariBul();

        if (toplananDelillerSeti.Contains(delilAdi)) return; 

        toplananDelillerSeti.Add(delilAdi);

        if (mevcutAsama == GorevAsamasi.RizaOfisArama) 
        {
            if (delilAdi == "USB Bellek" || delilAdi == "Güvenlik Kamera Kaydı" || delilAdi == "Çamurlu Lastik İzi") 
            {
                toplananIlkAramaDelilleri++; 
                GoreviGuncelle();
                SyncDiyalogYazisi();

                if (toplananIlkAramaDelilleri >= 2) 
                {
                    StartCoroutine(SesOynatVeAsamaGec("Dedektif_usb_Bulundu_IcSes", GorevAsamasi.KemalOfisArama)); 
                }
            }
        }
        else if (mevcutAsama == GorevAsamasi.KemalOfisArama) 
        {
            if (delilAdi == "Şirket Evrakları" || delilAdi == "Yırtık Kadın Fotoğrafı" || delilAdi == "Boş İlaç Şişesi") 
            {
                toplananIkinciAramaDelilleri++; 
                GoreviGuncelle();
                SyncDiyalogYazisi();

                if (toplananIkinciAramaDelilleri >= 3) 
                {
                    StartCoroutine(SesOynatVeAsamaGec("Dedektif_Evrak_Bulundu_IcSes", GorevAsamasi.AhmetYuzlesmeSecim)); 
                }
            }
        }
        else if (mevcutAsama == GorevAsamasi.KalanDelilleriTopla) 
        {
            if (delilAdi == "Spiral Taşlama Makinesi" || delilAdi == "Kırık Vinç Teli" || 
                delilAdi == "Kirlenmiş Baret" || delilAdi == "Murat'ın Gizli Mektubu" || 
                delilAdi == "Zimmet Kayıt Belgesi") 
            {
                toplananSonAramaDelilleri++; 
                GoreviGuncelle();
                SyncDiyalogYazisi();

                if (toplananSonAramaDelilleri >= 4) 
                {
                    StartCoroutine(SesOynatVeAsamaGec("Dedektif_Pano_Hazir_IcSes", GorevAsamasi.DelilTasnifPanosu)); 
                }
            }
        }
    }

    private void SyncDiyalogYazisi()
    {
        try
        {
            if (DiyalogYoneticisi.Instance != null && DiyalogYoneticisi.Instance.gorevYazisiText != null && gorevText != null) 
            {
                DiyalogYoneticisi.Instance.gorevYazisiText.text = gorevText.text; 
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[GÖREV SİSTEMİ] Diyalog senkronizasyonu atlandı: {ex.Message}");
        }
    }

    private System.Collections.IEnumerator SesOynatVeAsamaGec(string sesAdi, GorevAsamasi sonrakiAsama)
    {
        AudioClip clip = null;

        try
        {
            clip = Resources.Load<AudioClip>("Audio/Sounds/" + sesAdi);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[GÖREV SİSTEMİ] Ses dosyası yüklenemedi: {e.Message}");
        }

        if (icSesKaynagi != null && clip != null) 
        {
            try
            {
                icSesKaynagi.clip = clip; 
                icSesKaynagi.Play(); 
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[GÖREV SİSTEMİ] Ses çalma hatası: {e.Message}");
            }

            float elapsed = 0f;
            float duration = clip.length + 0.5f;

            while (elapsed < duration)
            {
                if (Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed && Keyboard.current.enterKey.wasPressedThisFrame)
                {
                    icSesKaynagi.Stop();
                    break;
                }
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
        }

        AsamaAtla(sonrakiAsama); 
    }

    public void OyunuKaybet()
    {
        if (kaybettinPanel != null) 
        {
            kaybettinPanel.SetActive(true); 
            Cursor.lockState = CursorLockMode.None; 
            Cursor.visible = true; 
        }
    }

    public void OyunuKazan()
    {
        if (tebriklerPanel != null) 
        {
            tebriklerPanel.SetActive(true); 
            Cursor.lockState = CursorLockMode.None; 
            Cursor.visible = true; 
        }
    }

    public void TekrarDene()
    {
        StopAllCoroutines();
        if (icSesKaynagi != null) icSesKaynagi.Stop();

        if (DelilYoneticisi.Instance != null)
        {
            DelilYoneticisi.Instance.DelilSayaciniSifirla();
        }

        Destroy(gameObject);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void AnaMenuyeDon()
    {
        Destroy(gameObject);
        SceneManager.LoadScene("AnaMenuSahnendekiAd"); 
    }

    public HashSet<string> GetToplananDeliller()
    {
        return toplananDelillerSeti;
    }

    public void MevcutAsamayiGuncelle(GorevAsamasi yeniAsama)
    {
        AsamaAtla(yeniAsama);
    }

    public void GorevYazisiGuncelle(string yeniYazi)
    {
        if (gorevText != null) 
        {
            gorevText.text = yeniYazi;
            SyncDiyalogYazisi();
        }
    }
}