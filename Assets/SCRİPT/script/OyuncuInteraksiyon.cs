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
        // Hafıza kilitlenmesini önlemek için her aramadan önce en yakın delili sıfırlayıp baştan tarıyoruz
        Collider[] yakinlar = Physics.OverlapSphere(transform.position, etkilesimMesafesi);

        DelilNesnesi enYakinDelil = null;
        float enYakinMesafe = etkilesimMesafesi;

        foreach (Collider col in yakinlar)
        {
            if (col.transform == transform || col.transform.IsChildOf(transform)) continue;

            DelilNesnesi delil = col.GetComponent<DelilNesnesi>();
            if (delil == null)
                delil = col.GetComponentInParent<DelilNesnesi>();

            // Eğer sahdede bir delil nesnesi varsa ve şu anki görev durumuna göre TOPLANABİLİRSE
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

        // Güncel yakındaki delili ata
        yakinDelil = enYakinDelil;

        // UI Güncelleme Alanı
        if (yakinDelil != null)
        {
            if (ipucuText != null)
            {
                ipucuText.gameObject.SetActive(true);
                ipucuText.text = "E - İncele";
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

        if (klavye.eKey.wasPressedThisFrame && !incelemeModu && !alModu && yakinDelil != null)
        {
            incelemeModu = true;
            if (ipucuText != null) ipucuText.gameObject.SetActive(false);

            if (animator != null)
            {
                animator.SetBool("Comeldi", true);
            }

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

        if (klavye.fKey.wasPressedThisFrame && alModu)
        {
            alModu = false;
            if (ipucuText != null)
            {
                ipucuText.gameObject.SetActive(false);
            }

            if (animator != null)
            {
                animator.SetBool("Comeldi", false);
            }

            if (yakinDelil != null)
            {
                // Delil yerdeyse eğilme kontrolünü başarıyla geçmesi için true gönderiyoruz
                yakinDelil.Topla(true); 
                yakinDelil = null;
            }
        }
    }
}