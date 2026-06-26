using UnityEngine;

public class DelilNesnesi : MonoBehaviour
{
    [Header("Bu Delilin Adı")]
    public string delilAdi = "Delil";

    [Header("Ses")]
    public AudioClip toplamaSesi;

    private bool toplandiMi = false;

    public bool ToplanabilirMi()
    {
        return !toplandiMi;
    }

    public void Topla()
    {
        if (toplandiMi) return;
        toplandiMi = true;

        if (toplamaSesi != null)
            AudioSource.PlayClipAtPoint(toplamaSesi, transform.position);

        // Genel delil yöneticisine bildiriliyor
        DelilYoneticisi.Instance.DelilBulundu(delilAdi);

        // ADIM 7: Not defteri yöneticisine delil ekleniyor
        if (NotDefteriYoneticisi.Instance != null)
        {
            NotDefteriYoneticisi.Instance.DelilEkle(delilAdi);
        }

        gameObject.SetActive(false);
    }
}