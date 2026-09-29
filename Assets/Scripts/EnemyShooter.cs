using UnityEngine;

// Add next to EnemyChase to make a ranged enemy.
public class EnemyShooter : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;   // drag Prefabs/EnemyBullet here
    [SerializeField] private float fireInterval = 2f;
    [SerializeField] private float range = 12f;         // won't shoot from across the map
    [SerializeField] private float muzzleOffset = 0.8f; // spawn outside own collider (bug B1)

    private Transform player;
    private float nextFireTime;

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;

        // Random first shot so a group doesn't fire on the same frame (bug B4)
        nextFireTime = Time.time + Random.Range(0.5f, fireInterval);
    }

    void Update()
    {
        if (player == null || bulletPrefab == null) return;
        if (Time.time < nextFireTime) return;

        Vector3 dir = player.position - transform.position;
        dir.y = 0f;
        if (dir.magnitude > range) return;
        dir.Normalize();

        Vector3 spawnPos = transform.position + dir * muzzleOffset;
        Instantiate(bulletPrefab, spawnPos, Quaternion.LookRotation(dir));

        nextFireTime = Time.time + fireInterval;
    }
}
