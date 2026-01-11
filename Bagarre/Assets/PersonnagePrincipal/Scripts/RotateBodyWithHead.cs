using UnityEngine;

public class RotateBodyWithHead : MonoBehaviour
{
    public Transform xrCamera;   // Main Camera
    public Transform bodyRoot;   // PersonnagePrincipal (PAS le XR Origin)
    public float rotationSpeed = 5f;  // pour lisser

    void LateUpdate()
    {
        if (xrCamera == null || bodyRoot == null) return;

        Vector3 forward = xrCamera.forward;
        forward.y = 0;

        if (forward.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(forward);
            bodyRoot.rotation = Quaternion.Slerp(
                bodyRoot.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}
