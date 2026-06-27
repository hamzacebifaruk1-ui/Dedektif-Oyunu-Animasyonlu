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

        if (klavye.tKey.wasPressedThisFrame && yakinNpc != null && !diyalogAcik)
        {
            DiyaloguBaslat(yakinNpc);
        }
    }

    void YakinNpcKontrol()
    {
        if (diyalogAcik) return;

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
            () => StartCoroutine(DiyaloguKapat())
        ));
    }

    IEnumerator DiyaloguKapat()
    {
        yield return new WaitForSeconds(0.5f);
        if (diyalogPanel != null) diyalogPanel.SetActive(false);
        diyalogAcik = false;
        if (yakinNpc != null && konusIpucu != null)
            konusIpucu.SetActive(true);
    }
}