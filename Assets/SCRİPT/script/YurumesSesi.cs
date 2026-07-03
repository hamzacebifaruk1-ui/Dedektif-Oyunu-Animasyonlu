using UnityEngine;
using UnityEngine.InputSystem;

public class YurumesSesi : MonoBehaviour
{
    [Header("Ayak Sesleri")]
    public AudioClip[] yurumesSesleri;
    public AudioClip[] kosmaSesleri;
    
    [Header("Zıplama Sesleri")]
    public AudioClip ziplamaSesi; 

    [Header("Ayarlar")]
    public float yuruyusAdimSuresi = 0.5f;
    public float kosmaAdimSuresi = 0.32f;
    
    [Range(0f, 1f)]
    public float ayakSesiSeviyesi = 0.4f; 
    [Range(0f, 2f)] 
    public float ziplamaSesiSeviyesi = 1.0f;

    private AudioSource audioSource;
    private CharacterController controller;
    
    private float adimZamanlayici = 0f;
    private bool oncekiKosmaDurumu = false;
    private bool havadaMi = false; 

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        controller = GetComponent<CharacterController>();

        if (audioSource != null)
        {
            audioSource.spatialBlend = 0f; 
            audioSource.loop = false;       
        }
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        // --- 1. ZIPLAMA KONTROLÜ (PlayClipAtPoint İle Kesintisiz Çalma) ---
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (ziplamaSesi != null)
            {
                // PlayClipAtPoint, sesi yürüme kodunun Stop komutundan tamamen bağımsız bir şekilde korur ve çalar.
                AudioSource.PlayClipAtPoint(ziplamaSesi, transform.position, ziplamaSesiSeviyesi);
            }
        }

        // Klavye yön tuşları kontrolü
        bool hareketTuslari = Keyboard.current.wKey.isPressed || 
                             Keyboard.current.aKey.isPressed || 
                             Keyboard.current.sKey.isPressed || 
                             Keyboard.current.dKey.isPressed;

        bool hareketEdiyor = controller.isGrounded && hareketTuslari;
        bool kosuyor = Keyboard.current.leftShiftKey.isPressed;

        // Havadan yere düşme kontrolü
        if (controller.isGrounded && havadaMi)
        {
            havadaMi = false;
            adimZamanlayici = 0f;
        }
        else if (!controller.isGrounded)
        {
            havadaMi = true;
        }

        // Koşma-Yürüme arası geçiş yumuşatması
        if (hareketEdiyor && kosuyor != oncekiKosmaDurumu)
        {
            audioSource.Stop();
            oncekiKosmaDurumu = kosuyor;
            adimZamanlayici = 0f;
        }

        // 2. YÜRÜME VE KOŞMA SESLERİ
        if (hareketEdiyor)
        {
            float gecerliSüre = kosuyor ? kosmaAdimSuresi : yuruyusAdimSuresi;
            adimZamanlayici += Time.deltaTime;

            if (adimZamanlayici >= gecerliSüre)
            {
                AudioClip[] secilenListe = kosuyor ? kosmaSesleri : yurumesSesleri;
                
                if (secilenListe != null && secilenListe.Length > 0)
                {
                    int index = Random.Range(0, secilenListe.Length);
                    
                    audioSource.pitch = kosuyor ? Random.Range(1.25f, 1.35f) : Random.Range(0.95f, 1.05f);
                    
                    audioSource.clip = secilenListe[index];
                    audioSource.volume = ayakSesiSeviyesi;
                    audioSource.Play();
                }
                adimZamanlayici = 0f;
            }
        }
        else
        {
            adimZamanlayici = 0f;
            // Eğer karakter havada değilse (yani sadece duruyorsa) adımları sustur
            if (!havadaMi && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}