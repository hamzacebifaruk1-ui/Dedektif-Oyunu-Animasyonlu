using UnityEngine;

[RequireComponent(typeof(Light))]
public class YanipSonen : MonoBehaviour
{
    public float hiz = 2f;
    public float minYogunluk = 0.2f;
    public float maxYogunluk = 1.8f;
    private Light isik;

    void Start()
    {
        isik = GetComponent<Light>();
    }

    void Update()
    {
        if (isik == null) return;
        
        // Pürüzsüz bir sinüs dalgası kullanarak ışık şiddetini dalgalandırır
        isik.intensity = Mathf.Lerp(minYogunluk, maxYogunluk, 
            (Mathf.Sin(Time.time * hiz) + 1f) / 2f);
    }
}