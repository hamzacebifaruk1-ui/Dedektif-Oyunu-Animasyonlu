using UnityEngine;

public class NPCGorevSistemi : MonoBehaviour
{
    public enum NPCTipi { Riza, Kemal, Ahmet }
    
    [Header("NPC Tanımı")]
    public NPCTipi npcTipi;

    [Header("Ses Dosyaları (Resources/Audio/Sounds/)")]
    public string ilkSorguSesAdi;
    public string finalAlayEtmeSesAdi; // Yanlış suçlandığında çalacak ses dosyası adı

    private AudioSource audioSource;
    private Transform oyuncuTransform;
    public float konusmaMesafesi = 3.0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        GameObject oyuncu = GameObject.FindGameObjectWithTag("Player");
        if (oyuncu != null) oyuncuTransform = oyuncu.transform;
    }

    void Update()
    {
        if (oyuncuTransform == null || GorevYoneticisi.Instance == null) return;

        float mesafe = Vector3.Distance(transform.position, oyuncuTransform.position);

        if (mesafe <= konusmaMesafesi && Input.GetKeyDown(KeyCode.E))
        {
            EtkileşimeGir();
        }
    }

    private void EtkileşimeGir()
    {
        GorevYoneticisi.GorevAsamasi asama = GorevYoneticisi.Instance.mevcutAsama;

        // ==============================================================
        // SIRA TABANLI İLK SORGULAMA KONTROLLERİ (Aşama 1, 2, 3)
        // ==============================================================
        if (asama == GorevYoneticisi.GorevAsamasi.RizaSorgu && npcTipi == NPCTipi.Riza)
        {
            SesOynat(ilkSorguSesAdi);
            GorevYoneticisi.Instance.AsamaAtla(GorevYoneticisi.GorevAsamasi.KemalSorgu);
        }
        else if (asama == GorevYoneticisi.GorevAsamasi.KemalSorgu && npcTipi == NPCTipi.Kemal)
        {
            SesOynat(ilkSorguSesAdi);
            GorevYoneticisi.Instance.AsamaAtla(GorevYoneticisi.GorevAsamasi.AhmetSorgu);
        }
        else if (asama == GorevYoneticisi.GorevAsamasi.AhmetSorgu && npcTipi == NPCTipi.Ahmet)
        {
            SesOynat(ilkSorguSesAdi);
            GorevYoneticisi.Instance.AsamaAtla(GorevYoneticisi.GorevAsamasi.RizaOfisArama);
        }

        // ==============================================================
        // SAHNE 5: AHMET YÜZLEŞMESİ VE SEÇİM PANELİ TETİKLEME (Aşama 6)
        // ==============================================================
        else if (asama == GorevYoneticisi.GorevAsamasi.AhmetYuzlesmeSecim && npcTipi == NPCTipi.Ahmet)
        {
            if (SecimYoneticisi.Instance != null)
            {
                SecimYoneticisi.Instance.SecimEkraniniAc();
                GorevYoneticisi.Instance.AsamaAtla(GorevYoneticisi.GorevAsamasi.KalanDelilleriTopla);
            }
        }

        // ==============================================================
        // FİNAL: GERÇEK KATİLİ SUÇLAMA / YANLIŞ KİŞİDE ALAY ETME (Aşama 9)
        // ==============================================================
        else if (asama == GorevYoneticisi.GorevAsamasi.FinalSuclama)
        {
            // Bizim kurgumuzda her halükarda gerçek suçlu ve azmettirici KEMAL MÜDÜR'dür!
            if (npcTipi == NPCTipi.Kemal)
            {
                // Doğru suçlama yapıldı
                GorevYoneticisi.Instance.OyunuKazan();
            }
            else
            {
                // Yanlış suçlama yapıldı (Rıza veya Ahmet seçildi)
                StartCoroutine(YanlisSuclamaOynat());
            }
        }
    }

    private void SesOynat(string dosyaAdi)
    {
        if (string.IsNullOrEmpty(dosyaAdi)) return;
        AudioClip clip = Resources.Load<AudioClip>("Audio/Sounds/" + dosyaAdi);
        if (clip != null && audioSource != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    private System.Collections.IEnumerator YanlisSuclamaOynat()
    {
        // Alay etme sesini çal
        AudioClip clip = Resources.Load<AudioClip>("Audio/Sounds/" + finalAlayEtmeSesAdi);
        if (clip != null && audioSource != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
            yield return new WaitForSeconds(clip.length + 0.5f);
        }
        else
        {
            yield return new WaitForSeconds(2f);
        }

        // Kaybetme panelini aç
        GorevYoneticisi.Instance.OyunuKaybet();
    }
}