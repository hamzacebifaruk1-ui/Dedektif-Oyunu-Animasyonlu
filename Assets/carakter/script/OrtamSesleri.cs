using UnityEngine;

public class OrtamSesleri : MonoBehaviour
{
    [Header("Ortam Sesleri")]
    public AudioClip limanAmbiyans;
    public AudioClip ruzgar;
    public AudioClip denizDalgasi;

    [Header("Ses Kaynakları")]
    private AudioSource limanSource;
    private AudioSource ruzgarSource;
    private AudioSource denizSource;

    void Start()
    {
        // Ses seviyelerini (Volume) büyük oranda kıstık ki ayak seslerini bastırmasın
        // Liman ve rüzgar genel atmosfer olduğu için 2D (is3D = false) kalmaya devam ediyor ama çok derinden gelecek
        limanSource = OlusturAudioSource(limanAmbiyans, 0.08f, false); 
        ruzgarSource = OlusturAudioSource(ruzgar, 0.04f, false);

        // Deniz dalgası sesini 3D (is3D = true) yapıyoruz. 
        // Bu objenin sahnedeki pozisyonu denizin olduğu yere yakın olmalıdır!
        denizSource = OlusturAudioSource(denizDalgasi, 0.2f, true);
    }

    AudioSource OlusturAudioSource(AudioClip clip, float volume, bool is3D)
    {
        if (clip == null) return null;

        GameObject obj = new GameObject("OrtamSes_" + clip.name);
        obj.transform.SetParent(transform);
        
        // Eğer ses 3D ise, bu alt objeyi ana objeyle aynı yere konumlandır
        obj.transform.localPosition = Vector3.zero;

        AudioSource source = obj.AddComponent<AudioSource>();
        source.clip = clip;
        source.loop = true;
        source.playOnAwake = true;
        source.volume = volume;

        if (is3D)
        {
            source.spatialBlend = 1f; // 1f = Tamamen 3D Ses
            source.rolloffMode = AudioRolloffMode.Logarithmic; // Uzaklaştıkça doğal azalma
            source.minDistance = 5f;  // 5 metreye kadar ses tam şiddetinde duyulur
            source.maxDistance = 35f; // 35 metreden sonra ses tamamen kesilir
        }
        else
        {
            source.spatialBlend = 0f; // 0f = 2D Ses (Her yerden eşit duyulur)
        }

        source.Play();
        return source;
    }
}