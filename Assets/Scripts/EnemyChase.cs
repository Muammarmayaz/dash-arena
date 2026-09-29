using UnityEngine;

public class EnemyChase : MonoBehaviour, IDamageable
{
    [SerializeField] private float chaseSpeed = 3.5f;
    [SerializeField] private int maxHealth = 1;
    [SerializeField] private float stopDistance = 0f;   // 0 = chase all the way (Ranged uses ~7)

    private Transform player;
    private int currentHealth;
    private bool isDead;

    void Start()
    {
        currentHealth = maxHealth;

        // Prefabs can't store scene references, so find the player by tag (bug E8)
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (player == null) return;

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.magnitude <= stopDistance) return;   // close enough, stay put

        direction.Normalize();
        transform.position += direction * chaseSpeed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        // The player's collider is on the Capsule child, so check its root
        if (other.transform.root.CompareTag("Player"))
        {
            Debug.Log("you died");
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;           // two hits in one frame won't double-kill (bug E3)

        currentHealth -= amount;
        if (currentHealth > 0) return;

        isDead = true;
        Destroy(gameObject);
    }
}
