using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BoutonJugement : MonoBehaviour
{
    public enum TypeBouton { GoodGuy, BadGuy }

    [Header("Configuration")]
    [SerializeField] private TypeBouton typeBouton;
    [SerializeField] private MenuAnalyse menuAnalyse;

    [Header("Gestion des personnages")]
    [SerializeField] private GestionnairePersonnages gestionnairePersonnages;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sonThankYou;
    [SerializeField] private AudioClip sonFuckYou;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable interactable;

    void Start()
    {
        interactable = GetComponentInChildren<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();

        if (interactable != null)
        {
            interactable.selectEntered.AddListener(OnBoutonAppuye);
            Debug.Log("[BoutonJugement] Bouton " + typeBouton + " prêt !");
        }
        else
        {
            Debug.LogError("[BoutonJugement] XRSimpleInteractable manquant sur " + gameObject.name);
        }
    }

    private void OnBoutonAppuye(SelectEnterEventArgs args)
    {
        Debug.Log("========================================");
        Debug.Log("[BoutonJugement] Bouton " + typeBouton + " appuyé !");
        Debug.Log("========================================");

        if (menuAnalyse == null)
        {
            Debug.LogError("[BoutonJugement] MenuAnalyse non assigné !");
            return;
        }

        if (gestionnairePersonnages == null)
        {
            Debug.LogError("[BoutonJugement] GestionnairePersonnages non assigné !");
            return;
        }

        // Récupérer le premier personnage de la file
        GameObject premierPersonnage = gestionnairePersonnages.ObtenirPremierPersonnage();
        if (premierPersonnage == null)
        {
            Debug.LogWarning("[BoutonJugement] Aucun personnage dans la file !");
            menuAnalyse.CacherMenu();
            return;
        }

        Debug.Log("[BoutonJugement] Premier personnage trouvé : " + premierPersonnage.name);

        DeplacementPersonnage deplacement = premierPersonnage.GetComponent<DeplacementPersonnage>();
        if (deplacement == null)
        {
            Debug.LogError("[BoutonJugement] Le personnage n'a pas de composant DeplacementPersonnage !");
            menuAnalyse.CacherMenu();
            return;
        }

        Debug.Log("[BoutonJugement] Composant DeplacementPersonnage trouvé sur " + premierPersonnage.name);

        // Vérifier si le personnage est déjà en déplacement
        if (deplacement.EnDeplacement)
        {
            Debug.LogWarning("[BoutonJugement] Le personnage est déjà en déplacement, commande ignorée !");
            return;
        }

        bool estFakeNews = menuAnalyse.EstFakeNews();
        Debug.Log("[BoutonJugement] Est Fake News ? " + estFakeNews);

        if (typeBouton == TypeBouton.GoodGuy)
        {
            Debug.Log("[BoutonJugement] Traitement bouton GOOD GUY...");

            // Jouer le son "Thank You"
            if (audioSource != null && sonThankYou != null)
            {
                audioSource.PlayOneShot(sonThankYou);
                Debug.Log("[BoutonJugement] Son 'Thank You' joué");
            }

            if (!estFakeNews)
            {
                // CORRECT - Article valide vers Good Guy (gauche)
                Debug.Log("✅ CORRECT - C'est un GOOD GUY - Déplacement vers la GAUCHE");
                deplacement.MarcherVersLaGauche();
            }
            else
            {
                // ERREUR - Fake news envoyée vers Good Guy (devrait aller vers Bad Guy)
                Debug.Log("❌ ERREUR - Mauvais jugement (c'était une fake news, elle devait aller vers Bad Guy !)");
                deplacement.MarcherVersLaGauche(); // Le personnage se déplace quand même
                TriggerGameOver(); // Mais on déclenche le Game Over
            }
        }
        else if (typeBouton == TypeBouton.BadGuy)
        {
            Debug.Log("[BoutonJugement] Traitement bouton BAD GUY...");

            // Jouer le son "Fuck You"
            if (audioSource != null && sonFuckYou != null)
            {
                audioSource.PlayOneShot(sonFuckYou);
                Debug.Log("[BoutonJugement] Son 'Fuck You' joué");
            }

            if (estFakeNews)
            {
                // CORRECT - Fake news vers Bad Guy (droite)
                Debug.Log("✅ CORRECT - C'est un BAD GUY - Déplacement vers la DROITE");
                deplacement.MarcherVersLaDroite();
            }
            else
            {
                // ERREUR - Article valide envoyé vers Bad Guy (devrait aller vers Good Guy)
                Debug.Log("❌ ERREUR - Il était gentil, il devait aller vers Good Guy !");
                deplacement.MarcherVersLaDroite(); // Le personnage se déplace quand même
                TriggerGameOver(); // Mais on déclenche le Game Over
            }
        }

        menuAnalyse.CacherMenu();
        Debug.Log("========================================");
    }

    /// <summary>
    /// Déclenche le Game Over de manière centralisée
    /// </summary>
    private void TriggerGameOver()
    {
        Debug.Log("[BoutonJugement] ⚠️ GAME OVER - Erreur de jugement détectée !");

        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.AfficherGameOver();
        }
        else
        {
            Debug.LogError("[BoutonJugement] GameOverManager.Instance est null !");
        }
    }

    void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnBoutonAppuye);
        }
    }
}