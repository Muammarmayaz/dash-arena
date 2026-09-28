using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;                          // drag Player in here
    public Vector3 offset = new Vector3(0f, 15f, 0f);  // height above target

    // LateUpdate runs AFTER every Update() this frame has finished,
    // so the player's position is guaranteed final before the camera reads it.
    // Update() here would cause a one-frame lag -> jitter.
    void LateUpdate()
    {
        transform.position = target.position + offset;
    }
}
