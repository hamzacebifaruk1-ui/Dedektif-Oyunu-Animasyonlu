using System.Collections.Generic;
using UnityEngine;

public class NpcDiyalog : MonoBehaviour
{
    public string npcAdi; // "Güvenlik Rıza", "Liman Müdürü Kemal", "İşçi Ahmet"
    public float etkilesimMesafesi = 3f;
    private Transform oyuncuTransform;

    [Header("Diyalog Aşamaları")]
    public List<DiyalogSatiri> ilkYalanlarDiyalogu = new List<DiyalogSatiri>();

    [Header("Yüzleşme Seçim Paneli")]
    public GameObject yuzlesmePaneli; // Ahmet'in seçim paneli

    private GameObject etkilesimUI;
    private bool yaziGosteriliyorMu = false;

    void Start()
    {
        hareket oyuncuScript = FindFirstObjectByType<hareket>();
        if (oyuncuScript != null) oyuncuTransform = oyuncuScript.transform;
        
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            Transform textTransform = canvas.transform.Find("EtkilesimYazisi");
            if (textTransform != null) etkilesimUI = textTransform.gameObject;
        }

        DiyaloglariDoldur();
    }

    void Update()
    {
        if (oyuncuTransform == null) return;

        float mesafe = Vector3.Distance(transform.position, oyuncuTransform.position);
        
        var klavye = UnityEngine.InputSystem.Keyboard.current;
        bool eBasildi = klavye != null && klavye.eKey.wasPressedThisFrame;

        if (mesafe <= etkilesimMesafesi)
        {
            if (etkilesimUI != null && !etkilesimUI.activeSelf && !DiyalogYoneticisi.Instance.diyalogPaneli.activeSelf)
            {
                etkilesimUI.SetActive(true);
                yaziGosteriliyorMu = true;
            }

            if (eBasildi && !DiyalogYoneticisi.Instance.diyalogPaneli.activeSelf)
            {
                if (GorevYoneticisi.Instance == null)
                {
                    Debug.LogError("[NpcDiyalog] GorevYoneticisi sahnede bulunamadı!");
                    return;
                }

                // 🎯 1. FİNAL AŞAMASI (Oyun Sonu Suçlama Kontrolü)
                if (GorevYoneticisi.Instance.mevcutAsama == GorevYoneticisi.GorevAsamasi.FinalSuclama)
                {
                    if (etkilesimUI != null) etkilesimUI.SetActive(false);

                    if (npcAdi.Contains("Rıza") || npcAdi.Contains("Riza"))
                    {
                        FinalSuclamaSistemi.Instance.SuclamaYap("Riza");
                    }
                    else if (npcAdi.Contains("Ahmet"))
                    {
                        FinalSuclamaSistemi.Instance.SuclamaYap("Ahmet");
                    }
                    else if (npcAdi.Contains("Kemal") || npcAdi.Contains("Kel"))
                    {
                        FinalSuclamaSistemi.Instance.SuclamaYap("Kel");
                    }

                    return; 
                }

                // 🎯 2. NORMAL ARAŞTIRMA AŞAMASI (Her zaman ilk baştaki diyaloğu tekrar eder)
                if (etkilesimUI != null) etkilesimUI.SetActive(false);

                // Eğer Ahmet için özel seçim paneli aşamasındaysak
                if (npcAdi == "İşçi Ahmet" && GorevYoneticisi.Instance.mevcutAsama == GorevYoneticisi.GorevAsamasi.AhmetYuzlesmeSecim)
                {
                    if (yuzlesmePaneli != null)
                    {
                        yuzlesmePaneli.SetActive(true); 
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                    }
                    return;
                }

                // Rıza veya diğer NPC'ler her konuşmada en baştaki ilk sesini tekrar eder
                DiyalogYoneticisi.Instance.DiyalogBaslat(ilkYalanlarDiyalogu, npcAdi);
            }
        }
        else
        {
            if (yaziGosteriliyorMu && etkilesimUI != null)
            {
                etkilesimUI.SetActive(false);
                yaziGosteriliyorMu = false;
            }
        }
    }

    private void DiyaloglariDoldur()
    {
        ilkYalanlarDiyalogu.Clear();

        if (npcAdi == "Güvenlik Rıza")
        {
            ilkYalanlarDiyalogu.Add(new DiyalogSatiri { konusmaciAdi = "Dedektif", textIcerik = "Rıza, kaza gecesi nöbet kulübesinde sorumlu personel sendin...", elevenLabsSesDosyaAdi = "Dedektif_Soru_Riza1" });
            ilkYalanlarDiyalogu.Add(new DiyalogSatiri { konusmaciAdi = "Güvenlik Rıza", textIcerik = "Amirim valla billa, ekmeğimin üzerine yemin ederim ki... Trafo patladı şalterler attı.", elevenLabsSesDosyaAdi = "Riza_Yalan_Trafo" });
        }
        else if (npcAdi == "Liman Müdürü Kemal")
        {
            ilkYalanlarDiyalogu.Add(new DiyalogSatiri { konusmaciAdi = "Dedektif", textIcerik = "Kemal Bey, Dün gece saaat 02:00'da genç bir işçi can verdi.", elevenLabsSesDosyaAdi = "Dedektif_Soru_Kemal1" });
            ilkYalanlarDiyalogu.Add(new DiyalogSatiri { konusmaciAdi = "Liman Müdürü Kemal", textIcerik = "Büyük bir trajedi Dedektif bey... Şirketin adını lekelemeyin...", elevenLabsSesDosyaAdi = "Kemal_Yalan_Sirket" });
        }
        else if (npcAdi == "İşçi Ahmet")
        {
            ilkYalanlarDiyalogu.Add(new DiyalogSatiri { konusmaciAdi = "Dedektif", textIcerik = "Ahmet, Murat vince tırmanmadan hemen önce aranızda ne yaşandı?", elevenLabsSesDosyaAdi = "Dedektif_Soru_Ahmet1" });
            ilkYalanlarDiyalogu.Add(new DiyalogSatiri { konusmaciAdi = "İşçi Ahmet", textIcerik = "Ben... Ben hiçbir şey görmedim amirim. Beni bu işlere bulaştırmayın...", elevenLabsSesDosyaAdi = "Ahmet_Korku_Beyan" });
        }
    }
}