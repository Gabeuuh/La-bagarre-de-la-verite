using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    public int maxHealth = 5;
    public int CurrentHealth;

    public BarreDeVie barreDeVie;
    public AudioClip hurtSound;
    public AudioClip DeathSound;

    public GameObject explosionPrefab;

    private AudioSource audioSource;
    private CameraShake cameraShake;

    void Start()
    {
        CurrentHealth = maxHealth;
        barreDeVie.SetMaxHealth(maxHealth);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        cameraShake = Camera.main.GetComponent<CameraShake>();
    }

    void Die()
    {
        Debug.Log("Le Boss est mort");

        if (DeathSound != null)
        {
            AudioSource.PlayClipAtPoint(DeathSound, transform.position);
        }
        // Spawn FX
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        // Destroy le boss
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("vie du boss : " + CurrentHealth);
        CurrentHealth -= damage;
        barreDeVie.SetHealth(CurrentHealth);

        if (hurtSound != null)
        {
            audioSource.PlayOneShot(hurtSound);
        }

        if (cameraShake != null)
        {
            cameraShake.Shake();
        }

        if (CurrentHealth <= 0)
        {
            Die();
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("glove"))
        {
            Debug.Log("Le boss a été frappé ! ");
            TakeDamage(3);
        }
        if (other.CompareTag("projectile"))
        {
            Debug.Log("Le boss a été touché par un projectile");
            TakeDamage(1);
        }
    }

}
