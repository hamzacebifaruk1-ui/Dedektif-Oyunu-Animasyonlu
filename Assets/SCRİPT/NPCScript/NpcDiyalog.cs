using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NpcDiyalog : MonoBehaviour
{
    [Header("NPC Bilgileri")]
    public string npcAdi = "NPC";

    [Header("Aşama 1 — Hikaye Başlangıcı")]
    [TextArea(3, 5)] public List<string> asama1Konusmalar = new List<string>();
    public List<AudioClip> asama1Sesleri = new List<AudioClip>();

    [Header("Aşama 2 — İlaç Kutusu / Ahmet Sorgusu")]
    [TextArea(3, 5)] public List<string> asama2Konusmalar = new List<string>();
    public List<AudioClip> asama2Sesleri = new List<AudioClip>();

    [Header("Aşama 3 — Defter ve Pano / Kemal Köşeye Sıkışma")]
    [TextArea(3, 5)] public List<string> asama3Konusmalar = new List<string>();
    public List<AudioClip> asama3Sesleri = new List<AudioClip>();

    [Header("Aşama 4 — Sabotaj Delilleri / Son Tehdit")]
    [TextArea(3, 5)] public List<string> asama4Konusmalar = new List<string>();
    public List<AudioClip> asama4Sesleri = new List<AudioClip>();

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
    }

    public string GetAd() { return npcAdi; }
    public void OyuncuYaklasti() { }
    public void OyuncuUzaklasti() { }

    public IEnumerator Konustur(System.Action<string> metinGosterici, System.Action diyalogBitti)
    {
        List<string> secilenKonusmalar = asama1Konusmalar;
        List<AudioClip> secilenSesler = asama1Sesleri;

        if (GorevYoneticisi.Instance != null)
        {
            // === AHMET DİYALOG AKIŞI ===
            if (npcAdi.Contains("Ahmet"))
            {
                if (GorevYoneticisi.Instance.odaVePanoIncelendi)
                {
                    secilenKonusmalar = asama3Konusmalar;
                    secilenSesler = asama3Sesleri;
                }
                else if (GorevYoneticisi.Instance.ilacKutusuAlindi)
                {
                    secilenKonusmalar = asama2Konusmalar;
                    secilenSesler = asama2Sesleri;
                }
            }
            // === KEMAL DİYALOG AKIŞI ===
            else if (npcAdi.Contains("Kemal"))
            {
                if (GorevYoneticisi.Instance.teknikDelillerAlindi)
                {
                    secilenKonusmalar = asama4Konusmalar;
                    secilenSesler = asama4Sesleri;
                }
                else if (GorevYoneticisi.Instance.odaVePanoIncelendi)
                {
                    secilenKonusmalar = asama3Konusmalar;
                    secilenSesler = asama3Sesleri;
                }
            }
            // === RIZA DİYALOG AKIŞI ===
            else if (npcAdi.Contains("Rıza") || npcAdi.Contains("Riza"))
            {
                if (GorevYoneticisi.Instance.teknikDelillerAlindi)
                {
                    secilenKonusmalar = asama2Konusmalar;
                    secilenSesler = asama2Sesleri;
                }
            }
        }

        for (int i = 0; i < secilenKonusmalar.Count; i++)
        {
            metinGosterici?.Invoke(secilenKonusmalar[i]);

            if (secilenSesler != null && i < secilenSesler.Count && secilenSesler[i] != null)
            {
                audioSource.clip = secilenSesler[i];
                audioSource.Play();
                yield return new WaitForSeconds(secilenSesler[i].length);
            }
            else
            {
                yield return new WaitForSeconds(4f);
            }
        }

        diyalogBitti?.Invoke();
    }
}