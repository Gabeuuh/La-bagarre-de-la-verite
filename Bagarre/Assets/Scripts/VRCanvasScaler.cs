using UnityEngine;

/// <summary>
/// Script pour ajuster automatiquement l'échelle d'un Canvas en mode VR
/// Attacher ce script au GameObject qui contient le Canvas
/// </summary>
public class VRCanvasScaler : MonoBehaviour
{
    [Header("Échelle du Canvas")]
    [Tooltip("Échelle du canvas lorsque le mode VR est actif")]
    public Vector3 vrScale = new Vector3(0.001f, 0.001f, 0.001f);
    
    [Tooltip("Échelle du canvas lorsque le mode VR n'est pas actif")]
    public Vector3 normalScale = new Vector3(0.68875f, 0.68875f, 0.68875f);
    
    [Header("Distance par rapport à la caméra")]
    [Tooltip("Distance du canvas par rapport à la caméra en VR (en mètres)")]
    public float distanceFromCamera = 0.5f;
    
    [Tooltip("Ajuster automatiquement la distance par rapport à la caméra")]
    public bool adjustDistance = true;

    private Canvas canvas;
    private bool isVRActive = false;
    private Vector3 originalLocalPosition;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("VRCanvasScaler nécessite un composant Canvas sur le même GameObject!");
            enabled = false;
            return;
        }

        // Sauvegarder la position locale originale
        originalLocalPosition = transform.localPosition;

        // Vérifier si le mode VR est actif
        isVRActive = UnityEngine.XR.XRSettings.enabled;
        
        // Appliquer l'échelle appropriée
        ApplyScale();
    }

    void Update()
    {
        // Vérifier si le statut VR a changé
        bool currentVRStatus = UnityEngine.XR.XRSettings.enabled;
        if (currentVRStatus != isVRActive)
        {
            isVRActive = currentVRStatus;
            ApplyScale();
        }
    }

    void ApplyScale()
    {
        if (isVRActive)
        {
            // Appliquer l'échelle VR
            transform.localScale = vrScale;
            
            // Ajuster la distance si nécessaire
            if (adjustDistance && canvas.renderMode == RenderMode.WorldSpace)
            {
                // Positionner le canvas devant la caméra
                Vector3 newPosition = originalLocalPosition;
                newPosition.z = distanceFromCamera;
                transform.localPosition = newPosition;
            }
            
            Debug.Log($"Canvas '{gameObject.name}' configuré pour VR avec échelle {vrScale}");
        }
        else
        {
            // Appliquer l'échelle normale
            transform.localScale = normalScale;
            
            // Restaurer la position originale
            if (adjustDistance)
            {
                transform.localPosition = originalLocalPosition;
            }
            
            Debug.Log($"Canvas '{gameObject.name}' configuré pour mode normal avec échelle {normalScale}");
        }
    }

    // Méthode pour ajuster manuellement l'échelle VR
    public void SetVRScale(float scale)
    {
        vrScale = new Vector3(scale, scale, scale);
        if (isVRActive)
        {
            ApplyScale();
        }
    }

    // Méthode pour ajuster la distance
    public void SetDistance(float distance)
    {
        distanceFromCamera = distance;
        if (isVRActive && adjustDistance)
        {
            ApplyScale();
        }
    }
}
