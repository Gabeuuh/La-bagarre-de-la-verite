using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public TMP_Text scoreText;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;  // Libère le curseur
        Cursor.visible = true;    
        int lastScore = PlayerPrefs.GetInt("LastScore", 0);
        scoreText.text = "Score : " + lastScore.ToString();
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
