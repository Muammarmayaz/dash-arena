using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    public GameObject bulletPrefab;   // drag the Bullet prefab in here
    public Transform spawnPoint;      // an empty child GameObject positioned at the gun muzzle

    void Update()
    {
        // GetMouseButtonDown fires ONCE per click.
        // GetMouseButton (no "Down") fires every frame the button is held -> bullet spam.
        if (Input.GetMouseButtonDown(0))
        {
            // transform.rotation = the player's current facing, so the bullet inherits
            // "which way is forward" at the moment it's fired.
            Instantiate(bulletPrefab, spawnPoint.position, transform.rotation);
        }
    }
}
