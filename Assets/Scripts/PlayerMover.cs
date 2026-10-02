using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 6f;   // tweak in Inspector

    // Which key of each pair was pressed most recently (-1 or +1)
    private float lastHorizontal;
    private float lastVertical;

    void Update()
    {
        // Bug fix (3 Oct): GetAxisRaw returns 0 while BOTH opposite keys are held
        // (A+D or W+S), so the player froze for a moment when switching direction
        // in a panic. Now the most recently pressed key wins, like most action games.
        float h = ReadAxis(KeyCode.A, KeyCode.LeftArrow, KeyCode.D, KeyCode.RightArrow, ref lastHorizontal);
        float v = ReadAxis(KeyCode.S, KeyCode.DownArrow, KeyCode.W, KeyCode.UpArrow, ref lastVertical);

        // Build a world-space direction from that input (y stays 0 — top-down, no jumping)
        Vector3 direction = new Vector3(h, 0f, v);

        // Without this, moving diagonally (h=1, v=1) gives length ~1.41 -> faster than moving straight
        if (direction.sqrMagnitude > 1f)
        {
            direction.Normalize();
        }

        // Time.deltaTime makes this frame-rate independent
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Face the direction we're moving. Standing still keeps the last facing.
        if (direction.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    // Returns -1, 0 or +1 for one axis. If both directions are held, the newest press wins.
    static float ReadAxis(KeyCode neg, KeyCode negAlt, KeyCode pos, KeyCode posAlt, ref float last)
    {
        if (Input.GetKeyDown(pos) || Input.GetKeyDown(posAlt)) last = 1f;
        if (Input.GetKeyDown(neg) || Input.GetKeyDown(negAlt)) last = -1f;

        bool negHeld = Input.GetKey(neg) || Input.GetKey(negAlt);
        bool posHeld = Input.GetKey(pos) || Input.GetKey(posAlt);

        if (negHeld && posHeld) return last;
        if (posHeld) return 1f;
        if (negHeld) return -1f;
        return 0f;
    }
}
