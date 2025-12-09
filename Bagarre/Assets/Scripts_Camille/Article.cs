using UnityEngine;

[System.Serializable]
public class Article
{
    public string titre;
    public string date;
    public string categorie;
    [TextArea(3, 10)]
    public string contenu;
    public bool estFakeNews;
    public string[] indicesProblematiques;
}