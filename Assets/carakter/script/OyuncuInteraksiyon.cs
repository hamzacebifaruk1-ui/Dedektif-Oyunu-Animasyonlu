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
        if (yakinDelil != null && !yakinDelil.ToplanabilirMi())
        {
            yakinDelil = null;
            ipucuText.gameObject.SetActive(false);
            return;
        }

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
            ipucuText.gameObject.SetActive(true);
            ipucuText.text = "E - İncele";
        }
        else
        {
            ipucuText.gameObject.SetActive(false);
        }
    }

    void EInputKontrol()
    {
        Keyboard klavye = Keyboard.current;
        if (klavye == null) return;

        if (klavye.eKey.wasPressedThisFrame && !incelemeModu && !alModu && yakinDelil != null)
        {
            incelemeModu = true;
            ipucuText.gameObject.SetActive(false);

            // Trigger yerine Bool — pozisyonda kalsın
            animator.SetBool("Comeldi", true);

            Invoke("IncelemeAnimasyonuBitti", 1.5f);
        }
    }

    void IncelemeAnimasyonuBitti()
    {
        incelemeModu = false;
        alModu = true;
        ipucuText.gameObject.SetActive(true);
        ipucuText.text = "F - Al";
    }

    void FInputKontrol()
    {
        Keyboard klavye = Keyboard.current;
        if (klavye == null) return;

        if (klavye.fKey.wasPressedThisFrame && alModu)
        {
            alModu = false;
            ipucuText.gameObject.SetActive(false);

            // Çömelme bitsin, ayağa kalk
            animator.SetBool("Comeldi", false);
         animator.SetBool("Comeldi", false);

            if (yakinDelil != null)
            {
                yakinDelil.Topla();
                yakinDelil = null;
            }
        }
    }
}