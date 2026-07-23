using UnityEngine;
using System.Collections;

public class AhmetSecimPaneli : MonoBehaviour
{
    [Header("UI & Obje Referansları")]
    public GameObject secimPaneli;
    public GameObject dolaptakiMektupObjesi;
    public GameObject jeneratordekiBelgeObjesi;

    private void Awake()
    {
        // Oyun başında iki gizli delili de deaktif yap
        GizliDelilleriGizle();
    }

    void Start()
    {
        GizliDelilleriGizle();
    }

    private void GizliDelilleriGizle()
    {
        if (dolaptakiMektupObjesi != null) dolaptakiMektupObjesi.SetActive(false);
        if (jeneratordekiBelgeObjesi != null) jeneratordekiBelgeObjesi.SetActive(false);
    }

    // 🔴 ÜST BUTON (Fotoğraf / Mektup Seçildi)
    public void Secenek1_FotografSecildi()
    {
        Debug.Log("🟢 [SEÇİM 1] Mektup Seçildi. Zimmet Belgesi tamamen yok ediliyor!");

        // 1. Sadece Mektup aktif olur
        if (dolaptakiMektupObjesi != null) dolaptakiMektupObjesi.SetActive(true);

        // 2. Zimmet Belgesi sahneden TAMAMEN SİLİNİR (Toplanamaz)
        if (jeneratordekiBelgeObjesi != null) 
        {
            Destroy(jeneratordekiBelgeObjesi);
        }

        // 3. Rota Belirle
        if (DelilYoneticisi.Instance != null)
        {
            DelilYoneticisi.Instance.RotaBelirle(DelilYoneticisi.OyunRotasi.KisiselHusumet);
        }

        // 4. Ses ve Diyalog
        if (GorevYoneticisi.Instance != null)
        {
            GorevYoneticisi.Instance.StartCoroutine(DiyalogVeSesOynat("Dedektif_Ahmet_Fotograf", "Ahmet_Dolap_Mektup"));
        }

        PaneliKapat();
    }

    // 🟡 ALT BUTON (Zimmet / Paralar Seçildi)
    public void Secenek2_ZimmetSecildi()
    {
        Debug.Log("🟡 [SEÇİM 2] Zimmet Belgesi Seçildi. Mektup tamamen yok ediliyor!");

        // 1. Sadece Zimmet Belgesi aktif olur
        if (jeneratordekiBelgeObjesi != null) jeneratordekiBelgeObjesi.SetActive(true);

        // 2. Mektup Objesi sahneden TAMAMEN SİLİNİR (Toplanamaz)
        if (dolaptakiMektupObjesi != null) 
        {
            Destroy(dolaptakiMektupObjesi);
        }

        // 3. Rota Belirle
        if (DelilYoneticisi.Instance != null)
        {
            DelilYoneticisi.Instance.RotaBelirle(DelilYoneticisi.OyunRotasi.SirketYolsuzlugu);
        }

        // 4. Ses ve Diyalog
        if (GorevYoneticisi.Instance != null)
        {
            GorevYoneticisi.Instance.StartCoroutine(DiyalogVeSesOynat("Dedektif_Ahmet_Zimmet", "Ahmet_Jenerator_Belge"));
        }

        PaneliKapat();
    }

    private void PaneliKapat()
    {
        if (secimPaneli != null) secimPaneli.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private IEnumerator DiyalogVeSesOynat(string dedektifSesAdi, string ahmetSesAdi)
    {
        AudioSource sesKaynagi = (GorevYoneticisi.Instance != null) ? GorevYoneticisi.Instance.icSesKaynagi : null;

        AudioClip dedektifClip = Resources.Load<AudioClip>("Audio/Dialogs/" + dedektifSesAdi);
        if (dedektifClip != null && sesKaynagi != null)
        {
            sesKaynagi.clip = dedektifClip;
            sesKaynagi.Play();
            yield return new WaitForSecondsRealtime(dedektifClip.length + 0.3f);
        }
        else
        {
            yield return new WaitForSecondsRealtime(1.5f);
        }

        AudioClip ahmetClip = Resources.Load<AudioClip>("Audio/Dialogs/" + ahmetSesAdi);
        if (ahmetClip != null && sesKaynagi != null)
        {
            sesKaynagi.clip = ahmetClip;
            sesKaynagi.Play();
            yield return new WaitForSecondsRealtime(ahmetClip.length + 0.3f);
        }
        else
        {
            yield return new WaitForSecondsRealtime(1.5f);
        }

        if (GorevYoneticisi.Instance != null)
        {
            GorevYoneticisi.Instance.AsamaAtla(GorevYoneticisi.GorevAsamasi.KalanDelilleriTopla);
            GorevYoneticisi.Instance.GorevYazisiGuncelle("<color=purple>GÖREV: Şantiyedeki gizli ipucunu ve kalan delilleri topla (0/4)</color>");
        }
    }
}