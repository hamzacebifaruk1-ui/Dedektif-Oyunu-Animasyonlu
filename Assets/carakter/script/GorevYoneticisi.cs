using UnityEngine;
using TMPro;

public class GorevYoneticisi : MonoBehaviour
{
    public static GorevYoneticisi Instance;

    [Header("UI Elemanları")]
    public TextMeshProUGUI gorevText;

    [Header("Başlangıç İpucu")]
    public GameObject kemalYerIpucu; 

    [Header("Görev Durumları")]
    public bool kemalleKonusuldu = false;
    public bool ilacKutusuAlindi = false; 
    public bool ahmetleKonusuldu = false; 
    public bool odaVePanoIncelendi = false; 
    public bool kemalPanikledi = false;    
    public bool teknikDelillerAlindi = false; 
    public bool rizaItirafEtti = false;
    public bool kameraKaydiBulundu = false; 

    [HideInInspector] public bool kirikTelAlindi = false;
    [HideInInspector] public bool kirikKancaAlindi = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (kemalYerIpucu != null) kemalYerIpucu.SetActive(true);
        GoreviListele();
    }

    public void GoreviListele()
    {
        if (gorevText == null) return;

        string guncelMetin = "";

        // Aşama 1: Kemal İlk Konuşma
        if (kemalleKonusuldu)
            guncelMetin += "<s>Görev: Müdür Kemal ile konuş.</s>\n";
        else
            guncelMetin += "<color=white>Görev: Şantiye Müdürü Kemal ile konuş ve bilgi al.</color>\n";

        // Aşama 2: İlaç Kutusu Arama
        if (kemalleKonusuldu)
        {
            if (ilacKutusuAlindi)
                guncelMetin += "<s>Görev: Vincin altındaki İlaç Kutusu'nu araştır.</s>\n";
            else
                guncelMetin += "<color=yellow>Görev: Vincin altındaki İlaç Kutusu'nu araştır.</color>\n";
        }

        // Aşama 3: Ahmet Sorgusu
        if (ilacKutusuAlindi)
        {
            if (ahmetleKonusuldu)
                guncelMetin += "<s>Görev: İlaç kutusunu sormak için İşçi Ahmet ile konuş.</s>\n";
            else
                guncelMetin += "<color=orange>Görev: İlaç kutusunu sormak için İşçi Ahmet ile konuş.</color>\n";
        }

        // Aşama 4: Oda ve Pano İnceleme
        if (ahmetleKonusuldu)
        {
            if (odaVePanoIncelendi)
                guncelMetin += "<s>Görev: Müdürün Odası'ndaki Defteri ve Pano'daki Notu incele.</s>\n";
            else
                guncelMetin += "<color=green>Görev: Müdürün Odası'ndaki Defteri ve Pano'daki Notu incele.</color>\n";
        }

        // Aşama 5: Kemal Panik / Köşeye Sıkıştırma
        if (odaVePanoIncelendi)
        {
            if (kemalPanikledi)
                guncelMetin += "<s>Görev: Eldeki yeni delilleri Müdür Kemal'e göster.</s>\n";
            else
                guncelMetin += "<color=red>Görev: Eldeki yeni delilleri Müdür Kemal'e göster.</color>\n";
        }

        // Aşama 6: Teknik Deliller (Tel ve Kanca)
        if (kemalPanikledi)
        {
            if (teknikDelillerAlindi)
                guncelMetin += "<s>Görev: Kemal'in bahsettiği Kırık Vinç Kancası ve Kırık Vinç Teli'ni bul.</s>\n";
            else
                guncelMetin += "<color=purple>Görev: Kemal'in bahsettiği Kırık Vinç Kancası ve Kırık Vinç Teli'ni bul.</color>\n";
        }

        // Aşama 7: Güvenlik Rıza Sorgusu ve USB Arama Aşamaları
        if (teknikDelillerAlindi)
        {
            // Rıza itiraf ettiyse artık USB aranıyor demektir
            if (rizaItirafEtti)
            {
                guncelMetin += "<s>Görev: Güvenlik Görevlisi Rıza'yı sabotaj delilleriyle sorgula.</s>\n";
                
                if (kameraKaydiBulundu)
                    guncelMetin += "<s>Görev: Rıza'nın bahsettiği trafoda gizlenen orijinal kamera kaydını (USB) bul!</s>\n";
                else
                    guncelMetin += "<color=cyan>Görev: Rıza'nın bahsettiği trafoda gizlenen orijinal kamera kaydını (USB) bul!</color>\n";
            }
            else
            {
                guncelMetin += "<color=lightblue>Görev: Güvenlik Görevlisi Rıza'yı sabotaj delilleriyle sorgula.</color>\n";
            }
        }

        // Oyun Sonu Final Kontrolü
        if (kameraKaydiBulundu)
        {
            guncelMetin += "<color=red><b>[FİNAL] Tüm deliller toplandı! Müdür Kemal ile son kez hesaplaş!</b></color>\n";
        }

        gorevText.text = guncelMetin;
    }

    public void KemalGoreviniTamamla()
    {
        if (!kemalleKonusuldu)
        {
            kemalleKonusuldu = true;
            if (kemalYerIpucu != null) kemalYerIpucu.SetActive(false);
        }
        GoreviListele();
    }

    public void KemalPanikGoreviniTamamla()
    {
        kemalPanikledi = true;
        GoreviListele();
    }

    public void RizaGoreviniTamamla()
    {
        rizaItirafEtti = true;
        GoreviListele();
    }

    public void IlacKutusuBulundu() { ilacKutusuAlindi = true; GoreviListele(); }
    public void AhmetGoreviniTamamla() { ahmetleKonusuldu = true; GoreviListele(); }
    public void OdaVePanoTamamla() { odaVePanoIncelendi = true; GoreviListele(); }
    public void TeknikDelillerTamamla() { teknikDelillerAlindi = true; GoreviListele(); }
    public void KameraKaydiBulunduTamamla() { kameraKaydiBulundu = true; GoreviListele(); }
}