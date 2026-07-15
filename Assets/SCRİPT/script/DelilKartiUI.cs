using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DelilKartiUI : MonoBehaviour
{
    [Header("UI Bileşenleri")]
    public TextMeshProUGUI delilAdiText;
    public Image delilResmi;
    
    [Header("Butonlar")]
    public Button gercekButonu;
    public Button sahteButonu;

    [Header("Buton Renkleri (Görsel Geri Bildirim)")]
    public Color secilmemisRenk = Color.white;
    public Color gercekRenk = Color.green;
    public Color sahteRenk = Color.red;

    private string bagliDelilAdi;
    private string oyuncuKarari = "Seçilmedi"; // "Gerçek", "Sahte" veya "Seçilmedi" olacak

    // ⚡ KART İLK OLUŞTURULDUĞUNDA ÇALIŞACAK KURULUM FONKSİYONU
    public void KartKurulumu(string delilIsmi)
    {
        bagliDelilAdi = delilIsmi;
        delilAdiText.text = delilIsmi;

        // Klasörden delil ismine uygun bir resmi otomatik yüklemeyi deneyelim
        // Resources/DelilResimleri/ klasöründe delil ismiyle aynı isimde görseller olmalı
        Sprite delilSprite = Resources.Load<Sprite>("DelilResimleri/" + delilIsmi);
        if (delilSprite != null)
        {
            delilResmi.sprite = delilSprite;
        }
        else
        {
            Debug.LogWarning($"[PANO] Resources/DelilResimleri/{delilIsmi} konumunda resim bulunamadı!");
        }

        // Butonların tıklama olaylarını (Listener) bağlıyoruz
        gercekButonu.onClick.RemoveAllListeners();
        gercekButonu.onClick.AddListener(GercekSecildi);

        sahteButonu.onClick.RemoveAllListeners();
        sahteButonu.onClick.AddListener(SahteSecildi);

        RenklendirmeyiGuncelle();
    }

    private void GercekSecildi()
    {
        oyuncuKarari = "Gerçek";
        Debug.Log($"[TASNİF] {bagliDelilAdi} oyuncu tarafından GERÇEK olarak etiketlendi.");
        RenklendirmeyiGuncelle();

        // 💡 Burada istersen kararları ana veri tabanına/görev yöneticisine de bildirebilirsin:
        // DelilKararKaydet(bagliDelilAdi, oyuncuKarari);
    }

    private void SahteSecildi()
    {
        oyuncuKarari = "Sahte";
        Debug.Log($"[TASNİF] {bagliDelilAdi} oyuncu tarafından SAHTE olarak etiketlendi.");
        RenklendirmeyiGuncelle();
    }

    // Oyuncu hangi butona bastıysa o butonun rengini değiştirip görsel geri bildirim veriyoruz
    private void RenklendirmeyiGuncelle()
    {
        if (oyuncuKarari == "Gerçek")
        {
            gercekButonu.image.color = gercekRenk;
            sahteButonu.image.color = secilmemisRenk;
        }
        else if (oyuncuKarari == "Sahte")
        {
            gercekButonu.image.color = secilmemisRenk;
            sahteButonu.image.color = sahteRenk;
        }
        else
        {
            gercekButonu.image.color = secilmemisRenk;
            sahteButonu.image.color = secilmemisRenk;
        }
    }

    // Kararı dışarıdan sorgulamak istersek (Örn: Final suçlamada doğru tasnif yaptı mı diye bakarken)
    public string GetOyuncuKarari()
    {
        return oyuncuKarari;
    }
}