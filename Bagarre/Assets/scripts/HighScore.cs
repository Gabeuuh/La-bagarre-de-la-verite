using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager instance;

    public TMP_Text highScoreText;

    private List<int> highScores = new List<int>();
    private int maxEntries = 5;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

void Start()
{
    int lastScore = PlayerPrefs.GetInt("LastScore", 0);
    if (lastScore > 0)
    {
        AddScore(lastScore);
        PlayerPrefs.DeleteKey("LastScore");
    }
    else
    {
        LoadScores();
    }

    DisplayScores();
}

    public void AddScore(int newScore)
    {
        LoadScores(); // important : toujours charger avant d'ajouter
        highScores.Add(newScore);
        highScores.Sort((a, b) => b.CompareTo(a)); // tri décroissant

        if (highScores.Count > maxEntries)
            highScores.RemoveAt(highScores.Count - 1);

        SaveScores();
    }

    void SaveScores()
    {
        for (int i = 0; i < highScores.Count; i++)
        {
            PlayerPrefs.SetInt("HighScore" + i, highScores[i]);
        }
        PlayerPrefs.Save();
    }

    void LoadScores()
    {
        highScores.Clear();
        for (int i = 0; i < maxEntries; i++)
        {
            if (PlayerPrefs.HasKey("HighScore" + i))
                highScores.Add(PlayerPrefs.GetInt("HighScore" + i));
        }
    }

    void DisplayScores()
    {
        if (highScoreText == null) return;

        highScoreText.text = "Meilleurs Scores :\n";
        for (int i = 0; i < highScores.Count; i++)
        {
            highScoreText.text += $"{i + 1}. {highScores[i]}\n";
        }
    }
}
