using UnityEngine;
using UnityEngine.SceneManagement;

// The player is IDamageable too, so enemies and bullets call TakeDamage
// without knowing they hit the player. Same idea as the prototype-01 bullet.
public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float invulnerableAfterHit = 1f;   // bugs E2 / H1

    private int currentHealth;
    private float invulnerableUntil;
    private bool isDead;
    private PlayerDash dash;

    public float Health01 => (float)currentHealth / maxHealth;   // 0..1 for the health bar
    public bool IsHurtInvulnerable => Time.time < invulnerableUntil;
    public bool IsFull => currentHealth >= maxHealth;

    void Awake()
    {
        currentHealth = maxHealth;
        dash = GetComponent<PlayerDash>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;                                    // two hits same frame = one death (bug R4)
        if (IsHurtInvulnerable) return;                        // just got hit, short grace period
        if (dash != null && dash.IsInvulnerable) return;       // dashing = untouchable (separate timer, bug H3)

        currentHealth = Mathf.Max(currentHealth - amount, 0);  // never below 0 (bug H2)
        invulnerableUntil = Time.time + invulnerableAfterHit;
        Debug.Log("Player hit. HP = " + currentHealth);

        if (currentHealth == 0) Die();
    }

    public void Heal(int amount)
    {
        if (isDead) return;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);   // never above max
        Debug.Log("Healed. HP = " + currentHealth);
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Player died. Restarting.");
        // Reload the scene = clean reset of everything (needs the scene in Build Profiles, bug R1)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
