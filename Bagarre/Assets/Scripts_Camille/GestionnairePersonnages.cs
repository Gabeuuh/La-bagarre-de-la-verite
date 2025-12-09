using UnityEngine;
using System.Collections.Generic;

public class GestionnairePersonnages : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private BanqueSoumissions banqueSoumissions;
    [SerializeField] private GameObject prefabHumanMale;
    [SerializeField] private ZoneDepot zoneDepot;

    [Header("Configuration de placement")]
    [SerializeField] private Vector3 positionDepart = new Vector3(0, 0, 0);
    [SerializeField] private float espacementEntrePersonnages = 1.5f; // Réduit pour une file plus serrée
    [SerializeField] private bool placementEnLigne = true;
    [SerializeField] private int personnagesParLigne = 3;

    [Header("Configuration de déplacement")]
    [SerializeField] private float vitesseDeplacementPersonnages = 2f;
    [SerializeField] private float distanceDeplacementGaucheDroite = 5f;

    private List<GameObject> personnagesInstancies = new List<GameObject>();
    private int personnagesEnMouvement = 0;

    void Start()
    {
        CreerPersonnagesPourSoumissions();
    }

    public void CreerPersonnagesPourSoumissions()
    {
        if (banqueSoumissions == null)
        {
            Debug.LogError("[GestionnairePersonnages] BanqueSoumissions non assignée !");
            return;
        }

        if (prefabHumanMale == null)
        {
            Debug.LogError("[GestionnairePersonnages] Prefab HumanMale non assigné !");
            return;
        }

        int nombreSoumissions = banqueSoumissions.toutesLesSoumissions.Length;
        Debug.Log($"[GestionnairePersonnages] Création de {nombreSoumissions} personnages");
        personnagesInstancies.Clear();

        for (int i = 0; i < nombreSoumissions; i++)
        {
            Vector3 position = CalculerPosition(i);
            Quaternion rotation = CalculerRotation(i);
            GameObject personnage = Instantiate(prefabHumanMale, position, rotation, transform);

            SoumissionArticle soumission = banqueSoumissions.toutesLesSoumissions[i];
            personnage.name = $"Journaliste_{i}_{soumission.journaliste.nom}";

            // Ajouter et configurer le composant DeplacementPersonnage
            DeplacementPersonnage deplacement = personnage.GetComponent<DeplacementPersonnage>();
            if (deplacement == null)
            {
                deplacement = personnage.AddComponent<DeplacementPersonnage>();
            }

            // Configurer les paramètres de déplacement
            deplacement.VitesseDeplacement = vitesseDeplacementPersonnages;
            deplacement.DistanceDeplacement = distanceDeplacementGaucheDroite;

            // Assigner l'animator si disponible
            Animator anim = personnage.GetComponent<Animator>();
            if (anim != null)
            {
                deplacement.AnimatorReference = anim;
            }

            // S'abonner aux événements de déplacement
            deplacement.onDeplacementTermine.AddListener(() => OnPersonnageDeplacementTermine(personnage));
            deplacement.onAvancementFileTermine.AddListener(() => OnPersonnageAvancementFileTermine(personnage));

            personnagesInstancies.Add(personnage);
        }

        Debug.Log($"[GestionnairePersonnages] {personnagesInstancies.Count} personnages créés avec succès");
    }

    private Vector3 CalculerPosition(int index)
    {
        if (placementEnLigne)
        {
            // File d'attente : chaque personnage est derrière le précédent (axe Z positif)
            return positionDepart + new Vector3(0, 0, index * espacementEntrePersonnages);
        }
        else
        {
            // Placement en grille (plusieurs files parallèles)
            int ligne = index / personnagesParLigne;
            int colonne = index % personnagesParLigne;

            return positionDepart + new Vector3(
                colonne * espacementEntrePersonnages,
                0,
                ligne * espacementEntrePersonnages
            );
        }
    }

    /// <summary>
    /// Le premier personnage regarde à droite (90°), les autres regardent vers l'avant (180°)
    /// </summary>
    private Quaternion CalculerRotation(int index)
    {
        // Le premier (index 0) regarde à droite, les autres regardent vers l'avant
        if (index == 0)
        {
            return Quaternion.Euler(0, 90, 0);
        }
        else
        {
            return Quaternion.Euler(0, 180, 0);
        }
    }

    /// <summary>
    /// Appelé quand un personnage a fini son déplacement (gauche ou droite)
    /// Supprime le personnage et fait avancer la file
    /// </summary>
    private void OnPersonnageDeplacementTermine(GameObject personnage)
    {
        // Retirer le personnage de la liste
        personnagesInstancies.Remove(personnage);

        // Détruire le GameObject
        Destroy(personnage);

        // Faire avancer toute la file
        AvancerLaFile();
    }

    /// <summary>
    /// Appelé quand un personnage a fini son avancement dans la file
    /// Ne fait rien (pas de suppression)
    /// </summary>
    private void OnPersonnageAvancementFileTermine(GameObject personnage)
    {
        personnagesEnMouvement--;

        // Quand tous les personnages ont fini de bouger, notifier la zone de dépôt
        if (personnagesEnMouvement == 0)
        {
            Debug.Log("[GestionnairePersonnages] Tous les personnages ont fini de bouger, notification de la zone de dépôt");
            NotifierFileAvancee();
        }
    }

    /// <summary>
    /// Notifie la zone de dépôt que la file a avancé et qu'un nouveau personnage est en face
    /// </summary>
    private void NotifierFileAvancee()
    {
        if (zoneDepot != null)
        {
            zoneDepot.OnFileAvancee();
        }
    }

    /// <summary>
    /// Fait avancer tous les personnages restants vers la position de départ
    /// Utilise le composant DeplacementPersonnage de chaque personnage
    /// </summary>
    private void AvancerLaFile()
    {
        if (personnagesInstancies.Count == 0)
        {
            Debug.Log("[GestionnairePersonnages] Plus de personnages dans la file !");
            return;
        }

        personnagesEnMouvement = 0;

        for (int i = 0; i < personnagesInstancies.Count; i++)
        {
            GameObject personnage = personnagesInstancies[i];
            if (personnage != null)
            {
                Vector3 nouvellePosition = CalculerPosition(i);
                Quaternion nouvelleRotation = CalculerRotation(i);

                DeplacementPersonnage deplacement = personnage.GetComponent<DeplacementPersonnage>();
                if (deplacement != null)
                {
                    // Utiliser le composant DeplacementPersonnage au lieu de la coroutine
                    deplacement.DeplacerVersPosition(nouvellePosition, nouvelleRotation);
                    personnagesEnMouvement++;
                }
                else
                {
                    Debug.LogError($"[GestionnairePersonnages] {personnage.name} n'a pas de composant DeplacementPersonnage !");
                }
            }
        }
    }


    public GameObject ObtenirPersonnage(int indexSoumission)
    {
        if (personnagesInstancies == null || indexSoumission < 0 || indexSoumission >= personnagesInstancies.Count)
            return null;

        return personnagesInstancies[indexSoumission];
    }

    public GameObject ObtenirPremierPersonnage()
    {
        if (personnagesInstancies.Count > 0)
            return personnagesInstancies[0];

        return null;
    }

    public int NombrePersonnagesRestants()
    {
        return personnagesInstancies.Count;
    }

    /// <summary>
    /// Obtient la soumission associée au premier personnage de la file
    /// </summary>
    public SoumissionArticle ObtenirSoumissionPremierPersonnage()
    {
        if (personnagesInstancies.Count == 0 || banqueSoumissions == null)
        {
            Debug.LogWarning($"[GestionnairePersonnages] ObtenirSoumissionPremierPersonnage - Impossible: personnages={personnagesInstancies.Count}, banque={(banqueSoumissions != null ? "OK" : "NULL")}");
            return null;
        }

        // Le premier personnage correspond toujours à l'index 0 de la banque
        // car lorsqu'on supprime un personnage, on décale les autres
        int indexDansBanque = banqueSoumissions.toutesLesSoumissions.Length - personnagesInstancies.Count;

        Debug.Log($"[GestionnairePersonnages] ObtenirSoumissionPremierPersonnage - Index calculé: {indexDansBanque}, Personnages restants: {personnagesInstancies.Count}");

        if (indexDansBanque < banqueSoumissions.toutesLesSoumissions.Length)
        {
            SoumissionArticle soumission = banqueSoumissions.toutesLesSoumissions[indexDansBanque];
            Debug.Log($"[GestionnairePersonnages] Soumission trouvée: {soumission.journaliste.nom}");
            return soumission;
        }

        return null;
    }

    public void DetruireTousLesPersonnages()
    {
        if (personnagesInstancies != null)
        {
            foreach (GameObject personnage in personnagesInstancies)
            {
                if (personnage != null)
                    Destroy(personnage);
            }
            personnagesInstancies.Clear();
        }
    }

    public void RecréerPersonnages()
    {
        DetruireTousLesPersonnages();
        CreerPersonnagesPourSoumissions();
    }
}