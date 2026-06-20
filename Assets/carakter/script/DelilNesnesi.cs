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

        DelilYoneticisi.Instance.DelilBulundu(delilAdi);
        gameObject.SetActive(false);
    }
}