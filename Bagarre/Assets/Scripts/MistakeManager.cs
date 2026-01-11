using UnityEngine;
using UnityEngine.SceneManagement;

public class MistakeManager : MonoBehaviour
{
    public static MistakeManager Instance { get; private set; }

    [Header("Config")]
    public int maxMistakes = 3;        // nombre d'erreurs avant reset
    public float maxRedAlpha = 0.7f;   // opacité max du rouge

    [Header("Overlay rouge")]
    public CanvasGroup redOverlay;     // CanvasGroup sur l'image rouge plein écran
    public float flashDuration = 0.2f; // petit flash quand tu te trompes

    private int _currentMistakes = 0;
    private bool _isFlashing = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Appelé quand le joueur fait une erreur (mauvais mot).
    /// </summary>
    public void RegisterMistake()
    {
        _currentMistakes++;
        Debug.Log($"[MistakeManager] Erreur {_currentMistakes}/{maxMistakes}");

        UpdateOverlayBaseAlpha();
        if (!_isFlashing && redOverlay != null)
            StartCoroutine(FlashOverlay());

        if (_currentMistakes >= maxMistakes)
        {
            Debug.Log("[MistakeManager] Limite atteinte, reset de la scène.");
            ReloadSceneFromBed();
        }
    }

    private void UpdateOverlayBaseAlpha()
    {
        if (redOverlay == null) return;

        float t = Mathf.Clamp01((float)_currentMistakes / maxMistakes);
        redOverlay.alpha = Mathf.Lerp(0f, maxRedAlpha, t);
    }

    private System.Collections.IEnumerator FlashOverlay()
    {
        if (redOverlay == null) yield break;

        _isFlashing = true;

        float baseAlpha = redOverlay.alpha;
        float peakAlpha = Mathf.Clamp01(baseAlpha + 0.2f);
        float t = 0f;

        while (t < flashDuration)
        {
            t += Time.deltaTime;
            float k = t / flashDuration;
            redOverlay.alpha = Mathf.Lerp(peakAlpha, baseAlpha, k);
            yield return null;
        }

        redOverlay.alpha = baseAlpha;
        _isFlashing = false;
    }

    private void ReloadSceneFromBed()
    {
        // Le PlayerSpawnManager replacera le joueur près du lit au Start()
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }

    public void ResetMistakes()
    {
        _currentMistakes = 0;

        if (redOverlay != null)
            redOverlay.alpha = 0f;

        Debug.Log("[MistakeManager] ResetMistakes -> compteur remis à 0 et overlay effacé.");
    }

}
