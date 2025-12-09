using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuAnalyse : MonoBehaviour
{
    [Header("UI - Article")]
    [SerializeField] private TMP_Text titreArticle;
    [SerializeField] private TMP_Text dateArticle;
    [SerializeField] private TMP_Text categorieArticle;
    [SerializeField] private TMP_Text contenuArticle;

    [Header("UI - Journaliste")]
    [SerializeField] private TMP_Text nomJournaliste;
    [SerializeField] private TMP_Text numeroCartePresse;
    [SerializeField] private TMP_Text nomMedia;
    [SerializeField] private TMP_Text dateExpiration;
    [SerializeField] private TMP_Text zoneAccreditation;

    [Header("Menu Hand")]
    [SerializeField] private GameObject contenuMenu;

    [Header("Gestionnaire")]
    [SerializeField] private GestionnairePersonnages gestionnairePersonnages;

    private GameObject documentAnalyse;
    private SoumissionArticle soumissionActuelle;

    void Start()
    {
        if (contenuMenu != null)
            contenuMenu.SetActive(false);
    }

    public void AfficherMenu(GameObject document)
    {
        Debug.Log("[MenuAnalyse] AfficherMenu appelé pour le document: " + document.name);

        if (contenuMenu == null)
        {
            Debug.LogError("[MenuAnalyse] contenuMenu est null !");
            return;
        }

        documentAnalyse = document;

        // Récupérer la soumission directement depuis le document
        SoumissionArticleComponent composantSoumission = document.GetComponent<SoumissionArticleComponent>();

        if (composantSoumission == null)
        {
            Debug.LogError("[MenuAnalyse] Le document n'a pas de SoumissionArticleComponent !");
            return;
        }

        if (composantSoumission.soumission == null)
        {
            Debug.LogError("[MenuAnalyse] Le SoumissionArticleComponent existe mais la soumission est null !");
            return;
        }

        Debug.Log("[MenuAnalyse] Récupération de la soumission depuis le document");
        soumissionActuelle = composantSoumission.soumission;

        if (soumissionActuelle.article == null)
        {
            Debug.LogError("[MenuAnalyse] La soumission n'a pas d'article !");
            return;
        }

        if (soumissionActuelle.journaliste == null)
        {
            Debug.LogError("[MenuAnalyse] La soumission n'a pas de journaliste !");
            return;
        }

        Debug.Log($"[MenuAnalyse] Article: {soumissionActuelle.article.titre}, Journaliste: {soumissionActuelle.journaliste.nom}");

        RegénérerMenu();
        Debug.Log("[MenuAnalyse] Menu activé avec les infos du document !");
    }

    /// <summary>
    /// Régénère le menu en le désactivant puis réactivant avec les nouvelles données
    /// Cela force le mesh à se reconstruire complètement
    /// </summary>
    public void RegénérerMenu()
    {
        Debug.Log($"[MenuAnalyse] RegénérerMenu - État actuel de contenuMenu: {(contenuMenu != null ? contenuMenu.activeSelf.ToString() : "NULL")}");

        // Activer le menu d'abord pour s'assurer qu'il est visible
        contenuMenu.SetActive(true);
        Debug.Log($"[MenuAnalyse] contenuMenu.SetActive(true) appelé");

        // Mettre à jour les informations pendant que le menu est actif
        AfficherInfosArticle(soumissionActuelle.article);
        AfficherInfosJournaliste(soumissionActuelle.journaliste);

        // Forcer le rafraîchissement des TextMeshPro
        Canvas.ForceUpdateCanvases();

        Debug.Log($"[MenuAnalyse] RegénérerMenu terminé - État final de contenuMenu: {contenuMenu.activeSelf}");
    }

    private void AfficherInfosArticle(Article article)
    {
        if (article == null)
        {
            Debug.LogError("[MenuAnalyse] Article est null !");
            return;
        }

        Debug.Log("[MenuAnalyse] Affichage article : " + article.titre);

        if (titreArticle != null)
        {
            titreArticle.text = article.titre;
            Debug.Log("[MenuAnalyse] Titre assigné : " + article.titre);
        }
        else
            Debug.LogError("[MenuAnalyse] titreArticle est NULL dans l'Inspector !");

        if (dateArticle != null)
        {
            dateArticle.text = "Date : " + article.date;
            Debug.Log("[MenuAnalyse] Date assignée : " + article.date);
        }
        else
            Debug.LogError("[MenuAnalyse] dateArticle est NULL dans l'Inspector !");

        if (categorieArticle != null)
        {
            categorieArticle.text = "Catégorie : " + article.categorie;
            Debug.Log("[MenuAnalyse] Catégorie assignée : " + article.categorie);
        }
        else
            Debug.LogError("[MenuAnalyse] categorieArticle est NULL dans l'Inspector !");

        if (contenuArticle != null)
        {
            contenuArticle.text = article.contenu;
            Debug.Log("[MenuAnalyse] Contenu assigné");
        }
        else
            Debug.LogError("[MenuAnalyse] contenuArticle est NULL dans l'Inspector !");
    }
    
    public bool EstFakeNews()
    {
        if (soumissionActuelle == null || soumissionActuelle.article == null)
        {
            Debug.LogWarning("[MenuAnalyse] Pas de soumission actuelle !");
            return false;
        }

        return soumissionActuelle.article.estFakeNews;
    }

    private void AfficherInfosJournaliste(CarteJournaliste journaliste)
    {
        if (journaliste == null)
        {
            Debug.LogError("[MenuAnalyse] Journaliste est null !");
            return;
        }

        Debug.Log("[MenuAnalyse] Affichage journaliste : " + journaliste.prenom + " " + journaliste.nom);

        if (nomJournaliste != null)
        {
            nomJournaliste.text = journaliste.prenom + " " + journaliste.nom;
            Debug.Log("[MenuAnalyse] Nom journaliste assigné : " + journaliste.prenom + " " + journaliste.nom);
        }
        else
            Debug.LogError("[MenuAnalyse] nomJournaliste est NULL dans l'Inspector !");

        if (numeroCartePresse != null)
        {
            numeroCartePresse.text = "N° Carte : " + journaliste.numeroCartePresse;
            Debug.Log("[MenuAnalyse] Numéro carte assigné");
        }
        else
            Debug.LogError("[MenuAnalyse] numeroCartePresse est NULL dans l'Inspector !");

        if (nomMedia != null)
        {
            nomMedia.text = "Média : " + journaliste.nomMedia;
            Debug.Log("[MenuAnalyse] Média assigné");
        }
        else
            Debug.LogError("[MenuAnalyse] nomMedia est NULL dans l'Inspector !");

        if (dateExpiration != null)
        {
            dateExpiration.text = "Expire le : " + journaliste.dateExpiration;
            Debug.Log("[MenuAnalyse] Date expiration assignée");
        }
        else
            Debug.LogError("[MenuAnalyse] dateExpiration est NULL dans l'Inspector !");

        if (zoneAccreditation != null)
        {
            zoneAccreditation.text = "Zone : " + journaliste.zoneAccreditation;
            Debug.Log("[MenuAnalyse] Zone assignée");
        }
        else
            Debug.LogError("[MenuAnalyse] zoneAccreditation est NULL dans l'Inspector !");
    }

    public void CacherMenu()
    {
        Debug.Log($"[MenuAnalyse] CacherMenu appelé");
        if (contenuMenu != null)
        {
            contenuMenu.SetActive(false);
            Debug.Log($"[MenuAnalyse] contenuMenu désactivé");
        }

        soumissionActuelle = null;
    }

    // Méthode utile pour savoir si la soumission actuelle est valide ou non
    public bool EstSoumissionValide()
    {
        if (soumissionActuelle == null || soumissionActuelle.article == null)
            return false;

        return !soumissionActuelle.article.estFakeNews;
    }
}