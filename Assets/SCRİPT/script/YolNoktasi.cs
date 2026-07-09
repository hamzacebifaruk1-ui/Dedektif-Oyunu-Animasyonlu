using System.Collections.Generic;
using UnityEngine;

public class YolNoktasi : MonoBehaviour
{
    [Header("Bu Noktaya Doğrudan Bağlı Diğer Yol Noktaları")]
    public List<YolNoktasi> komsular = new List<YolNoktasi>();

    // Editörde yolları rahatça görebilmek için çizgiler çizer
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.5f);

        if (komsular == null) return;
        Gizmos.color = Color.green;
        foreach (var komsu in komsular)
        {
            if (komsu != null)
            {
                Gizmos.DrawLine(transform.position, komsu.transform.position);
            }
        }
    }
}