using UnityEngine;
using Unity.XR.CoreUtils;

public class StartLetterGameOnInteract : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("Le parent LetterGameRoot, désactivé au départ.")]
    public GameObject letterGameRoot;

    [Tooltip("Point où placer le XR Origin quand le joueur 's'assoit'.")]
    public Transform sitPoint;

    [Tooltip("Ton XR Origin / XR Rig.")]
    public XROrigin xrOrigin;

    private bool _hasStarted = false;

    // 👉 Cette méthode sera appelée depuis l'event du XR Simple Interactable
    public void StartLetterGame()
    {
        if (_hasStarted)
        {
            Debug.Log("[StartLetterGameOnInteract] Déjà lancé, on ignore.");
            return;
        }

        _hasStarted = true;
        Debug.Log("[StartLetterGameOnInteract] Lancement du jeu de la lettre.");

        // 1. Téléporter le joueur à la chaise / devant la table
        if (xrOrigin != null && sitPoint != null)
        {
            xrOrigin.transform.position = sitPoint.position;
            xrOrigin.transform.rotation = sitPoint.rotation;
            Debug.Log("[StartLetterGameOnInteract] XR Origin déplacé au SitPoint.");
        }
        else
        {
            Debug.LogWarning("[StartLetterGameOnInteract] xrOrigin ou sitPoint n'est pas assigné !");
        }

        // 2. Activer le mini-jeu
        if (letterGameRoot != null)
        {
            letterGameRoot.SetActive(true);
            Debug.Log("[StartLetterGameOnInteract] LetterGameRoot activé.");
        }
        else
        {
            Debug.LogWarning("[StartLetterGameOnInteract] letterGameRoot n'est pas assigné !");
        }
    }
}
