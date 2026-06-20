using UnityEngine;

public class YanipSonen : MonoBehaviour
{
    public float hiz = 2f;
    public float minYogunluk = 0f;
    public float maxYogunluk = 2f;
    private Light isik;

    void Start()
    {
        isik = GetComponent<Light>();
    }

    void Update()
    {
        isik.intensity = Mathf.Lerp(minYogunluk, maxYogunluk, 
            (Mathf.Sin(Time.time * hiz) + 1f) / 2f);
    }
}