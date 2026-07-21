using UnityEngine;
using System.Collections;

public class AhmetYuzlesmePaneli : MonoBehaviour
{
    [Header("UI & Obje Referansları")]
    public GameObject yuzlesmePaneli;
    public GameObject dolaptakiMektupObjesi;
    public GameObject jeneratordekiBelgeObjesi;

    // Inspector'da Üst Buton (Fotoğraf) OnClick olayına bu fonksiyonu bağla:
    public void FotografSecildi()
    {
        KararVer(1);
    }

    // Inspector'da Alt Buton (Zimmet) OnClick olayına bu fonksiyonu bağla:
    public void ZimmetSecildi()
    {
        KararVer(2);
    }

    public void KararVer(int secimNo)
    {
        if (yuzlesmePaneli != null) 
            yuzlesmePaneli.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (secimNo == 1) // 🔴 Fotoğraf / Aşk Yolu
        {
            Debug.Log("[HİKAYE] Yırtık Fotoğraf Rotaları Seçildi.");
            if (dolaptakiMektupObjesi != null) dolaptakiMektupObjesi.SetActive(true);
            if (jeneratordekiBelgeObjesi != null) jeneratordekiBelgeObjesi.SetActive(false);

            if (GorevYoneticisi.Instance != null)
            {
                GorevYoneticisi.Instance.StartCoroutine(SesleriOynat("Dedektif_Ahmet_Fotograf", "Ahmet_Dolap_Mektup"));
            }
        }
        else if (secimNo == 2) // 🟡 Zimmet / Paralar Yolu
        {
            Debug.Log("[HİKAYE] Zimmet Belgeleri Rotaları Seçildi.");
            if (jeneratordekiBelgeObjesi != null) jeneratordekiBelgeObjesi.SetActive(true);
            if (dolaptakiMektupObjesi != null) dolaptakiMektupObjesi.SetActive(false);

            if (GorevYoneticisi.Instance != null)
            {
                GorevYoneticisi.Instance.StartCoroutine(SesleriOynat("Dedektif_Ahmet_Zimmet", "Ahmet_Jenerator_Belge"));
            }
        }
    }

    private IEnumerator SesleriOynat(string dedektifSes, string ahmetSes)
    {
        AudioSource sesKaynagi = (GorevYoneticisi.Instance != null) ? GorevYoneticisi.Instance.icSesKaynagi : null;

        // 1. Dedektif Ses Dosyası
        AudioClip clip1 = Resources.Load<AudioClip>("Audio/Dialogs/" + dedektifSes);
        if (clip1 != null && sesKaynagi != null)
        {
            sesKaynagi.clip = clip1;
            sesKaynagi.Play();
            yield return new WaitForSecondsRealtime(clip1.length + 0.3f);
        }
        else
        {
            Debug.LogWarning($"[SES BULUNAMADI] Resources/Audio/Dialogs/{dedektifSes} yolu kontrol edilmeli!");
            yield return new WaitForSecondsRealtime(1.5f);
        }

        // 2. Ahmet Ses Dosyası
        AudioClip clip2 = Resources.Load<AudioClip>("Audio/Dialogs/" + ahmetSes);
        if (clip2 != null && sesKaynagi != null)
        {
            sesKaynagi.clip = clip2;
            sesKaynagi.Play();
            yield return new WaitForSecondsRealtime(clip2.length + 0.3f);
        }
        else
        {
            Debug.LogWarning($"[SES BULUNAMADI] Resources/Audio/Dialogs/{ahmetSes} yolu kontrol edilmeli!");
            yield return new WaitForSecondsRealtime(1.5f);
        }

        // 3. Görev Aşaması ve Metin Güncellemesi
        if (GorevYoneticisi.Instance != null)
        {
            GorevYoneticisi.Instance.AsamaAtla(GorevYoneticisi.GorevAsamasi.KalanDelilleriTopla);
            GorevYoneticisi.Instance.GorevYazisiGuncelle("<color=purple>GÖREV: Şantiyedeki gizli ipucunu ve kalan delilleri topla (0/4)</color>");
        }
    }
}