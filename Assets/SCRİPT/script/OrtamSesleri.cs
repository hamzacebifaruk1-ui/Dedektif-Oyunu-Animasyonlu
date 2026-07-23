using UnityEngine;

public class OrtamSesleri : MonoBehaviour
{
    [Header("Ortam Sesleri")]
    public AudioClip limanAmbiyans;
    public AudioClip ruzgar;
    public AudioClip denizDalgasi;

    [Header("Arka Plan Müziği")]
    public AudioClip gerilimMuzigi; // Dedektiflik gerilim müziği buraya eklenecek
    [Range(0f, 1f)] public float muziKSesSeviyesi = 0.25f; // Müzik seviyesini Inspector'dan kolayca ayarla

    [Header("Ses Kaynakları")]
    private AudioSource limanSource;
    private AudioSource ruzgarSource;
    private AudioSource denizSource;
    private AudioSource muziKSource;

    void Start()
    {
        // SES SEVİYELERİ YÜKSELTİLDİ:
        // Liman ambiyansını daha belirgin yaptık (0.08f -> 0.35f)
        limanSource = OlusturAudioSource(limanAmbiyans, 0.35f, false); 
        
        // Rüzgar sesini arkadan hafifçe destekleyecek şekilde artırdık (0.04f -> 0.20f)
        ruzgarSource = OlusturAudioSource(ruzgar, 0.20f, false);

        // Deniz dalgası 3D olduğu ve yaklaştıkça duyulacağı için biraz daha güçlü yaptık (0.2f -> 0.55f)
        denizSource = OlusturAudioSource(denizDalgasi, 0.55f, true);

        // Gerilim Müziği (2D olarak sürekli döngüde çalacak):
        muziKSource = OlusturAudioSource(gerilimMuzigi, muziKSesSeviyesi, false);
    }

    AudioSource OlusturAudioSource(AudioClip clip, float volume, bool is3D)
    {
        if (clip == null) return null;

        GameObject obj = new GameObject("OrtamSes_" + clip.name);
        obj.transform.SetParent(transform);
        
        obj.transform.localPosition = Vector3.zero;

        AudioSource source = obj.AddComponent<AudioSource>();
        source.clip = clip;
        source.loop = true;
        source.playOnAwake = true;
        source.volume = volume;

        if (is3D)
        {
            source.spatialBlend = 1f; 
            source.rolloffMode = AudioRolloffMode.Logarithmic; 
            source.minDistance = 5f;  
            source.maxDistance = 35f; 
        }
        else
        {
            source.spatialBlend = 0f; 
        }

        source.Play();
        return source;
    }
}