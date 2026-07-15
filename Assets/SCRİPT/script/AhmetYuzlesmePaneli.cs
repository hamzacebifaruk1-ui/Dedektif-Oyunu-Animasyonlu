using UnityEngine;
using UnityEngine.UI;

public class AhmetYuzlesmePaneli : MonoBehaviour
{
    [Header("Seçim Butonları")]
    public Button inanButonu;
    public Button tehditEtButonu;

    void Start()
    {
        // Butonlara tıklama olaylarını bağlıyoruz
        if (inanButonu != null)
            inanButonu.onClick.AddListener(() => KararVer(1));

        if (tehditEtButonu != null)
            tehditEtButonu.onClick.AddListener(() => KararVer(2));
    }

    public void KararVer(int secimYolu)
    {
        if (secimYolu == 1)
        {
            Debug.Log("[HİKAYE] Dedektif Ahmet'e inanmayı seçti.");
            // İleride buraya inanma yoluyla ilgili özel diyalog veya ses ekleyebilirsin
        }
        else
        {
            Debug.Log("[HİKAYE] Dedektif Ahmet'i tehdit ederek konuşturmayı seçti.");
        }

        // 1. Paneli kapat
        gameObject.SetActive(false);

        // 2. Fareyi tekrar oyuna kilitle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 3. Görev yöneticisini "Kalan Delilleri Toplama" aşamasına geçir!
        if (GorevYoneticisi.Instance != null)
        {
            GorevYoneticisi.Instance.AsamaAtla(GorevYoneticisi.GorevAsamasi.KalanDelilleriTopla);
        }
    }
}