using UnityEngine;
using UnityEngine.InputSystem; 
using UnityEngine.UI;

public class KapiKONTROL : MonoBehaviour
{
    [Header("Kapı Ayarları")]
    public float acilmaAcisi = 90f; 
    public float acilmaHizi = 3f;   

    [Header("Mesafe Ayarları")]
    public float etkilesimMesafesi = 3f; 

    [Header("Ses Ayarları")]
    public AudioSource audioSource;  // Kapı üzerindeki AudioSource
    public AudioClip kapiAcSes;      // Kapı açılma ses dosyası
    public AudioClip kapiKapatSes;   // Kapı kapanma ses dosyası

    [Header("Referanslar (UI)")]
    public Transform oyuncuTransform; 
    public GameObject uiYazisiObjesi; 
    public Text uiYazisiText;        

    private bool acikMi = false;
    private Quaternion kapaliRotasyon;
    private Quaternion acikRotasyon;
    private bool oyuncuYakinMi = false;

    void Start()
    {
        kapaliRotasyon = transform.localRotation;
        acikRotasyon = kapaliRotasyon * Quaternion.Euler(0, acilmaAcisi, 0);

        // Eğer AudioSource ataiınmadıysa obje üzerindekini otomatik al
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (oyuncuTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                oyuncuTransform = player.transform;
        }

        if (uiYazisiObjesi != null) uiYazisiObjesi.SetActive(false);
    }

    void Update()
    {
        if (oyuncuTransform == null) return;

        float mesafe = Vector3.Distance(transform.position, oyuncuTransform.position);

        if (mesafe <= etkilesimMesafesi)
        {
            oyuncuYakinMi = true;
            if (uiYazisiObjesi != null && !uiYazisiObjesi.activeSelf)
            {
                uiYazisiObjesi.SetActive(true);
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
            acikMi = !acikMi;
            
            // SES ÇALMA MANTIĞI
            if (audioSource != null)
            {
                if (acikMi && kapiAcSes != null)
                {
                    audioSource.PlayOneShot(kapiAcSes);
                }
                else if (!acikMi && kapiKapatSes != null)
                {
                    audioSource.PlayOneShot(kapiKapatSes);
                }
            }

            if (uiYazisiText != null) uiYazisiText.text = acikMi ? "[E] Kapat" : "[E] Aç";
        }

        Quaternion hedefRotasyon = acikMi ? acikRotasyon : kapaliRotasyon;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, hedefRotasyon, Time.deltaTime * acilmaHizi);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, etkilesimMesafesi);
    }
}