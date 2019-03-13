using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HitScript : MonoBehaviour {

    public Image hitMarker;
    public float markerDuration = 0.33f;

    Color baseMarkerColor;

    void Awake()
    {
        baseMarkerColor = hitMarker.color;
        hitMarker.color = new Color(hitMarker.color.r, hitMarker.color.g, hitMarker.color.b, 0);
    }

    public void HitMarker()
    {
        hitMarker.color = baseMarkerColor;
        StopAllCoroutines();
        StartCoroutine(Hit());
    }

    IEnumerator Hit()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / markerDuration;

            Color newColor = new Color(hitMarker.color.r, hitMarker.color.g, hitMarker.color.b, Mathf.Lerp(hitMarker.color.a, 0, t));
            hitMarker.color = newColor;

            yield return null;
        }
    }
}
