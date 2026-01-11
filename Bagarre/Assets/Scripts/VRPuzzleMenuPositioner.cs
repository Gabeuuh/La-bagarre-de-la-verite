using UnityEngine;

/// <summary>
/// Script pour positionner automatiquement le menu du puzzle devant la caméra en VR
/// À attacher au Canvas du menu puzzle
/// </summary>
public class VRPuzzleMenuPositioner : MonoBehaviour
{
    [Header("Configuration VR")]
    [Tooltip("Distance du menu par rapport à la caméra en VR (en mètres)")]
    public float distanceFromCamera = 2.0f;
    
    [Tooltip("Décalage vertical par rapport à la ligne de vue (en mètres)")]
    public float verticalOffset = 0f;
    
    [Tooltip("Échelle du canvas en mode VR")]
    public Vector3 vrScale = new Vector3(0.002f, 0.002f, 0.002f);
    
    [Tooltip("Échelle du canvas en mode normal")]
    public Vector3 normalScale = new Vector3(0.68875f, 0.68875f, 0.68875f);

    [Header("Références")]
    [Tooltip("Référence à la caméra XR (Main Camera)")]
    public Camera xrCamera;

    private Canvas canvas;
    private bool isVRActive = false;
    private bool hasBeenPositioned = false;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("VRPuzzleMenuPositioner nécessite un composant Canvas!");
            enabled = false;
            return;
        }

        // Trouver la caméra si non assignée
        if (xrCamera == null)
        {
            xrCamera = Camera.main;
            if (xrCamera == null)
            {
                Debug.LogError("Aucune caméra trouvée!");
                enabled = false;
                return;
            }
        }

        // Vérifier si le mode VR est actif
        isVRActive = UnityEngine.XR.XRSettings.enabled;
        
        if (isVRActive)
        {
            PositionCanvasInVR();
        }
        else
        {
            // En mode normal, garder l'échelle actuelle
            transform.localScale = normalScale;
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
                PositionCanvasInVR();
            }
        }
    }

    /// <summary>
    /// Positionne le canvas devant la caméra en mode VR
    /// </summary>
    void PositionCanvasInVR()
    {
        if (xrCamera == null || canvas == null) return;

        // S'assurer que le canvas est en mode World Space
        if (canvas.renderMode != RenderMode.WorldSpace)
        {
            Debug.LogWarning($"Le canvas '{gameObject.name}' doit être en mode World Space pour fonctionner en VR!");
            return;
        }

        // Appliquer l'échelle VR
        transform.localScale = vrScale;

        // Calculer la position devant la caméra
        Vector3 cameraForward = xrCamera.transform.forward;
        Vector3 cameraPosition = xrCamera.transform.position;
        Vector3 cameraUp = xrCamera.transform.up;

        // Position cible : devant la caméra à la distance spécifiée
        Vector3 targetPosition = cameraPosition + (cameraForward * distanceFromCamera) + (cameraUp * verticalOffset);
        
        // Si le canvas est enfant de la caméra, utiliser position locale
        if (transform.parent == xrCamera.transform || IsChildOfCamera(transform))
        {
            // Convertir en position locale
            targetPosition = xrCamera.transform.InverseTransformPoint(targetPosition);
            transform.localPosition = targetPosition;
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            // Position monde
            transform.position = targetPosition;
            // Faire face à la caméra
            transform.LookAt(cameraPosition);
            transform.Rotate(0, 180, 0); // Inverser pour faire face à la caméra
        }

        hasBeenPositioned = true;
        Debug.Log($"Canvas '{gameObject.name}' positionné pour VR à {distanceFromCamera}m de la caméra avec échelle {vrScale}");
    }

    /// <summary>
    /// Vérifie si le transform est enfant de la caméra
    /// </summary>
    bool IsChildOfCamera(Transform t)
    {
        Transform current = t.parent;
        while (current != null)
        {
            if (current == xrCamera.transform)
                return true;
            current = current.parent;
        }
        return false;
    }

    /// <summary>
    /// Méthode publique pour repositionner le canvas manuellement
    /// </summary>
    public void RepositionCanvas()
    {
        if (isVRActive)
        {
            PositionCanvasInVR();
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
            PositionCanvasInVR();
        }
    }

    /// <summary>
    /// Ajuste l'échelle VR
    /// </summary>
    public void SetVRScale(float scale)
    {
        vrScale = new Vector3(scale, scale, scale);
        if (isVRActive && hasBeenPositioned)
        {
            transform.localScale = vrScale;
        }
    }

    // Gizmo pour visualiser la position dans l'éditeur
    void OnDrawGizmosSelected()
    {
        if (xrCamera != null && Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Vector3 cameraPos = xrCamera.transform.position;
            Vector3 targetPos = transform.position;
            Gizmos.DrawLine(cameraPos, targetPos);
            Gizmos.DrawWireSphere(targetPos, 0.1f);
        }
    }
}
