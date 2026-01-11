using UnityEngine;
using Unity.XR.CoreUtils;

public class ChairStartLetterGame : MonoBehaviour
{
    [Header("Références")]
    public GameObject letterGameRoot; // Parent du mini-jeu (désactivé au départ)
    public Transform sitPoint;        // Point où "asseoir" le joueur
    public XROrigin xrOrigin;         // Ton XR Origin / XR Rig

    private bool _hasStarted = false;

    // 👉 Appelée par l'event du XR Simple Interactable de la chaise
    public void StartLetterGame()
    {
        if (_hasStarted)
        {
            Debug.Log("[ChairStartLetterGame] Déjà lancé, on ignore.");
            return;
        }

        _hasStarted = true;
        Debug.Log("[ChairStartLetterGame] Lancement du jeu de la lettre.");

        // 1. Téléporter le joueur au sitPoint
        if (xrOrigin != null && sitPoint != null)
        {
            xrOrigin.transform.position = sitPoint.position;
            xrOrigin.transform.rotation = sitPoint.rotation;
            Debug.Log("[ChairStartLetterGame] XR Origin déplacé au SitPoint.");
        }
        else
        {
            Debug.LogWarning("[ChairStartLetterGame] xrOrigin ou sitPoint n'est pas assigné !");
        }

        // 2. Désactiver les scripts de locomotion sur le XR Origin
        if (xrOrigin != null)
        {
            // a) Couper le CharacterController (plus de déplacement basé dessus)
            var cc = xrOrigin.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
                Debug.Log("[ChairStartLetterGame] CharacterController désactivé.");
            }

            // b) Désactiver XRInputModalityManager + XRGazeAssistance
            var behaviours = xrOrigin.GetComponents<MonoBehaviour>();
            foreach (var b in behaviours)
            {
                if (b == null) continue;

                string typeName = b.GetType().Name;

                if (typeName == "XRInputModalityManager" ||
                    typeName == "XRGazeAssistance")
                {
                    b.enabled = false;
                    Debug.Log("[ChairStartLetterGame] Désactivation de " + typeName);
                }
            }
        }

        // 3. Activer le mini-jeu de la lettre
        if (letterGameRoot != null)
        {
            letterGameRoot.SetActive(true);
            Debug.Log("[ChairStartLetterGame] LetterGameRoot activé.");
        }
        else
        {
            Debug.LogWarning("[ChairStartLetterGame] letterGameRoot n'est pas assigné !");
        }
    }
}
