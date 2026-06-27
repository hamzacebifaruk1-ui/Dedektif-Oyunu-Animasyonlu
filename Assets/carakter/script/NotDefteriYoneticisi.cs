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
    public GameObject delilKartiPrefab; // Boş kalırsak kod otomatik oluşturur

    private bool acik = false;
    private List<string> bulunanDeliller = new List<string>();
    private List<string> bulunanNotlar = new List<string>();

    // Yeni Paket Hikayeye Göre Güncellenmiş Dedektif Notları
    private Dictionary<string, string> delilNotlari = new Dictionary<string, string>()
    {
        { "Yırtık Bakım Defteri", "Bakım kayıtları sahte. Biri bu defterin sayfalarını bilerek yırtmış." },
        { "Kırık Vinç Teli", "Bu çelik tel yıpranmayla kopmaz. Ağzı spiral taşıyla kesilmiş gibi duruyor." },
        { "Anonim Not", "Murat'ın baretine sıkıştırılan not: 'Konuşursan sonun limanın dibi olur' yazıyor." },
        { "Güvenlik Kamera Kaydı", "Gece 02:00 kayıtları. Müdür Kemal'in elinde bir aletle vinç dairesine girdiğini gösteriyor." },
        { "İlaç Kutusu", "Murat'ın kullandığı ağır göz ilacı. Gece vardiyasında çalışması yasal olarak imkansızdı." }
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
        bulunanDeliller.Clear();
        bulunanNotlar.Clear();

        if (notDefteriPanel != null)
            notDefteriPanel.SetActive(false);
    }

    void Update()
    {
        Keyboard klavye = Keyboard.current;
        if (klavye == null) return;

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

            if (acik)
            {
                DefterGuncelle();
            }
        }
    }

    void DefterGuncelle()
    {
        if (icerikAlani == null) return;

        foreach (Transform child in icerikAlani)
        {
            Destroy(child.gameObject);
        }

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

        LayoutElement layoutElement = kart.AddComponent<LayoutElement>();
        layoutElement.minHeight = 110;
        layoutElement.preferredWidth = 900; 

        Image kartArkaplan = kart.AddComponent<Image>();
        kartArkaplan.color = new Color(0.15f, 0.1f, 0.05f, 0.9f);

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