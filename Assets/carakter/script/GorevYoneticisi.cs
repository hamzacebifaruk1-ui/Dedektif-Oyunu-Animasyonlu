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
    public bool kameraKaydiBulundu = false; 
    public bool rizaItirafEtti = false;

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

        if (kemalleKonusuldu)
            guncelMetin += "<s>Görev: Müdür Kemal ile konuş.</s>\n";
        else
            guncelMetin += "<color=white>Görev: Şantiye Müdürü Kemal ile konuş ve bilgi al.</color>\n";

        if (kemalleKonusuldu)
        {
            if (ilacKutusuAlindi)
                guncelMetin += "<s>Görev: Vincin altındaki İlaç Kutusu'nu araştır.</s>\n";
            else
                guncelMetin += "<color=yellow>Görev: Kemal'in bahsettiği vinç altındaki İlaç Kutusu'nu bul.</color>\n";
        }

        if (ilacKutusuAlindi)
        {
            if (ahmetleKonusuldu)
                guncelMetin += "<s>Görev: İlaç kutusunu sormak için İşçi Ahmet ile konuş.</s>\n";
            else
                guncelMetin += "<color=orange>Görev: Şimdi ilacı sormak için İşçi Ahmet ile konuş.</color>\n";
        }

        if (ahmetleKonusuldu)
        {
            if (odaVePanoIncelendi)
                guncelMetin += "<s>Görev: Müdürün Odası'ndaki Defteri ve Pano'daki Notu incele.</s>\n";
            else
                guncelMetin += "<color=green>Görev: Müdürün Odası'ndaki Yırtık Defteri ve Pano'daki Anonim Notu ele geçir.</color>\n";
        }

        if (odaVePanoIncelendi)
        {
            if (kemalPanikledi)
                guncelMetin += "<s>Görev: Eldeki yeni delilleri Müdür Kemal'e göster.</s>\n";
            else
                guncelMetin += "<color=red>Görev: Eldeki yeni delillerle Müdür Kemal'i köşeye sıkıştır!</color>\n";
        }

        if (kemalPanikledi)
        {
            if (teknikDelillerAlindi)
                guncelMetin += "<s>Görev: Vincin kopan tellerini ve kancasını incele.</s>\n";
            else
                guncelMetin += "<color=purple>Görev: Kemal'in bahsettiği Kırık Vinç Kancası ve Kırık Vinç Teli'ni bul.</color>\n";
        }

        if (teknikDelillerAlindi)
        {
            if (kameraKaydiBulundu)
                guncelMetin += "<s>Görev: Trafonun üstündeki USB kamera kaydını al.</s>\n";
            else if (rizaItirafEtti)
                guncelMetin += "<color=cyan>Görev: Rıza'nın bahsettiği trafoda gizlenen orijinal kamera kaydını (USB) bul!</color>\n";
            else
                guncelMetin += "<color=lightblue>Görev: Güvenlik Görevlisi Rıza'yı sabotaj delilleriyle sorgula.</color>\n";
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