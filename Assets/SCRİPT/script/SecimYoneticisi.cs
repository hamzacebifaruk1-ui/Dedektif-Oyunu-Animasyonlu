using UnityEngine;
using UnityEngine.UI;

public class SecimYoneticisi : MonoBehaviour
{
    public static SecimYoneticisi Instance;

    [Header("UI Panelleri")]
    public GameObject secimPanel; 
    public Button sirketYolsuzluguButon; 
    public Button kisiselHusumetButon; 

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (secimPanel != null) secimPanel.SetActive(false);

        if (sirketYolsuzluguButon != null)
            sirketYolsuzluguButon.onClick.AddListener(YolsuzlukRotasiSecildi);

        if (kisiselHusumetButon != null)
            kisiselHusumetButon.onClick.AddListener(HusumetRotasiSecildi);
    }

    public void SecimEkraniniAc()
    {
        if (secimPanel != null)
        {
            secimPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void YolsuzlukRotasiSecildi()
    {
        if (DelilYoneticisi.Instance != null)
        {
            DelilYoneticisi.Instance.RotaBelirle(DelilYoneticisi.OyunRotasi.SirketYolsuzlugu);
        }
        SecimiKapat();
    }

    private void HusumetRotasiSecildi()
    {
        if (DelilYoneticisi.Instance != null)
        {
            DelilYoneticisi.Instance.RotaBelirle(DelilYoneticisi.OyunRotasi.KisiselHusumet);
        }
        SecimiKapat();
    }

    private void SecimiKapat()
    {
        if (secimPanel != null) secimPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // DÜZELTME: Eski metot yerine yeni aşama geçiş sistemini tetikliyoruz
        if (GorevYoneticisi.Instance != null)
        {
            GorevYoneticisi.Instance.AsamaAtla(GorevYoneticisi.GorevAsamasi.KalanDelilleriTopla);
        }
    }
}