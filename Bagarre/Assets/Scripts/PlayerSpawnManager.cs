using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    public Transform xrOrigin;    // ton XR Origin (XR Rig)
    public Transform startPoint;  // l'empty "PlayerStart" à côté du lit

    private void Start()
    {
        if (xrOrigin != null && startPoint != null)
        {
            xrOrigin.position = startPoint.position;
            xrOrigin.rotation = startPoint.rotation;
        }
        else
        {
            Debug.LogWarning("[PlayerSpawnManager] xrOrigin ou startPoint n'est pas assigné.");
        }
    }
}
