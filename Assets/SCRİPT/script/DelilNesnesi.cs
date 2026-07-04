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

        // Kanca arındırıldı, sadece Tel kontrolü aktif
        if (delilAdi.Contains("Tel"))
        {
            if (!GorevYoneticisi.Instance.kemalPanikledi) return false;
        }

        if (delilAdi.Contains("Kamera") || delilAdi.Contains("Kaydı") || delilAdi.Contains("USB"))
        {
            if (!GorevYoneticisi.Instance.rizaItirafEtti) return false;
        }

        return !toplandiMi;
    }

    public void Topla(bool karakterEgildiMi)
    {
        if (!ToplanabilirMi()) return;

        // Eğer delil yerdeyse ve karakter eğilmediyse engelle. Masadaysa geçişe izin ver.
        if (delilKonumu == DelilKonumu.Yerde && !karakterEgildiMi) return; 

        toplandiMi = true;

        // İşlem bittiğinde karakterin donup kalmaması için yürüme kilidini açıyoruz
        hareket oyuncuScripti = FindFirstObjectByType<hareket>();
        if (oyuncuScripti != null)
        {
            oyuncuScripti.hareketEdebilirMi = true;
        }

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
            else if (delilAdi.Contains("Tel"))
            {
                GorevYoneticisi.Instance.kirikTelAlindi = true;
                GorevYoneticisi.Instance.TeknikDelillerTamamla();
                Debug.Log("[SİSTEM] Teknik delil (Vinç Teli) başarıyla alındı!");
            }
            else if (delilAdi.Contains("Kamera") || delilAdi.Contains("Kaydı") || delilAdi.Contains("USB"))
            {
                GorevYoneticisi.Instance.KameraKaydiBulunduTamamla();
            }
        }

        gameObject.SetActive(false);
    }
}