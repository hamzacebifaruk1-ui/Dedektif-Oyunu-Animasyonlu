using UnityEngine;
using UnityEngine.InputSystem;

public class YeniKamera : MonoBehaviour
{
    [Header("Hedef")]
    public Transform karakter;

    [Header("Kamera Mesafe Ayarları")]
    public float kameraUzakligi = 2.2f;
    
    [Tooltip("Kameranın yerden yüksekliği. Sabit dünya ekseninde hesaplanır, yerin altına girmez.")]
    public float kameraYuksekligi = 1.3f;
    
    [Tooltip("Kamerayı sağ/sol omuza kaydırma (Örn: 0.5 sağ omuz)")]
    public float omuzKaymasi = 0.5f; 

    [Header("Fare Ayarları")]
    public float fareHassasiyeti = 0.5f; 
    public float minimumAci = -20f; // Yere çok fazla dik bakıp zemini delmesin diye daralttık
    public float maksimumAci = 50f;
    public float takipYumusakligi = 15f; 

    [Header("Engel Kontrolü")]
    public LayerMask engelLayerMask;
    public float engelOfseti = 0.2f; 

    private float xAci = 0f;
    private float yAci = 0f;
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
    }

    void LateUpdate()
    {
        if (karakter == null) return;

        Mouse fare = Mouse.current;
        if (fare == null) return;

        Vector2 fareDelta = fare.delta.ReadValue();

        yAci += fareDelta.x * fareHassasiyeti * 0.1f;
        xAci -= fareDelta.y * fareHassasiyeti * 0.1f;
        xAci = Mathf.Clamp(xAci, minimumAci, maksimumAci);

        Quaternion hedefRotasyon = Quaternion.Euler(xAci, yAci, 0f);
        transform.rotation = hedefRotasyon;
        
        // STABİLİTE: Rig pozisyonunu karaktere tamamen kilitliyoruz (Titreşimi önlemek için)
        transform.position = karakter.position;

        // --- DÜNYA EKSENLİ OMUZ HESAPLAMA ---
        Vector3 arkaYon = -transform.forward;
        Vector3 sagYon = transform.right;
        
        Vector3 idealKonum = transform.position + (arkaYon * kameraUzakligi) + (sagYon * omuzKaymasi) + (Vector3.up * kameraYuksekligi);
        
        // HATA BURADAYDI: Boşluk silindi ve değişken ismi birleştirildi
        Vector3 isinBaslangicNoktasi = transform.position + (Vector3.up * kameraYuksekligi);
        Vector3 raycastHedefYon = (idealKonum - isinBaslangicNoktasi).normalized;
        float toplamMesafe = Vector3.Distance(isinBaslangicNoktasi, idealKonum);

        RaycastHit hit;
        if (Physics.Raycast(isinBaslangicNoktasi, raycastHedefYon, out hit, toplamMesafe, engelLayerMask))
        {
            Vector3 engelKonumu = hit.point + transform.forward * engelOfseti;
            
            // Yere çarpma durumunda ekstra koruma: Yerin altına girmeyi engeller
            if(engelKonumu.y < karakter.position.y + 0.2f) 
            {
                engelKonumu.y = karakter.position.y + 0.2f;
            }
            
            anaKamera.transform.position = Vector3.Lerp(anaKamera.transform.position, engelKonumu, takipYumusakligi * Time.deltaTime);
        }
        else
        {
            anaKamera.transform.position = Vector3.Lerp(anaKamera.transform.position, idealKonum, takipYumusakligi * Time.deltaTime);
        }

        anaKamera.transform.rotation = transform.rotation;
    }

    public float GetYAci() { return yAci; }
}