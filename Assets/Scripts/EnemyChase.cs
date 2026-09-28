using UnityEngine;

public class EnemyChase : MonoBehaviour, IDamageable
{
    public Transform player;                           // drag Player in here
    [SerializeField] private float chaseSpeed = 3.5f;   // keep below player's moveSpeed

    void Update()
    {
        // Vector FROM enemy TO player. Reverse this order and the enemy flees instead of chasing.
        Vector3 direction = player.position - transform.position;

        // Zero out y so height differences don't make the enemy drift up/down
        direction.y = 0f;

        // Strip the distance, keep only the direction. Without this, the enemy
        // speeds up when far away and slows down when close (homing-missile feel).
        direction.Normalize();

        transform.position += direction * chaseSpeed * Time.deltaTime;
    }

    // Fires when the enemy's trigger collider touches something else.
    // Used here for "enemy touches player" -> death message.
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("you died");
        }
    }

    // The enemy's own answer to "what does taking damage mean for me?"
    // A boss or barrel would implement this completely differently.
    public void TakeDamage(int amount)
    {
        Destroy(gameObject);
    }
}
