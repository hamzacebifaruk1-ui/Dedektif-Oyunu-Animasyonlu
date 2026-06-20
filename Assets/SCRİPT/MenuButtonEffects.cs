using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButtonEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [Header("Boyut Ayarları")]
    public float buyumeOrani = 1.15f;
    public float animasyonHizi = 10f;
    
    [Header("Sesler")]
    public AudioClip hoverSes;
    public AudioClip clickSes;
    
    [Header("Görsel Efekt Kontrolü")]
    public bool cerceveOlsun = true;
    public bool titremeOlsun = false; 
    public Color outlineRenk = Color.orange;
    
    [Header("Patlama Efekti (Sadece Çıkış İçin)")]
    public ParticleSystem patlamaEfekti; 

    private Vector3 normalBoyut;
    private Vector3 hedefBoyut;
    private Vector3 orjinalLocalPos;
    private AudioSource audioSource;
    private Outline outline;
    private bool fareUzerinde = false;

    void Start()
    {
        normalBoyut = transform.localScale;
        orjinalLocalPos = transform.localPosition;
        hedefBoyut = normalBoyut;
        
        // AudioSource bileşenini kur
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // Outline (Çerçeve) bileşenini kur
        outline = GetComponent<Outline>();
        if (outline == null) outline = gameObject.AddComponent<Outline>();
        
        outline.effectColor = outlineRenk;
        outline.effectDistance = new Vector2(3, -3);
        outline.enabled = false;
    }

    void Update()
    {
        // Boyut geçişini yumuşat
        transform.localScale = Vector3.Lerp(transform.localScale, hedefBoyut, Time.deltaTime * animasyonHizi);

        // Titreme Efekti (Sadece titremeOlsun seçiliyse ve fare üzerindeyse)
        if (fareUzerinde && titremeOlsun)
        {
            float x = Random.Range(-0.8f, 0.8f);
            float y = Random.Range(-0.8f, 0.8f);
            transform.localPosition = orjinalLocalPos + new Vector3(x, y, 0);
        }
        else
        {
            // Fare çekildiğinde veya titreme kapalıyken orjinal pozisyona yumuşak dön
            transform.localPosition = Vector3.Lerp(transform.localPosition, orjinalLocalPos, Time.deltaTime * animasyonHizi);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        fareUzerinde = true;
        hedefBoyut = normalBoyut * buyumeOrani;
        
        if (cerceveOlsun && outline != null) outline.enabled = true;

        if (hoverSes != null) audioSource.PlayOneShot(hoverSes);
        
        if (patlamaEfekti != null) patlamaEfekti.Play();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        fareUzerinde = false;
        hedefBoyut = normalBoyut;
        if (outline != null) outline.enabled = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (clickSes != null) audioSource.PlayOneShot(clickSes);
        // Basılma hissi için boyutu anlık küçült (Update içindeki Lerp bunu düzeltecektir)
        transform.localScale = normalBoyut * 0.85f; 
    }
}