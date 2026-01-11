using UnityEngine;

public class PuzzleLauncher : MonoBehaviour
{
    public GameObject puzzleRoot;   // parent du puzzle (PuzzleRoot)
    public Menu puzzleMenu;         // script Menu de Faramira (celui qui a OnClickPlay)

    private bool _started = false;

    public void StartPuzzle()
    {
        if (_started) return;
        _started = true;

        // Activer le puzzle dans la scène
        if (puzzleRoot != null)
            puzzleRoot.SetActive(true);

        // Lancer le puzzle Faramira (équivalent bouton Play d’origine)
        if (puzzleMenu != null)
            puzzleMenu.OnClickPlay();
    }
}
