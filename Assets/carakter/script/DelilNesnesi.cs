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
        if (!ToplanabilirMi()) 
        {
            Debug.Log("Bu delili incelemek için henüz çok erken!");
            return;
        }

        if (delilKonumu == DelilKonumu.Yerde && !karakterEgildiMi)
        {
            Debug.Log("Bu delil yerde, eğilerek incelemeliyim!");
            return; 
        }

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
            else if (delilAdi.Contains("Defter") || delilAdi.Contains("Not"))
            {
                int sayac = 0;
                if (DelilYoneticisi.Instance != null) sayac = DelilYoneticisi.Instance.BulunanDelilSayisiniGetir();
                if (sayac >= 3) GorevYoneticisi.Instance.OdaVePanoTamamla();
            }
            else if (delilAdi.Contains("Kanca") || delilAdi.Contains("Tel"))
            {
                if (delilAdi.Contains("Tel")) GorevYoneticisi.Instance.kirikTelAlindi = true;
                if (delilAdi.Contains("Kanca")) GorevYoneticisi.Instance.kirikKancaAlindi = true;

                if (GorevYoneticisi.Instance.kirikTelAlindi && GorevYoneticisi.Instance.kirikKancaAlindi)
                {
                    GorevYoneticisi.Instance.TeknikDelillerTamamla();
                }
                else
                {
                    GorevYoneticisi.Instance.GoreviListele();
                }
            }
            else if (delilAdi.Contains("Kamera") || delilAdi.Contains("Kaydı") || delilAdi.Contains("USB"))
            {
                GorevYoneticisi.Instance.KameraKaydiBulunduTamamla();
            }
        }

        gameObject.SetActive(false);
    }
}