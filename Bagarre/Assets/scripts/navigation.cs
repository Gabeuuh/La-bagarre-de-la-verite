using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuNavigation : MonoBehaviour
{
        void Start()
    {
        // Enregistre la scène actuelle comme "LastScene"
        PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
    }
    
    public void LoadHighScoreScene()
    {
        SceneManager.LoadScene("HighScoreScene"); // mets le nom exact de ta scène
    }
        public void LoadPreviousScene()
    {
        SceneManager.LoadScene("Jeu"); // mets le nom exact de ta scène
    }
    
}
