using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public int score = 0;
    public TMP_Text scoreText;

    public void SaveScore()
{
    PlayerPrefs.SetInt("LastScore", score);
    PlayerPrefs.Save();
}

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void AddPoint()
    {
        score+= 10;
        scoreText.text = "" + score;
    }
}
