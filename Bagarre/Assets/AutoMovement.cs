using UnityEngine;

public class MiniBossAI : MonoBehaviour
{
    [Header("Cible (XR Origin)")]
    public Transform target;

    [Header("Déplacement aléatoire")]
    public float moveRadius = 5f;
    public float moveSpeed = 2f;
    public float minWaitTime = 1f;
    public float maxWaitTime = 3f;

    [Header("Rotation vers le joueur")]
    public float rotationSpeed = 5f;

    private Vector3 originPosition;
    private Vector3 currentDestination;
    private float waitTimer = 0f;
    private float currentWaitDuration = 0f;

    private Rigidbody rb;   // 👈 nouveau

    void Awake()
    {
        rb = GetComponent<Rigidbody>();  // récupère le rigidbody
    }

    void Start()
    {
        // centre de la zone = position de départ
        originPosition = transform.position;
        PickNewDestination();
    }

    void Update()
    {
        if (target != null)
        {
            LookAtTarget();
        }
    }

    void FixedUpdate()
    {
        // mouvement physique = dans FixedUpdate
        MoveRandomly();
    }

    void LookAtTarget()
    {
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
        if (waitTimer < currentWaitDuration)
        {
            waitTimer += Time.fixedDeltaTime;
            return;
        }

        // position actuelle à plat
        Vector3 flatPos = rb.position;
        flatPos.y = originPosition.y;

        if (Vector3.Distance(flatPos, currentDestination) < 0.2f)
        {
            PickNewDestination();
        }
        else
        {
            Vector3 direction = (currentDestination - flatPos).normalized;

            // déplacement pour cette frame physique
            float delta = Time.fixedDeltaTime;
            Vector3 move = direction * moveSpeed * delta;

            rb.MovePosition(rb.position + move);
        }
    }

    void PickNewDestination()
    {
        float randX = Random.Range(-moveRadius, moveRadius);
        float randZ = Random.Range(-moveRadius, moveRadius);

        currentDestination = originPosition + new Vector3(randX, 0f, randZ);

        currentWaitDuration = Random.Range(minWaitTime, maxWaitTime);
        waitTimer = 0f;
    }
}
