using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour {

    PlayerController pc;
    Text text;
    RectTransform rt;
    float baseWidth;

	void Start ()
    {
        pc = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        text = GetComponentInChildren<Text>();
        rt = GetComponent<RectTransform>();
        baseWidth = rt.rect.width;
	}
	
    public void UpdateHealthBar()
    {
        if (pc != null)
        {
            text.text = pc.Health.ToString();
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, baseWidth * ((float)pc.Health / pc.baseHealth));
        }
    }
}
