using TMPro;
using UnityEngine;

public class PanneauNews : MonoBehaviour
{
    [Header("Liste des News")]
    public News[] listeNews;

    [Header("UI")]
    [SerializeField] private TMP_Text texteNews;

    void Awake()
    {
        InitialiserNews();
    }

    void Start()
    {
        AfficherToutesLesNews();
    }

    private void AfficherToutesLesNews()
    {
        string affichageNews = "=== ACTUALITÉS ===\n\n";

        foreach (News news in listeNews)
        {
            affichageNews += $"{news.titre}\n";
            affichageNews += $"📅 {news.date} | 📂 {news.categorie}\n";
            affichageNews += $"{news.contenu}\n";
            affichageNews += "─────────────────────────\n\n";
        }

        texteNews.text = affichageNews;
    }

    private void InitialiserNews()
    {
        listeNews = new News[]
        {
            new News
            {
                titre = "Nouveau plan de rénovation urbaine à Paris",
                date = "8 Décembre 2025",
                categorie = "Politique",
                contenu = "La mairie annonce un investissement de 2 milliards pour rénover les quartiers nord."
            },
            new News
            {
                titre = "Festival de musique gratuit ce week-end",
                date = "6 Décembre 2025",
                categorie = "Société",
                contenu = "Le parc de la Villette accueillera 50 artistes pendant trois jours."
            },
            new News
            {
                titre = "Alerte météo : tempête prévue sur la côte",
                date = "5 Décembre 2025",
                categorie = "Sécurité",
                contenu = "Météo France recommande d'éviter les déplacements ce week-end."
            }
        };
    }
}