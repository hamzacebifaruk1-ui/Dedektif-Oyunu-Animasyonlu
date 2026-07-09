using UnityEngine;
using UnityEngine.InputSystem; 
using TMPro;

public class DelilIncelemeSistemi : MonoBehaviour
{
    public static DelilIncelemeSistemi Instance;

    [Header("UI Elemanları")]
    public GameObject incelemePaneli;     
    public TextMeshProUGUI delilAdiText;  

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
    private float gecerliBoyut; // Inspector'dan gelen boyutu hafızada tutmak için ekledik

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

        if (Keyboard.current.shiftKey.wasPressedThisFrame)
        {
            IncelemeyiBitir();
            return;
        }

        if (gecerliModel != null && incelemeKamerasi != null)
        {
            // --- ZOOM SİSTEMİ ---
            float scrollDegeri = Mouse.current.scroll.ReadValue().y;
            if (scrollDegeri != 0)
            {
                gecerliMesafe -= (Mathf.Sign(scrollDegeri) * zoomHizi * 0.05f);
                gecerliMesafe = Mathf.Clamp(gecerliMesafe, minimumMesafe, maksimumMesafe);
            }

            Vector3 hedefPozisyon = incelemeKamerasi.transform.position + incelemeKamerasi.transform.forward * gecerliMesafe;
            gecerliModel.transform.position = hedefPozisyon;

            // KESİN ÇÖZÜM: Boyutu her karede zorunlu olarak güncelle ki eski boyutuna geri kaçamasın!
            gecerliModel.transform.localScale = Vector3.one * gecerliBoyut; 

            // --- 360 DÖNDÜRME ---
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

    public void IncelemeyiBaslat(GameObject delilPrefab, string delilAd, float ozelBoyut, float ozelMesafe)
    {
        if (incelemePaneli == null) return;

        if (anaOyunKamerasi != null) anaOyunKamerasi.gameObject.SetActive(false);

        if (incelemeKamerasi != null) 
        {
            incelemeKamerasi.gameObject.SetActive(true);
            incelemeKamerasi.clearFlags = CameraClearFlags.SolidColor;
            incelemeKamerasi.backgroundColor = Color.black;
        }

        incelemePaneli.SetActive(true);
        if (delilAdiText != null) delilAdiText.text = delilAd;

        if (gecerliModel != null) Destroy(gecerliModel);

        if (delilPrefab != null && incelemeKamerasi != null)
        {
            gecerliMesafe = ozelMesafe; 
            gecerliBoyut = ozelBoyut; // Gelen özel boyutu hafızaya alıyoruz

            Vector3 dogumPozisyonu = incelemeKamerasi.transform.position + incelemeKamerasi.transform.forward * gecerliMesafe;
            gecerliModel = Instantiate(delilPrefab, dogumPozisyonu, Quaternion.identity);
            
            // İlk doğduğunda boyutu ata
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

    public void IncelemeyiBitir()
    {
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