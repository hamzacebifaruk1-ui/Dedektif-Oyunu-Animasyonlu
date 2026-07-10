using UnityEngine;
using UnityEngine.InputSystem; 
using TMPro;

public class DelilIncelemeSistemi : MonoBehaviour
{
    public static DelilIncelemeSistemi Instance;

    [Header("UI Elemanları")]
    public GameObject incelemePaneli; 
    public TextMeshProUGUI delilAdiText; 
    public TextMeshProUGUI yardimciIpuclariText; 

    [Header("Kamera Ayarları (Sürükle-Bırak)")] 
    public Camera anaOyunKamerasi; 
    public Camera incelemeKamerasi; 

    [Header("Döndürme Ayarları")] 
    public float dondurmeHizi = 0.2f; 

    [Header("Scroll Zoom Hız Ayarları")] 
    public float zoomHizi = 15.0f; 
    public float minimumMesafe = 0.05f; 
    public float maksimumMesafe = 5.0f; 

    private GameObject gecerliModel;
    private Vector2 sonFarePozisyonu;
    private float gecerliMesafe;
    private float gecerliBoyut;
    private string m_gecerliDelilAdi; 

    // Arayüz butonlarının kontrol edeceği durum değişkeni
    [HideInInspector] public bool gecerliDelilGercekMi = true; 

    private int toplananDelilSayisi = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (incelemePaneli != null) incelemePaneli.SetActive(false); 
        if (incelemeKamerasi != null) incelemeKamerasi.gameObject.SetActive(false); 
    }

    private void Update()
    {
        if (incelemePaneli == null || !incelemePaneli.activeSelf || Mouse.current == null || Keyboard.current == null) return; 

        Keyboard klavye = Keyboard.current; 

        if (klavye.shiftKey.wasPressedThisFrame) 
        {
            IncelemeyiBitir(); 
            return;
        }

        if (gecerliModel != null && incelemeKamerasi != null)
        {
            float scrollDegeri = Mouse.current.scroll.ReadValue().y; 
            if (scrollDegeri != 0) 
            {
                gecerliMesafe -= (Mathf.Sign(scrollDegeri) * zoomHizi * 0.05f); 
                gecerliMesafe = Mathf.Clamp(gecerliMesafe, minimumMesafe, maksimumMesafe); 
            }

            Vector3 hedefPozisyon = incelemeKamerasi.transform.position + incelemeKamerasi.transform.forward * gecerliMesafe; 
            gecerliModel.transform.position = hedefPozisyon; 
            gecerliModel.transform.localScale = Vector3.one * gecerliBoyut; 

            Vector2 mevcutFarePozisyonu = Mouse.current.position.ReadValue(); 

            if (Mouse.current.leftButton.wasPressedThisFrame) 
            {
                sonFarePozisyonu = mevcutFarePozisyonu; 
            }

            if (Mouse.current.leftButton.isPressed) 
            {
                Vector2 fareDelta = mevcutFarePozisyonu - sonFarePozisyonu; 
                gecerliModel.transform.Rotate(incelemeKamerasi.transform.up, -fareDelta.x * dondurmeHizi, Space.World); 
                gecerliModel.transform.Rotate(incelemeKamerasi.transform.right, fareDelta.y * dondurmeHizi, Space.World); 
                sonFarePozisyonu = mevcutFarePozisyonu; 
            }
        }
    }

    // DelilNesnesi.cs içindeki çağrıyla tam uyumlu orijinal parametre yapısı
    public void IncelemeyiBaslat(GameObject delilPrefab, string delilAd, float ozelBoyut, float ozelMesafe) 
    {
        if (incelemePaneli == null) return; 

        m_gecerliDelilAdi = delilAd; 

        if (anaOyunKamerasi != null) anaOyunKamerasi.gameObject.SetActive(false); 

        if (incelemeKamerasi != null) 
        {
            incelemeKamerasi.gameObject.SetActive(true); 
            incelemeKamerasi.clearFlags = CameraClearFlags.SolidColor; 
            incelemeKamerasi.backgroundColor = Color.black; 
        }

        incelemePaneli.SetActive(true); 
        if (delilAdiText != null) delilAdiText.text = delilAd + " İnceleniyor..."; 

        if (yardimciIpuclariText != null) 
            yardimciIpuclariText.text = "Fare Sol Tık: Döndür  |  Scroll: Zoom  |  Butonlar ile Karar Ver"; 

        if (gecerliModel != null) Destroy(gecerliModel); 

        if (delilPrefab != null && incelemeKamerasi != null) 
        {
            gecerliMesafe = ozelMesafe; 
            gecerliBoyut = ozelBoyut; 

            Vector3 dogumPozisyonu = incelemeKamerasi.transform.position + incelemeKamerasi.transform.forward * gecerliMesafe; 
            gecerliModel = Instantiate(delilPrefab, dogumPozisyonu, Quaternion.identity); 
            gecerliModel.transform.localScale = Vector3.one * gecerliBoyut; 

            Rigidbody rb = gecerliModel.GetComponent<Rigidbody>(); 
            if (rb != null) rb.isKinematic = true; 

            Collider col = gecerliModel.GetComponent<Collider>(); 
            if (col != null) col.enabled = false; 
        }

        hareket oyuncuScripti = FindFirstObjectByType<hareket>(); 
        if (oyuncuScripti != null) oyuncuScripti.hareketEdebilirMi = false; 
        
        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true; 
    }

    // UI'daki butonların (Gerçek / Sahte) tetikleyeceği yeni metot
    public void SecimYap(bool oyuncununTahminiGercekMi)
    {
        if (oyuncununTahminiGercekMi == gecerliDelilGercekMi)
        {
            Debug.Log("Doğru Karar!");
        }
        else
        {
            Debug.Log("Hatalı Karar!");
        }

        OyuncuDelilOlarakOnayladi();
    }

    private void OyuncuDelilOlarakOnayladi() 
    {
        if (string.IsNullOrEmpty(m_gecerliDelilAdi)) return; 

        toplananDelilSayisi++;
        if (DiyalogYoneticisi.Instance != null && DiyalogYoneticisi.Instance.delilYazisiText != null)
        {
            DiyalogYoneticisi.Instance.delilYazisiText.text = "Toplanan Delil: " + toplananDelilSayisi + " / 5";
        }

        HaritaRotaCizici[] tumCiziciler = FindObjectsByType<HaritaRotaCizici>(FindObjectsSortMode.None);
        foreach (HaritaRotaCizici cizici in tumCiziciler)
        {
            cizici.HedefiDegistir(null); 
        }

        if (toplananDelilSayisi >= 5)
        {
            if (DiyalogYoneticisi.Instance != null && DiyalogYoneticisi.Instance.gorevYazisiText != null)
            {
                DiyalogYoneticisi.Instance.gorevYazisiText.text = "GÖREV GÜNCELLENDİ:\nŞüphelilerle yüzleş ve yalanlarını ortaya çıkar!";
            }
        }

        if (NotDefteriYoneticisi.Instance != null)
            NotDefteriYoneticisi.Instance.DelilEkle(m_gecerliDelilAdi); 

        if (DelilYoneticisi.Instance != null)
            DelilYoneticisi.Instance.DelilBulundu(m_gecerliDelilAdi); 

        IncelemeyiBitir(); 
    }

    public void IncelemeyiBitir() 
    {
        m_gecerliDelilAdi = ""; 
        if (incelemePaneli != null) incelemePaneli.SetActive(false); 
        if (gecerliModel != null) Destroy(gecerliModel); 

        if (incelemeKamerasi != null) incelemeKamerasi.gameObject.SetActive(false); 
        if (anaOyunKamerasi != null) anaOyunKamerasi.gameObject.SetActive(true); 

        hareket oyuncuScripti = FindFirstObjectByType<hareket>(); 
        if (oyuncuScripti != null) oyuncuScripti.hareketEdebilirMi = true; 

        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false; 
    }
}