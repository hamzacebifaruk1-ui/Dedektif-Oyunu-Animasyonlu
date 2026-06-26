using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class NotDefteriYoneticisi : MonoBehaviour
{
    public static NotDefteriYoneticisi Instance;

    [Header("UI Elemanlari")]
    public GameObject notDefteriPanel;
    public Transform icerikAlani; // Scroll View -> Content nesnesi

    [Header("Prefab")]
    public GameObject delilKartiPrefab; // Boş kalırsa kod otomatik oluşturur

    private bool acik = false;
    private List<string> bulunanDeliller = new List<string>();
    private List<string> bulunanNotlar = new List<string>();

    // Delil adına göre dedektif notları
    private Dictionary<string, string> delilNotlari = new Dictionary<string, string>()
    {
        { "Yırtık Bakım Defteri", "Bakım kayıtları sahte. Biri bu defterde oynama yapmış." },
        { "Kırık Vinç Teli", "Bu tel yıpranmadan kopmaz. Bilerek mi kesildi?" },
        { "Anonim Not", "Bu notu kim yazdı? Murat uyarılmış ama gitmemiş." },
        { "Güvenlik Kamera Kaydı", "Gece 2'de makine odasında kim var?" },
        { "İlaç Kutusu", "Murat'ın ilacı burada. Görmesi bozuktu, gece çalışması yasaktı." }
    };

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Yeni bir oyuna başlandığında listelerin temiz olduğundan emin oluyoruz
        bulunanDeliller.Clear();
        bulunanNotlar.Clear();

        if (notDefteriPanel != null)
            notDefteriPanel.SetActive(false);
    }

    void Update()
    {
        Keyboard klavye = Keyboard.current;
        if (klavye == null) return;

        // TAB tuşuna basıldığında defteri aç/kapat
        if (klavye.tabKey.wasPressedThisFrame)
        {
            acik = !acik;
            
            if (notDefteriPanel != null)
                notDefteriPanel.SetActive(acik);

            if (acik)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                DefterGuncelle();
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    public void DelilEkle(string delilAdi)
    {
        Debug.Log("NotDefteriYoneticisi: DelilEkle tetiklendi! Gelen isim: '" + delilAdi + "'");

        if (!bulunanDeliller.Contains(delilAdi))
        {
            bulunanDeliller.Add(delilAdi);
            
            string not = delilNotlari.ContainsKey(delilAdi) ? delilNotlari[delilAdi] : "İnceleniyor...";
            bulunanNotlar.Add(not);

            // Eğer oyuncu defter açıkken delil toplarsa listeyi anında yenile
            if (acik)
            {
                DefterGuncelle();
            }
        }
    }

    void DefterGuncelle()
    {
        if (icerikAlani == null) return;

        // Eski kartları temizle
        foreach (Transform child in icerikAlani)
        {
            Destroy(child.gameObject);
        }

        // Henüz hiç delil toplanmadıysa uyarı yazısı çıkar
        if (bulunanDeliller.Count == 0)
        {
            GameObject bosKart = new GameObject("BosKart");
            bosKart.transform.SetParent(icerikAlani, false);
            TextMeshProUGUI bosText = bosKart.AddComponent<TextMeshProUGUI>();
            bosText.text = "Henüz delil bulunamadı...";
            bosText.fontSize = 22;
            bosText.color = new Color(0.6f, 0.6f, 0.6f);
            bosText.alignment = TextAlignmentOptions.Center;
            return;
        }

        // Her delil için bir kart oluştur veya prefab'dan üret
        for (int i = 0; i < bulunanDeliller.Count; i++)
        {
            if (delilKartiPrefab != null)
            {
                GameObject kart = Instantiate(delilKartiPrefab, icerikAlani);
                TextMeshProUGUI[] textler = kart.GetComponentsInChildren<TextMeshProUGUI>();
                if (textler.Length >= 2)
                {
                    textler[0].text = (i + 1) + ". " + bulunanDeliller[i];
                    textler[1].text = bulunanNotlar[i];
                }
            }
            else
            {
                KartOlustur(i);
            }
        }
    }

    void KartOlustur(int index)
    {
        GameObject kart = new GameObject("Kart_" + index);
        kart.transform.SetParent(icerikAlani, false);

        RectTransform kartRect = kart.AddComponent<RectTransform>();
        kartRect.sizeDelta = new Vector2(0, 110);

        // Layout Element sayesinde genişlik otomatik olarak Content nesnesine uyum sağlar
        LayoutElement layoutElement = kart.AddComponent<LayoutElement>();
        layoutElement.minHeight = 110;
        layoutElement.preferredWidth = 900; 

        Image kartArkaplan = kart.AddComponent<Image>();
        kartArkaplan.color = new Color(0.15f, 0.1f, 0.05f, 0.9f);

        // Delil adı
        GameObject adObj = new GameObject("Ad");
        adObj.transform.SetParent(kart.transform, false);
        RectTransform adRect = adObj.AddComponent<RectTransform>();
        adRect.anchorMin = new Vector2(0, 0.5f);
        adRect.anchorMax = new Vector2(1, 1);
        adRect.offsetMin = new Vector2(20, 5);
        adRect.offsetMax = new Vector2(-20, -5);

        TextMeshProUGUI adText = adObj.AddComponent<TextMeshProUGUI>();
        adText.text = "🔍 " + bulunanDeliller[index];
        adText.fontSize = 22;
        adText.fontStyle = FontStyles.Bold;
        adText.color = new Color(1f, 0.55f, 0f);
        adText.enableWordWrapping = true;
        adText.overflowMode = TextOverflowModes.Ellipsis;

        // Dedektif notu
        GameObject notObj = new GameObject("Not");
        notObj.transform.SetParent(kart.transform, false);
        RectTransform notRect = notObj.AddComponent<RectTransform>();
        notRect.anchorMin = new Vector2(0, 0);
        notRect.anchorMax = new Vector2(1, 0.5f);
        notRect.offsetMin = new Vector2(20, 5);
        notRect.offsetMax = new Vector2(-20, -5);

        TextMeshProUGUI notText = notObj.AddComponent<TextMeshProUGUI>();
        notText.text = "▸ " + bulunanNotlar[index];
        notText.fontSize = 18;
        notText.color = new Color(0.85f, 0.85f, 0.85f);
        notText.enableWordWrapping = true;
    }
}