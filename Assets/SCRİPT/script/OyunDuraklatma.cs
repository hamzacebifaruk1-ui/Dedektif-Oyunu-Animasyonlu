using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Unity Yeni Giriş Sistemi kütüphanesi

public class OyunDuraklatma : MonoBehaviour
{
    [Header("Duraklatma Arayüzü")]
    public GameObject duraklatmaPaneli; // Sahnedeki ESC menü paneli
    
    private bool oyunDurdurulduMu = false;

    void Start()
    {
        // Oyun başında menünün kapalı olduğundan emin oluyoruz
        if (duraklatmaPaneli != null) duraklatmaPaneli.SetActive(false);
    }

    void Update()
    {
        // Yeni Giriş Sistemi: Klavyeden ESC (Escape) tuşuna basılıp basılmadığını kontrol eder
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (oyunDurdurulduMu)
            {
                DevamEt();
            }
            else
            {
                OyunuDurdur();
            }
        }
    }

    public void OyunuDurdur()
    {
        if (duraklatmaPaneli != null) 
        {
            duraklatmaPaneli.SetActive(true);
            duraklatmaPaneli.transform.SetAsLastSibling(); // Paneli katmanlarda her zaman en öne getirir
        }
        
        Time.timeScale = 0f; // Oyun içi zamanı, fiziği ve hareketleri tamamen dondurur
        oyunDurdurulduMu = true;

        // Fareyi görünür yap ve serbest bırak
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void DevamEt()
    {
        if (duraklatmaPaneli != null) duraklatmaPaneli.SetActive(false);
        
        Time.timeScale = 1f; // Zamanı normale döndürür, oyun akar
        oyunDurdurulduMu = false;

        // Fareyi tekrar oyuna kilitler (FPS kamerası kontrolü için)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void AnaMenuyeDon()
    {
        Time.timeScale = 1f; // Zamanı sıfırlamayı unutmuyoruz, yoksa menü de donuk kalır!
        SceneManager.LoadScene(0); // Build Settings'deki 0. sahneyi (Ana Menü) yükler
    }

    // ÇIKIŞ BUTONU İÇİN TETİKLENECEK YENİ FONKSİYON
    public void OyundanCik()
    {
        Debug.Log("Oyundan çıkılıyor..."); // Unity Editöründe çalıştığını görmek için log atar
        Application.Quit(); // Derlenmiş (.exe) oyunu tamamen kapatır
    }
}