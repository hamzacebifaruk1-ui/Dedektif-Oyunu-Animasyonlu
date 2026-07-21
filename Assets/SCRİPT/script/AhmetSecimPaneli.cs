using UnityEngine;
using System.Collections;

public class AhmetSecimPaneli : MonoBehaviour
{
    [Header("UI & Obje Referansları")]
    public GameObject secimPaneli;
    public GameObject dolaptakiMektupObjesi;
    public GameObject jeneratordekiBelgeObjesi;

    void Start()
    {
        // Oyun ilk açıldığında her iki gizli delili de zorla KAPA!
        if (dolaptakiMektupObjesi != null) dolaptakiMektupObjesi.SetActive(false);
        if (jeneratordekiBelgeObjesi != null) jeneratordekiBelgeObjesi.SetActive(false);
    }

    // 🔴 ÜST BUTON (Fotoğraf / Mektup)
    public void Secenek1_FotografSecildi()
    {
        Debug.Log("🟢 [BUTON 1] Yırtık Fotoğraf Seçildi.");

        if (dolaptakiMektupObjesi != null) dolaptakiMektupObjesi.SetActive(true);
        if (jeneratordekiBelgeObjesi != null) jeneratordekiBelgeObjesi.SetActive(false);

        if (GorevYoneticisi.Instance != null)
        {
            GorevYoneticisi.Instance.StartCoroutine(DiyalogVeSesOynat("Dedektif_Ahmet_Fotograf", "Ahmet_Dolap_Mektup"));
        }

        PaneliKapat();
    }

    // 🟡 ALT BUTON (Zimmet / Paralar)
    public void Secenek2_ZimmetSecildi()
    {
        Debug.Log("🟡 [BUTON 2] Zimmet Belgeleri Seçildi.");

        if (jeneratordekiBelgeObjesi != null) jeneratordekiBelgeObjesi.SetActive(true);
        if (dolaptakiMektupObjesi != null) dolaptakiMektupObjesi.SetActive(false);

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

        // 1. Dedektif Ses Kontrolü
        AudioClip dedektifClip = Resources.Load<AudioClip>("Audio/Dialogs/" + dedektifSesAdi);
        if (dedektifClip != null && sesKaynagi != null)
        {
            sesKaynagi.clip = dedektifClip;
            sesKaynagi.Play();
            yield return new WaitForSecondsRealtime(dedektifClip.length + 0.3f);
        }
        else
        {
            Debug.LogError($"❌ [SES EKSİK] Resources/Audio/Dialogs/{dedektifSesAdi} dosyası bulunamadı!");
            yield return new WaitForSecondsRealtime(1.5f);
        }

        // 2. Ahmet Ses Kontrolü
        AudioClip ahmetClip = Resources.Load<AudioClip>("Audio/Dialogs/" + ahmetSesAdi);
        if (ahmetClip != null && sesKaynagi != null)
        {
            sesKaynagi.clip = ahmetClip;
            sesKaynagi.Play();
            yield return new WaitForSecondsRealtime(ahmetClip.length + 0.3f);
        }
        else
        {
            Debug.LogError($"❌ [SES EKSİK] Resources/Audio/Dialogs/{ahmetSesAdi} dosyası bulunamadı!");
            yield return new WaitForSecondsRealtime(1.5f);
        }

        if (GorevYoneticisi.Instance != null)
        {
            GorevYoneticisi.Instance.AsamaAtla(GorevYoneticisi.GorevAsamasi.KalanDelilleriTopla);
            GorevYoneticisi.Instance.GorevYazisiGuncelle("<color=purple>GÖREV: Şantiyedeki gizli ipucunu ve kalan delilleri topla (0/4)</color>");
        }
    }
}