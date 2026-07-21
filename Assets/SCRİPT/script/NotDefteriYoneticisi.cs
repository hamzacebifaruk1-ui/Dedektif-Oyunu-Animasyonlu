using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class NotDefteriYoneticisi : MonoBehaviour
{
    public static NotDefteriYoneticisi Instance;

    [Header("UI Elemanları")]
    public GameObject notDefteriPanel;
    public Transform icerikAlani; 

    [Header("Prefab")]
    public GameObject delilKartiPrefab; 

    private bool acik = false;
    private List<string> bulunanDeliller = new List<string>();
    private List<string> bulunanNotlar = new List<string>();

    // 🔍 DELİLLERİN TAM LİSTESİ VE DEDEKTİF NOTLARI
    private Dictionary<string, string> delilNotlari = new Dictionary<string, string>()
    {
        // delilNotlari sözlüğünün içine eklenen yeni satırlar:
        { "Murat'ın Gizli Mektubu", "Murat'ın ölmeden önce Ahmet'in dolabına bıraktığı mektup. Kemal Müdür ile kızı arasındaki ilişkiyi ve tehditleri kanıtlıyor." },
        { "Zimmet Kayıt Belgesi", "Jeneratör odasında gizlenmiş evrak. Kemal Müdür'ün sahte faturalarla bütçeyi boşalttığını kesinleştiriyor." },
        { "Şirket Evrakları", "Liman Müdürü Kemal'in şirket bütçesinden çaldığını, kalitesiz ekipman alımı yaptığını ve mali yolsuzluklarını kanıtlayan naylon faturalar." },
        { "Yırtık Bakım Defteri", "Bakım kayıtları sahte. Biri vinç bakımının yapıldığını gizlemek için sayfaları bilerek yırtmış." },
        { "USB Bellek", "Güvenlik kulübesinden alındı. Güvenlik Rıza'nın 'elektrikler kesildi' yalanını çürüten sistem loglarını içeriyor." },
        { "Yırtık Kadın Fotoğrafı", "Murat'ın, Kemal Müdür'ün kızıyla çekilmiş fotoğrafı. Cinayetin arkasındaki kişisel motifi ve şantajı kanıtlıyor." },
        { "Boş İlaç Şişesi", "Murat'a intihar süsü vermek için olay yerine bırakılmış sahte sakinleştirici şişesi." },
        { "Çamurlu Lastik İzi", "Olay gecesi şantiyeye giren lüks araca ait izler. Kemal Müdür'ün özel aracıyla birebir uyuşuyor." },
        { "Spiral Taşlama Makinesi", "Korkuluk demirlerini ve vinç vidalarını kasıtlı olarak zayıflatmak/kesmek için kullanılmış alet." },
        { "Kırık Vinç Teli", "Vinç telinin doğal aşınmayla değil, spiral taşlama aletiyle kesilerek zayıflatıldığını gösteren halat parçası." },
        { "Kirlenmiş Baret", "Olay yerinin uzağında bulunmuş, dedektifi şaşırtmak ve vakit kaybettirmek için bırakılmış sahte iz." }
        
    };

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        bulunanDeliller.Clear();
        bulunanNotlar.Clear();
        if (notDefteriPanel != null) notDefteriPanel.SetActive(false);
    }

    void Update()
    {
        Keyboard klavye = Keyboard.current;
        if (klavye == null) return;

        if (klavye.tabKey.wasPressedThisFrame)
        {
            acik = !acik;
            if (notDefteriPanel != null) notDefteriPanel.SetActive(acik);

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

    // Metin karşılaştırma hatalarını yok eden arama fonksiyonu
    private string MetniTemizle(string input)
    {
        if (string.IsNullOrEmpty(input)) return "";
        return input.ToLower()
                    .Replace("_", "")
                    .Replace(" ", "")
                    .Replace("ı", "i")
                    .Replace("ğ", "g")
                    .Replace("ü", "u")
                    .Replace("ş", "s")
                    .Replace("ö", "o")
                    .Replace("ç", "c");
    }

    public void DelilEkle(string delilAdi)
    {
        if (!bulunanDeliller.Contains(delilAdi))
        {
            bulunanDeliller.Add(delilAdi);
            
            string bulunanNotText = "İnceleniyor...";
            string gelenTemiz = MetniTemizle(delilAdi);

            foreach (var kvp in delilNotlari)
            {
                string anahtarTemiz = MetniTemizle(kvp.Key);
                if (gelenTemiz.Contains(anahtarTemiz) || anahtarTemiz.Contains(gelenTemiz))
                {
                    bulunanNotText = kvp.Value;
                    break;
                }
            }

            bulunanNotlar.Add(bulunanNotText);
            if (acik) DefterGuncelle();
        }
    }

    public string NotuGetir(string delilAdi)
    {
        string gelenTemiz = MetniTemizle(delilAdi);
        foreach (var kvp in delilNotlari)
        {
            string anahtarTemiz = MetniTemizle(kvp.Key);
            if (gelenTemiz.Contains(anahtarTemiz) || anahtarTemiz.Contains(gelenTemiz)) 
                return kvp.Value;
        }
        return "Açıklama bulunamadı.";
    }

    void DefterGuncelle()
    {
        if (icerikAlani == null) return;

        foreach (Transform child in icerikAlani) Destroy(child.gameObject);

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
    }
}