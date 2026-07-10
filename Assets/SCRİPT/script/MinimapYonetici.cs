using UnityEngine;
using System.Collections.Generic;

public class MinimapYonetici : MonoBehaviour
{
    [System.Serializable]
    public class NpcIkonEslesmesi
    {
        public string npcAdi;
        public Transform npcTransform;
        public RectTransform ikonRect;
    }

    [Header("Zorunlu Referanslar")]
    public Transform dedektifTransform;
    public RectTransform haritaDokusuPaneli;
    public RectTransform oyuncuOkIkonu;

    [Header("NPC İkonları")]
    public List<NpcIkonEslesmesi> npcIkonlari = new List<NpcIkonEslesmesi>();

    [Header("Kalibrasyon Noktası A")]
    public Transform kalibrasyonA_Dunya;
    public Vector2 kalibrasyonA_Piksel;

    [Header("Kalibrasyon Noktası B")]
    public Transform kalibrasyonB_Dunya;
    public Vector2 kalibrasyonB_Piksel;

    [Header("Harita Görseli Ayarları")]
    public Vector2 gorselCozunurluk = new Vector2(1024, 1024);

    void LateUpdate()
    {
        if (dedektifTransform == null || haritaDokusuPaneli == null) return;
        if (kalibrasyonA_Dunya == null || kalibrasyonB_Dunya == null) return;

        Vector2 oyuncuPiksel = DunyayiPikseleCevir(dedektifTransform.position);

        haritaDokusuPaneli.anchoredPosition = -oyuncuPiksel;

        if (oyuncuOkIkonu != null)
        {
            float karakterYRotasyonu = dedektifTransform.eulerAngles.y;
            oyuncuOkIkonu.localRotation = Quaternion.Euler(0, 0, -karakterYRotasyonu);
        }

        foreach (var npc in npcIkonlari)
        {
            if (npc.npcTransform == null || npc.ikonRect == null) continue;

            Vector2 npcPiksel = DunyayiPikseleCevir(npc.npcTransform.position);
            npc.ikonRect.anchoredPosition = npcPiksel - oyuncuPiksel;
        }
    }

    Vector2 DunyayiPikseleCevir(Vector3 dunyaPozisyonu)
    {
        Vector3 a = kalibrasyonA_Dunya.position;
        Vector3 b = kalibrasyonB_Dunya.position;

        float oranX = Mathf.InverseLerp(a.z, b.z, dunyaPozisyonu.z);
        float oranY = Mathf.InverseLerp(a.x, b.x, dunyaPozisyonu.x);

        float pikselX = Mathf.Lerp(kalibrasyonA_Piksel.x, kalibrasyonB_Piksel.x, oranX);
        float pikselY = gorselCozunurluk.y - Mathf.Lerp(kalibrasyonA_Piksel.y, kalibrasyonB_Piksel.y, oranY);

        float olcekX = haritaDokusuPaneli.rect.width / gorselCozunurluk.x;
        float olcekY = haritaDokusuPaneli.rect.height / gorselCozunurluk.y;

        float haritaX = (pikselX - gorselCozunurluk.x / 2f) * olcekX;
        float haritaY = (pikselY - gorselCozunurluk.y / 2f) * olcekY;

        return new Vector2(haritaX, haritaY);
    }

    public void NpcIkonlariniGoster(bool durum)
    {
        foreach (var npc in npcIkonlari)
        {
            if (npc.ikonRect != null)
            {
                npc.ikonRect.gameObject.SetActive(durum);
            }
        }
    }
}