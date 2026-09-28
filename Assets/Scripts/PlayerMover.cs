
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 6f;   // tweak in Inspector

   void Update()
{
    
    float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Build a world-space direction from that input (y stays 0 — top-down, no jumping)
        Vector3 direction = new Vector3(h, 0f, v);

        // Without this, moving diagonally (h=1, v=1) gives length ~1.41 -> faster than moving straight
        if (direction.sqrMagnitude > 1f)
        {
            direction.Normalize();
        }

        // Time.deltaTime makes this frame-rate independent
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Face the direction we're moving, so PlayerShooter's transform.rotation
        // actually points somewhere other than world +z. Standing still keeps
        // whatever facing you last moved in.
        if (direction.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}