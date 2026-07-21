using UnityEngine;
using TMPro;

public class GizliDelilObje : MonoBehaviour
{
    [Header("Delil Ayarları")]
    public string delilAdi = "Murat'ın Gizli Mektubu"; // Inspector'dan doğru ismi kontrol et!
    public float etkilesimMesafesi = 4f; 
    
    [Header("UI Referansı")]
    public TextMeshProUGUI etkilesimText;

    private Transform oyuncuTransform;

    void OnEnable()
    {
        ReferanslariBul();
    }

    void Start()
    {
        ReferanslariBul();
    }

    private void ReferanslariBul()
    {
        // 1. Oyuncuyu Bul (Tag, Script veya Kamera üzerinden)
        if (oyuncuTransform == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) 
            {
                oyuncuTransform = playerObj.transform;
            }
            else
            {
                hareket h = FindFirstObjectByType<hareket>();
                if (h != null) oyuncuTransform = h.transform;
                else if (Camera.main != null) oyuncuTransform = Camera.main.transform;
            }
        }

        // 2. Etkileşim Yazısını Bul (PASİF/KAPALI OLSA BİLE BULUR)
        if (etkilesimText == null)
        {
            TextMeshProUGUI[] tumYazilar = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
            foreach (var txt in tumYazilar)
            {
                if (txt.gameObject.name == "EtkilesimYazisi")
                {
                    etkilesimText = txt;
                    break;
                }
            }
        }
    }

    void Update()
    {
        if (oyuncuTransform == null)
        {
            ReferanslariBul();
            return;
        }

        float mesafe = Vector3.Distance(transform.position, oyuncuTransform.position);
        var klavye = UnityEngine.InputSystem.Keyboard.current;

        // Oyuncu nesneye yeterince yakınsa:
        if (mesafe <= etkilesimMesafesi)
        {
            if (etkilesimText != null)
            {
                if (!etkilesimText.gameObject.activeSelf) 
                    etkilesimText.gameObject.SetActive(true);

                etkilesimText.text = $"[E] {delilAdi} İncele / Al";
            }

            // E Tuşuna basılırsa
            if (klavye != null && klavye.eKey.wasPressedThisFrame)
            {
                DeliliTopla();
            }
        }
        else
        {
            // Uzaklaşınca yazıyı gizle
            if (etkilesimText != null && etkilesimText.gameObject.activeSelf && etkilesimText.text.Contains(delilAdi))
            {
                etkilesimText.gameObject.SetActive(false);
            }
        }
    }

    private void DeliliTopla()
    {
        if (etkilesimText != null) 
            etkilesimText.gameObject.SetActive(false);

        if (NotDefteriYoneticisi.Instance != null)
        {
            NotDefteriYoneticisi.Instance.DelilEkle(delilAdi);
        }

        if (GorevYoneticisi.Instance != null)
        {
            GorevYoneticisi.Instance.DelilToplandi(delilAdi);
        }

        Debug.Log("[GİZLİ DELİL] Başarıyla Toplandı: " + delilAdi);
        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, etkilesimMesafesi);
    }
}