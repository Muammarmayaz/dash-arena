using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifetime = 4f;
    [SerializeField] private int damage = 1;

    void Start()
    {
        Destroy(gameObject, lifetime);   // missed bullets clean themselves up (B3)
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        // Only the player counts. Enemies are ignored, so no friendly fire (B1, B2)
        Transform root = other.transform.root;
        if (!root.CompareTag("Player")) return;

        // Same interface as the enemies: the bullet doesn't know what it hit
        if (root.TryGetComponent(out IDamageable target))
        {
            target.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
