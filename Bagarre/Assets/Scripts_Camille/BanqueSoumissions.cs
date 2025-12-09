using UnityEngine;

public class BanqueSoumissions : MonoBehaviour
{
    public SoumissionArticle[] toutesLesSoumissions;

    void Awake()
    {
        toutesLesSoumissions = new SoumissionArticle[]
        {
            // ✅ SOUMISSION 1 - VALIDE
            new SoumissionArticle
            {
                journaliste = new CarteJournaliste
                {
                    nom = "Durand",
                    prenom = "Marie",
                    numeroCartePresse = "CP-2025-4892",
                    nomMedia = "Le Quotidien National",
                    dateExpiration = "15/03/2026",
                },
                article = new Article
                {
                    titre = "Nouveau plan de rénovation urbaine à Paris",
                    date = "8 Décembre 2025",
                    categorie = "Politique",
                    contenu = "La mairie annonce un investissement de 2 milliards pour rénover les quartiers nord.",
                    estFakeNews = false,
                    indicesProblematiques = new string[] { }
                }
            },

            // ❌ SOUMISSION 2 - INVALIDE (évènement non produit)
            new SoumissionArticle
            {
                journaliste = new CarteJournaliste
                {
                    nom = "Leroy",
                    prenom = "Thomas",
                    numeroCartePresse = "CP-2025-3156",
                    nomMedia = "Info24",
                    dateExpiration = "17/12/2025",
                    zoneAccreditation = "Provence",
                },
                article = new Article
                {
                    titre = "Ouverture d'une usine de batteries électriques",
                    date = "7 Décembre 2025",
                    categorie = "Économie",
                    contenu = "Une nouvelle usine créera 500 emplois dans la région.",
                    estFakeNews = true,
                    indicesProblematiques = new string[] { "évènement non produit" }
                }
            },

            // ✅ SOUMISSION 3 - VALIDE
            new SoumissionArticle
            {
                journaliste = new CarteJournaliste
                {
                    nom = "Mercier",
                    prenom = "Jeanne",
                    numeroCartePresse = "CP-2025-7723",
                    nomMedia = "Le Quotidien National",
                    dateExpiration = "22/06/2026",
                    zoneAccreditation = "Île-de-France",
                },
                article = new Article
                {
                    titre = "Festival de musique gratuit ce week-end",
                    date = "6 Décembre 2025",
                    categorie = "Société",
                    contenu = "Le parc de la Villette accueillera 50 artistes pendant trois jours.",
                    estFakeNews = false,
                    indicesProblematiques = new string[] { }
                }
            },

            // ❌ SOUMISSION 4 - INVALIDE (Carte expirée + spécialité incorrecte)
            new SoumissionArticle
            {
                journaliste = new CarteJournaliste
                {
                    nom = "Bonnet",
                    prenom = "Pierre",
                    numeroCartePresse = "CP-2025-5581",
                    nomMedia = "La Tribune Locale",
                    dateExpiration = "30/09/2025",
                    zoneAccreditation = "Bretagne",
                },
                article = new Article
                {
                    titre = "Alerte météo : tempête prévue sur la côte",
                    date = "5 Décembre 2025",
                    categorie = "Sécurité",
                    contenu = "Météo France recommande d'éviter les déplacements ce week-end.",
                    estFakeNews = true,
                    indicesProblematiques = new string[] { "Spécialité incorrecte : Sport au lieu de Sécurité + carte expirée" }
                }
            },

            // ❌ SOUMISSION 5 - INVALIDE (News non présente dasn le tableau des news)
            new SoumissionArticle
            {
                journaliste = new CarteJournaliste
                {
                    nom = "Fabre",
                    prenom = "Claire",
                    numeroCartePresse = "CP-2025-6634",
                    nomMedia = "Info24",
                    dateExpiration = "18/11/2026",
                    zoneAccreditation = "Provence",
                },
                article = new Article
                {
                    titre = "Épidémie de grippe en Bretagne",
                    date = "4 Décembre 2025",
                    categorie = "Santé",
                    contenu = "Les hôpitaux bretons font face à une hausse des admissions.",
                    estFakeNews = true,
                    indicesProblematiques = new string[] { "Evenement non produit" }
                }
            }
        };
    }

    public SoumissionArticle ObtenirSoumission(int index)
    {
        if (index >= 0 && index < toutesLesSoumissions.Length)
            return toutesLesSoumissions[index];
        
        return null;
    }

    public SoumissionArticle ObtenirSoumissionAleatoire()
    {
        int index = Random.Range(0, toutesLesSoumissions.Length);
        return toutesLesSoumissions[index];
    }
}