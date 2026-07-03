using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class IlkYukleme : MonoBehaviour
{
    [Header("UI Elemanları")]
    public Slider yuklemeBari;
    public TextMeshProUGUI yuzdeText;
    public Transform donenSimge;

    [Header("Yavaşlatma Ayarları")]
    [Range(2f, 10f)]
    public float dolumSuresi = 5f; // Barın dolması en az 5 saniye sürsün
    public float donmeHizi = 120f;

    void Start()
    {
        // Fareyi gizle
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Yükleme işlemini başlat
        StartCoroutine(ArkaPlandaYukle(1)); 
    }

    void Update()
    {
        // Simgeyi döndür
        if (donenSimge != null)
        {
            donenSimge.Rotate(Vector3.forward * -donmeHizi * Time.deltaTime);
        }
    }

    IEnumerator ArkaPlandaYukle(int sahneIndex)
    {
        // Sahneyi arkada hazırlamaya başla ama hemen açma
        AsyncOperation operasyon = SceneManager.LoadSceneAsync(sahneIndex);
        operasyon.allowSceneActivation = false; 

        float gecenSure = 0f;

        // Bar 0'dan 1'e (yani %100'e) belirlediğimiz dolumSuresi içinde gidecek
        while (gecenSure < dolumSuresi)
        {
            gecenSure += Time.deltaTime;
            
            // İlerlemeyi hesapla (0 ile 1 arası)
            float ilerleme = Mathf.Clamp01(gecenSure / dolumSuresi);

            // Görseli güncelle
            if (yuklemeBari != null) yuklemeBari.value = ilerleme;
            if (yuzdeText != null) yuzdeText.text = "% " + (ilerleme * 100f).ToString("F0");

            yield return null;
        }

        // Bar %100 olduktan sonra yarım saniye "Tamamlandı" hissi için bekle
        yield return new WaitForSeconds(0.5f);

        // Artık ana menüyü açabilirsin!
        operasyon.allowSceneActivation = true;
    }
}