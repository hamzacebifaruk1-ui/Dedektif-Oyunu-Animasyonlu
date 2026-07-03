using UnityEngine;

public class DelilNesnesi : MonoBehaviour
{
    public enum DelilKonumu { Yerde, Masada }

    [Header("Bu Delilin Adı")]
    public string delilAdi = "Delil";

    [Header("Ses")]
    public AudioClip toplamaSesi;

    [Header("Akıllı Delil Ayarları")]
    public DelilKonumu delilKonumu;

    private bool toplandiMi = false;

    public bool ToplanabilirMi()
    {
        if (GorevYoneticisi.Instance == null) return !toplandiMi;

        if (delilAdi.Contains("Ilac") || delilAdi.Contains("İlaç"))
        {
            if (!GorevYoneticisi.Instance.kemalleKonusuldu) return false;
        }

        if (delilAdi.Contains("Defter") || delilAdi.Contains("Not"))
        {
            if (!GorevYoneticisi.Instance.ahmetleKonusuldu) return false;
        }

        if (delilAdi.Contains("Kanca") || delilAdi.Contains("Tel"))
        {
            if (!GorevYoneticisi.Instance.kemalPanikledi) return false;
        }

        return !toplandiMi;
    }

    public void Topla(bool karakterEgildiMi)
    {
        if (!ToplanabilirMi()) return;

        if (delilKonumu == DelilKonumu.Yerde && !karakterEgildiMi) return; 

        toplandiMi = true;

        if (toplamaSesi != null)
            AudioSource.PlayClipAtPoint(toplamaSesi, transform.position);

        if (DelilYoneticisi.Instance != null)
            DelilYoneticisi.Instance.DelilBulundu(delilAdi);

        if (NotDefteriYoneticisi.Instance != null)
            NotDefteriYoneticisi.Instance.DelilEkle(delilAdi);

        if (GorevYoneticisi.Instance != null)
        {
            if (delilAdi.Contains("Ilac") || delilAdi.Contains("İlaç"))
            {
                GorevYoneticisi.Instance.IlacKutusuBulundu();
            }
            // DÜZELTME: Defter ve Not kısmı buradan kaldırıldı, yetki DelilYoneticisi'ne devredildi!
            else if (delilAdi.Contains("Kanca") || delilAdi.Contains("Tel"))
            {
                GorevYoneticisi.Instance.kirikTelAlindi = true;
                GorevYoneticisi.Instance.kirikKancaAlindi = true;
                GorevYoneticisi.Instance.TeknikDelillerTamamla();
                Debug.Log("[SİSTEM] Teknik deliller başarıyla eşitlendi ve yeni görev açıldı!");
            }
            else if (delilAdi.Contains("Kamera") || delilAdi.Contains("Kaydı") || delilAdi.Contains("USB"))
            {
                GorevYoneticisi.Instance.KameraKaydiBulunduTamamla();
            }
        }

        gameObject.SetActive(false);
    }
}