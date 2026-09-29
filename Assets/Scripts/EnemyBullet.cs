using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifetime = 4f;

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
        if (!other.transform.root.CompareTag("Player")) return;

        Debug.Log("player hit by bullet");   // becomes real damage in Block 7
        Destroy(gameObject);
    }
}
