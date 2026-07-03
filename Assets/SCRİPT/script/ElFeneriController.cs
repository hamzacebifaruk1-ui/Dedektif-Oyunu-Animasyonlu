using UnityEngine;
using UnityEngine.InputSystem;

public class ElFeneriController : MonoBehaviour
{
    [Header("Ses Ayarları")]
    public AudioClip fenerAcmaSesi;    
    public AudioClip fenerKapatmaSesi; 
    
    [Range(0f, 1f)]
    public float sesSeviyesi = 1f; 

    [Header("Bileşen Bağlantısı")]
    public AudioSource fenerAudioSource; 

    private Light elFeneriIsigi; 
    private bool fenerAcikMi = false;

    void Start()
    {
        // 1. ADIM: Audio Source bağlantısını kontrol et
        if (fenerAudioSource == null)
        {
            fenerAudioSource = GetComponent<AudioSource>();
        }

        if (fenerAudioSource != null)
        {
            fenerAudioSource.spatialBlend = 0f; 
            fenerAudioSource.volume = sesSeviyesi;
        }

        // 2. ADIM: Işık bileşenini bul (İster adı ElFeneri olsun ister FenerIsigi)
        // Senin müfettişinde "FenerIsigi" yazıyor, bu yüzden doğrudan alt objelerdeki Light'ı aratıyoruz
        elFeneriIsigi = GetComponentInChildren<Light>();

        if (elFeneriIsigi != null)
        {
            elFeneriIsigi.enabled = false; // Oyun başlarken kapalı olsun
        }
    }

    void Update()
    {
        // Karakterin hareket durumundan TAMAMEN BAĞIMSIZ tuş kontrolü
        // Klavyeden 'L' tuşuna basılıp basılmadığını en yalın haliyle kontrol ediyoruz
        if (Keyboard.current != null && Keyboard.current.lKey.wasPressedThisFrame)
        {
            FeneriGuncelle();
        }
    }

    void FeneriGuncelle()
    {
        // Durumu tersine çevir (Açıksa kapat, kapalıysa aç)
        fenerAcikMi = !fenerAcikMi;

        // Işığı değiştir
        if (elFeneriIsigi != null)
        {
            elFeneriIsigi.enabled = fenerAcikMi;
        }

        // Sesi oynat
        if (fenerAudioSource != null)
        {
            fenerAudioSource.volume = sesSeviyesi;

            if (fenerAcikMi)
            {
                if (fenerAcmaSesi != null)
                {
                    fenerAudioSource.clip = fenerAcmaSesi;
                    fenerAudioSource.Play();
                }
            }
            else
            {
                if (fenerKapatmaSesi != null)
                {
                    fenerAudioSource.clip = fenerKapatmaSesi;
                    fenerAudioSource.Play();
                }
            }
        }
    }
}