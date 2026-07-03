using UnityEngine;
using UnityEngine.InputSystem;

public class YeniKamera : MonoBehaviour
{
    [Header("Hedef")]
    public Transform karakter;

    [Header("Kamera Mesafe Ayarları")]
    public float kameraUzakligi = 2.5f; // İdeal uzaklık
    public float kameraYuksekligi = 1.4f; // Karakterin boy hizası

    [Header("Fare Hassasiyet Ayarları")]
    public float fareHassasiyetiX = 0.08f; 
    public float fareHassasiyetiY = 0.05f; 
    public float dikeyYumusatma = 10f;     
    public float minimumAci = -20f; // Aşağı bakma sınırı       
    public float maksimumAci = 45f; // Yukarı bakma sınırı        

    [Header("Kamera Takip Akıcılığı")]
    public float takipYumusakligi = 15f; 

    [Header("Engel Kontrolü")]
    public LayerMask engelLayerMask;
    public float engelOfseti = 0.2f; 

    private float xAci = 0f;
    private float yAci = 0f;
    private float hedefXAci = 0f; 
    private Camera anaKamera;

    void Start()
    {
        anaKamera = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (karakter != null)
        {
            yAci = karakter.eulerAngles.y;
        }

        // Kameranın kırpma mesafesini en ideal ve hatasız değere sabitliyoruz
        if (anaKamera != null)
        {
            anaKamera.nearClipPlane = 0.05f;
        }
    }

    void LateUpdate()
    {
        if (karakter == null || anaKamera == null) return;

        Mouse fare = Mouse.current;
        if (fare == null) return;

        Vector2 fareDelta = fare.delta.ReadValue();
        float dt = Time.unscaledDeltaTime;

        // Fare dönüş hesaplamaları
        yAci += fareDelta.x * fareHassasiyetiX;
        hedefXAci -= fareDelta.y * fareHassasiyetiY;
        hedefXAci = Mathf.Clamp(hedefXAci, minimumAci, maksimumAci);
        
        xAci = Mathf.Lerp(xAci, hedefXAci, dikeyYumusatma * dt);

        Quaternion hedefRotasyon = Quaternion.Euler(xAci, yAci, 0f);
        transform.rotation = hedefRotasyon;
        
        // Rig merkezini karakterin pozisyonuna kilitliyoruz
        transform.position = karakter.position;

        // Başlangıç noktasını karakterin biraz yukarısı (göğs hizası) yapıyoruz
        Vector3 baslangicNoktasi = transform.position + (Vector3.up * kameraYuksekligi);
        
        // Kameranın durması gereken ideal arkadaki pozisyon
        Vector3 idealKonum = baslangicNoktasi - (transform.forward * kameraUzakligi);

        RaycastHit hit;
        Vector3 finalKameraKonumu = idealKonum;

        // Kameranın arkaya doğru attığı ışın (Duvar kontrolü)
        Vector3 isinYonu = (idealKonum - baslangicNoktasi).normalized;
        
        // Işını karakterin hemen dibinden değil, 0.3 metre arkasından başlatıyoruz ki karakterin kendi vücuduna çarpmasın!
        Vector3 güvenliBaslangic = baslangicNoktasi + (isinYonu * 0.3f); 
        float isinMesafesi = kameraUzakligi - 0.3f;

        if (Physics.Raycast(güvenliBaslangic, isinYonu, out hit, isinMesafesi, engelLayerMask))
        {
            // Duvara çarparsa kamerayı duvardan biraz öne çekiyoruz
            finalKameraKonumu = hit.point + transform.forward * engelOfseti;
        }

        // --- KESİN KORUMA: Karakterin içine girmeyi imkansız yapıyoruz ---
        float karaktereOlanMesafe = Vector3.Distance(baslangicNoktasi, finalKameraKonumu);
        if (karaktereOlanMesafe < 0.6f)
        {
            // Kamera hiçbir koşulda karakterin 0.6 metre yakınına giremez, orada kilitlenir
            finalKameraKonumu = baslangicNoktasi - (transform.forward * 0.6f);
        }

        // Kamerayı yumuşakça pozisyona yerleştir ve rotasyonu eşitle
        anaKamera.transform.position = Vector3.Lerp(anaKamera.transform.position, finalKameraKonumu, takipYumusakligi * dt);
        anaKamera.transform.rotation = transform.rotation;
    }

    public float GetYAci() { return yAci; }
}