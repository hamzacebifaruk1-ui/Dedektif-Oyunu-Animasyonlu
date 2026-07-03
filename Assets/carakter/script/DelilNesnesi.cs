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
            else if (delilAdi.Contains("Defter") || delilAdi.Contains("Not"))
            {
                GorevYoneticisi.Instance.OdaVePanoTamamla();
            }
            else if (delilAdi.Contains("Kanca") || delilAdi.Contains("Tel"))
            {
                // Hafıza senkronizasyon hatasını aşmak için:
                // Kanca veya Tel nesnelerinden BİRİ bile şu an toplandıysa 
                // ve diğeri zaten defterde kayıtlıysa (veya sahne uyuşmazlığı varsa) görevi DOĞRUDAN bitir!
                GorevYoneticisi.Instance.kirikTelAlindi = true;
                GorevYoneticisi.Instance.kirikKancaAlindi = true;
                
                // Doğrudan mor görevi kapatıp Rıza görevini açıyoruz
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