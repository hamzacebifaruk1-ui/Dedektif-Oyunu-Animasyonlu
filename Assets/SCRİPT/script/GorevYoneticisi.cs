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
    public bool finalHesaplasmaBitti = false; // YENİ: Final hesaplaşma kontrolü

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
        if (kemalleKonusuldu) { }
        else
        {
            guncelMetin = "<color=white>Görev: Şantiye Müdürü Kemal ile konuş ve bilgi al.</color>\n";
            guncelIpucu = "İpucu: Müdür Kemal, şantiyenin girişindeki iki katlı idari binanın üst katındaki ofisindedir.";
        }

        // Aşama 2: İlaç Kutusu Arama
        if (kemalleKonusuldu)
        {
            if (ilacKutusuAlindi) { }
            else
            {
                guncelMetin = "<color=yellow>Görev: Vincin altındaki İlaç Kutusu'nu araştır.</color>\n";
                if(guncelIpucu == "") guncelIpucu = "İpucu: Kazanın gerçekleştiği büyük sarı vincin ayaklarının etrafındaki gölgelik alanları kontrol et.";
            }
        }

        // Aşama 3: Ahmet Sorgusu
        if (ilacKutusuAlindi)
        {
            if (ahmetleKonusuldu) { }
            else
            {
                guncelMetin = "<color=orange>Görev: İlaç kutusunu sormak için İşçi Ahmet ile konuş.</color>\n";
                if(guncelIpucu == "") guncelIpucu = "İpucu: İşçi Ahmet, şantiyenin arka tarafındaki konteyner yatılı bölgesinde mola veriyor.";
            }
        }

        // Aşama 4: Oda ve Pano Inceleme
        if (ahmetleKonusuldu)
        {
            if (odaVePanoIncelendi) { }
            else
            {
                guncelMetin = "<color=green>Görev: Müdürün Odası'ndaki Defteri ve Pano'daki Notu incele.</color>\n";
                if(guncelIpucu == "") guncelIpucu = "İpucu: İdari binaya gizlice tekrar gir. Müdürün masasındaki defteri ve duvardaki panoyu iyice incele (İki delili de bulmalısın).";
            }
        }

        // Aşama 5: Kemal Panik / Köşeye Sıkıştırma
        if (odaVePanoIncelendi)
        {
            if (kemalPanikledi) { }
            else
            {
                guncelMetin = "<color=red>Görev: Eldeki yeni delilleri Müdür Kemal'e göster.</color>\n";
                if(guncelIpucu == "") guncelIpucu = "İpucu: Bulduğun şüpheli notu ve defteri yanına alarak üst kattaki Müdür Kemal'le yüzleş.";
            }
        }

        // Aşama 6: Teknik Deliller (Tel ve Kanca)
        if (kemalPanikledi)
        {
            if (teknikDelillerAlindi) { }
            else
            {
                guncelMetin = "<color=purple>Görev: Kemal'in bahsettiği Kırık Vinç Teli'ni bul.</color>\n";
                if(guncelIpucu == "") guncelIpucu = "İpucu: Vinç dairesinin altındaki hurda deposuna ve sahil kenarındaki kırık tel parçalarına göz at.";
            }
        }

        // Aşama 7: Güvenlik Rıza Sorgusu ve USB Arama Aşamaları
        if (teknikDelillerAlindi)
        {
            if (rizaItirafEtti)
            {
                if (kameraKaydiBulundu) { }
                else
                {
                    guncelMetin = "<color=#00FF66>Görev: Rıza'nın bahsettiği trafoda gizlenen orijinal kamera kaydını (USB) bul! Trafo sesine dikkat et. </color>\n";
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
            if (finalHesaplasmaBitti)
            {
                guncelMetin = "<color=#FFD700><b>[FİNAL] Karar Anı: Suçluyu Seç!</b></color>\n";
                guncelIpucu = "İpucu: Masandaki dosyalara bakarak gerçek suçlunun kim olduğuna karar ver.";
            }
            else
            {
                guncelMetin = "<color=red><b>[FİNAL] Tüm deliller toplandı! Müdür Kemal ile son kez hesaplaş!</b></color>\n";
                guncelIpucu = "İpucu: Elindeki USB kamera kaydıyla Müdür Kemal'in odasına gir ve suçunu itiraf ettir!";
            }
        }

        gorevText.text = guncelMetin;
        if (ipucuText != null) ipucuText.text = guncelIpucu;

        // --- DİNAMİK MESAFE TAKİP TETİKLEYİCİLERİ ---
        if (HedefTakipci.Instance != null)
        {
            if (!kemalleKonusuldu) HedefTakipci.Instance.HedefDegistir(kemalObjesi, "Müdür Kemal");
            else if (kemalleKonusuldu && !ilacKutusuAlindi) HedefTakipci.Instance.HedefDegistir(null, ""); 
            else if (ilacKutusuAlindi && !ahmetleKonusuldu) HedefTakipci.Instance.HedefDegistir(ahmetObjesi, "İşçi Ahmet");
            else if (ahmetleKonusuldu && !odaVePanoIncelendi) HedefTakipci.Instance.HedefDegistir(odadakiDelilObjesi, "Müdürün Odası");
            else if (odaVePanoIncelendi && !kemalPanikledi) HedefTakipci.Instance.HedefDegistir(kemalObjesi, "Müdür Kemal");
            else if (teknikDelillerAlindi && !rizaItirafEtti) HedefTakipci.Instance.HedefDegistir(rizaObjesi, "Güvenlik Rıza");
            else if (kameraKaydiBulundu && !finalHesaplasmaBitti) HedefTakipci.Instance.HedefDegistir(kemalObjesi, "Müdür Kemal");
            else HedefTakipci.Instance.HedefDegistir(null, "");
        }
    }

    // YENİ: Kemal ile son hesaplaşma diyalogu bittiğinde çağrılacak fonksiyon
    public void FinalHesaplasmaTamamla()
    {
        finalHesaplasmaBitti = true;
        GoreviListele();

        // Delil yöneticisindeki o güzel animasyonlu görev bitti panelini tetikliyoruz
        if (DelilYoneticisi.Instance != null)
        {
            DelilYoneticisi.Instance.GorevTamamlandiPaneliAc();
        }
        else if (SecimYoneticisi.Instance != null)
        {
            // Eğer DelilYoneticisi sahneye bağlı değilse direkt seçim ekranını açar (Güvenlik Önlemi)
            SecimYoneticisi.Instance.SecimEkraniniAc();
        }
    }

    public void KemalGoreviniTamamla() { if (!kemalleKonusuldu) kemalleKonusuldu = true; GoreviListele(); }
    public void KemalPanikGoreviniTamamla() { kemalPanikledi = true; GoreviListele(); }
    public void RizaGoreviniTamamla() { rizaItirafEtti = true; GoreviListele(); }
    public void IlacKutusuBulundu() { ilacKutusuAlindi = true; GoreviListele(); }
    public void AhmetGoreviniTamamla() { ahmetleKonusuldu = true; GoreviListele(); }
    public void OdaVePanoTamamla() { odaVePanoIncelendi = true; GoreviListele(); }
    public void TeknikDelillerTamamla() { teknikDelillerAlindi = true; GoreviListele(); }
    public void KameraKaydiBulunduTamamla() { kameraKaydiBulundu = true; GoreviListele(); }
}