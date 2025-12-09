using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class Shooter : MonoBehaviour
{
    [Header("Tir")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireDelay = 0.1f;
    public AudioClip shotSound;

    [Header("Input")]
    public InputActionReference fireAction;   // XRI Right Interaction / Activate Value

    private AudioSource audioSource;
    private bool isHeld = false;
    private float lastShotTime = 0f;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;         // 👈 référence au grab interactable

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        if (grab != null)
        {
            // On branche les events ici, plus besoin de le faire dans l’Inspector
            grab.selectEntered.AddListener(OnSelectEntered);
            grab.selectExited.AddListener(OnSelectExited);
            Debug.Log("[Shooter] Grab events registered sur " + gameObject.name);
        }
        else
        {
            Debug.LogError("[Shooter] Aucun XRGrabInteractable trouvé sur " + gameObject.name);
        }
    }

    void OnEnable()
    {
        if (fireAction != null)
        {
            fireAction.action.Enable();
            Debug.Log("[Shooter] FireAction ENABLED : " + fireAction.action.name);
        }
        else
        {
            Debug.LogWarning("[Shooter] FireAction est NULL !");
        }
    }

    void OnDisable()
    {
        if (fireAction != null)
            fireAction.action.Disable();
    }

    void Update()
    {
        float triggerValue = 0f;

        if (fireAction != null)
            triggerValue = fireAction.action.ReadValue<float>();

        Debug.Log($"[Shooter] Update - isHeld={isHeld}, triggerValue={triggerValue}");

        if (!isHeld) return;
        if (fireAction == null) return;

        if (triggerValue > 0.2f)   // seuil de pression
        {
            TryShoot();
        }
    }

    void TryShoot()
    {
        if (Time.time - lastShotTime < fireDelay)
            return;

        lastShotTime = Time.time;

        Debug.Log("[Shooter] TryShoot appelé");
        Shoot();
    }

    void Shoot()
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("[Shooter] bulletPrefab n'est PAS assigné dans l'Inspector !");
            return;
        }

        if (firePoint == null)
        {
            Debug.LogError("[Shooter] firePoint n'est PAS assigné dans l'Inspector !");
            return;
        }

        Debug.Log("[Shooter] SHOOT ! Instanciation de la balle.");

        if (shotSound != null)
            audioSource.PlayOneShot(shotSound);

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            firePoint.rotation
        );

        Debug.Log("[Shooter] Bullet instanciée : " + bullet.name);
    }

    // ============================
    //  Events de grab / release
    // ============================

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        isHeld = true;
        Debug.Log("[Shooter] OnSelectEntered par : " + args.interactorObject.transform.name);
    }

    void OnSelectExited(SelectExitEventArgs args)
    {
        isHeld = false;
        Debug.Log("[Shooter] OnSelectExited");
    }
}
