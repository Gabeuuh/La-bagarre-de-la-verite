using UnityEngine;

public class MiniBossAI : MonoBehaviour
{
    [Header("Cible (XR Origin)")]
    public Transform target;             // XR Origin / joueur VR

    [Header("Déplacement aléatoire")]
    public float moveRadius = 5f;        // rayon dans lequel il se balade
    public float moveSpeed = 2f;         // vitesse de déplacement
    public float minWaitTime = 1f;       // temps mini avant de changer de destination
    public float maxWaitTime = 3f;       // temps maxi avant de changer de destination

    [Header("Rotation vers le joueur")]
    public float rotationSpeed = 5f;     // vitesse à laquelle il tourne pour regarder le joueur

    private Vector3 originPosition;
    private Vector3 currentDestination;
    private float waitTimer = 0f;
    private float currentWaitDuration = 0f;

    void Start()
    {
        // On mémorise la position de départ comme centre de la zone
        originPosition = transform.position;

        // Première destination
        PickNewDestination();
    }

    void Update()
    {
        if (target != null)
        {
            LookAtTarget();
        }

        MoveRandomly();
    }

    void LookAtTarget()
    {
        // Direction vers le joueur (on garde la même hauteur pour ne pas qu'il se penche)
        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    void MoveRandomly()
    {
        // On attend un peu entre deux déplacements pour éviter qu'il change de direction trop vite
        if (waitTimer < currentWaitDuration)
        {
            waitTimer += Time.deltaTime;
            return;
        }

        // On se déplace vers la destination actuelle
        Vector3 flatPos = transform.position;
        flatPos.y = originPosition.y; // garder le flic sur le même plan (au cas où)

        // Si on est proche de la destination, on en choisit une nouvelle
        if (Vector3.Distance(flatPos, currentDestination) < 0.2f)
        {
            PickNewDestination();
        }
        else
        {
            Vector3 direction = (currentDestination - flatPos).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }

    void PickNewDestination()
    {
        // On choisit un point aléatoire dans un carré autour de la position d'origine
        float randX = Random.Range(-moveRadius, moveRadius);
        float randZ = Random.Range(-moveRadius, moveRadius);

        currentDestination = originPosition + new Vector3(randX, 0f, randZ);

        // On relance un petit temps d'attente avant de bouger vers ce point
        currentWaitDuration = Random.Range(minWaitTime, maxWaitTime);
        waitTimer = 0f;
    }
}
