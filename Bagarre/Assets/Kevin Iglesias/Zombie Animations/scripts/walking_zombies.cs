using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class walking_zombies : MonoBehaviour
{
    public int MaxHealth = 5;
    public int CurrentHealth;
    public float WalkSpeed = 5;
    public Transform target;
    public healtbar healthBar;

    public GameObject deathEffectPrefab;
    public AudioClip deathSound; // 🎵 Ajouté pour le son de mort
    private bool isDead = false;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        CurrentHealth = 0;
        healthBar.SetMaxHealt(MaxHealth);
    }

    void TakeDamage(int damage)
    {
        if (isDead) return;

        Debug.Log(CurrentHealth);
        CurrentHealth += damage;
        Debug.Log("après coup" + CurrentHealth);
        healthBar.SetHealt(CurrentHealth);

        if (CurrentHealth >= MaxHealth)
        {
            ScoreManager.instance.AddPoint();
            PlayDeathEffect();
        }
    }

    void PlayDeathEffect()
    {
        isDead = true;

        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position); // 🔊 joue indépendamment du zombie
        }

        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject); // détruit tout de suite
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("LoveBullet"))
        {
            Debug.Log("Touché !");
            TakeDamage(1);
            Destroy(collision.gameObject);
        }
    }

    void Update()
    {
        if (isDead) return;

        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, WalkSpeed * Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    public class ZombieDamage : MonoBehaviour
    {
        public int damage = 1;
    }

    public int damage = 1;

    void OnCollisionEnterPlayer(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Le zombie a touché le joueur");

            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }
}
