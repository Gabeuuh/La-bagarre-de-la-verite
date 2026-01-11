using UnityEngine;

public class VRAvatarAligner : MonoBehaviour
{
    [Header("Références XR")]
    public Transform xrCamera;       // Main Camera

    [Header("Avatar")]
    public Transform avatarRoot;     // PersonnagePrincipal
    public Transform headBone;       // HeadTarget

    [Header("Réglages")]
    [Tooltip("Hauteur fixe des pieds (Y monde)")]
    public float fixedFeetY = 0f;

    [Tooltip("Décalage entre la tête VR et la tête de l’avatar")]
    public Vector3 viewOffset = new Vector3(0f, 0.0f, -0.05f);

    Vector3 headLocalOffset;

    void Start()
    {
        if (!xrCamera || !avatarRoot || !headBone)
        {
            Debug.LogError("VRAvatarAligner : références manquantes.");
            enabled = false;
            return;
        }

        // Offset local entre le root de l’avatar et l’os de tête
        headLocalOffset = avatarRoot.InverseTransformPoint(headBone.position);
    }

    void LateUpdate()
    {
        // Offset du bone dans le monde
        Vector3 worldHeadOffset = avatarRoot.rotation * headLocalOffset;
        Vector3 worldViewOffset = avatarRoot.rotation * viewOffset;

        // Position désirée du root
        Vector3 desiredPos = xrCamera.position - worldHeadOffset + worldViewOffset;

        // ❗ On force la hauteur des pieds
        desiredPos.y = fixedFeetY;

        avatarRoot.position = desiredPos;
    }
}
