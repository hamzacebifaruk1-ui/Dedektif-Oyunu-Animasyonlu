using UnityEngine;
using TMPro;

public class HedefTakipci : MonoBehaviour
{
    public static HedefTakipci Instance;

    [Header("UI Elemanı")]
    public TextMeshProUGUI mesafeText; // Ekrandaki yazı alanı

    [Header("Oyuncu Referansı")]
    public Transform oyuncu; // Dedektifin kendisi (Main Camera veya Player nesnesi)

    private Transform aktifHedef;
    private string hedefIsmi = "";
    private float yokOlmaMesafesi = 3f; // Kaç metre kala yazı gizlensin?

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (mesafeText != null) mesafeText.text = "";
    }

    public void HedefDegistir(GameObject yeniHedefTarget, string npcAdi)
    {
        if (yeniHedefTarget != null)
        {
            aktifHedef = yeniHedefTarget.transform;
            hedefIsmi = npcAdi;
        }
        else
        {
            aktifHedef = null;
            hedefIsmi = "";
            if (mesafeText != null) mesafeText.text = "";
        }
    }

    void Update()
    {
        if (aktifHedef == null || oyuncu == null || mesafeText == null) return;

        // 1. Mesafeyi Hesapla
        float mesafe = Vector3.Distance(oyuncu.position, aktifHedef.position);

        if (mesafe <= yokOlmaMesafesi)
        {
            mesafeText.text = "";
            return;
        }

        // 2. Yönü ve Açıyı Hesapla (Sağ-Sol-Ön Kontrolü)
        Vector3 hedefeGidenYon = (aktifHedef.position - oyuncu.position).normalized;
        float aci = Vector3.SignedAngle(oyuncu.forward, hedefeGidenYon, Vector3.up);

        string yonMetni = "";

        // SADECE METİN: Hiçbir sembol veya sprite barındırmaz, hata verme ihtimali sıfırdır.
        if (aci >= -20f && aci <= 20f)
        {
            yonMetni = "İlerle";
        }
        else if (aci > 20f && aci <= 100f)
        {
            yonMetni = "Sağa Dön";
        }
        else if (aci < -20f && aci >= -100f)
        {
            yonMetni = "Sola Dön";
        }
        else
        {
            yonMetni = "Arkana Dön";
        }

        // 3. Ekrana Düz Metin Olarak Yazdır
        // ÇIKTI ÖRNEĞİ: Müdür Kemal (107m) | İlerle
        mesafeText.text = $"{hedefIsmi} ({Mathf.RoundToInt(mesafe)}m)  |  <color=yellow>{yonMetni}</color>";
    }
}