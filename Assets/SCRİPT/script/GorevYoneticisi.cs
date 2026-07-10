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
    public GameObject odadakiDelilObjesi; 

    [Header("Görev Durumları")]
    public bool kemalleKonusuldu = false;
    public bool ilacKutusuAlindi = false; 
    public bool ahmetleKonusuldu = false; 
    public bool odaVePanoIncelendi = false; 
    public bool kemalPanikledi = false;    
    public bool teknikDelillerAlindi = false; 
    public bool rizaItirafEtti = false;
    public bool kameraKaydiBulundu = false; 
    public bool finalHesaplasmaBitti = false; 

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

        // Aşama 1: Kemal İlk Konuşma (Açık hedef yok)
        if (!kemalleKonusuldu)
        {
            guncelMetin = "<color=white>Soruşturma Başladı: Olay yerindeki en yetkili isimden ilk bilgileri al.</color>\n";
            guncelIpucu = "Düşünce: Şantiyenin idari işlerinden sorumlu kişiyle görüşmeliyim. Odası buralarda bir yerde olmalı.";
        }
        // Aşama 2: İlaç Kutusu Arama (Yer söylenmiyor, vinç etrafı ima ediliyor)
        else if (kemalleKonusuldu && !ilacKutusuAlindi)
        {
            guncelMetin = "<color=yellow>Şüphe: Olay yerinde gözden kaçan fiziksel bir iz veya kalıntı var mı? Çevreyi tara.</color>\n";
            guncelIpucu = "Düşünce: Kazanın meydana geldiği o devasa vincin yakınlarında şüpheli bir şeyler kalmış olabilir.";
        }
        // Aşama 3: Ahmet Sorgusu (Ahmet denmiyor, görgü tanığı deniyor)
        else if (ilacKutusuAlindi && !ahmetleKonusuldu)
        {
            guncelMetin = "<color=orange>Sorgu: Bulunan tıbbi malzemeyi şantiyede çalışan işçilere sor.</color>\n";
            guncelIpucu = "Düşünce: Kazayı en yakından gören, dinlenme alanlarındaki işçilerden biri bu kutunun kime ait olduğunu bilebilir.";
        }
        // Aşama 4: Oda ve Pano Inceleme (Gizlice girme vurgusu)
        else if (ahmetleKonusuldu && !odaVePanoIncelendi)
        {
            guncelMetin = "<color=green>Arama: Resmi ifadeler çelişiyor. Yönetim ofisindeki belgeleri gizlice incele.</color>\n";
            guncelIpucu = "Düşünce: Ofisteki panoda ve masanın üzerinde saklanan evraklar resmi rapordan daha fazlasını anlatıyor olabilir.";
        }
        // Aşama 5: Kemal Panik / Köşeye Sıkıştırma
        else if (odaVePanoIncelendi && !kemalPanikledi)
        {
            guncelMetin = "<color=red>Yüzleşme: Belgelerdeki çelişkileri idareye sun ve tepkisini ölç.</color>\n";
            guncelIpucu = "Düşünce: Bulduğum notları müdüre gösterdiğimde vereceği tepki, suçluluk psikolojisini ele verecektir.";
        }
        // Aşama 6: Teknik Deliller (Tel ve Kanca)
        else if (kemalPanikledi && !teknikDelillerAlindi)
        {
            guncelMetin = "<color=purple>Analiz: Kazaya sebep olan mekanik parçayı (Kırık Vinç Teli) bul ve incele.</color>\n";
            guncelIpucu = "Düşünce: Metal yorgunluğu mu yoksa sabotaj mı? Parçalar sahil kenarına ya da hurdalığa atılmış olmalı.";
        }
        // Aşama 7: Güvenlik Rıza Sorgusu ve USB Arama Aşamaları
        else if (teknikDelillerAlindi && !kameraKaydiBulundu)
        {
            if (!rizaItirafEtti)
            {
                guncelMetin = "<color=lightblue>Sorgu: Olay gecesi çevre güvenliğinden sorumlu olan personeli sıkıştır.</color>\n";
                guncelIpucu = "Düşünce: Giriş kapısındaki kulübede duran görevli, o gece kimlerin şantiyeye sızdığını kesinlikle biliyor.";
            }
            else
            {
                guncelMetin = "<color=#00FF66>Arama: Saklanan gerçeğin peşine düş. Gizlenen dijital kaydı (USB) bul.</color>\n";
                guncelIpucu = "Düşünce: Şantiyenin dış sınırındaki o yüksek gerilim trafosunun etrafından tuhaf sesler geliyor, orayı kurcalamalıyım.";
            }
        }
        // Oyun Sonu Final Kontrolü
        else if (kameraKaydiBulundu)
        {
            if (!finalHesaplasmaBitti)
            {
                guncelMetin = "<color=red><b>[HESAPLAŞMA] Elindeki tüm kartları masaya dökme vakti.</b></color>\n";
                guncelIpucu = "Düşünce: Tüm veriler toplandı. İdari ofise geri dönüp son bir görüşme yapma zamanı.";
            }
            else
            {
                guncelMetin = "<color=#FFD700><b>[FİNAL] Karar Anı: Suçlu Kim?</b></color>\n";
                guncelIpucu = "Düşünce: Dedektif masasına dön. Elindeki delillere göre faili seç. Geri dönüşü yok!";
            }
        }

        gorevText.text = guncelMetin;
        if (ipucuText != null) ipucuText.text = guncelIpucu;

        // --- HEDEF TAKİPÇİ DEĞİŞİKLİĞİ (Artık pusula oyuncuyu direkt adama götürmeyecek, serbest arayacak!) ---
        if (HedefTakipci.Instance != null)
        {
            // Oyuncu tamamen kaybolmasın diye sadece ilk görevde yön gösteriyoruz, sonrasında pusulayı kapatıyoruz!
            if (!kemalleKonusuldu) HedefTakipci.Instance.HedefDegistir(kemalObjesi, "İdari Ofis");
            else HedefTakipci.Instance.HedefDegistir(null, ""); // Diğer tüm aşamalarda hedef boş kalıyor, oyuncu kendi bulacak!
        }
    }

    public void FinalHesaplasmaTamamla()
    {
        finalHesaplasmaBitti = true;
        GoreviListele();

        if (DelilYoneticisi.Instance != null)
        {
            DelilYoneticisi.Instance.GorevTamamlandiPaneliAc();
        }
        else if (SecimYoneticisi.Instance != null)
        {
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