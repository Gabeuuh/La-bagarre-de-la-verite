using UnityEngine;

public class WordKillZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var word = other.GetComponentInParent<WordData>();
        if (word != null)
        {
            Destroy(word.gameObject);
        }
    }
}
