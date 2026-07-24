# Dedektif-Oyunu-Animasyonlu
<div align="center">

# 🕵️‍♂️ DEDEKTİF OYUNU
### *Karadeniz Limanı Cinayeti*

**Gerçeği bulmak için tek bir gecen var.**

<br>

![Unity](https://img.shields.io/badge/Unity-6000.2.9f1-000000?style=for-the-badge&logo=unity&logoColor=white)
![URP](https://img.shields.io/badge/Render-URP%2017.2-1B7AC7?style=for-the-badge)
![C#](https://img.shields.io/badge/C%23-.NET-239120?style=for-the-badge&logo=csharp&logoColor=white)
![Platform](https://img.shields.io/badge/Platform-Windows-0078D6?style=for-the-badge&logo=windows&logoColor=white)
![Dil](https://img.shields.io/badge/Dil-Türkçe-E30A17?style=for-the-badge)
![Versiyon](https://img.shields.io/badge/Sürüm-V__5.7-orange?style=for-the-badge)

<br>

*Sisli bir liman. Yalan söyleyen üç şüpheli. Sekiz delil — dördü sahte.*

---

</div>

<br>

## 📖 Hikâye

> **14 Kasım. Gece 02:30.**
> **Karadeniz Limanı — 3 Numaralı Yükleme İskelesi.**
>
> Vinç operatörü **Murat Çelik**, 18 metre yüksekten düşerek can verdi.
> Polis raporu *"ekipman arızası ve işçi ihmali"* dedi. Dosya kapatıldı.
>
> Ama bu sabah dedektiflik ofisinin telefonu acı acı çaldı.
> Murat'ın eşi ağlayarak konuşuyordu:
>
> *"Kocam tehdit ediliyordu dedektif. Onu o vince zorla çıkardılar, lütfen yardım edin!"*
>
> Sen bu şehrin en karanlık gizemlerini çözen dedektifsin.
> **Andın olsun ki, bu gece şantiyede gerçeği bulmak için tek bir şansın var.**

<br>

## ✨ Öne Çıkan Özellikler

<table>
<tr>
<td width="50%" valign="top">

### 🔍 Delil Toplama & İnceleme
Delilleri yerden al, elinde **3D olarak döndür**, yakınlaştır ve detayları incele. Her nesne bir hikâye anlatıyor.

</td>
<td width="50%" valign="top">

### 🧩 Gerçek / Sahte Tasnif Sistemi
Topladığın **8 delilin 4'ü sahte**. Katil seni yanlış yöne sürüklemek için tuzaklar bıraktı. Panoda doğru etiketle.

</td>
</tr>
<tr>
<td width="50%" valign="top">

### 🎙️ Seslendirilmiş Diyaloglar
NPC'lerle daktilo efektli diyalog sistemi + **ElevenLabs** ile üretilmiş Türkçe seslendirme ve dedektifin iç sesi.

</td>
<td width="50%" valign="top">

### ⚖️ Sonucu Değiştiren Seçimler
Ahmet ile yüzleşmede **iki delilden yalnızca birini** alabilirsin. Seçmediğin delil sonsuza dek yok olur.

</td>
</tr>
<tr>
<td width="50%" valign="top">

### 🗺️ Harita & Rota Sistemi
`M` ile açılan büyük harita, canlı minimap, hedefe çizilen rota çizgisi ve ekran kenarında hedef takipçisi.

</td>
<td width="50%" valign="top">

### 🌒 Atmosferik Gece Limanı
Titreyen sokak lambaları, varil ateşi, jeneratör uğultusu, gerçek liman ambiyans sesleri ve el feneri mekaniği.

</td>
</tr>
<tr>
<td width="50%" valign="top">

### 📓 Dedektif Not Defteri
`TAB` ile açılan not defteri; topladığın her delilin detaylı analizini ve bağlantılarını tutar.

</td>
<td width="50%" valign="top">

### 🎬 Sinematik Giriş
Oyun, yazı yazma efektli ve seslendirilmiş bir açılış sinematiğiyle başlar. `SPACE` ile geçilebilir.

</td>
</tr>
</table>

<br>

## 🎯 Görev Akışı

Oyun **10 aşamalı** bir senaryo motoru (`GorevYoneticisi`) üzerinden ilerler:

```
┌─────────────────────────────────────────────────────────────┐
│  1️⃣  Giriş Sinematiği      Limana varış, dosyanın açılışı   │
├─────────────────────────────────────────────────────────────┤
│  2️⃣  Rıza Sorgusu          Güvenlik görevlisinin ifadesi     │
│  3️⃣  Kemal Sorgusu         Liman müdürünün ifadesi           │
│  4️⃣  Ahmet Sorgusu         İşçinin ifadesi                   │
├─────────────────────────────────────────────────────────────┤
│  5️⃣  Kulübe Araması        2 delil  ·  Rıza'nın yalanı       │
│  6️⃣  Ofis Araması          3 delil  ·  Kemal'in sırları      │
├─────────────────────────────────────────────────────────────┤
│  7️⃣  Ahmet Yüzleşmesi      ⚠️  Geri dönüşü olmayan seçim     │
│  8️⃣  Son Arama             4 delil  ·  Lambaları takip et    │
├─────────────────────────────────────────────────────────────┤
│  9️⃣  Delil Tasnifi         GERÇEK ✅ / SAHTE ❌ etiketle     │
│  🔟  Final Suçlama         Tek şans. Doğru kişiyi tutukla.   │
└─────────────────────────────────────────────────────────────┘
```

> 💡 **İpucu sistemi:** Bir aşamada takılırsan, oyun belli bir süre sonra otomatik olarak ipucu metni gösterir.

<br>

## 👥 Şüpheliler

| | Karakter | Rol | Şüphe Sebebi |
|:---:|:---|:---|:---|
| 🛡️ | **Güvenlik Rıza** | Kulübe görevlisi | *"O gece elektrikler kesildi"* diyor — ama sistem logları başka şey söylüyor. |
| 💼 | **Liman Müdürü Kemal** | Şantiye yöneticisi | Mali kayıtlarda boşluklar, kasada saklanan evraklar, kızıyla ilgili bir sır. |
| 🔧 | **İşçi Ahmet** | Vinç ekibi | Murat'ın dostuydu. Bir şeyler biliyor ama konuşmaktan korkuyor. |

<br>

## 🗂️ Delil Dosyası

Topladığın 8 delilin **yarısı katilin bıraktığı tuzak**. Panoda yanlış etiketlersen dosyayı çökertirsin.

| # | Delil | Nerede Bulunur | Ne Anlatıyor |
|:--:|:---|:---|:---|
| 01 | 💾 **USB Bellek** | Güvenlik kulübesi | Rıza'nın *"elektrik kesildi"* yalanını çürüten sistem logları |
| 02 | 🖼️ **Yırtık Kadın Fotoğrafı** | Müdür odası | Murat ile Kemal'in kızı — cinayetin kişisel motifi |
| 03 | ⚙️ **Spiral Taşlama Makinesi** | Şantiye sahası | Vinç vidalarını kasten zayıflatmak için kullanılmış alet |
| 04 | 🚗 **Çamurlu Lastik İzi** | Şantiye girişi | Olay gecesi giren lüks araç — Kemal'in aracıyla birebir uyuşuyor |
| 05 | 📕 **Yırtık Bakım Defteri** | Kulübe çevresi | Sayfalar bilerek yırtılmış — dikkat dağıtıcı |
| 06 | 💊 **Boş İlaç Şişesi** | Ofis dolabı | İntihar süsü vermek için bırakılmış sahte sakinleştirici |
| 07 | 🪢 **Kırık Vinç Teli** | Vinç altı | Kesilmiş gibi görünüyor ama gerçek kanıt değil |
| 08 | ⛑️ **Kirlenmiş Baret** | Olay yerinden uzakta | Dedektife vakit kaybettirmek için bırakılmış sahte iz |

**Ek belgeler** — senaryo dallarına göre yalnızca biri elinize geçer:

- 📜 **Murat'ın Gizli Mektubu** — Ahmet'in dolabında. Kemal ile kızı arasındaki ilişkiyi ve tehditleri kanıtlıyor.
- 🧾 **Zimmet Kayıt Belgesi** — Jeneratör odasında. Kemal'in sahte faturalarla bütçeyi boşalttığını kesinleştiriyor.

<br>

## 🎮 Kontroller

<table>
<tr><th align="left">Hareket</th><th align="left">Tuş</th></tr>
<tr><td>Yürüme</td><td><kbd>W</kbd> <kbd>A</kbd> <kbd>S</kbd> <kbd>D</kbd></td></tr>
<tr><td>Koşma</td><td><kbd>Shift</kbd> + Yön</td></tr>
<tr><td>Zıplama</td><td><kbd>Space</kbd></td></tr>
<tr><td>Kamera</td><td>🖱️ Fare hareketi</td></tr>
</table>

<table>
<tr><th align="left">Etkileşim</th><th align="left">Tuş</th></tr>
<tr><td>Delil al / NPC ile konuş / Kapı aç</td><td><kbd>E</kbd></td></tr>
<tr><td>İnceleme moduna gir / çık</td><td><kbd>F</kbd></td></tr>
<tr><td>İncelenen nesneyi döndür</td><td>🖱️ Sol tık + sürükle</td></tr>
<tr><td>Yakınlaştır / uzaklaştır</td><td>🖱️ Scroll</td></tr>
<tr><td>El feneri</td><td><kbd>L</kbd></td></tr>
</table>

<table>
<tr><th align="left">Arayüz</th><th align="left">Tuş</th></tr>
<tr><td>Büyük harita</td><td><kbd>M</kbd></td></tr>
<tr><td>Not defteri</td><td><kbd>TAB</kbd></td></tr>
<tr><td>Delil / Tasnif panosu</td><td><kbd>I</kbd></td></tr>
<tr><td>Diyaloğu hızlı geç</td><td><kbd>Shift</kbd> + <kbd>Enter</kbd></td></tr>
<tr><td>Sinematiği geç</td><td><kbd>Space</kbd> / <kbd>Enter</kbd></td></tr>
<tr><td>Duraklat / Menü</td><td><kbd>Esc</kbd></td></tr>
</table>

<br>

## 🚀 Kurulum

### Gereksinimler

- **Unity 6000.2.9f1** (Unity 6) — farklı sürümler asset uyumsuzluğu yaratabilir
- **Git** (repo ~3 GB'tır, indirme uzun sürebilir)
- ~8 GB boş disk alanı (Library klasörü dahil)

### Adımlar

```bash
# 1 — Depoyu klonla
git clone https://github.com/hamzacebifaruk1-ui/Dedektif-Oyunu-Animasyonlu.git
cd Dedektif-Oyunu-Animasyonlu

# 2 — Unity Hub → Add → bu klasörü seç
# 3 — Unity 6000.2.9f1 ile aç (ilk import 10-20 dk sürebilir)
# 4 — Assets/Scenes/YuklemeEkrani.unity sahnesini aç ve Play'e bas
```

> ⚠️ İlk açılışta Unity tüm asset'leri yeniden import eder. Sabırlı ol — arka planda 12.000'den fazla dosya işleniyor.

<br>

## 🗺️ Sahne Yapısı

Build Settings sırasıyla:

| # | Sahne | Açıklama |
|:--:|:---|:---|
| `0` | **YuklemeEkrani** | Açılış yükleme ekranı (async loading) |
| `1` | **AnaMenu** | Ana menü — Başla / Ayarlar / Çıkış |
| `2` | **GirisSahnesi** | Sinematik giriş, hikâyenin anlatımı |
| `3` | **SampleScene** | 🎯 **Ana oyun sahnesi** — Karadeniz Limanı |
| `4` | **AyarlarSahnesi** | Ses, grafik ve kontrol ayarları |

<br>

## 🏗️ Teknik Mimari

Tüm oyun kodu `Assets/SCRİPT/` altında, **~5.200 satır C#** ve **39 script**.

```
Assets/SCRİPT/
│
├── 🎬 Oyun Akışı
│   ├── OyunManager.cs           # Singleton · 7 aşamalı global durum makinesi
│   ├── GorevYoneticisi.cs       # 🌟 Ana senaryo motoru (458 satır) · 10 aşama
│   ├── SecimYoneticisi.cs       # Dallanan senaryo kararları
│   └── SinematikGiris.cs        # Yazı efektli açılış sinematiği
│
├── 🔍 Delil Sistemi
│   ├── DelilYoneticisi.cs       # Merkezi delil kaydı ve envanteri
│   ├── DelilTasnifPanosu.cs     # 🌟 Gerçek/Sahte analiz panosu (354 satır)
│   ├── DelilIncelemeSistemi.cs  # 3D döndür-yakınlaştır inceleme modu
│   ├── DelilPanosuSistemi.cs    # Delil kartları arayüzü
│   ├── DelilNesnesi.cs          # Sahnedeki toplanabilir delil bileşeni
│   ├── DelilVerisi.cs           # Delil veri modeli
│   ├── DelilKartiUI.cs          # Kart görselleştirme
│   └── GizliDelilObje.cs        # Saklı deliller (ışık ipuçlarıyla bulunur)
│
├── 💬 NPC & Diyalog
│   ├── DiyalogYoneticisi.cs     # 🌟 Daktilo efekti + ses senkronu (270 satır)
│   ├── NpcDiyalog.cs            # NPC'ye bağlı diyalog verisi
│   ├── NpcEtkilesim.cs          # Yakınlık algılama ve etkileşim
│   ├── NPCGorevSistemi.cs       # NPC bazlı görev tetikleyicileri
│   └── NpcIsimlik.cs            # Kafa üstü isim etiketi
│
├── 🏃 Oyuncu
│   ├── hareket.cs               # CharacterController · yürü/koş/zıpla/çömel
│   ├── YeniKamera.cs            # 3. şahıs orbital kamera
│   ├── OyuncuInteraksiyon.cs    # Raycast tabanlı etkileşim sistemi
│   ├── ElFeneriController.cs    # El feneri + pil/açı kontrolü
│   └── YurumesSesi.cs           # Zemin bazlı ayak sesi sistemi
│
├── 🖥️ Arayüz & Navigasyon
│   ├── BuyukHaritaYonetici.cs   # Tam ekran harita (M)
│   ├── MinimapYonetici.cs       # Canlı minimap
│   ├── HaritaRotaCizici.cs      # Harita üstü rota çizimi
│   ├── UIRotaCizgisi.c.cs       # Dünya-uzayı rota çizgisi
│   ├── HedefTakipci.cs          # Ekran kenarı hedef göstergesi
│   ├── NotDefteriYoneticisi.cs  # Dedektif not defteri (TAB)
│   ├── OyunDuraklatma.cs        # Duraklatma menüsü (Esc)
│   ├── AnaMenuYoneticisi.cs     # Ana menü yönlendirmesi
│   ├── MenuButtonEffects.cs     # Buton hover/tıklama efektleri
│   └── IlkYukleme.cs            # Async sahne yükleme ekranı
│
├── 🌫️ Atmosfer
│   ├── FlickeringLight.cs       # Titreyen sokak lambaları (ipucu sistemi)
│   ├── AtesTitresimi.cs         # Varil ateşi ışık animasyonu
│   ├── OrtamSesleri.cs          # Liman ambiyans döngüsü
│   ├── TrafoSesKontrol.cs       # Jeneratör/trafo uğultusu
│   ├── KapiKONTROL.cs           # Etkileşimli kapılar
│   ├── YanipSonen.cs            # Genel yanıp sönme efekti
│   └── YolNoktasi.cs            # Waypoint işaretçileri
│
└── ⚖️ Final
    ├── AhmetSecimPaneli.cs      # Geri dönüşsüz delil seçimi
    ├── AhmetYuzlesmePaneli.cs   # Yüzleşme diyalog arayüzü
    ├── AhmetSecimKontrol.cs     # Seçim sonucu doğrulama
    └── FinalSuclamaSistemi.cs   # Suçlama · Tebrikler / Tekrar Dene
```

### Tasarım Notları

- **Singleton mimarisi** — `OyunManager`, `GorevYoneticisi`, `DiyalogYoneticisi`, `DelilTasnifPanosu` ve `FinalSuclamaSistemi` `DontDestroyOnLoad` ile sahneler arası taşınır.
- **Yeni Input System** (`com.unity.inputsystem` 1.14.2) — tüm girdi `Keyboard.current` üzerinden okunur, eski Input Manager kullanılmaz.
- **Çift tetikleme koruması** — `GorevYoneticisi` içindeki `HashSet<string>` sayesinde aynı delil iki kez sayılmaz.
- **Sahne bağımsız referans bulma** — `SceneManager.sceneLoaded` event'i ile UI referansları her sahnede yeniden bağlanır.
- **Coroutine tabanlı akış** — sinematikler, ipucu zamanlayıcıları ve ses senkronu coroutine'lerle yönetilir.

<br>

## 📦 Kullanılan Paketler & Asset'ler

**Unity Paketleri**

`Universal RP 17.2` · `AI Navigation 2.0.9` · `Input System 1.14.2` · `Timeline 1.8.9` · `Post Processing 3.5.0` · `TextMesh Pro` · `Visual Scripting 1.9.8`

**Üçüncü Parti Asset'ler**

| Asset | Kullanım |
|:---|:---|
| **Gentleland — Steampunk UI** | Dedektif temalı arayüz elemanları |
| **SlimUI — Modern Menu 1** | Ana menü ve ayarlar arayüzü |
| **SoftTouch UI** | Yumuşak geçişli UI bileşenleri |
| **TirgamesAssets — Factory** | Şantiye ve fabrika yapıları |
| **AllSky Free** | Gece gökyüzü skybox'ı |
| **Rowlan Fullscreen** | Editör tam ekran oynatma aracı |
| **FolderColor** | Editör klasör renklendirme |

**Ses**

Ambiyans kayıtları (liman gece atmosferi), fon müziği ve **ElevenLabs** ile üretilmiş Türkçe karakter seslendirmeleri.

<br>

## 🔮 Bilinen Sorunlar & Yol Haritası

- [ ] `GorevYoneticisi.cs:438` — placeholder sahne adı (`"AnaMenuSahnendekiAd"`) düzeltilmeli
- [ ] `AhmetSecimKontrol.cs,.cs` — dosya adındaki virgül hatası
- [ ] Büyük binary asset'ler için **Git LFS** entegrasyonu (repo şu an ~3 GB)
- [ ] `Assets/_Recovery/` ve kök dizindeki artık dosyaların temizliği
- [ ] Yinelenen el feneri script'lerinin (`ElFeneriController` / `ElFeneriKontrol`) birleştirilmesi
- [ ] Kaydetme / yükleme sistemi
- [ ] Ek senaryo dalları ve alternatif sonlar

<br>

## 🎭 Spoiler Bölgesi

<details>
<summary><b>⚠️ Katilin kim olduğunu öğrenmek için tıkla</b></summary>

<br>

**Gerçek suçlu: Liman Müdürü Kemal ("Kel")**

Kemal, şirket bütçesini sahte faturalarla boşaltıyor ve kalitesiz ekipman alıyordu. Murat bunu fark etti. Üstüne bir de Kemal'in kızıyla ilişkisi vardı — bu kişisel husumeti tetikledi.

Kemal, vinç telini **spiral taşlama makinesiyle** kesip zayıflattı, olay gecesi lüks aracıyla şantiyeye girdi (**çamurlu lastik izi**), Murat'ı vince çıkmaya zorladı ve ardından intihar süsü vermek için **boş ilaç şişesi** bıraktı.

Rıza'nın *"elektrik kesildi"* yalanı ise korkudan söylenmiş bir örtbastı — **USB bellekteki loglar** bunu çürütüyor.

**Doğru cevap için gereken 4 gerçek delil:**
USB Bellek · Yırtık Kadın Fotoğrafı · Spiral Taşlama Makinesi · Çamurlu Lastik İzi

**Katilin bıraktığı 4 sahte iz:**
Yırtık Bakım Defteri · Boş İlaç Şişesi · Kırık Vinç Teli · Kirlenmiş Baret

</details>

<br>

---

<div align="center">

### 🕯️ *"Andım olsun ki, bu gece gerçeği bulacağım."*

**Geliştirici:** [@hamzacebifaruk1-ui](https://github.com/hamzacebifaruk1-ui)

<sub>Unity 6 ile ❤️ ve ☕ kullanılarak geliştirildi.</sub>

<br>

⭐ Beğendiysen yıldız vermeyi unutma!

</div>
