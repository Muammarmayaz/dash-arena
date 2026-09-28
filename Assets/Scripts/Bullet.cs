using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private int damage = 1;

    void Start()
    {
        // Self-destruct after 3 seconds so missed bullets don't live forever
        // and pile up eating frames.
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        // Translate moves relative to THIS object's own rotation, so Vector3.forward
        // means the bullet's forward — which it inherited from the player when fired.
        transform.Translate(Vector3.forward * bulletSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // Ignore solid, non-trigger colliders (the floor, the player's own body).
        // Only Enemy is set up as a trigger, so this is what keeps the bullet
        // alive long enough to actually reach something.
        if (!other.isTrigger) return;

        // "Does this thing have a TakeDamage method? If so, call it. I don't know
        // or care what it actually is." This is the whole point of the interface —
        // the bullet never checks tags or types.
        if (other.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            damageable.TakeDamage(damage);
        }

        // Destroyed either way — hitting a wall or anything non-damageable still ends the bullet.
        Destroy(gameObject);
    }
}
