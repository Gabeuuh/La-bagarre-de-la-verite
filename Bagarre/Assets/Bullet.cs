using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float maxDistance = 20f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
            Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit Player, bullet détruite");
            Destroy(gameObject);
        }
        // sinon : ne rien faire → laisse la balle continuer
        // elle se détruira à maxDistance
    }
}
