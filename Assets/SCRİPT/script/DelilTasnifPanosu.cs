using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class DelilTasnifPanosu : MonoBehaviour
{
    public static DelilTasnifPanosu Instance;

    [Header("UI Elemanları")]
    public GameObject panoPanel;
    public Transform icerikAlani; 
    public TextMeshProUGUI durumText;
    public Button onaylaButonu;

    [Header("Ses Ayarları")]
    [Tooltip("Dedektif_Pano_Hazir_IcSe ses dosyasını buraya sürükle")]
    public AudioClip panoHazirSesi;
    private AudioSource audioSource;

    // 🎯 8 DELİLİN DOĞRU ANALİZ HARİTASI
    private Dictionary<string, bool> dogruDelilDurumlari = new Dictionary<string, bool>()
    {
        { "USB Bellek", true },                 // GERÇEK
        { "Yırtık Kadın Fotoğrafı", true },     // GERÇEK
        { "Spiral Taşlama Makinesi", true },   // GERÇEK
        { "Çamurlu Lastik İzi", true },         // GERÇEK

        { "Yırtık Bakım Defteri", false },     // SAHTE
        { "Boş İlaç Şişesi", false },           // SAHTE
        { "Kırık Vinç Teli", false },           // SAHTE
        { "Kirlenmiş Baret", false }            // SAHTE
    };

    private Dictionary<string, bool> oyuncuSecimleri = new Dictionary<string, bool>();

    private bool panoAcik = false;
    private bool otomatikAcildiMi = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Start()
    {
        if (panoPanel != null) panoPanel.SetActive(false);

        if (onaylaButonu != null)
        {
            onaylaButonu.onClick.RemoveAllListeners();
            onaylaButonu.onClick.AddListener(AnalizEtAndOnayla);
        }
    }

    void Update()
    {
        // 8. Delil incelemesi bittiğinde ÖNCE SES ÇALAR, SONRA PANO AÇILIR
        if (GorevYoneticisi.Instance != null && GorevYoneticisi.Instance.mevcutAsama == GorevYoneticisi.GorevAsamasi.DelilTasnifPanosu)
        {
            bool incelemeAcikMi = (DelilIncelemeSistemi.Instance != null && 
                                   DelilIncelemeSistemi.Instance.incelemePaneli != null && 
                                   DelilIncelemeSistemi.Instance.incelemePaneli.activeSelf);

            if (!otomatikAcildiMi && !incelemeAcikMi)
            {
                otomatikAcildiMi = true;
                StartCoroutine(SesCalVePanoyuAc());
            }
        }

        // 'I' Tuşu ile Manuel Açma / Kapama
        Keyboard klavye = Keyboard.current;
        if (klavye != null && klavye.iKey.wasPressedThisFrame)
        {
            bool incelemeAcikMi = (DelilIncelemeSistemi.Instance != null && 
                                   DelilIncelemeSistemi.Instance.incelemePaneli != null && 
                                   DelilIncelemeSistemi.Instance.incelemePaneli.activeSelf);

            if (!incelemeAcikMi && GorevYoneticisi.Instance != null && 
               (GorevYoneticisi.Instance.mevcutAsama == GorevYoneticisi.GorevAsamasi.DelilTasnifPanosu ||
                GorevYoneticisi.Instance.mevcutAsama == GorevYoneticisi.GorevAsamasi.FinalSuclama))
            {
                PanoDurumunuDegistir(!panoAcik);
            }
        }
    }

    private IEnumerator SesCalVePanoyuAc()
    {
        if (panoHazirSesi != null && audioSource != null)
        {
            audioSource.PlayOneShot(panoHazirSesi);
            yield return new WaitForSeconds(panoHazirSesi.length);
        }

        PanoDurumunuDegistir(true);
    }

    public void PanoDurumunuDegistir(bool ac)
    {
        panoAcik = ac;
        if (panoPanel != null) panoPanel.SetActive(panoAcik);

        if (panoAcik)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            PanoyuOlusturUI();
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void PanoyuOlusturUI()
    {
        if (icerikAlani == null || panoPanel == null) return;

        DuzeniniGarantile();

        foreach (Transform child in icerikAlani) Destroy(child.gameObject);

        if (durumText != null) durumText.text = "Her delil için [GERÇEK] veya [SAHTE] etiketini seçin.";

        foreach (var delil in dogruDelilDurumlari)
        {
            KartOlustur(delil.Key);
        }
    }

    void DuzeniniGarantile()
    {
        // 1. ANA PANOLARIN BOYUTUNU BÜYÜTÜYORUZ (700 x 620 px)
        RectTransform panelRt = panoPanel.GetComponent<RectTransform>();
        if (panelRt != null)
        {
            panelRt.anchorMin = new Vector2(0.5f, 0.5f);
            panelRt.anchorMax = new Vector2(0.5f, 0.5f);
            panelRt.pivot = new Vector2(0.5f, 0.5f);
            panelRt.sizeDelta = new Vector2(700, 620); // Boyut yükseltildi
        }

        // 2. İÇERİK ALANI DİKEY DÜZENLEYİCİ
        VerticalLayoutGroup vlg = icerikAlani.GetComponent<VerticalLayoutGroup>();
        if (vlg == null) vlg = icerikAlani.gameObject.AddComponent<VerticalLayoutGroup>();
        
        vlg.childControlWidth = true;
        vlg.childControlHeight = false;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.spacing = 6;
        vlg.padding = new RectOffset(10, 10, 5, 5);

        // 3. İÇERİK ALANI ALAN DÜZENİ (%15 ile %85 arası, butonla asla çakışmaz)
        RectTransform icerikRt = icerikAlani.GetComponent<RectTransform>();
        if (icerikRt != null)
        {
            icerikRt.anchorMin = new Vector2(0.04f, 0.15f);
            icerikRt.anchorMax = new Vector2(0.96f, 0.85f);
            icerikRt.offsetMin = Vector2.zero;
            icerikRt.offsetMax = Vector2.zero;
        }

        // 4. UYARI YAZISI (EN ÜSTTE)
        if (durumText != null)
        {
            durumText.transform.SetParent(panoPanel.transform, false);
            RectTransform dtRt = durumText.GetComponent<RectTransform>();
            if (dtRt != null)
            {
                dtRt.anchorMin = new Vector2(0.05f, 0.86f);
                dtRt.anchorMax = new Vector2(0.95f, 0.97f);
                dtRt.offsetMin = Vector2.zero;
                dtRt.offsetMax = Vector2.zero;
            }
            durumText.alignment = TextAlignmentOptions.Center;
        }

        // 5. ONAYLA BUTONU (TAM EN ALTTA, HİÇBİR YAZIYA BİNMEYECEK)
        if (onaylaButonu != null)
        {
            onaylaButonu.transform.SetParent(panoPanel.transform, false);
            RectTransform obRt = onaylaButonu.GetComponent<RectTransform>();
            if (obRt != null)
            {
                obRt.anchorMin = new Vector2(0.20f, 0.03f);
                obRt.anchorMax = new Vector2(0.80f, 0.12f);
                obRt.offsetMin = Vector2.zero;
                obRt.offsetMax = Vector2.zero;
            }
        }
    }

    private void KartOlustur(string delilAdi)
    {
        GameObject kart = new GameObject("Kart_" + delilAdi);
        kart.transform.SetParent(icerikAlani, false);

        RectTransform kartRt = kart.AddComponent<RectTransform>();
        kartRt.sizeDelta = new Vector2(0, 42);

        LayoutElement le = kart.AddComponent<LayoutElement>();
        le.minHeight = 42;
        le.preferredHeight = 42;

        Image kartBg = kart.AddComponent<Image>();
        kartBg.color = new Color(0.18f, 0.18f, 0.18f, 0.95f);
        kartBg.raycastTarget = false;

        HorizontalLayoutGroup layout = kart.AddComponent<HorizontalLayoutGroup>();
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = true;
        layout.padding = new RectOffset(12, 12, 5, 5);
        layout.spacing = 10;

        // Metin
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(kart.transform, false);
        TextMeshProUGUI txt = textObj.AddComponent<TextMeshProUGUI>();
        txt.text = delilAdi;
        txt.fontSize = 16;
        txt.fontStyle = FontStyles.Bold;
        txt.color = Color.white;
        txt.alignment = TextAlignmentOptions.MidlineLeft;
        txt.raycastTarget = false;

        LayoutElement txtLe = textObj.AddComponent<LayoutElement>();
        txtLe.flexibleWidth = 1;

        // GERÇEK Butonu
        GameObject gBtnObj = new GameObject("GercekBtn");
        gBtnObj.transform.SetParent(kart.transform, false);
        Image gImg = gBtnObj.AddComponent<Image>();
        gImg.raycastTarget = true;
        Button gBtn = gBtnObj.AddComponent<Button>();

        LayoutElement gLe = gBtnObj.AddComponent<LayoutElement>();
        gLe.preferredWidth = 110;

        GameObject gTxtObj = new GameObject("Text");
        gTxtObj.transform.SetParent(gBtnObj.transform, false);
        TextMeshProUGUI gTxt = gTxtObj.AddComponent<TextMeshProUGUI>();
        gTxt.text = "GERÇEK"; 
        gTxt.fontSize = 14; 
        gTxt.fontStyle = FontStyles.Bold;
        gTxt.alignment = TextAlignmentOptions.Center; 
        gTxt.color = Color.white;
        gTxt.raycastTarget = false;

        RectTransform gTxtRt = gTxtObj.GetComponent<RectTransform>(); 
        gTxtRt.anchorMin = Vector2.zero; gTxtRt.anchorMax = Vector2.one;
        gTxtRt.offsetMin = Vector2.zero; gTxtRt.offsetMax = Vector2.zero;

        // SAHTE Butonu
        GameObject sBtnObj = new GameObject("SahteBtn");
        sBtnObj.transform.SetParent(kart.transform, false);
        Image sImg = sBtnObj.AddComponent<Image>();
        sImg.raycastTarget = true;
        Button sBtn = sBtnObj.AddComponent<Button>();

        LayoutElement sLe = sBtnObj.AddComponent<LayoutElement>();
        sLe.preferredWidth = 110;

        GameObject sTxtObj = new GameObject("Text");
        sTxtObj.transform.SetParent(sBtnObj.transform, false);
        TextMeshProUGUI sTxt = sTxtObj.AddComponent<TextMeshProUGUI>();
        sTxt.text = "SAHTE"; 
        sTxt.fontSize = 14; 
        sTxt.fontStyle = FontStyles.Bold;
        sTxt.alignment = TextAlignmentOptions.Center; 
        sTxt.color = Color.white;
        sTxt.raycastTarget = false;

        RectTransform sTxtRt = sTxtObj.GetComponent<RectTransform>(); 
        sTxtRt.anchorMin = Vector2.zero; sTxtRt.anchorMax = Vector2.one;
        sTxtRt.offsetMin = Vector2.zero; sTxtRt.offsetMax = Vector2.zero;

        System.Action RenkGuncelle = () =>
        {
            if (oyuncuSecimleri.ContainsKey(delilAdi))
            {
                bool secim = oyuncuSecimleri[delilAdi];
                gImg.color = secim ? new Color(0f, 0.7f, 0.1f) : new Color(0.3f, 0.3f, 0.3f);
                sImg.color = !secim ? new Color(0.8f, 0.1f, 0.1f) : new Color(0.3f, 0.3f, 0.3f);
            }
            else
            {
                gImg.color = new Color(0.3f, 0.3f, 0.3f);
                sImg.color = new Color(0.3f, 0.3f, 0.3f);
            }
        };

        RenkGuncelle();

        gBtn.onClick.AddListener(() => { oyuncuSecimleri[delilAdi] = true; RenkGuncelle(); });
        sBtn.onClick.AddListener(() => { oyuncuSecimleri[delilAdi] = false; RenkGuncelle(); });
    }

    public void AnalizEtAndOnayla()
    {
        if (oyuncuSecimleri.Count < dogruDelilDurumlari.Count)
        {
            if (durumText != null)
                durumText.text = "<color=yellow>Lütfen tüm 8 delili işaretleyin!</color>";
            return;
        }

        bool hepsiDogru = true;
        foreach (var kvp in dogruDelilDurumlari)
        {
            if (!oyuncuSecimleri.ContainsKey(kvp.Key) || oyuncuSecimleri[kvp.Key] != kvp.Value)
            {
                hepsiDogru = false;
                break;
            }
        }

        if (hepsiDogru)
        {
            if (durumText != null)
                durumText.text = "<color=green>Tebrikler! Delil analizi doğru. Artık gerçek suçluyu tutuklayabilirsin!</color>";

            Invoke("FinalAsamasinaGec", 1.8f);
        }
        else
        {
            if (durumText != null)
                durumText.text = "<color=red>Hatalı tespit! Sahte ve gerçek delilleri tekrar gözden geçir.</color>";
        }
    }

    private void FinalAsamasinaGec()
    {
        PanoDurumunuDegistir(false);

        if (GorevYoneticisi.Instance != null)
        {
            GorevYoneticisi.Instance.AsamaAtla(GorevYoneticisi.GorevAsamasi.FinalSuclama);
        }
    }
}