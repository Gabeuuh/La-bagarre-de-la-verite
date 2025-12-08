using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Pause()
{
    Debug.Log("🛑 Le menu pause s'active !");
    pauseMenuUI.SetActive(true);
    Time.timeScale = 0f;
    isPaused = true;
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
}


    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // à adapter selon le nom de ta scène menu
    }
}
