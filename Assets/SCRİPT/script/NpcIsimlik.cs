using UnityEngine;
using TMPro;

public class NpcIsimlik : MonoBehaviour
{
    [Header("UI Ayarları")]
    public GameObject isimCanvas; // NPC'nin kafasındaki Canvas
    
    [Header("Mesafe Ayarları")]
    public float gorunmeMesafesi = 6f; // Kaç metreden sonra isim görünsün?
    
    private Transform oyuncuKamera;

    void Start()
    {
        if (Camera.main != null)
        {
            oyuncuKamera = Camera.main.transform;
        }
        
        if (isimCanvas != null) isimCanvas.SetActive(false);
    }

    void Update()
    {
        if (oyuncuKamera == null || isimCanvas == null) return;

        // Oyuncu ile NPC arasındaki mesafeyi ölçüyoruz
        float mesafe = Vector3.Distance(transform.position, oyuncuKamera.position);

        if (mesafe <= gorunmeMesafesi)
        {
            isimCanvas.SetActive(true);
            
            // Tabela etkisi: İsmin sürekli kameraya bakmasını sağlıyoruz (Billboard)
            isimCanvas.transform.LookAt(isimCanvas.transform.position + oyuncuKamera.forward);
        }
        else
        {
            isimCanvas.SetActive(false);
        }
    }
}