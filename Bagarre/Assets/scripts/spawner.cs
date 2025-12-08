using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    public GameObject Zombie;
    public float spawnRadius = 1f;

    private float spawnInterval = 4f;
    private float gameStartTime;

    private void Start()
    {
        gameStartTime = Time.time;

        // Spawn initial
        SpawnZombie();

        // Démarre la coroutine de spawn
        StartCoroutine(SpawnZombies());
    }

    private IEnumerator SpawnZombies()
    {
        while (true)
        {
            // Calcule le temps écoulé depuis le début du jeu
            float elapsedTime = Time.time - gameStartTime;

            // Ajuste l'intervalle de spawn en fonction du temps écoulé
            if (elapsedTime >= 120f)
                spawnInterval = 1f;
            else if (elapsedTime >= 90f)
                spawnInterval = 1.5f;
            else if (elapsedTime >= 60f)
                spawnInterval = 2f;
            else if (elapsedTime >= 30f)
                spawnInterval = 3f;
            else
                spawnInterval = 4f;

            // Attend l'intervalle de temps avant de spawner le prochain zombie
            yield return new WaitForSeconds(spawnInterval);

            // Spawn du zombie
            SpawnZombie();
        }
    }

    private void SpawnZombie()
    {
        Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPosition = transform.position + new Vector3(randomOffset.x, 0, randomOffset.y);

        Instantiate(Zombie, spawnPosition, Quaternion.identity);
    }
}
