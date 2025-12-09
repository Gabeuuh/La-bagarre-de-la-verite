using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using System.Collections.Generic;

public class ZoneDepot : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private string tagObjetAccepte = "Document";

    [Header("Génération de documents")]
    [SerializeField] private GameObject prefabDocument;
    [SerializeField] private Transform pointSpawnDocument;

    [Header("Menu")]
    [SerializeField] private MenuAnalyse menuAnalyse;
    [SerializeField] private GestionnairePersonnages gestionnairePersonnages;

    private HashSet<GameObject> objetsDansZone = new HashSet<GameObject>();
    private GameObject documentActuel;

    public GameObject handmenu;

    void Start()
    {
        if (menuAnalyse == null)
            Debug.LogWarning("[ZoneDepot] ATTENTION : MenuAnalyse n'est pas assigné !");

        if (gestionnairePersonnages == null)
            Debug.LogWarning("[ZoneDepot] ATTENTION : GestionnairePersonnages n'est pas assigné !");

        // Générer le premier document au démarrage avec un petit délai
        StartCoroutine(GenererPremierDocumentAvecDelai());
    }

    /// <summary>
    /// Coroutine pour générer le premier document après que tout soit initialisé
    /// </summary>
    private IEnumerator GenererPremierDocumentAvecDelai()
    {
        // Attendre la fin de la frame pour être sûr que tous les Start() sont exécutés
        yield return new WaitForEndOfFrame();

        Debug.Log("[ZoneDepot] Génération du premier document");
        GenererNouveauDocument();
    }

    void OnEnable()
    {
        // S'abonner aux événements du gestionnaire de personnages
        if (gestionnairePersonnages != null)
        {
            // Ici on peut s'abonner à un événement quand un personnage avance
            // Pour l'instant on va créer une méthode publique appelée manuellement
        }
    }

    void OnDisable()
    {
        // Se désabonner des événements
    }

    /// <summary>
    /// Appelée par le GestionnairePersonnages quand la file avance
    /// Génère un nouveau document pour le nouveau personnage
    /// </summary>
    public void OnFileAvancee()
    {
        Debug.Log("[ZoneDepot] La file a avancé, génération d'un nouveau document pour le nouveau personnage");
        GenererNouveauDocument();
    }

    /// <summary>
    /// Génère un nouveau document avec les informations du personnage actuel
    /// </summary>
    public void GenererNouveauDocument()
    {
        if (prefabDocument == null)
        {
            Debug.LogError("[ZoneDepot] Prefab de document non assigné !");
            return;
        }

        if (pointSpawnDocument == null)
        {
            Debug.LogError("[ZoneDepot] Point de spawn du document non assigné !");
            return;
        }

        // Détruire l'ancien document s'il existe
        if (documentActuel != null)
        {
            Debug.Log("[ZoneDepot] Destruction de l'ancien document");
            Destroy(documentActuel);
            documentActuel = null;
            objetsDansZone.Clear();
        }

        // Récupérer la soumission du personnage actuel
        if (gestionnairePersonnages != null)
        {
            SoumissionArticle soumissionActuelle = gestionnairePersonnages.ObtenirSoumissionPremierPersonnage();

            if (soumissionActuelle != null)
            {
                // Créer un nouveau document
                documentActuel = Instantiate(prefabDocument, pointSpawnDocument.position, pointSpawnDocument.rotation);

                // Assigner les données de la soumission au document
                SoumissionArticleComponent composantSoumission = documentActuel.GetComponent<SoumissionArticleComponent>();
                if (composantSoumission != null)
                {
                    composantSoumission.soumission = soumissionActuelle;
                    Debug.Log($"[ZoneDepot] Nouveau document créé pour {soumissionActuelle.journaliste.nom}");
                }
                else
                {
                    Debug.LogWarning("[ZoneDepot] Le prefab de document n'a pas de composant SoumissionArticleComponent !");
                }

                // Ne pas ajouter le document dans la zone automatiquement
                // Il sera ajouté quand le joueur le déposera dans la zone
                Debug.Log($"[ZoneDepot] Document généré et prêt à être déposé dans la zone");
            }
            else
            {
                Debug.LogWarning("[ZoneDepot] Aucune soumission disponible pour générer un document");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[ZoneDepot] OnTriggerEnter: {other.gameObject.name} avec tag: {other.tag}");

        if (!other.CompareTag(tagObjetAccepte))
        {
            Debug.Log($"[ZoneDepot] Tag refusé: {other.tag} != {tagObjetAccepte}");
            return;
        }

        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable = other.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        if (grabInteractable != null)
        {
            objetsDansZone.Add(other.gameObject);
            Debug.Log($"[ZoneDepot] Document ajouté dans la zone. Total objets: {objetsDansZone.Count}");

            // Vérifier si le document est actuellement tenu
            bool estTenu = grabInteractable.isSelected;
            Debug.Log($"[ZoneDepot] Document est tenu: {estTenu}");

            if (estTenu)
            {
                // Si le document est tenu, attendre qu'il soit lâché
                grabInteractable.selectExited.AddListener(OnObjetLache);
                Debug.Log($"[ZoneDepot] Listener OnObjetLache ajouté");
            }
            else
            {
                // Si le document n'est pas tenu (déjà posé), ouvrir le menu directement
                Debug.Log($"[ZoneDepot] Document non tenu, ouverture directe du menu");
                documentActuel = other.gameObject;
                OuvrirMenu(other.gameObject);
            }
        }
        else
        {
            Debug.LogWarning($"[ZoneDepot] XRGrabInteractable non trouvé sur {other.gameObject.name}");
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log($"[ZoneDepot] OnTriggerExit: {other.gameObject.name}");

        if (objetsDansZone.Contains(other.gameObject))
        {
            objetsDansZone.Remove(other.gameObject);
            Debug.Log($"[ZoneDepot] Document retiré de la zone. Total objets: {objetsDansZone.Count}");

            // Désactiver le menu si aucun objet n'est dans la zone
            if (objetsDansZone.Count == 0)
            {
                Debug.Log($"[ZoneDepot] Plus de documents dans la zone, cache le menu");
                if (menuAnalyse != null)
                    menuAnalyse.CacherMenu();
                documentActuel = null;
            }
        }
    }

    private void OnObjetLache(SelectExitEventArgs args)
    {
        GameObject objetLache = args.interactableObject.transform.gameObject;
        Debug.Log($"[ZoneDepot] OnObjetLache appelé pour: {objetLache.name}");

        // Vérifier si l'objet est toujours dans la zone
        if (objetsDansZone.Contains(objetLache))
        {
            Debug.Log($"[ZoneDepot] L'objet {objetLache.name} est bien dans la zone, ouverture du menu");
            documentActuel = objetLache;
            OuvrirMenu(objetLache);
        }
        else
        {
            Debug.LogWarning($"[ZoneDepot] L'objet {objetLache.name} n'est plus dans la zone, menu non ouvert");
        }

        // Se désabonner
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable = objetLache.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.selectExited.RemoveListener(OnObjetLache);
            Debug.Log($"[ZoneDepot] Listener OnObjetLache retiré");
        }
    }

    private void OuvrirMenu(GameObject objet)
    {
        Debug.Log($"[ZoneDepot] OuvrirMenu appelé pour: {objet.name}");

        if (menuAnalyse == null)
        {
            Debug.LogError("[ZoneDepot] menuAnalyse est NULL !");
            return;
        }

        Debug.Log($"[ZoneDepot] Appel de menuAnalyse.AfficherMenu");
        // MenuAnalyse gère l'activation du menu
        menuAnalyse.AfficherMenu(objet);
    }
}