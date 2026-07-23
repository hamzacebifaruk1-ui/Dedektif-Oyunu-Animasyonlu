using UnityEngine;
using UnityEngine.InputSystem; 
using UnityEngine.UI; // Unity UI sistemi için gerekli

public class KapiKONTROL : MonoBehaviour
{
    [Header("Kapı Ayarları")]
    public float acilmaAcisi = 90f; // Kapının kaç derece açılacağı (ters açılıyorsa -90 yap)
    public float acilmaHizi = 3f;   // Kapının açılma/kapanma hızı

    [Header("Mesafe Ayarları")]
    public float etkilesimMesafesi = 3f; // Oyuncunun kapıya ne kadar yakın olması gerektiği

    [Header("Referanslar (UI)")]
    public Transform oyuncuTransform; // Oyuncunun Transform'u
    public GameObject uiYazisiObjesi; // "E ile Aç" yazısını içeren UI Paneli/Objesi (Örn: Canvas içindeki bir Panel veya Text)
    public Text uiYazisiText;        // Eğer direkt Text bileşenini atayacaksanız

    private bool acikMi = false;
    private Quaternion kapaliRotasyon;
    private Quaternion acikRotasyon;
    private bool oyuncuYakinMi = false;

    void Start()
    {
        // Kapının başlangıçtaki duruşunu (kapali halini) kaydet
        kapaliRotasyon = transform.localRotation;
        
        // Y ekseninde belirlenen açı kadar dönmüş halini hesapla
        acikRotasyon = kapaliRotasyon * Quaternion.Euler(0, acilmaAcisi, 0);

        // Eğer oyuncu atanmadıysa "Player" etiketli objeyi otomatik bul
        if (oyuncuTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                oyuncuTransform = player.transform;
        }

        // Başlangıçta UI yazısını kapa
        if (uiYazisiObjesi != null) uiYazisiObjesi.SetActive(false);
    }

    void Update()
    {
        if (oyuncuTransform == null) return;

        // Oyuncu ile kapı arasındaki mesafeyi ölç
        float mesafe = Vector3.Distance(transform.position, oyuncuTransform.position);

        // Mesafe kontrolü ve UI Güncelleme
        if (mesafe <= etkilesimMesafesi)
        {
            oyuncuYakinMi = true;
            if (uiYazisiObjesi != null && !uiYazisiObjesi.activeSelf)
            {
                uiYazisiObjesi.SetActive(true);
                // Yazıyı duruma göre güncelle (İsteğe bağlı)
                if (uiYazisiText != null) uiYazisiText.text = acikMi ? "[E] Kapat" : "[E] Aç";
            }
        }
        else
        {
            oyuncuYakinMi = false;
            if (uiYazisiObjesi != null && uiYazisiObjesi.activeSelf)
            {
                uiYazisiObjesi.SetActive(false);
            }
        }

       if (oyuncuYakinMi && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
{
    Debug.Log("E tuşuna basıldı! Kapı durumu değişiyor..."); // Konsola basım mesajı yollar
    acikMi = !acikMi;
    
    if (uiYazisiText != null) uiYazisiText.text = acikMi ? "[E] Kapat" : "[E] Aç";
}

        // Kapının rotasyonunu yumuşak bir şekilde (Slerp) hedef açıya doğru döndür
        Quaternion hedefRotasyon = acikMi ? acikRotasyon : kapaliRotasyon;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, hedefRotasyon, Time.deltaTime * acilmaHizi);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, etkilesimMesafesi);
    }
}