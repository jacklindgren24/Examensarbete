using UnityEngine;
using UnityEngine.UI;

public class ScoreCounter : MonoBehaviour
{
    static int score;
    public static int Score
    {
        get { return score; }
        set
        {
            score = value;
            if (scoreText != null) scoreText.text = Score.ToString();
        }
    }

    static Text scoreText;

    void Awake()
    {
        Score = 0;
        scoreText = GetComponent<Text>();	
	}
}
