using UnityEngine;

[RequireComponent(typeof(Light))]
public class PulseLight : MonoBehaviour
{
    public Light targetLight;
    public float minIntensity = 1f;
    public float maxIntensity = 3f;
    public float pulseSpeed = 2f;

    private void Awake()
    {
        if (targetLight == null)
            targetLight = GetComponent<Light>();
    }

    private void Update()
    {
        if (targetLight == null) return;

        // t oscille entre 0 et 1
        float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
        targetLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
    }
}
