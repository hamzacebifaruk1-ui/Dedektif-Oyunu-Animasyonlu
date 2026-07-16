using System.Collections.Generic;
using UnityEngine;

public class NpcDiyalog : MonoBehaviour
{
    public string npcAdi; // "Güvenlik Rıza", "Liman Müdürü Kemal", "İşçi Ahmet"
    public float etkilesimMesafesi = 3f;
    private Transform oyuncuTransform;

    [Header("Diyalog Aşamaları")]
    public List<DiyalogSatiri> ilkYalanlarDiyalogu = new List<DiyalogSatiri>();
    public List<DiyalogSatiri> yuzlesmeSuclamaDiyalogu = new List<DiyalogSatiri>();

    [Header("Yüzleşme Seçim Paneli")]
    public GameObject yuzlesmePaneli; // ⚡ Ahmet'in seçim panelini buraya sürükleyeceğiz!

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
                if (etkilesimUI != null) etkilesimUI.SetActive(false);

                // --- SENARYO GEREĞİ DİNAMİK YÜZLEŞME KONTROLLERİ ---
                bool yuzlesmeAktifMi = false;

                // GorevYoneticisi referans kontrolü
                if (GorevYoneticisi.Instance == null)
                {
                    Debug.LogError("[NpcDiyalog] GorevYoneticisi sahnede bulunamadı!");
                    return;
                }

                HashSet<string> toplananlar = GorevYoneticisi.Instance.GetToplananDeliller();

                if (npcAdi == "Güvenlik Rıza")
                {
                    bool usbBulundu = toplananlar != null && (toplananlar.Contains("USB Bellek") || toplananlar.Contains("Güvenlik Kamera Kaydı"));
                    yuzlesmeAktifMi = usbBulundu || GorevYoneticisi.Instance.mevcutAsama >= GorevYoneticisi.GorevAsamasi.KemalOfisArama;
                }
                else if (npcAdi == "Liman Müdürü Kemal")
                {
                    bool evrakBulundu = toplananlar != null && toplananlar.Contains("Şirket Evrakları");
                    yuzlesmeAktifMi = evrakBulundu || GorevYoneticisi.Instance.mevcutAsama >= GorevYoneticisi.GorevAsamasi.AhmetYuzlesmeSecim;
                }
                else if (npcAdi == "İşçi Ahmet")
                {
                    // ⚡ Ahmet ile Yüzleşme Seçim Aşaması Kontrolü
                    if (GorevYoneticisi.Instance.mevcutAsama == GorevYoneticisi.GorevAsamasi.AhmetYuzlesmeSecim)
                    {
                        // Sahne üzerindeki Seçim Panelini doğrudan açıyoruz!
                        if (yuzlesmePaneli != null)
                        {
                            yuzlesmePaneli.SetActive(true); 
                            Cursor.lockState = CursorLockMode.None;
                            Cursor.visible = true;
                        }
                        else
                        {
                            Debug.LogError("[HATA] NpcDiyalog scriptindeki 'Yuzlesme Paneli' alanı boş! Lütfen AhmetSecimPaneli'ni Inspector'dan sürükle.");
                        }
                        return; 
                    }
                    else if (GorevYoneticisi.Instance.mevcutAsama > GorevYoneticisi.GorevAsamasi.AhmetYuzlesmeSecim)
                    {
                        Debug.Log("[AHMET] Ahmet ile yüzleşme zaten tamamlandı. Eski diyalog engellendi.");
                        return;
                    }
                }

                // Normal diyalog akışı
                if (yuzlesmeAktifMi)
                {
                    DiyalogYoneticisi.Instance.DiyalogBaslat(yuzlesmeSuclamaDiyalogu, npcAdi);
                }
                else
                {
                    DiyalogYoneticisi.Instance.DiyalogBaslat(ilkYalanlarDiyalogu, npcAdi);
                }
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
        yuzlesmeSuclamaDiyalogu.Clear();

        if (npcAdi == "Güvenlik Rıza")
        {
            ilkYalanlarDiyalogu.Add(new DiyalogSatiri { konusmaciAdi = "Dedektif", textIcerik = "Rıza, kaza gecesi nöbet kulübesinde sorumlu personel sendin...", elevenLabsSesDosyaAdi = "Dedektif_Soru_Riza1" });
            ilkYalanlarDiyalogu.Add(new DiyalogSatiri { konusmaciAdi = "Güvenlik Rıza", textIcerik = "Amirim valla billa, ekmeğimin üzerine yemin ederim ki... Trafo patladı şalterler attı.", elevenLabsSesDosyaAdi = "Riza_Yalan_Trafo" });

            yuzlesmeSuclamaDiyalogu.Add(new DiyalogSatiri { konusmaciAdi = "Dedektif", textIcerik = "Elimde orijinal USB bellek var Rıza! Loglara göre elektrik falan kesilmemiş!", elevenLabsSesDosyaAdi = "Dedektif_Usb_Bulundu_IcSes" });
            yuzlesmeSuclamaDiyalogu.Add(new DiyalogSatiri { konusmaciAdi = "Güvenlik Rıza", textIcerik = "Amirim... Ne olur merhamet edin! Müdür Kemal Bey kaza akşamı yanıma geldi...", elevenLabsSesDosyaAdi = "Riza_Oyuncuyu_Asagilama" });
            yuzlesmeSuclamaDiyalogu.Add(new DiyalogSatiri { konusmaciAdi = "Dedektif İç Ses", textIcerik = "Rıza paçasını kurtarmak için suçu direkt Müdür Kemal'in üzerine yıkıyor...", elevenLabsSesDosyaAdi = "Dedektif_Kulube_Ipucu" });
        }
        else if (npcAdi == "Liman Müdürü Kemal")
        {
            // ⚡ HATA BURADAYDI: elevenLabsSecDosyaAdi -> elevenLabsSesDosyaAdi olarak düzeltildi!
            ilkYalanlarDiyalogu.Add(new DiyalogSatiri { konusmaciAdi = "Dedektif", textIcerik = "Kemal Bey, Dün gece saaat 02:00'da genç bir işçi can verdi.", elevenLabsSesDosyaAdi = "Dedektif_Soru_Kemal1" });
            ilkYalanlarDiyalogu.Add(new DiyalogSatiri { konusmaciAdi = "Liman Müdürü Kemal", textIcerik = "Büyük bir tajedi Dedektif bey? Şieketin adını lekelmeyein...", elevenLabsSesDosyaAdi = "Kemal_Yalan_Sirket" });

            yuzlesmeSuclamaDiyalogu.Add(new DiyalogSatiri { konusmaciAdi = "Dedektif", textIcerik = "Holdingin gücünün arkasına saklanmayı bırak! Yolsuzluk evrakları elimde!", elevenLabsSesDosyaAdi = "Dedektif_Evrak_Bulundu_IcSes" });
            yuzlesmeSuclamaDiyalogu.Add(new DiyalogSatiri { konusmaciAdi = "Liman Müdürü Kemal", textIcerik = "Tamam, evet! Malzemeden kıstım ama katil değilim! İşçi Ahmet'e bakın!", elevenLabsSesDosyaAdi = "Kemal_Itiraf_Final" });
            yuzlesmeSuclamaDiyalogu.Add(new DiyalogSatiri { konusmaciAdi = "Dedektif İç Ses", textIcerik = "Kemal mali yolsuzluklarını kabul etti ama cinayeti Ahmet'in üzerine atıyor...", elevenLabsSesDosyaAdi = "Dedektif_Ofis_Ipucu" });
        }
        else if (npcAdi == "İşçi Ahmet")
        {
            ilkYalanlarDiyalogu.Add(new DiyalogSatiri { konusmaciAdi = "Dedektif", textIcerik = "Ahmet, Murat vince tırmanmadan hemen önce aranızda ne yaşandı?", elevenLabsSesDosyaAdi = "Dedektif_Soru_Ahmet1" });
            ilkYalanlarDiyalogu.Add(new DiyalogSatiri { konusmaciAdi = "İşçi Ahmet", textIcerik = "Ben... Ben hiçbir şey görmedim amirim. Beni bu işlere bulaştırmayın...", elevenLabsSesDosyaAdi = "Ahmet_Korku_Beyan" });
        }
    }
}