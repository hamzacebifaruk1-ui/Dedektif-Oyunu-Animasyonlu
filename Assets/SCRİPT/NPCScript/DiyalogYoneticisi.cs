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
                if (!GorevYoneticisi.Instance.kemalleKonusuldu && !npcAdi.Contains("Kemal"))
                {
                    StartCoroutine(MudurUyarisiGoster("Önce Şantiye Müdürü Kemal ile konuşup kaza hakkında bilgi almalıyım..."));
                    return; 
                }

                if (GorevYoneticisi.Instance.kemalleKonusuldu && !GorevYoneticisi.Instance.ilacKutusuAlindi && !npcAdi.Contains("Kemal"))
                {
                    StartCoroutine(MudurUyarisiGoster("Müdürün bahsettiği vinç altındaki o İlaç Kutusu'nu bulmadan kimseyi sorgulayamam..."));
                    return;
                }

                if (GorevYoneticisi.Instance.ahmetleKonusuldu && !GorevYoneticisi.Instance.odaVePanoIncelendi && (npcAdi.Contains("Kemal") || npcAdi.Contains("Rıza") || npcAdi.Contains("Riza")))
                {
                    StartCoroutine(MudurUyarisiGoster("Ahmet'in bahsettiği Müdürün Odası'ndaki Defteri ve Pano'daki Notu bulmalıyım."));
                    return;
                }
            }

            DiyaloguBaslat(yakinNpc);
        }
    }

    void YakinNpcKontrol()
    {
        if (diyalogAcik || uyariGosteriliyor) return;

        Collider[] yakinlar = Physics.OverlapSphere(transform.position, etkilesimMesafesi);
        NpcDiyalog enYakin = null;
        float enYakinMesafe = etkilesimMesafesi;

        foreach (Collider col in yakinlar)
        {
            NpcDiyalog npc = col.GetComponent<NpcDiyalog>();
            if (npc == null) npc = col.GetComponentInParent<NpcDiyalog>();

            if (npc != null)
            {
                float mesafe = Vector3.Distance(transform.position, npc.transform.position);
                if (mesafe < enYakinMesafe)
                {
                    enYakinMesafe = mesafe;
                    enYakin = npc;
                }
            }
        }

        if (enYakin != yakinNpc)
        {
            if (yakinNpc != null) yakinNpc.OyuncuUzaklasti();
            yakinNpc = enYakin;
            if (yakinNpc != null)
            {
                yakinNpc.OyuncuYaklasti();
                if (konusIpucu != null) konusIpucu.SetActive(true);
            }
            else
            {
                if (konusIpucu != null) konusIpucu.SetActive(false);
            }
        }
    }

    void DiyaloguBaslat(NpcDiyalog npc)
    {
        diyalogAcik = true;
        if (diyalogPanel != null) diyalogPanel.SetActive(true);
        if (konusIpucu != null) konusIpucu.SetActive(false);
        if (npcAdiText != null) npcAdiText.text = npc.GetAd();
        if (konusmaText != null) konusmaText.text = "";

        StartCoroutine(npc.Konustur(
            metin => { if (konusmaText != null) konusmaText.text = metin; },
            () => StartCoroutine(DiyaloguKapat(npc.GetAd()))
        ));
    }

    IEnumerator DiyaloguKapat(string npcAdi)
    {
        yield return new WaitForSeconds(0.5f);
        if (diyalogPanel != null) diyalogPanel.SetActive(false);
        diyalogAcik = false;

        // DIALOG BITTIĞINDE GÖREVLERI TETIKLE
        if (GorevYoneticisi.Instance != null)
        {
            if (npcAdi.Contains("Kemal"))
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
        
        if (npcAdiText != null) npcAdiText.text = "Dedektif";
        if (konusmaText != null) konusmaText.text = mesaj;

        yield return new WaitForSeconds(3f);

        if (diyalogPanel != null) diyalogPanel.SetActive(false);
        uyariGosteriliyor = false;
        if (yakinNpc != null && konusIpucu != null) konusIpucu.SetActive(true);
    }
}