using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class DiyalogSatiri
{
    public string konusmaciAdi;
    [TextArea(3, 5)] public string textIcerik;
    public string elevenLabsSesDosyaAdi;
}

public class DiyalogYoneticisi : MonoBehaviour
{
    public static DiyalogYoneticisi Instance;

    [Header("UI Elemanları")]
    public GameObject diyalogPaneli;
    public TextMeshProUGUI konusmaciText;
    public TextMeshProUGUI diyalogText;
    public TextMeshProUGUI gorevYazisiText; 
    public TextMeshProUGUI delilYazisiText;

    [Header("Ses Kaynağı")]
    public AudioSource audioSource;

    [Header("Yazı Ayarları")]
    public float yaziHizi = 0.03f;

    [Header("Rota Ayarları")]
    public Transform ilkRotaHedefi;

    private HashSet<string> konusulanNpcListesi = new HashSet<string>();
    private List<DiyalogSatiri> mevcutDiyalogListesi;
    private int gecerliSatirIndex = 0;
    public bool diyalogAktif = false;
    private Coroutine daktiloCoroutine;
    private bool yaziAkiyorMu = false;
    private string tamMetin = "";
    private string suAnkiNpcAdi = "";

    [Header("Görev Durumu")]
    public bool gorevHazir = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        GameObject gorevObjesi = GameObject.Find("GorevYazisi"); 
        if (gorevObjesi != null) gorevYazisiText = gorevObjesi.GetComponent<TextMeshProUGUI>();

        GameObject delilObjesi = GameObject.Find("DelilYazisi");
        if (delilObjesi != null)
        {
            delilYazisiText = delilObjesi.GetComponent<TextMeshProUGUI>();
            delilObjesi.SetActive(false); 
        }

        // --- SAHNE 2 BAŞLANGICI: DEDEKTİFİN GİRİŞ İÇ SESİNİ TETİKLE ---
        Invoke("GirisMonologuOynat", 1f);
    }

    void GirisMonologuOynat()
    {
        List<DiyalogSatiri> girisDiyalogu = new List<DiyalogSatiri>
        {
            new DiyalogSatiri {
                konusmaciAdi = "Dedektif İç Ses",
                textIcerik = "Dün gece saat 02:30... Bu şantiyede genç bir işçi, Murat, yüksekten düşerek can verdi. Şirket 'basit bir kaza' deyip üstünü kapatmaya çalışıyor. Ama benim içimde kötü bir his var. Katil ya da katiller hala burada. Gerçeği bulmadan bu kapıdan çıkmayacağım.",
                elevenLabsSesDosyaAdi = "Dedektif_Giris_IcSes"
            }
        };
        DiyalogBaslat(girisDiyalogu, "GirisIcSes");
    }

    void Update()
    {
        var klavye = UnityEngine.InputSystem.Keyboard.current;
        var fare = UnityEngine.InputSystem.Mouse.current;

        bool spaceBasildi = klavye != null && klavye.spaceKey.wasPressedThisFrame;
        bool solTiklandi = fare != null && fare.leftButton.wasPressedThisFrame;

        if (diyalogAktif && (spaceBasildi || solTiklandi))
        {
            if (yaziAkiyorMu)
            {
                DurdurVeMetniTamamla();
            }
            else
            {
                SonrakiSatiraGec();
            }
        }
    }

    public void DiyalogBaslat(List<DiyalogSatiri> yeniDiyalog, string npcAdi)
    {
        mevcutDiyalogListesi = yeniDiyalog;
        gecerliSatirIndex = 0;
        diyalogAktif = true;
        suAnkiNpcAdi = npcAdi;
        diyalogPaneli.SetActive(true);

        hareket oyuncuHareketi = FindFirstObjectByType<hareket>();
        if (oyuncuHareketi != null) oyuncuHareketi.enabled = false;

        SatiriOynat();
    }

    void SatiriOynat()
    {
        if (gecerliSatirIndex >= mevcutDiyalogListesi.Count)
        {
            DiyaloguBitir();
            return;
        }

        DiyalogSatiri satir = mevcutDiyalogListesi[gecerliSatirIndex];
        konusmaciText.text = satir.konusmaciAdi;
        tamMetin = satir.textIcerik;

        if (satir.konusmaciAdi.Contains("İç Ses"))
        {
            diyalogText.color = Color.cyan;
            diyalogText.fontStyle = FontStyles.Italic;
        }
        else
        {
            diyalogText.color = Color.white;
            diyalogText.fontStyle = FontStyles.Normal;
        }

        audioSource.Stop();
        AudioClip klip = Resources.Load<AudioClip>("Audio/Dialogs/" + satir.elevenLabsSesDosyaAdi);
        if (klip != null)
        {
            audioSource.clip = klip;
            audioSource.Play();
        }

        if (daktiloCoroutine != null) StopCoroutine(daktiloCoroutine);
        daktiloCoroutine = StartCoroutine(YaziyiDokCoroutine(tamMetin));
    }

    IEnumerator YaziyiDokCoroutine(string metin)
    {
        diyalogText.text = "";
        yaziAkiyorMu = true;

        foreach (char harf in metin.ToCharArray())
        {
            diyalogText.text += harf;
            yield return new WaitForSeconds(yaziHizi);
        }

        yaziAkiyorMu = false;
    }

    void DurdurVeMetniTamamla()
    {
        if (daktiloCoroutine != null) StopCoroutine(daktiloCoroutine);
        diyalogText.text = tamMetin;
        yaziAkiyorMu = false;
    }

    void SonrakiSatiraGec()
    {
        gecerliSatirIndex++;
        SatiriOynat();
    }

    void DiyaloguBitir()
    {
        diyalogAktif = false;
        diyalogPaneli.SetActive(false);
        audioSource.Stop();

        if (!string.IsNullOrEmpty(suAnkiNpcAdi) && suAnkiNpcAdi != "GirisIcSes")
        {
            konusulanNpcListesi.Add(suAnkiNpcAdi);
            GorevKontrolEt();
        }

        hareket oyuncuHareketi = FindFirstObjectByType<hareket>();
        if (oyuncuHareketi != null) oyuncuHareketi.enabled = true;
    }

    void GorevKontrolEt()
    {
        if (konusulanNpcListesi.Contains("Güvenlik Rıza") && 
            konusulanNpcListesi.Contains("Liman Müdürü Kemal") && 
            konusulanNpcListesi.Contains("İşçi Ahmet"))
        {
            gorevHazir = true;

            if (gorevYazisiText != null)
            {
                gorevYazisiText.text = "GÖREV GÜNCELLENDİ:\nŞantiyedeki gizli delilleri araştır.";
            }

            if (delilYazisiText != null)
            {
                delilYazisiText.gameObject.SetActive(true);
                delilYazisiText.text = "Toplanan Delil: 0 / 8"; // 8 Delilli yeni mantık tablosuna eşitlendi!
            }

            // Harita İkonlarını Göster
            BuyukHaritaYonetici buyukHarita = FindFirstObjectByType<BuyukHaritaYonetici>();
            if (buyukHarita != null) buyukHarita.NpcIkonlariniGoster(true);

            MinimapYonetici minimap = FindFirstObjectByType<MinimapYonetici>();
            if (minimap != null) minimap.NpcIkonlariniGoster(true);
            
            // İlk sorgu bittiğinde çalan ses
            audioSource.PlayOneShot(Resources.Load<AudioClip>("Audio/Dialogs/Dedektif_IlkSorgu_Sonrasi"));
        }
    }
}