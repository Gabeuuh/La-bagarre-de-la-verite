using UnityEngine;


public class WordMover : MonoBehaviour
{
    Vector3 _direction = Vector3.left;
    float _speed = 0.5f;
    float _lifeTime = 10f;
    float _timer = 0f;

    UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable _grab;   // 👈

    public void Init(Vector3 direction, float speed, float lifeTime)
    {
        _direction = direction.normalized;
        _speed = speed;
        _lifeTime = lifeTime;
    }

    void Awake()
    {
        _grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }

    void Update()
    {
        // Si le mot est en train d’être tenu, on ne le bouge plus automatiquement
        if (_grab != null && _grab.isSelected)
            return;

        transform.position += _direction * _speed * Time.deltaTime;

        _timer += Time.deltaTime;
        if (_timer >= _lifeTime)
            Destroy(gameObject);
    }
}
