using UnityEngine;
using UnityEngine.InputSystem;

public class ElFeneriKontrol : MonoBehaviour
{
    [Header("Fener")]
    public GameObject fenerIsigi;
    public GameObject fenerModeli;

    private bool fenerAcik = false;

    void Start()
    {
        // Başta fener kapalı
        if (fenerIsigi != null) fenerIsigi.SetActive(false);
    }

    void Update()
    {
        Keyboard klavye = Keyboard.current;
        if (klavye == null) return;

        if (klavye.lKey.wasPressedThisFrame)
        {
            fenerAcik = !fenerAcik;
            if (fenerIsigi != null)
                fenerIsigi.SetActive(fenerAcik);
        }
    }
}