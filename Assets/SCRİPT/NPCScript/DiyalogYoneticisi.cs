using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class DiyalogYoneticisi : MonoBehaviour
{
    public static DiyalogYoneticisi Instance;

    [Header("UI Elemanları")]
    public GameObject diyalogPanel;
    public TextMeshProUGUI npcAdiText;
    public TextMeshProUGUI konusmaText;
    public GameObject konusIpucu;

    [Header("Ayarlar")]
    public float etkilesimMesafesi = 2.5f;

    private NpcDiyalog yakinNpc = null;
    private bool diyalogAcik = false;
    private bool uyariGosteriliyor = false;

    void Awake() { Instance = this; }

    void Start()
    {
        if (diyalogPanel != null) diyalogPanel.SetActive(false);
        if (konusIpucu != null) konusIpucu.SetActive(false);
    }

    void Update()
    {
        YakinNpcKontrol();

        Keyboard klavye = Keyboard.current;
        if (klavye == null) return;

        if (klavye.tKey.wasPressedThisFrame && yakinNpc != null && !diyalogAcik && !uyariGosteriliyor)
        {
            string npcAdi = yakinNpc.GetAd();

            if (GorevYoneticisi.Instance != null)
            {
                if (npcAdi.Contains("Kemal") && GorevYoneticisi.Instance.kameraKaydiBulundu)
                {
                    if (GorevYoneticisi.Instance.kemalleKonusuldu && !GorevYoneticisi.Instance.ilacKutusuAlindi)
                    {
                        StartCoroutine(MudurUyarisiGoster("Evlat, vincin oradaki delillere bak demiştin, git önce onları araştır!"));
                        return;
                    }
                    else if (GorevYoneticisi.Instance.ahmetleKonusuldu && !GorevYoneticisi.Instance.odaVePanoIncelendi)
                    {
                        StartCoroutine(MudurUyarisiGoster("Odamda ne işin var? Çık dışarı!"));
                        return;
                    }
                }
            }

            diyalogAcik = true;
            if (konusIpucu != null) konusIpucu.SetActive(false);
            if (diyalogPanel != null) diyalogPanel.SetActive(true);
            if (npcAdiText != null) npcAdiText.text = npcAdi;
            if (konusmaText != null) konusmaText.text = "";

            // Diyalog esnasında oyuncunun yürümesini engellemek için kilit koyuyoruz
            hareket oyuncuScripti = FindFirstObjectByType<hareket>();
            if (oyuncuScripti != null) oyuncuScripti.hareketEdebilirMi = false;

            // HATANIN DÜZELTİLDİĞİ YER: Orijinal 'Konustur' fonksiyonunu kendi yapısıyla çağırıyoruz
            StartCoroutine(yakinNpc.Konustur(
                metin => { if (konusmaText != null) konusmaText.text = metin; },
                () => DiyaloguBitir()
            ));
        }
    }

    void YakinNpcKontrol()
    {
        if (diyalogAcik || uyariGosteriliyor) return;

        Collider[] yakinlar = Physics.OverlapSphere(transform.position, etkilesimMesafesi);
        NpcDiyalog enYakinNpc = null;
        float enYakinMesafe = etkilesimMesafesi;

        foreach (Collider col in yakinlar)
        {
            if (col.transform == transform || col.transform.IsChildOf(transform)) continue;

            NpcDiyalog npc = col.GetComponent<NpcDiyalog>();
            if (npc == null) npc = col.GetComponentInParent<NpcDiyalog>();

            if (npc != null)
            {
                float mesafe = Vector3.Distance(transform.position, npc.transform.position);
                if (mesafe < enYakinMesafe)
                {
                    enYakinMesafe = mesafe;
                    enYakinNpc = npc;
                }
            }
        }

        yakinNpc = enYakinNpc;

        if (yakinNpc != null && konusIpucu != null)
            konusIpucu.SetActive(true);
        else if (konusIpucu != null)
            konusIpucu.SetActive(false);
    }

    void DiyaloguBitir()
    {
        diyalogAcik = false;
        if (diyalogPanel != null) diyalogPanel.SetActive(false);

        // Diyalog bitince oyuncunun hareket kilidini kaldırıyoruz
        hareket oyuncuScripti = FindFirstObjectByType<hareket>();
        if (oyuncuScripti != null) oyuncuScripti.hareketEdebilirMi = true;

        if (yakinNpc != null && GorevYoneticisi.Instance != null)
        {
            string npcAdi = yakinNpc.GetAd();

            // === FİNAL TETİKLEME KONTROLÜ ===
            if (npcAdi.Contains("Kemal") && GorevYoneticisi.Instance.kameraKaydiBulundu)
            {
                GorevYoneticisi.Instance.FinalHesaplasmaTamamla();
            }
            // === NORMAL GÖREV AKIŞ KONTROLLERİ ===
            else if (npcAdi.Contains("Kemal"))
            {
                if (GorevYoneticisi.Instance.odaVePanoIncelendi && !GorevYoneticisi.Instance.kemalPanikledi)
                    GorevYoneticisi.Instance.KemalPanikGoreviniTamamla();
                else if (!GorevYoneticisi.Instance.kemalleKonusuldu)
                    GorevYoneticisi.Instance.KemalGoreviniTamamla();
            }
            else if (npcAdi.Contains("Ahmet"))
            {
                if (GorevYoneticisi.Instance.ilacKutusuAlindi && !GorevYoneticisi.Instance.ahmetleKonusuldu)
                    GorevYoneticisi.Instance.AhmetGoreviniTamamla();
            }
            else if (npcAdi.Contains("Rıza") || npcAdi.Contains("Riza"))
            {
                if (GorevYoneticisi.Instance.teknikDelillerAlindi && !GorevYoneticisi.Instance.rizaItirafEtti)
                    GorevYoneticisi.Instance.RizaGoreviniTamamla();
            }
        }

        if (yakinNpc != null && konusIpucu != null)
            konusIpucu.SetActive(true);
    }

    IEnumerator MudurUyarisiGoster(string mesaj)
    {
        uyariGosteriliyor = true;
        if (konusIpucu != null) konusIpucu.SetActive(false);
        if (diyalogPanel != null) diyalogPanel.SetActive(true);
        
        if (npcAdiText != null) npcAdiText.text = "Müdür Kemal";
        if (konusmaText != null) konusmaText.text = mesaj;

        hareket oyuncuScripti = FindFirstObjectByType<hareket>();
        if (oyuncuScripti != null) oyuncuScripti.hareketEdebilirMi = false;

        yield return new WaitForSeconds(3f);

        if (diyalogPanel != null) diyalogPanel.SetActive(false);
        if (oyuncuScripti != null) oyuncuScripti.hareketEdebilirMi = true;
        uyariGosteriliyor = false;

        YakinNpcKontrol();
    }
}