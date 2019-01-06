using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour {

    PlayerController pc;
    Text text;
    RectTransform rt;

	void Start ()
    {
        pc = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        text = GetComponentInChildren<Text>();
        rt = GetComponent<RectTransform>();
	}
	
    public void UpdateHealthBar()
    {
        text.text = pc.Health.ToString();
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, pc.Health * (rt.rect.width / 100));
    }
}
