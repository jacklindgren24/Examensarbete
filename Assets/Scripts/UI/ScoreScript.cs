using UnityEngine;
using UnityEngine.UI;

public class ScoreScript : MonoBehaviour
{
    static int score = 0;
    public static int Score
    {
        get { return score; }
        set
        {
            score = value;
            scoreText.text = Score.ToString();
        }
    }

    static Text scoreText;

    void Awake()
    {
        scoreText = GetComponent<Text>();	
	}
}
