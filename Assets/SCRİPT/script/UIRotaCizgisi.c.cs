using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class UIRotaCizgisi : MaskableGraphic
{
    private List<Vector2> noktalar = new List<Vector2>();

    [Header("Çizgi Ayarları")]
    public float kalinlik = 8f;

    public void RotayiGuncelle(List<Vector2> yeniNoktalar)
    {
        noktalar = yeniNoktalar;
        SetVerticesDirty();
    }

    public void RotayiTemizle()
    {
        noktalar.Clear();
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        if (noktalar == null || noktalar.Count < 2) return;

        // Çizgi segmentlerini oluştur
        for (int i = 0; i < noktalar.Count - 1; i++)
        {
            Vector2 p1 = noktalar[i];
            Vector2 p2 = noktalar[i + 1];
            Vector2 yon = (p2 - p1).normalized;
            Vector2 normal = new Vector2(-yon.y, yon.x) * (kalinlik / 2f);

            int idx = vh.currentVertCount;

            vh.AddVert(p1 - normal, color, Vector2.zero);
            vh.AddVert(p1 + normal, color, Vector2.zero);
            vh.AddVert(p2 + normal, color, Vector2.zero);
            vh.AddVert(p2 - normal, color, Vector2.zero);

            vh.AddTriangle(idx, idx + 1, idx + 2);
            vh.AddTriangle(idx, idx + 2, idx + 3);
        }

        // Köşe bağlantı noktalarını kare dolgularla kapat (Joint)
        float yaricap = kalinlik / 2f;
        for (int i = 1; i < noktalar.Count - 1; i++)
        {
            Vector2 merkez = noktalar[i];
            int idx = vh.currentVertCount;

            vh.AddVert(merkez + new Vector2(-yaricap, -yaricap), color, Vector2.zero);
            vh.AddVert(merkez + new Vector2(-yaricap, yaricap), color, Vector2.zero);
            vh.AddVert(merkez + new Vector2(yaricap, yaricap), color, Vector2.zero);
            vh.AddVert(merkez + new Vector2(yaricap, -yaricap), color, Vector2.zero);

            vh.AddTriangle(idx, idx + 1, idx + 2);
            vh.AddTriangle(idx, idx + 2, idx + 3);
        }
    }
}