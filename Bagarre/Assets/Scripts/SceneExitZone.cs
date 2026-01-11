using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneExitZone : MonoBehaviour
{
    [Header("Config")]
    [Tooltip("Si false, on utilise nextSceneName au lieu du buildIndex+1")]
    public bool useNextBuildIndex = true;

    [Tooltip("Nom de la scène à charger si useNextBuildIndex = false")]
    public string nextSceneName;

    [Tooltip("Tag du joueur (XR Origin)")]
    public string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        // On ne déclenche que si c'est le joueur
        if (!other.CompareTag(playerTag))
            return;

        Debug.Log("[SceneExitZone] Joueur entré dans la zone, chargement de la scène suivante.");

        if (useNextBuildIndex)
        {
            var current = SceneManager.GetActiveScene();
            SceneManager.LoadScene(current.buildIndex + 1);
        }
        else if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("[SceneExitZone] Aucune cible de scène définie.");
        }
    }
}
