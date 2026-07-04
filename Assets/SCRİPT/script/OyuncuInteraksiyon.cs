using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class OyuncuInteraksiyon : MonoBehaviour
{
    [Header("Ayarlar")]
    public float etkilesimMesafesi = 3f;

    [Header("UI")]
    public TextMeshProUGUI ipucuText;

    [Header("Animasyon")]
    private Animator animator;

    private DelilNesnesi yakinDelil = null;
    private bool incelemeModu = false;
    private bool alModu = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (ipucuText == null)
        {
            Debug.LogError("IpucuText bağlı değil!");
            return;
        }

        ipucuText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!incelemeModu && !alModu)
        {
            YakinDelilKontrol();
        }

        EInputKontrol();
        FInputKontrol();
    }

    void YakinDelilKontrol()
    {
        Collider[] yakinlar = Physics.OverlapSphere(transform.position, etkilesimMesafesi);

        DelilNesnesi enYakinDelil = null;
        float enYakinMesafe = etkilesimMesafesi;

        foreach (Collider col in yakinlar)
        {
            if (col.transform == transform || col.transform.IsChildOf(transform)) continue;

            DelilNesnesi delil = col.GetComponent<DelilNesnesi>();
            if (delil == null)
                delil = col.GetComponentInParent<DelilNesnesi>();

            if (delil != null && delil.ToplanabilirMi())
            {
                float mesafe = Vector3.Distance(transform.position, delil.transform.position);
                if (mesafe < enYakinMesafe)
                {
                    enYakinMesafe = mesafe;
                    enYakinDelil = delil;
                }
            }
        }

        yakinDelil = enYakinDelil;

        if (yakinDelil != null)
        {
            if (ipucuText != null)
            {
                ipucuText.gameObject.SetActive(true);
                
                // --- MASADA VE YERDE AYRIMI ---
                if (yakinDelil.delilKonumu == DelilNesnesi.DelilKonumu.Masada)
                {
                    ipucuText.text = "F - İncele";
                }
                else
                {
                    ipucuText.text = "E - İncele";
                }
            }
        }
        else
        {
            if (ipucuText != null && !incelemeModu && !alModu)
            {
                ipucuText.gameObject.SetActive(false);
            }
        }
    }

    void EInputKontrol()
    {
        Keyboard klavye = Keyboard.current;
        if (klavye == null) return;

        // E tuşu yalnızca YERDEKİ deliller için çalışır
        if (klavye.eKey.wasPressedThisFrame && !incelemeModu && !alModu && yakinDelil != null && yakinDelil.delilKonumu == DelilNesnesi.DelilKonumu.Yerde)
        {
            incelemeModu = true;
            if (ipucuText != null) ipucuText.gameObject.SetActive(false);

            if (animator != null)
            {
                animator.SetBool("Comeldi", true);
            }

            // Toplama anında oyuncunun hareket etmesini engellemek için kilidi kapatıyoruz
            hareket oyuncuScripti = GetComponent<hareket>();
            if (oyuncuScripti != null) oyuncuScripti.hareketEdebilirMi = false;

            Invoke("IncelemeAnimasyonuBitti", 1.5f);
        }
    }

    void IncelemeAnimasyonuBitti()
    {
        incelemeModu = false;
        alModu = true;
        if (ipucuText != null)
        {
            ipucuText.gameObject.SetActive(true);
            ipucuText.text = "F - Al";
        }
    }

    void FInputKontrol()
    {
        Keyboard klavye = Keyboard.current;
        if (klavye == null) return;

        if (yakinDelil == null) return;

        // 1. MASADAKİ DELİL KONTROLÜ (Direkt ayakta F ile toplama)
        if (yakinDelil.delilKonumu == DelilNesnesi.DelilKonumu.Masada)
        {
            if (klavye.fKey.wasPressedThisFrame && !incelemeModu && !alModu)
            {
                if (ipucuText != null) ipucuText.gameObject.SetActive(false);
                
                // Ayakta aldığımız için hareket kilidini koyup hemen toplatıyoruz
                hareket oyuncuScripti = GetComponent<hareket>();
                if (oyuncuScripti != null) oyuncuScripti.hareketEdebilirMi = false;

                yakinDelil.Topla(false); // Karakter eğilmedi -> false
                yakinDelil = null;
            }
        }
        // 2. YERDEKİ DELİL KONTROLÜ (Eğildikten sonra F ile toplama)
        else if (yakinDelil.delilKonumu == DelilNesnesi.DelilKonumu.Yerde)
        {
            if (klavye.fKey.wasPressedThisFrame && alModu)
            {
                alModu = false;
                if (ipucuText != null) ipucuText.gameObject.SetActive(false);

                if (animator != null)
                {
                    animator.SetBool("Comeldi", false);
                }

                yakinDelil.Topla(true); // Karakter eğildi -> true
                yakinDelil = null;
            }
        }
    }
}