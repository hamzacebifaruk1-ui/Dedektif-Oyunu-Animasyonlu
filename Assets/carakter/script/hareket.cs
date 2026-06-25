using UnityEngine;
using UnityEngine.InputSystem;

public class hareket : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float yuruyusHizi = 3f;
    public float kosmaHizi = 6f;
    public float donmeHizi = 15f;
    public float yerCekimi = -9.81f;

    [Header("Zıplama Ayarları")]
    public float ziplamaGucu = 5f;

    [Header("Çömelme Ayarları")]
    public float normalYukseklik = 1.8f;
    public float comelYukseklik = 0.9f;
    public float normalMerkez = 0.9f;
    public float comelMerkez = 0.45f;

    [Header("Bileşenler")]
    private CharacterController controller;
    private Animator animator;
    private YeniKamera kameraScript;

    private float dikeyHiz;
    private Vector3 smoothHareketYonu;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        kameraScript = FindFirstObjectByType<YeniKamera>();
    }

    void Update()
    {
        Keyboard klavye = Keyboard.current;
        if (klavye == null) return;

        bool comeldi = animator.GetBool("Comeldi");

        // 1. GİRDİLERİ OKU
        float yatay = 0f;
        float dikey = 0f;
        if (klavye.wKey.isPressed) dikey += 1f;
        if (klavye.sKey.isPressed) dikey -= 1f;
        if (klavye.aKey.isPressed) yatay -= 1f;
        if (klavye.dKey.isPressed) yatay += 1f;

        Vector3 girdi = new Vector3(yatay, 0f, dikey).normalized;

        // 2. SHIFT KONTROLÜ
        bool shiftBasili = klavye.leftShiftKey.isPressed;
        bool kosuyorMu = false;

        Vector3 finalHareket = Vector3.zero;

        if (!comeldi)
        {
            // Koşma şartı: Shift basılı VE karakter hareket ediyor
            kosuyorMu = shiftBasili && girdi.magnitude > 0.1f;
            
            // HIZ TAKILMASINI ÖNLEYEN KESKİN GEÇİŞ: 
            // Lerp kullanmıyoruz, Shift basılıysa net koşma hızı, bırakıldıysa net yürüme hızı!
            float anlikHiz = kosuyorMu ? kosmaHizi : yuruyusHizi;

            if (girdi.magnitude >= 0.1f)
            {
                float kameraAci = kameraScript.GetYAci();
                float hedefAci = Mathf.Atan2(girdi.x, girdi.z) * Mathf.Rad2Deg + kameraAci;

                float donAci = Mathf.LerpAngle(transform.eulerAngles.y, hedefAci, donmeHizi * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0f, donAci, 0f);

                Vector3 hareketYonu = Quaternion.Euler(0f, hedefAci, 0f) * Vector3.forward;
                smoothHareketYonu = Vector3.Lerp(smoothHareketYonu, hareketYonu, 20f * Time.deltaTime);
                
                // Kararsız hızı değil, direkt netleşen hızı çarpıyoruz
                finalHareket = smoothHareketYonu.normalized * anlikHiz;
            }
            else
            {
                smoothHareketYonu = Vector3.zero;
            }

            // --- ZIPlAMA KISITLAMASI ---
            if (klavye.spaceKey.wasPressedThisFrame && controller.isGrounded && !kosuyorMu)
            {
                dikeyHiz = ziplamaGucu;
                animator.SetTrigger("Jump");
            }
        }
        else
        {
            smoothHareketYonu = Vector3.zero;
            girdi = Vector3.zero;
            kosuyorMu = false;
        }

        // 3. YERÇEKİMİ
        if (controller.isGrounded && dikeyHiz < 0)
            dikeyHiz = -2f;
        else
            dikeyHiz += yerCekimi * Time.deltaTime;

        finalHareket.y = dikeyHiz;
        controller.Move(finalHareket * Time.deltaTime);

        // 4. ANIMATÖR KÜT GEÇİŞİ (Yavaşlığı Bitiren Kısım)
        // Değerleri Lerp yapmadan direkt küt diye gönderiyoruz.
        float animHizi = kosuyorMu ? 1f : (girdi.magnitude > 0.1f ? 0.5f : 0f);
        
        animator.SetFloat("Speed", animHizi); // Direkt hedef değeri vererek animasyon yumuşamasını sıfırladık
        animator.SetBool("isRunning", kosuyorMu);

        // 5. ÇÖMELME BOYUTLARI
        if (comeldi)
        {
            controller.height = Mathf.Lerp(controller.height, comelYukseklik, 10f * Time.deltaTime);
            controller.center = new Vector3(0, Mathf.Lerp(controller.center.y, comelMerkez, 10f * Time.deltaTime), 0);
        }
        else
        {
            controller.height = Mathf.Lerp(controller.height, normalYukseklik, 10f * Time.deltaTime);
            controller.center = new Vector3(0, Mathf.Lerp(controller.center.y, normalMerkez, 10f * Time.deltaTime), 0);
        }
    }
}