using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TrafoSesKontrol : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("3D Ses Sınırları")]
    [Tooltip("Oyuncu trafoya bu mesafeden daha çok yaklaşırsa ses maksimum seviyede kalır.")]
    [SerializeField] private float enYakinMesafe = 2.0f;

    [Tooltip("Sesin tamamen duyulmaz olacağı en uzak mesafe.")]
    [SerializeField] private float enUzakMesafe = 15.0f;

    private void Start()
    {
        // Objeye bağlı olan AudioSource bileşenini otomatik alıyoruz
        audioSource = GetComponent<AudioSource>();

        // Kodun ve 3D sesin hatasız çalışması için zorunlu ayarlar:
        audioSource.loop = true;          // Ses sürekli başa sarıp çalsın
        audioSource.playOnAwake = true;   // Oyun başlar başlamaz ses devreye girsin
        
        // KRİTİK AYAR: Sesi %100 3D yapar. Sol/Sağ kulaklık dengesi ve mesafe algısı açılır.
        audioSource.spatialBlend = 1.0f; 

        // Mesafe sınırlarını koda geçiriyoruz
        audioSource.minDistance = enYakinMesafe;
        audioSource.maxDistance = enUzakMesafe;

        // Sesin mesafeyle doğrusal (lineer) olarak azalmasını sağlıyoruz
        audioSource.rolloffMode = AudioRolloffMode.Linear;

        // Ayarlardan sonra sesi oynat
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}