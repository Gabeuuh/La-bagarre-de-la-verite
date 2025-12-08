using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToPreviousScene : MonoBehaviour
{
    public void LoadPreviousScene()
    {
        string previousScene = PlayerPrefs.GetString("LastScene", "");

        if (!string.IsNullOrEmpty(previousScene))
        {
            SceneManager.LoadScene(previousScene);
        }
        else
        {
            Debug.LogWarning(" Aucune scène précédente enregistrée !");
        }
    }
}
