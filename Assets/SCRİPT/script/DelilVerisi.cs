using UnityEngine;

[CreateAssetMenu(fileName = "YeniDelil", menuName = "Dedektiflik/Delil Verisi")]
public class DelilVerisi : ScriptableObject
{
    public string delilAdi;
    [TextArea(3, 5)]
    public string notDefteriMetni; // TAB menüsünde ve inceleme ekranında yazacak yazı
   public bool gercekDelilMi; // Boşluğu kaldırdık ve Türkçe karakteri düzelttik
    public AudioClip dedektifIcSes; // ElevenLabs'ten indirdiğin ses dosyası
    public GameObject delilPrefab; // 3D döndürme ekranında dönecek olan klon obje
}