using UnityEngine;

/// <summary>
/// Script pour positionner et dimensionner correctement le JigsawBoard (parent des tuiles) en VR
/// À attacher au GameObject JigsawBoard qui contient les tuiles du puzzle
/// </summary>
public class VRJigsawBoardPositioner : MonoBehaviour
{
    [Header("Configuration VR")]
    [Tooltip("Distance du plateau de jeu par rapport à la caméra en VR (en mètres)")]
    public float distanceFromCamera = 2.5f;
    
    [Tooltip("Décalage vertical par rapport à la ligne de vue (en mètres)")]
    public float verticalOffset = 0f;
    
    [Tooltip("Échelle du plateau en mode VR (ajuste la taille des tuiles)")]
    public float vrScale = 0.0008f;
    
    [Tooltip("Échelle du plateau en mode normal")]
    public float normalScale = 1.0f;

    [Header("Références")]
    [Tooltip("Référence à la caméra XR (Main Camera)")]
    public Camera xrCamera;
    
    [Tooltip("Canvas de référence pour aligner le plateau (optionnel)")]
    public Canvas referenceCanvas;
    
    [Tooltip("Si vrai, aligner le plateau avec le canvas de référence")]
    public bool alignWithCanvas = true;

    private bool isVRActive = false;
    private bool hasBeenPositioned = false;
    private Vector3 originalPosition;
    private Vector3 originalScale;

    void Awake()
    {
        // Sauvegarder la position et l'échelle originales
        originalPosition = transform.position;
        originalScale = transform.localScale;
    }

    void Start()
    {
        // Trouver la caméra si non assignée
        if (xrCamera == null)
        {
            xrCamera = Camera.main;
            if (xrCamera == null)
            {
                Debug.LogError("VRJigsawBoardPositioner: Aucune caméra trouvée!");
                enabled = false;
                return;
            }
        }

        // Vérifier si le mode VR est actif
        isVRActive = UnityEngine.XR.XRSettings.enabled;
        
        if (isVRActive)
        {
            PositionBoardInVR();
        }
        else
        {
            // En mode normal, garder l'échelle et position originales
            transform.localScale = Vector3.one * normalScale;
        }
    }

    void Update()
    {
        // Vérifier si le statut VR a changé
        bool currentVRStatus = UnityEngine.XR.XRSettings.enabled;
        if (currentVRStatus != isVRActive)
        {
            isVRActive = currentVRStatus;
            if (isVRActive)
            {
                PositionBoardInVR();
            }
            else
            {
                // Restaurer position et échelle normales
                transform.position = originalPosition;
                transform.localScale = Vector3.one * normalScale;
            }
        }
    }

    /// <summary>
    /// Positionne le plateau de jeu devant la caméra en mode VR
    /// </summary>
    void PositionBoardInVR()
    {
        if (xrCamera == null) return;

        // Appliquer l'échelle VR (plus petite pour être visible en VR)
        transform.localScale = Vector3.one * vrScale;

        Vector3 targetPosition;
        
        // Si on a un canvas de référence et qu'on doit s'aligner avec
        if (referenceCanvas != null && alignWithCanvas)
        {
            // Positionner le plateau au même endroit que le canvas
            targetPosition = referenceCanvas.transform.position;
            
            // Appliquer les décalages si nécessaire
            Vector3 cameraUp = xrCamera.transform.up;
            targetPosition += cameraUp * verticalOffset;
            
            // Copier la rotation du canvas
            transform.rotation = referenceCanvas.transform.rotation;
        }
        else
        {
            // Calculer la position devant la caméra
            Vector3 cameraForward = xrCamera.transform.forward;
            Vector3 cameraPosition = xrCamera.transform.position;
            Vector3 cameraUp = xrCamera.transform.up;

            // Position cible : devant la caméra à la distance spécifiée
            targetPosition = cameraPosition + (cameraForward * distanceFromCamera) + (cameraUp * verticalOffset);
            
            // Faire face à la caméra
            transform.LookAt(cameraPosition);
            transform.Rotate(0, 180, 0); // Inverser pour que les tuiles soient visibles
        }
        
        // Position dans l'espace monde
        transform.position = targetPosition;

        hasBeenPositioned = true;
        string alignmentInfo = (referenceCanvas != null && alignWithCanvas) ? $" aligné avec canvas '{referenceCanvas.name}'" : "";
        Debug.Log($"JigsawBoard positionné pour VR à {distanceFromCamera}m de la caméra avec échelle {vrScale}{alignmentInfo}");
    }

    /// <summary>
    /// Méthode publique pour repositionner le plateau manuellement
    /// Utile après la génération des tuiles
    /// </summary>
    public void RepositionBoard()
    {
        if (isVRActive)
        {
            PositionBoardInVR();
        }
    }

    /// <summary>
    /// Ajuste la distance par rapport à la caméra
    /// </summary>
    public void SetDistance(float distance)
    {
        distanceFromCamera = distance;
        if (isVRActive && hasBeenPositioned)
        {
            PositionBoardInVR();
        }
    }

    /// <summary>
    /// Ajuste l'échelle VR
    /// </summary>
    public void SetVRScale(float scale)
    {
        vrScale = scale;
        if (isVRActive && hasBeenPositioned)
        {
            transform.localScale = Vector3.one * vrScale;
        }
    }

    /// <summary>
    /// Vérifie si le mode VR est actif
    /// </summary>
    public bool IsVRMode()
    {
        return isVRActive;
    }

    // Gizmo pour visualiser la position dans l'éditeur
    void OnDrawGizmosSelected()
    {
        if (xrCamera != null && Application.isPlaying && isVRActive)
        {
            Gizmos.color = Color.cyan;
            Vector3 cameraPos = xrCamera.transform.position;
            Vector3 boardCenter = transform.position;
            Gizmos.DrawLine(cameraPos, boardCenter);
            Gizmos.DrawWireCube(boardCenter, Vector3.one * 0.2f);
            
            // Dessiner la zone du plateau
            Gizmos.color = Color.yellow;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(500, 500, 0.1f));
        }
    }
}
