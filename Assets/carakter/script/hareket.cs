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

        if (!comeldi)
        {
            // Koşma hızı için hem shift basılı olmalı hem de hareket ediyor olmalı
            bool kosuyorMu = shiftBasili && girdi.magnitude > 0.1f;
            float hedefHiz = kosuyorMu ? kosmaHizi : yuruyusHizi;

            if (girdi.magnitude >= 0.1f)
            {
                float kameraAci = kameraScript.GetYAci();
                float hedefAci = Mathf.Atan2(girdi.x, girdi.z) * Mathf.Rad2Deg + kameraAci;

                float donAci = Mathf.LerpAngle(transform.eulerAngles.y, hedefAci, donmeHizi * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0f, donAci, 0f);

                Vector3 hareketYonu = Quaternion.Euler(0f, hedefAci, 0f) * Vector3.forward;
                smoothHareketYonu = Vector3.Lerp(smoothHareketYonu, hareketYonu, 10f * Time.deltaTime);
                
                controller.Move(smoothHareketYonu.normalized * hedefHiz * Time.deltaTime);
            }
            else
            {
                smoothHareketYonu = Vector3.Lerp(smoothHareketYonu, Vector3.zero, 10f * Time.deltaTime);
            }

            // --- ZIPlAMA KISITLAMASI BURADA ---
            // Sadece yerdeyken VE KOŞMUYORKEN zıplayabilir
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
            shiftBasili = false;
        }

        // 3. YERÇEKİMİ
        if (controller.isGrounded && dikeyHiz < 0)
            dikeyHiz = -2f;
        else
            dikeyHiz += yerCekimi * Time.deltaTime;

        controller.Move(new Vector3(0, dikeyHiz * Time.deltaTime, 0));

        // 4. ANIMATÖR GÜNCELLEME
        float animHizi = (shiftBasili && girdi.magnitude > 0.1f) ? 1f : (girdi.magnitude > 0.1f ? 0.5f : 0f);
        float smoothAnimSpeed = Mathf.Lerp(animator.GetFloat("Speed"), animHizi, 12f * Time.deltaTime);
        
        animator.SetFloat("Speed", smoothAnimSpeed);
        animator.SetBool("isRunning", (shiftBasili && girdi.magnitude > 0.1f));

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