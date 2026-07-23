using System.Collections;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    [Header("Işık Ayarları")]
    public Light targetLight;

    [Header("Şiddet (Intensity) Ayarları")]
    public float minIntensity = 0.1f;
    public float maxIntensity = 2.5f;

    [Header("Zamanlama Ayarları")]
    public float minWaitTime = 0.02f;
    public float maxWaitTime = 0.2f;

    [Header("Ses Ayarları")]
    public AudioSource audioSource;     // Ses bileşeni
    public AudioClip[] flickerClips;   // Rastgele çalacak kısa cızırtı/çıtırtı sesleri
    public bool syncVolumeWithLight = true; // Ses seviyesi ışığın parlaklığına göre değişsin mi?

    private void Start()
    {
        if (targetLight == null)
            targetLight = GetComponent<Light>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        StartCoroutine(FlickerRoutine());
    }

    private IEnumerator FlickerRoutine()
    {
        while (true)
        {
            if (targetLight != null)
            {
                // Rastgele ışık şiddeti seç
                float randomIntensity = Random.Range(minIntensity, maxIntensity);
                targetLight.intensity = randomIntensity;

                // Ses efektlerini tetikle
                PlayFlickerSound(randomIntensity);

                // Bekleme süresi
                float randomWait = Random.Range(minWaitTime, maxWaitTime);
                yield return new WaitForSeconds(randomWait);
            }
            else
            {
                yield return null;
            }
        }
    }

    private void PlayFlickerSound(float currentIntensity)
    {
        if (audioSource == null) return;

        // 1. Eğer ses klipleri eklendiyse rastgele birini çal
        if (flickerClips != null && flickerClips.Length > 0)
        {
            // Sadece ışık yüksek şiddette çaktığında ses çalsın (opsiyonel mantık)
            if (Random.value > 0.4f) 
            {
                AudioClip clip = flickerClips[Random.Range(0, flickerClips.Length)];
                audioSource.PlayOneShot(clip, Random.Range(0.3f, 0.8f));
            }
        }

        // 2. Işık şiddetiyle ses yüksekliğini eşitle (Sürekli cızırtı sesi döngüdeyse çok iyi çalışır)
        if (syncVolumeWithLight && audioSource.loop)
        {
            float normalizedIntensity = Mathf.InverseLerp(minIntensity, maxIntensity, currentIntensity);
            audioSource.volume = normalizedIntensity;
        }
    }
}