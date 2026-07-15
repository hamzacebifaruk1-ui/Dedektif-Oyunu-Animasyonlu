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

    // Delil Sayaçları
    private int toplananIlkAramaDelilleri = 0; 
    private int toplananIkinciAramaDelilleri = 0; 
    private int toplananSonAramaDelilleri = 0; 

    // Çift Tetiklenmeyi Önleyen Liste
    private HashSet<string> toplananDelillerSeti = new HashSet<string>();

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
        StartCoroutine(GirisSinematiginiOynat()); 
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ReferanslariBul();
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
        if (gorevText == null) return; 

        string guncelMetin = ""; 
        string guncelIpucu = ""; 

        switch (mevcutAsama) 
        {
            case GorevAsamasi.GirisSinematigi:
                guncelMetin = "<color=gray>Giriş: Olay mahalline varılıyor...</color>"; 
                guncelIpucu = "Düşünce: Murat'ın düştüğü yerde bir şeyler dönüyor. Kanıtları bulmalıyım."; 
                break;

            case GorevAsamasi.RizaSorgu:
                guncelMetin = "<color=white>GÖREV: Güvenlik Rıza ile konuş ve ilk ifadesini al.</color>"; 
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
                guncelIpucu = "Düşünce: Rıza elektrik kesildi dedi ama yalan söylüyor gibi. Güvenlik Kamera Kaydını ve dışarıdaki lastik izlerini ara."; 
                break;

            case GorevAsamasi.KemalOfisArama:
                guncelMetin = $"<color=orange>GÖREV: Kemal Müdürün odasını gizlice araştır ({toplananIkinciAramaDelilleri}/3).</color>"; 
                guncelIpucu = "Düşünce: Kasadaki evrakları, masadaki yırtık fotoğrafı ve dolaptaki ilaç şişesini bul."; 
                break;

            case GorevAsamasi.AhmetYuzlesmeSecim:
                guncelMetin = "<color=red>GÖREV: Ahmet ile yüzleş ve ona doğru soruları sor.</color>"; 
                guncelIpucu = "İpucu: Ahmet'in yanına git ve 'E' tuşuna basarak kader seçimini yap."; 
                break;

            case GorevAsamasi.KalanDelilleriTopla:
                guncelMetin = $"<color=purple>GÖREV: Şantiyede kalan mekanik ve çevre delillerini topla ({toplananSonAramaDelilleri}/3).</color>"; 
                guncelIpucu = "Düşünce: Ahmet'in itiraflarından sonra atölyedeki spiral makinesini, kesik teli ve bareti bulmalıyım."; 
                break;

            case GorevAsamasi.DelilTasnifPanosu:
                guncelMetin = "<color=lightblue>GÖREV: Toplanan kanıtları panoda analiz et.</color>"; 
                guncelIpucu = "İpucu: 'I' tuşuna basarak Envanter Panosunu aç ve delilleri [GERÇEK] - [SAHTE] olarak etiketle."; 
                break;

            case GorevAsamasi.FinalSuclama:
                guncelMetin = "<color=red><b>[FİNAL] GÖREV: Gerçek suçluyu bularak onu suçla ve tutukla!</b></color>"; 
                guncelIpucu = "İpucu: Suçlunun yanına giderek etkileşime gir. Yanlış kişiyi suçlarsan her şey biter."; 
                break;
        }

        gorevText.text = guncelMetin; 
        if (ipucuText != null) ipucuText.text = guncelIpucu; 
    }

    public void NPCIleKonusmaBitti(string npcAdi)
    {
        if (string.IsNullOrEmpty(npcAdi)) return;

        string nameClean = npcAdi.Trim().ToLower();
        Debug.Log($"[GÖREV SİSTEMİ] Gelen İsim: {npcAdi} | Mevcut Aşama: {mevcutAsama}");

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

        if (toplananDelillerSeti.Contains(delilAdi))
        {
            Debug.Log($"[GÖREV SİSTEMİ] {delilAdi} zaten toplanmıştı. Çift sayım engellendi.");
            return; 
        }

        toplananDelillerSeti.Add(delilAdi);
        Debug.Log($"[GÖREV SİSTEMİ] Yeni delil toplandı: {delilAdi}");

        if (mevcutAsama == GorevAsamasi.RizaOfisArama) 
        {
            if (delilAdi == "USB Bellek" || delilAdi == "Güvenlik Kamera Kaydı" || delilAdi == "Çamurlu Lastik İzi") 
            {
                toplananIlkAramaDelilleri++; 
                if (gorevText != null) 
                    gorevText.text = $"<color=yellow>GÖREV: Güvenlik kulübesini araştır ({toplananIlkAramaDelilleri}/2).</color>"; 
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
                if (gorevText != null) 
                    gorevText.text = $"<color=orange>GÖREV: Kemal Müdürün odasını araştır ({toplananIkinciAramaDelilleri}/3).</color>"; 
                SyncDiyalogYazisi();

                if (toplananIkinciAramaDelilleri >= 3) 
                {
                    StartCoroutine(SesOynatVeAsamaGec("Dedektif_Evrak_Bulundu_IcSes", GorevAsamasi.AhmetYuzlesmeSecim)); 
                }
            }
        }
        else if (mevcutAsama == GorevAsamasi.KalanDelilleriTopla) 
        {
            if (delilAdi == "Spiral Taşlama Makinesi" || delilAdi == "Kırık Vinç Teli" || delilAdi == "Kirlenmiş Baret") 
            {
                toplananSonAramaDelilleri++; 
                if (gorevText != null) 
                    gorevText.text = $"<color=purple>GÖREV: Kalan mekanik delilleri topla ({toplananSonAramaDelilleri}/3).</color>"; 
                SyncDiyalogYazisi();

                if (toplananSonAramaDelilleri >= 3) 
                {
                    StartCoroutine(SesOynatVeAsamaGec("Dedektif_Pano_Hazir_IcSes", GorevAsamasi.DelilTasnifPanosu)); 
                }
            }
        }
    }

    private void SyncDiyalogYazisi()
    {
        if (DiyalogYoneticisi.Instance != null && DiyalogYoneticisi.Instance.gorevYazisiText != null && gorevText != null) 
        {
            DiyalogYoneticisi.Instance.gorevYazisiText.text = gorevText.text; 
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
                    Debug.Log("[HIZLI GEÇ] Dedektif iç sesi oyuncu tarafından geçildi.");
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
    }

    public void AnaMenuyeDon()
    {
        SceneManager.LoadScene("AnaMenuSahnendekiAd"); 
    }

    // ⚡ YENİ EKLENEN KÖPRÜ FONKSİYONU
    // DelilPanosuSistemi'nin toplanan delillere ulaşabilmesi için bu fonksiyon şart!
    public HashSet<string> GetToplananDeliller()
    {
        return toplananDelillerSeti;
    }
}