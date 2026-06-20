using UnityEngine;
using TMPro;
using System.Collections;

public class NpcDiyalog : MonoBehaviour
{
    [Header("NPC Bilgileri")]
    public string npcAdi = "NPC";

    [Header("Aşama 1 — Delil Yok (0-1 delil)")]
    [TextArea] public string[] asama1Konusmalar;
    public AudioClip[] asama1Sesleri;

    [Header("Aşama 2 — Bazı Deliller (2-3 delil)")]
    [TextArea] public string[] asama2Konusmalar;
    public AudioClip[] asama2Sesleri;

    [Header("Aşama 3 — Tüm Deliller (4-5 delil)")]
    [TextArea] public string[] asama3Konusmalar;
    public AudioClip[] asama3Sesleri;

    private AudioSource audioSource;
    private bool konusuyor = false;
    private bool oyuncuYakinda = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public string GetAd() { return npcAdi; }
    public bool KonusuyorMu() { return konusuyor; }
    public bool OyuncuYakindaMi() { return oyuncuYakinda; }
    public void OyuncuYaklasti() { oyuncuYakinda = true; }
    public void OyuncuUzaklasti()
    {
        oyuncuYakinda = false;
        konusuyor = false;
    }

    string[] MevcutKonusmalariGetir()
    {
        int delilSayisi = DelilYoneticisi.Instance.BulunanDelilSayisiniGetir();

        if (delilSayisi <= 1)
            return asama1Konusmalar;
        else if (delilSayisi <= 3)
            return asama2Konusmalar;
        else
            return asama3Konusmalar;
    }

    AudioClip[] MevcutSesleriGetir()
    {
        int delilSayisi = DelilYoneticisi.Instance.BulunanDelilSayisiniGetir();

        if (delilSayisi <= 1)
            return asama1Sesleri;
        else if (delilSayisi <= 3)
            return asama2Sesleri;
        else
            return asama3Sesleri;
    }

    public IEnumerator Konustur(System.Action<string> metinCallback, System.Action bitis)
    {
        konusuyor = true;
        string[] konusmalar = MevcutKonusmalariGetir();
        AudioClip[] sesler = MevcutSesleriGetir();

        for (int i = 0; i < konusmalar.Length; i++)
        {
            metinCallback?.Invoke(konusmalar[i]);

            if (sesler != null && i < sesler.Length && sesler[i] != null)
            {
                audioSource.clip = sesler[i];
                audioSource.Play();
                yield return new WaitWhile(() => audioSource.isPlaying);
            }
            else
            {
                yield return new WaitForSeconds(konusmalar[i].Length * 0.06f);
            }

            yield return new WaitForSeconds(0.5f);
        }

        konusuyor = false;
        bitis?.Invoke();
    }
}