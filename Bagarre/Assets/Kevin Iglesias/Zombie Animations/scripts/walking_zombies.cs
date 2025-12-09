using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    [Header("Vie")]
    public int maxHealth = 5;
    public int currentHealth;

    public BarreDeVie barreDeVie;          // même système que ton boss

    [Header("Déplacement vers le joueur")]
    public float walkSpeed = 5f;
    public Transform target;

    [Header("Audio")]
    public AudioClip hurtSound;
    public AudioClip deathSound;

    [Header("FX de mort")]
    public GameObject deathEffectPrefab;

    private AudioSource audioSource;
    private bool isDead = false;

    void Start()
    {
        // Cible = joueur (XR Origin / Player)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            target = player.transform;

        // Vie de départ
        currentHealth = maxHealth;

        if (barreDeVie != null)
        {
            barreDeVie.SetMaxHealth(maxHealth);
            barreDeVie.SetHealth(currentHealth);
        }

        // AudioSource comme sur le boss
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (isDead) return;

        // 🔁 Déplacement : même logique que ton ancien walking_zombies
        if (target != null)
        {
            // avance vers le joueur
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                walkSpeed * Time.deltaTime
            );

            // regarde le joueur
            Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * 5f
            );
        }
    }

    // =============================
    //      GESTION DES DÉGÂTS
    // =============================
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        Debug.Log("Vie zombie avant coup : " + currentHealth);
        currentHealth -= damage;
        Debug.Log("Vie zombie après coup : " + currentHealth);

        if (barreDeVie != null)
            barreDeVie.SetHealth(currentHealth);

        if (hurtSound != null)
            audioSource.PlayOneShot(hurtSound);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Zombie mort");

        if (deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, transform.position);

        if (deathEffectPrefab != null)
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);

        // Si tu utilises toujours ton score manager
        if (ScoreManager.instance != null)
            ScoreManager.instance.AddPoint();

        Destroy(gameObject);
    }

    // =============================
    //  COLLISIONS (comme le boss)
    // =============================
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("glove"))
        {
            Debug.Log("Zombie touché par le gant !");
            TakeDamage(1);
        }

        if (other.CompareTag("projectile"))
        {
            Debug.Log("Zombie touché par un projectile !");
            TakeDamage(1);
        }
        if (other.CompareTag("bullet"))
        {
            Debug.Log("Le boss a été touché par un projectile");
            TakeDamage(1);
        }
    }
}
