using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    void Start()
    {
        // Enregistre la scène actuelle comme "LastScene"
        PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
    }
    public void StartGame()
    {
        SceneManager.LoadScene("Jeu"); // ⚠️ remplace par le nom réel de ta scène de jeu
    }

    public void LoadHighScore()
    {
        SceneManager.LoadScene("HighScoreScene"); // ⚠️ remplace par le nom réel de ta scène de jeu
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitter le jeu");
    }



}
