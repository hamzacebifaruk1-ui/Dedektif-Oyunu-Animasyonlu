using UnityEngine;
using System.Collections.Generic;

public class AtesTitresimi : MonoBehaviour
{
    [Header("Referanslar")]
    public Light hedefIsik; // Titreyecek olan Point Light

    [Header("Şiddet (Intensity) Ayarları")]
    public float minSiddet = 1.0f; // En sönük an
    public float maxSiddet = 3.0f; // En parlak an

    [Header("Menzil (Range) Ayarları")]
    public float minMenzil = 5.0f; // En dar aydınlatma
    public float maxMenzil = 7.0f; // En geniş aydınlatma

    [Header("Hız / Pürüzsüzlük Ayarı")]
    [Range(1, 50)]
    public int pruzsuzluk = 10; // Değer artarsa titreşim daha yavaş ve "kandil" gibi olur. Düşerse "kıvılcım" gibi hızlı olur.

    // FIFO kuyruğu (ilk giren ilk çıkar) pürüzsüzleştirme için kullanılır
    private Queue<float> siddetKuyrugu = new Queue<float>();
    private float sonSiddetToplami = 0;
    private Queue<float> menzilKuyrugu = new Queue<float>();
    private float sonMenzilToplami = 0;

    void Start()
    {
        // Eğer script'in bağlı olduğu objede Light varsa ve atanmamışsa otomatik al
        if (hedefIsik == null)
        {
            hedefIsik = GetComponent<Light>();
        }

        // Başlangıç kuyruklarını doldur
        if (hedefIsik != null)
        {
            InitializeQueue(siddetKuyrugu, ref sonSiddetToplami, hedefIsik.intensity);
            InitializeQueue(menzilKuyrugu, ref sonMenzilToplami, hedefIsik.range);
        }
    }

    void Update()
    {
        if (hedefIsik == null) return;

        // --- Şiddet (Intensity) Hesaplama ---
        UpdateParametre(siddetKuyrugu, ref sonSiddetToplami, minSiddet, maxSiddet, out float yeniSiddet);
        hedefIsik.intensity = yeniSiddet;

        // --- Menzil (Range) Hesaplama ---
        UpdateParametre(menzilKuyrugu, ref sonMenzilToplami, minMenzil, maxMenzil, out float yeniMenzil);
        hedefIsik.range = yeniMenzil;
    }

    // Kuyruğu pürüzsüzlük değeri kadar doldurur
    private void InitializeQueue(Queue<float> queue, ref float sum, float startValue)
    {
        queue.Clear();
        sum = 0;
        for (int i = 0; i < pruzsuzluk; i++)
        {
            queue.Enqueue(startValue);
            sum += startValue;
        }
    }

    // Rastgele yeni bir değer üretip ortalamasını alarak pürüzsüzleştirir
    private void UpdateParametre(Queue<float> queue, ref float sum, float min, float max, out float smoothedValue)
    {
        // En eski değeri çıkar
        while (queue.Count >= pruzsuzluk)
        {
            sum -= queue.Dequeue();
        }

        // Rastgele yeni bir değer üret ve ekle
        float newVal = Random.Range(min, max);
        queue.Enqueue(newVal);
        sum += newVal;

        // Ortalamayı al
        smoothedValue = sum / queue.Count;
    }
}