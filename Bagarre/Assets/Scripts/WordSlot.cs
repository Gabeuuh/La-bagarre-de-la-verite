using UnityEngine;

public class WordSlot : MonoBehaviour
{
    // Identifiant logique du slot (ex : "SUJET", "ACTION", "LIEU")
    public string slotId = "SUJET";

    // Position exacte où le mot doit se coller
    public Transform snapPoint;

    // Référence au mot actuellement posé (optionnel)
    [HideInInspector]
    public WordData currentWord;
}
