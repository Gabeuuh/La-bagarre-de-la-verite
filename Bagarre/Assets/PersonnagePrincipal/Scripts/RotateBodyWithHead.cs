using UnityEngine;

public class RotateBodyWithHead : MonoBehaviour
{
    public Transform xrCamera;
    public Transform bodyRoot;

    void LateUpdate()
    {
        Vector3 forward = xrCamera.forward;
        forward.y = 0;

        if (forward.sqrMagnitude > 0.01f)
        {
            bodyRoot.rotation = Quaternion.LookRotation(forward);
        }
    }
}
