using UnityEngine;
using TMPro;

public class GorevYoneticisi : MonoBehaviour
{
    public static GorevYoneticisi Instance;

    [Header("UI Elemanları")]
    public TextMeshProUGUI gorevText;
    public TextMeshProUGUI ipucuText; 

    [Header("Mesafe Hedef Objeleri (Mesafe Takibi İçin)")]
    public GameObject kemalObjesi;
    public GameObject ahmetObjesi;
    public GameObject rizaObjesi;
    public GameObject odadakiDelilObjesi; // Pano veya Defter objesi

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
        GoreviListele();
    }

    public void GoreviListele()
    {
        if (gorevText == null) return;

        string guncelMetin = "";
        string guncelIpucu = ""; 

        // Aşama 1: Kemal İlk Konuşma
        if (kemalleKonusuldu)
        {
            // Eski görev bittiği için guncelMetin'e ekleme yapmıyoruz, ekran temiz kalıyor.
        }
        else
        {
            guncelMetin = "<color=white>Görev: Şantiye Müdürü Kemal ile konuş ve bilgi al.</color>\n";
            guncelIpucu = "İpucu: Müdür Kemal, şantiyenin girişindeki iki katlı idari binanın üst katındaki ofisindedir.";
        }

        // Aşama 2: İlaç Kutusu Arama
        if (kemalleKonusuldu)
        {
            if (ilacKutusuAlindi)
            {
                // Görev bitti, ekrandan gizlendi.
            }
            else
            {
                guncelMetin = "<color=yellow>Görev: Vincin altındaki İlaç Kutusu'nu araştır.</color>\n";
                if(guncelIpucu == "") guncelIpucu = "İpucu: Kazanın gerçekleştiği büyük sarı vincin ayaklarının etrafındaki gölgelik alanları kontrol et.";
            }
        }

        // Aşama 3: Ahmet Sorgusu
        if (ilacKutusuAlindi)
        {
            if (ahmetleKonusuldu)
            {
                // Görev bitti, ekrandan gizlendi.
            }
            else
            {
                guncelMetin = "<color=orange>Görev: İlaç kutusunu sormak için İşçi Ahmet ile konuş.</color>\n";
                if(guncelIpucu == "") guncelIpucu = "İpucu: İşçi Ahmet, şantiyenin arka tarafındaki konteyner yatılı bölgesinde mola veriyor.";
            }
        }

        // Aşama 4: Oda ve Pano Inceleme
        if (ahmetleKonusuldu)
        {
            if (odaVePanoIncelendi)
            {
                // Görev bitti, ekrandan gizlendi.
            }
            else
            {
                guncelMetin = "<color=green>Görev: Müdürün Odası'ndaki Defteri ve Pano'daki Notu incele.</color>\n";
                if(guncelIpucu == "") guncelIpucu = "İpucu: İdari binaya gizlice tekrar gir. Müdürün masasındaki defteri ve duvardaki panoyu iyice incele (İki delili de bulmalısın).";
            }
        }

        // Aşama 5: Kemal Panik / Köşeye Sıkıştırma
        if (odaVePanoIncelendi)
        {
            if (kemalPanikledi)
            {
                // Görev bitti, ekrandan gizlendi.
            }
            else
            {
                guncelMetin = "<color=red>Görev: Eldeki yeni delilleri Müdür Kemal'e göster.</color>\n";
                if(guncelIpucu == "") guncelIpucu = "İpucu: Bulduğun şüpheli notu ve defteri yanına alarak üst kattaki Müdür Kemal'le yüzleş.";
            }
        }

        // Aşama 6: Teknik Deliller (Tel ve Kanca)
        if (kemalPanikledi)
        {
            if (teknikDelillerAlindi)
            {
                // Görev bitti, ekrandan gizlendi.
            }
            else
            {
                guncelMetin = "<color=purple>Görev: Kemal'in bahsettiği Kırık Vinç Kancası ve Kırık Vinç Teli'ni bul.</color>\n";
                if(guncelIpucu == "") guncelIpucu = "İpucu: Vinç dairesinin altındaki hurda deposuna ve sahil kenarındaki kırık tel parçalarına göz at.";
            }
        }

        // Aşama 7: Güvenlik Rıza Sorgusu ve USB Arama Aşamaları
        if (teknikDelillerAlindi)
        {
            if (rizaItirafEtti)
            {
                if (kameraKaydiBulundu)
                {
                    // Görev bitti, ekrandan gizlendi.
                }
                else
                {
                    guncelMetin = "<color=cyan>Görev: Rıza'nın bahsettiği trafoda gizlenen orijinal kamera kaydını (USB) bul!</color>\n";
                    if(guncelIpucu == "") guncelIpucu = "İpucu: Şantiyenin dış sınırındaki yüksek gerilim trafosunun arka kapak panelini incele.";
                }
            }
            else
            {
                guncelMetin = "<color=lightblue>Görev: Güvenlik Görevlisi Rıza'yı sabotaj delilleriyle sorgula.</color>\n";
                if(guncelIpucu == "") guncelIpucu = "İpucu: Girişteki nizamiye kulübesinde nöbet tutan Güvenlik Rıza'nın yanına git.";
            }
        }

        // Oyun Sonu Final Kontrolü
        if (kameraKaydiBulundu)
        {
            guncelMetin = "<color=red><b>[FİNAL] Tüm deliller toplandı! Müdür Kemal ile son kez hesaplaş!</b></color>\n";
            guncelIpucu = "İpucu: Elindeki USB kamera kaydıyla Müdür Kemal'in odasına gir ve suçunu itiraf ettir!";
        }

        gorevText.text = guncelMetin;
        if (ipucuText != null) ipucuText.text = guncelIpucu;

        // --- DİNAMİK MESAFE TAKİP TETİKLEYİCİLERİ ---
        if (HedefTakipci.Instance != null)
        {
            if (!kemalleKonusuldu)
            {
                HedefTakipci.Instance.HedefDegistir(kemalObjesi, "Müdür Kemal");
            }
            else if (kemalleKonusuldu && !ilacKutusuAlindi)
            {
                HedefTakipci.Instance.HedefDegistir(null, ""); 
            }
            else if (ilacKutusuAlindi && !ahmetleKonusuldu)
            {
                HedefTakipci.Instance.HedefDegistir(ahmetObjesi, "İşçi Ahmet");
            }
            else if (ahmetleKonusuldu && !odaVePanoIncelendi)
            {
                HedefTakipci.Instance.HedefDegistir(odadakiDelilObjesi, "Müdürün Odası");
            }
            else if (odaVePanoIncelendi && !kemalPanikledi)
            {
                HedefTakipci.Instance.HedefDegistir(kemalObjesi, "Müdür Kemal");
            }
            else if (teknikDelillerAlindi && !rizaItirafEtti)
            {
                HedefTakipci.Instance.HedefDegistir(rizaObjesi, "Güvenlik Rıza");
            }
            else if (kameraKaydiBulundu)
            {
                HedefTakipci.Instance.HedefDegistir(kemalObjesi, "Müdür Kemal");
            }
            else
            {
                HedefTakipci.Instance.HedefDegistir(null, "");
            }
        }
    }

    public void KemalGoreviniTamamla()
    {
        if (!kemalleKonusuldu) kemalleKonusuldu = true;
        GoreviListele();
    }

    public void KemalPanikGoreviniTamamla() { kemalPanikledi = true; GoreviListele(); }
    public void RizaGoreviniTamamla() { rizaItirafEtti = true; GoreviListele(); }
    public void IlacKutusuBulundu() { ilacKutusuAlindi = true; GoreviListele(); }
    public void AhmetGoreviniTamamla() { ahmetleKonusuldu = true; GoreviListele(); }
    public void OdaVePanoTamamla() { odaVePanoIncelendi = true; GoreviListele(); }
    public void TeknikDelillerTamamla() { teknikDelillerAlindi = true; GoreviListele(); }
    public void KameraKaydiBulunduTamamla() { kameraKaydiBulundu = true; GoreviListele(); }
}