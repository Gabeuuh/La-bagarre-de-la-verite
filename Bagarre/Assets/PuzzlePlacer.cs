using UnityEngine;

public class PuzzlePlacer : MonoBehaviour
{
    [Header("Réglages de position")]
    public float distanceFromHead = 2f;    // distance devant le joueur
    public float heightOffset = 0f;       // décalage vertical optionnel

    void Start()
    {
        // On récupère la caméra du joueur (caméra XR)
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("PuzzlePlacer : aucune Main Camera trouvée.");
            return;
        }

        // On projette le forward sur le plan horizontal (pour éviter de regarder vers le haut/bas)
        Vector3 forwardFlat = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized;
        if (forwardFlat.sqrMagnitude < 0.001f)
        {
            forwardFlat = cam.transform.forward;
        }

        // Position : devant la tête + petit offset vertical
        Vector3 targetPos = cam.transform.position
                            + forwardFlat * distanceFromHead
                            + Vector3.up * heightOffset;

        transform.position = targetPos;

        // On oriente le puzzle pour qu'il fasse face au joueur
        transform.rotation = Quaternion.LookRotation(forwardFlat, Vector3.up);
    }
}
