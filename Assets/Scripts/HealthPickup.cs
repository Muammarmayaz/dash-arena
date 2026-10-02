using UnityEngine;

// Spins, bobs, heals the player on touch, and vanishes after a while.
// RequireComponent + Awake set up the collider and Rigidbody in code,
// so the pickup can't be mis-wired in the Inspector (bug U6).
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class HealthPickup : MonoBehaviour
{
    [SerializeField] private int healAmount = 3;      // 3 of 10 HP (Max Health raised to 10, 3 Oct)
    [SerializeField] private float lifetime = 12f;
    [SerializeField] private float spinSpeed = 90f;     // degrees per second
    [SerializeField] private float bobHeight = 0.15f;
    [SerializeField] private float bobSpeed = 3f;

    private Vector3 startPos;
    private float spawnTime;
    private Renderer[] renderers;

    void Awake()
    {
        SphereCollider col = GetComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = 0.6f;

        Rigidbody rb = GetComponent<Rigidbody>();       // triggers need a Rigidbody on one side (bug E1)
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    void Start()
    {
        startPos = transform.position;
        spawnTime = Time.time;
        renderers = GetComponentsInChildren<Renderer>();
    }

    void Update()
    {
        float age = Time.time - spawnTime;
        if (age > lifetime)
        {
            Destroy(gameObject);
            return;
        }

        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.World);
        transform.position = startPos + Vector3.up * Mathf.Sin(age * bobSpeed) * bobHeight;

        // Blink for the last 3 seconds so the player knows it's about to vanish
        bool visible = age < lifetime - 3f || Mathf.Repeat(age * 6f, 1f) > 0.3f;
        foreach (Renderer r in renderers) r.enabled = visible;
    }

    // Stay (not Enter): if you're standing on it at full HP and then get hit, it still works
    void OnTriggerStay(Collider other)
    {
        Transform root = other.transform.root;
        if (!root.CompareTag("Player")) return;                     // enemies can't take it
        if (!root.TryGetComponent(out PlayerHealth health)) return;
        if (health.IsFull) return;                                  // leave it for when you need it

        health.Heal(healAmount);
        Destroy(gameObject);
    }
}
