using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class DiyalogYoneticisi : MonoBehaviour
{
    public static DiyalogYoneticisi Instance;

    [Header("UI")]
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
        diyalogPanel.SetActive(false);
        konusIpucu.SetActive(false);
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

        Collider[] yakinlar = Physics.OverlapSphere(
            transform.position, etkilesimMesafesi);

        NpcDiyalog enYakin = null;
        float enYakinMesafe = etkilesimMesafesi;

        foreach (Collider col in yakinlar)
        {
            NpcDiyalog npc = col.GetComponent<NpcDiyalog>();
            if (npc == null)
                npc = col.GetComponentInParent<NpcDiyalog>();

            if (npc != null)
            {
                float mesafe = Vector3.Distance(
                    transform.position, npc.transform.position);
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
                konusIpucu.SetActive(true);
            }
            else
            {
                konusIpucu.SetActive(false);
            }
        }
    }

    void DiyaloguBaslat(NpcDiyalog npc)
    {
        diyalogAcik = true;
        diyalogPanel.SetActive(true);
        konusIpucu.SetActive(false);
        npcAdiText.text = npc.GetAd();
        konusmaText.text = "";

        StartCoroutine(npc.Konustur(
            metin => konusmaText.text = metin,
            () => StartCoroutine(DiyaloguKapat())
        ));
    }

    IEnumerator DiyaloguKapat()
    {
        yield return new WaitForSeconds(1f);
        diyalogPanel.SetActive(false);
        diyalogAcik = false;
        if (yakinNpc != null)
            konusIpucu.SetActive(true);
    }
}