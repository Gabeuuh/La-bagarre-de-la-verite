using System.Collections;
using UnityEngine;

public class AutoShooter : MonoBehaviour
{
    [Header("Tir")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireDelay = 0.1f; // toutes les 0.1s pour le test

    void Start()
    {
        Debug.Log("AutoShooter.Start sur " + gameObject.name + " - fireDelay = " + fireDelay);
        StartCoroutine(ShootLoop());
    }

    private IEnumerator ShootLoop()
    {
        while (true)
        {
            Debug.Log("Coroutine: j'attends " + fireDelay + "s");
            yield return new WaitForSeconds(fireDelay);

            Debug.Log("Coroutine: je tire");
            Shoot();
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("AutoShooter: bulletPrefab n'est PAS assigné dans l'Inspector !");
            return;
        }

        if (firePoint == null)
        {
            Debug.LogError("AutoShooter: firePoint n'est PAS assigné dans l'Inspector !");
            return;
        }

        GameObject bullet = Instantiate(
    bulletPrefab,
    firePoint.position,
    firePoint.transform.rotation
);


        Debug.Log("Bullet instanciée: " + bullet.name);
    }
}
