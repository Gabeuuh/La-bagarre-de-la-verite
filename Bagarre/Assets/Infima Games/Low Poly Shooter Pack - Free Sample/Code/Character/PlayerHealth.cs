using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public int CurrentHealth;

    public BarreDeVie barreDeVie;
    public AudioClip hurtSound;

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

    public void TakeDamage(int damage)
    {
        Debug.Log(CurrentHealth);
        CurrentHealth -= damage;
        Debug.Log("zombie damage " + CurrentHealth);
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
            Debug.Log("Le joueur est mort");

            // Sauvegarder le score actuel
            PlayerPrefs.SetInt("LastScore", ScoreManager.instance.score);
            PlayerPrefs.Save();

            // Charger la scène GameOver
            SceneManager.LoadScene("GameOver");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("bullet"))
        {
            Debug.Log("Le joueur a été touché (trigger)");
            TakeDamage(1);

            // optionnel : détruire la balle
            Destroy(other.gameObject);
        }
    }

}
