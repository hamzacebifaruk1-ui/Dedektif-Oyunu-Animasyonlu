using UnityEngine;
using UnityEngine.InputSystem;

public class YeniKamera : MonoBehaviour
{
    [Header("Hedef")]
    public Transform karakter;

    [Header("Kamera Ayarları")]
    public float fareHassasiyeti = 0.5f; // Hassasiyeti biraz dengeledik
    public float minimumAci = -30f;
    public float maksimumAci = 60f;
    public float kameraUzakligi = 3f;
    public float kameraYuksekligi = 1.6f;
    public float takipYumusakligi = 15f; // Kameranın akıcılığı için

    [Header("Engel Kontrolü")]
    public LayerMask engelLayerMask;
    public float engelOfseti = 0.2f; // Duvara çok yapışmaması için

    private float xAci = 0f;
    private float yAci = 0f;
    private Camera anaKamera;
    private Vector3 mevcutKameraKonumu;

    void Start()
    {
        anaKamera = Camera.main;

        // Fareyi kilitle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Başlangıç açısını karakterden al
        if (karakter != null)
        {
            yAci = karakter.eulerAngles.y;
            mevcutKameraKonumu = transform.position;
        }
    }

    void LateUpdate()
    {
        if (karakter == null) return;

        // Fare girişi
        Mouse fare = Mouse.current;
        if (fare == null) return;

        // Delta değerlerini oku
        Vector2 fareDelta = fare.delta.ReadValue();

        // Açıları güncelle (UnscaledDeltaTime kullanımı FPS düşüşlerinde hızı korur)
        yAci += fareDelta.x * fareHassasiyeti * 0.1f;
        xAci -= fareDelta.y * fareHassasiyeti * 0.1f;
        xAci = Mathf.Clamp(xAci, minimumAci, maksimumAci);

        // KameraRig (bu scriptin takılı olduğu obje) konum ve rotasyonu
        Quaternion hedefRotasyon = Quaternion.Euler(xAci, yAci, 0f);
        transform.rotation = hedefRotasyon;
        transform.position = karakter.position + Vector3.up * kameraYuksekligi;

        // --- OPTİMİZE EDİLMİŞ ENGEL KONTROLÜ ---
        Vector3 arkaYon = -transform.forward;
        Vector3 idealKonum = transform.position + (arkaYon * kameraUzakligi);
        
        RaycastHit hit;
        // Raycast'i karakterin biraz üzerinden (kameraRig pozisyonundan) arkaya doğru atıyoruz
        if (Physics.Raycast(transform.position, arkaYon, out hit, kameraUzakligi, engelLayerMask))
        {
            // Eğer engel varsa, vurulan noktadan biraz öne (karaktere doğru) çekiyoruz
            Vector3 engelKonumu = hit.point + transform.forward * engelOfseti;
            anaKamera.transform.position = Vector3.Lerp(anaKamera.transform.position, engelKonumu, takipYumusakligi * Time.deltaTime);
        }
        else
        {
            // Engel yoksa ideal uzaklığa yumuşakça git
            anaKamera.transform.position = Vector3.Lerp(anaKamera.transform.position, idealKonum, takipYumusakligi * Time.deltaTime);
        }

        // Kamera her zaman Rig'in rotasyonuna bakmalı
        anaKamera.transform.rotation = transform.rotation;
    }

    public float GetYAci() { return yAci; }
}