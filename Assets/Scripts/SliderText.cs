using UnityEngine;
using UnityEngine.UI;

public class SliderText : MonoBehaviour {

    Slider slider;
    Text text;

	void Awake()
    {
        text = GetComponent<Text>();

        Transform parent = transform;
        while (slider == null)
        {
            parent = parent.parent;
            slider = parent.GetComponent<Slider>();
        }
	}
	
	void LateUpdate()
    {
        text.text = slider.value.ToString();
        if (text.text.Length > 4) text.text = text.text.Remove(3);
	}
}
