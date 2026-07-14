using UnityEngine;

public class OyunManager : MonoBehaviour
{
    // Singleton Tasarım Kalıbı: Diğer scriptlerin bu manager'a kolayca erişmesini sağlar.
    public static OyunManager Instance { get; private set; }

    [Header("Oyunun Genel Durumu")]
    public OyunSahneleri mevcutSahne = OyunSahneleri.Sahne1_Giris;
    public string aktifRota = ""; // "SirketYolsuzluk" veya "KisiselHusumet"
    public bool hareketEdebilirMi = false;

    [Header("Sahne 2 & 3 Sorgu Kilitleri")]
    public bool kemalleKonusuldu = false;
    public bool vincKancasiAlindi = false;
    public bool rizaSorgulandi = false;
    public bool ahmetSorgulandi = false;

    [Header("Delil Takibi")]
    public int toplananDelilSayisi = 0;
    public const int ToplamDelilIhtiyaci = 8;

    // Oyunun aşamalarını temsil eden Enum yapısı
    public enum OyunSahneleri
    {
        Sahne1_Giris,
        Sahne2_Sorgu,
        Sahne3_Kulube,
        Sahne4_Ofis,
        Sahne5_Secim,
        Sahne6_Pano,
        Sahne7_Final
    }

    private void Awake()
    {
        // Singleton kurulumu
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Sahneler arası geçişte bu obje silinmesin
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Oyuna giriş sinematiği ile başlıyoruz, karakter hareketi kilitli
        GecisYap(OyunSahneleri.Sahne1_Giris);
    }

    // Sahneler arası mantıksal geçişi yöneten ana fonksiyon
    public void GecisYap(OyunSahneleri yeniSahne)
    {
        mevcutSahne = yeniSahne;
        Debug.Log("Oyun Durumu Güncellendi: " + mevcutSahne.ToString());

        switch (mevcutSahne)
        {
            case OyunSahneleri.Sahne1_Giris:
                hareketEdebilirMi = false;
                // Burada giriş sinematiği ve ElevenLabs ses tetiklenecek
                break;

            case OyunSahneleri.Sahne2_Sorgu:
                hareketEdebilirMi = true;
                break;

            case OyunSahneleri.Sahne3_Kulube:
                // Güvenlik kulübesinin kapı kilidini açacak tetikleyici buraya bağlanacak
                break;

            case OyunSahneleri.Sahne4_Ofis:
                // Müdürün odasının kapı kilidini açacak tetikleyici buraya bağlanacak
                break;

            case OyunSahneleri.Sahne5_Secim:
                hareketEdebilirMi = false; // Seçim ekranında karakter dursun
                break;

            case OyunSahneleri.Sahne6_Pano:
                // Dedektif panosu arayüzü tetiklenecek
                break;

            case OyunSahneleri.Sahne7_Final:
                // Hava durumu değişecek, polis sirenleri hazırda bekleyecek
                break;
        }
    }

    // Delil toplandığında çağrılacak fonksiyon
    public void DelilEkle()
    {
        toplananDelilSayisi++;
        Debug.Log("Yeni delil toplandı! Toplam Delil: " + toplananDelilSayisi);

        if (toplananDelilSayisi >= ToplamDelilIhtiyaci)
        {
            GecisYap(OyunSahneleri.Sahne6_Pano);
        }
    }
}