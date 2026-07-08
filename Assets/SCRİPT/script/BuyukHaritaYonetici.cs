using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic; // Listeleme için gerekli kütüphane eklendi

// Büyük (M tuşuyla açılan) haritayı yöneten script.
// Oyuncu ikonu takibi + NPC takibi + sağ-click sürükleme (pan) + scroll zoom içerir.
public class BuyukHaritaYonetici : MonoBehaviour
{
    [System.Serializable]
    public class NpcIkonEslesmesi
    {
        public string npcAdi;
        public Transform npcTransform;
        public RectTransform ikonRect;
    }

    [Header("Panel ve Görsel Referansları")]
    public GameObject buyukHaritaPaneli;
    public RectTransform genisHaritaResmiRect;
    public RectTransform oyuncuIkonuRect;
    public Transform dedektifTransform;

    [Header("NPC İkonları")]
    public List<NpcIkonEslesmesi> npcIkonlari = new List<NpcIkonEslesmesi>();

    [Header("Kalibrasyon (Aynı Dünya Noktaları, Minimap'le Aynı Piksel Değerleri)")]
    public Transform kalibrasyonA_Dunya;
    public Vector2 kalibrasyonA_Piksel;
    public Transform kalibrasyonB_Dunya;
    public Vector2 kalibrasyonB_Piksel;

    [Header("Bu Haritanın Görsel Çözünürlüğü")]
    public Vector2 gorselCozunurluk = new Vector2(1024, 1024);

    [Header("Zoom Ayarları")]
    public float minZoom = 1f;
    public float maxZoom = 3f;
    public float zoomHassasiyeti = 0.1f;

    [Header("Pan (Sürükleme) Ayarları")]
    public RectTransform haritaViewport; // Buyuk_Harita_Paneli'nen RectTransform'u

    private bool haritaAcik = false;
    private bool suruklemeAktif = false;
    private Vector2 sonFarePozisyonu;

    void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb != null && kb.mKey.wasPressedThisFrame)
        {
            haritaAcik = !haritaAcik;
            buyukHaritaPaneli.SetActive(haritaAcik);
            Cursor.lockState = haritaAcik ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = haritaAcik;

            if (haritaAcik)
            {
                genisHaritaResmiRect.localScale = Vector3.one;
                genisHaritaResmiRect.anchoredPosition = Vector2.zero;
            }
        }

        if (!haritaAcik) return;

        GuncelleOyuncuIkonu();
        GuncellePanVeZoom();
    }

    void GuncelleOyuncuIkonu()
    {
        if (dedektifTransform == null || genisHaritaResmiRect == null) return;
        if (kalibrasyonA_Dunya == null || kalibrasyonB_Dunya == null) return;

        Vector3 a = kalibrasyonA_Dunya.position;
        Vector3 b = kalibrasyonB_Dunya.position;
        Vector3 oyuncu = dedektifTransform.position;

        float oranX = Mathf.InverseLerp(a.z, b.z, oyuncu.z);
        float oranY = Mathf.InverseLerp(a.x, b.x, oyuncu.x);

        float pikselX = Mathf.Lerp(kalibrasyonA_Piksel.x, kalibrasyonB_Piksel.x, oranX);
        float pikselY = gorselCozunurluk.y - Mathf.Lerp(kalibrasyonA_Piksel.y, kalibrasyonB_Piksel.y, oranY);

        float olcekX = genisHaritaResmiRect.rect.width / gorselCozunurluk.x;
        float olcekY = genisHaritaResmiRect.rect.height / gorselCozunurluk.y;

        float haritaX = (pikselX - gorselCozunurluk.x / 2f) * olcekX;
        float haritaY = (pikselY - gorselCozunurluk.y / 2f) * olcekY;

        oyuncuIkonuRect.anchoredPosition = new Vector2(haritaX, haritaY);

        float karakterYRotasyonu = dedektifTransform.eulerAngles.y;
        oyuncuIkonuRect.localRotation = Quaternion.Euler(0, 0, -karakterYRotasyonu);

        // === NPC İkonlarını Güncelle ===
        foreach (var npc in npcIkonlari)
        {
            if (npc.npcTransform == null || npc.ikonRect == null) continue;

            Vector3 npcOyuncu = npc.npcTransform.position;

            float npcOranX = Mathf.InverseLerp(a.z, b.z, npcOyuncu.z);
            float npcOranY = Mathf.InverseLerp(a.x, b.x, npcOyuncu.x);

            float npcPikselX = Mathf.Lerp(kalibrasyonA_Piksel.x, kalibrasyonB_Piksel.x, npcOranX);
            float npcPikselY = gorselCozunurluk.y - Mathf.Lerp(kalibrasyonA_Piksel.y, kalibrasyonB_Piksel.y, npcOranY);

            float npcHaritaX = (npcPikselX - gorselCozunurluk.x / 2f) * olcekX;
            float npcHaritaY = (npcPikselY - gorselCozunurluk.y / 2f) * olcekY;

            npc.ikonRect.anchoredPosition = new Vector2(npcHaritaX, npcHaritaY);
        }
    }

    void GuncellePanVeZoom()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null || genisHaritaResmiRect == null) return;

        // === SAĞ CLICK İLE SÜRÜKLEME ===
        if (mouse.rightButton.wasPressedThisFrame)
        {
            suruklemeAktif = true;
            sonFarePozisyonu = mouse.position.ReadValue();
        }
        else if (mouse.rightButton.wasReleasedThisFrame)
        {
            suruklemeAktif = false;
        }

        if (suruklemeAktif)
        {
            Vector2 simdikiFare = mouse.position.ReadValue();
            Vector2 fark = simdikiFare - sonFarePozisyonu;
            genisHaritaResmiRect.anchoredPosition += fark;
            sonFarePozisyonu = simdikiFare;
            SinirlariKisitla();
        }

        // === SCROLL İLE ZOOM ===
        float scroll = mouse.scroll.ReadValue().y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            float yeniOlcek = Mathf.Clamp(
                genisHaritaResmiRect.localScale.x + scroll * zoomHassasiyeti * 0.01f,
                minZoom, maxZoom);
            genisHaritaResmiRect.localScale = new Vector3(yeniOlcek, yeniOlcek, 1f);
            SinirlariKisitla();
        }
    }

    void SinirlariKisitla()
    {
        if (haritaViewport == null) return;

        Vector2 haritaBoyutu = genisHaritaResmiRect.rect.size * genisHaritaResmiRect.localScale.x;
        Vector2 viewportBoyutu = haritaViewport.rect.size;

        float maxX = Mathf.Max(0, (haritaBoyutu.x - viewportBoyutu.x) / 2f);
        float maxY = Mathf.Max(0, (haritaBoyutu.y - viewportBoyutu.y) / 2f);

        Vector2 pos = genisHaritaResmiRect.anchoredPosition;
        pos.x = Mathf.Clamp(pos.x, -maxX, maxX);
        pos.y = Mathf.Clamp(pos.y, -maxY, maxY);
        genisHaritaResmiRect.anchoredPosition = pos;
    }
}