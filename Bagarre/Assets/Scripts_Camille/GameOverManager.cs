using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverManager : MonoBehaviour
{
    [Header("Références UI")]
    [SerializeField] private GameObject panneauGameOver;

    [Header("Configuration")]
    [SerializeField] private float delaiRedemarrage = 2f;

    private static GameOverManager instance;

    public static GameOverManager Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Cacher le panneau au démarrage et configurer le Canvas
        if (panneauGameOver != null)
        {
            panneauGameOver.SetActive(false);

            // S'assurer que le Canvas parent est en Screen Space Overlay
            Canvas canvas = panneauGameOver.GetComponentInParent<Canvas>();
            if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                Debug.LogWarning("[GameOverManager] Le Canvas n'est pas en mode Screen Space Overlay. Configuration automatique...");
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 1000; // Le mettre au-dessus de tout
            }
        }
        else
        {
            Debug.LogError("[GameOverManager] Panneau Game Over non assigné ! Veuillez l'assigner dans l'inspecteur.");
        }
    }

    /// <summary>
    /// Affiche l'écran de Game Over et redémarre le jeu après un délai
    /// </summary>
    public void AfficherGameOver()
    {
        Debug.Log("[GameOverManager] GAME OVER - Redémarrage dans " + delaiRedemarrage + " secondes");

        // Afficher le panneau Game Over
        if (panneauGameOver != null)
        {
            panneauGameOver.SetActive(true);
        }

        // Lancer la coroutine de redémarrage
        StartCoroutine(RedemarrerApresDelai());
    }

    /// <summary>
    /// Coroutine qui attend le délai puis redémarre la scène
    /// </summary>
    private IEnumerator RedemarrerApresDelai()
    {
        // Attendre le délai spécifié
        yield return new WaitForSeconds(delaiRedemarrage);

        Debug.Log("[GameOverManager] Redémarrage de la scène...");

        // Redémarrer la scène actuelle
        RedemarrerJeu();
    }

    /// <summary>
    /// Redémarre la scène actuelle
    /// </summary>
    public void RedemarrerJeu()
    {
        Scene sceneActuelle = SceneManager.GetActiveScene();
        SceneManager.LoadScene(sceneActuelle.name);
    }

    /// <summary>
    /// Méthode pour cacher manuellement le Game Over (si besoin)
    /// </summary>
    public void CacherGameOver()
    {
        if (panneauGameOver != null)
        {
            panneauGameOver.SetActive(false);
        }
    }
}
